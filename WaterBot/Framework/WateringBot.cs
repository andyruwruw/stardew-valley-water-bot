using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using WaterBot.Framework.Models;

namespace WaterBot.Framework;

/// <summary>
/// State machine that orchestrates automated crop watering.
/// All game state mutation happens on the main thread via <see cref="IGameLoopEvents.UpdateTicked"/>.
/// Uses <see cref="FarmerMover"/> for smooth player movement instead of PathFindController.
/// </summary>
internal sealed class WateringBot
{
    private enum BotState
    {
        Idle,
        Walking,
        Watering,
        WaitingForAnimation,
        Refilling,
        WaitingAfterRefill
    }

    private readonly IModHelper _helper;
    private readonly ModConfig _config;
    private readonly FarmerMover _mover = new();

    // Current operation state
    private BotState _state = BotState.Idle;
    private TileGrid _grid = new();
    private List<CropGroup> _groupRoute = new();
    private List<WateringAction> _currentActions = new();
    private int _currentGroupIndex;
    private int _currentActionIndex;
    private int _delayTicksRemaining;
    private WateringAction? _refillAction;

    /// <summary>Whether the bot is currently running.</summary>
    public bool IsActive => _state != BotState.Idle;

    public WateringBot(IModHelper helper, ModConfig config)
    {
        _helper = helper;
        _config = config;

        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Player.Warped += OnWarped;
    }

    /// <summary>Start the watering bot from the player's current position.</summary>
    public void Start()
    {
        var location = Game1.currentLocation;
        var player = Game1.player;

        // Load and analyze the map
        _grid.Load(location);

        if (_grid.CropTiles.Count == 0)
        {
            Logger.Debug("WateringBot: no crops need watering.");
            return;
        }

        // Group crops
        var groups = _config.UseSmallGrouping
            ? CropGrouper.GroupByMinimalCover(_grid.CropTiles, _grid)
            : CropGrouper.GroupByAdjacency(_grid.CropTiles, _grid);

        if (groups.Count == 0)
        {
            Logger.Debug("WateringBot: no groups produced.");
            return;
        }

        // Order groups by nearest-neighbor
        _groupRoute = RoutePlanner.OrderGroups(groups, player.TilePoint);
        _currentGroupIndex = 0;

        // Plan watering path for first group
        _currentActions = RoutePlanner.PlanWateringPath(_groupRoute[0], player.TilePoint, _grid);
        _currentActionIndex = 0;

        if (_currentActions.Count == 0)
        {
            Logger.Debug("WateringBot: first group produced no actions.");
            return;
        }

        _state = BotState.Walking;
        ShowMessage("process.start", HUDMessage.newQuest_type);
        Logger.Info("WateringBot: started.");

        // Check if we need to refill before starting
        if (GetWateringCan() is WateringCan can && can.WaterLeft <= 0)
        {
            BeginRefill();
            return;
        }

        WalkToCurrentAction();
    }

    /// <summary>Stop the bot immediately (user interrupt).</summary>
    public void Stop()
    {
        if (!IsActive) return;

        _state = BotState.Idle;
        _mover.Stop();
        ShowMessage("process.interrupt", HUDMessage.error_type);
        Logger.Info("WateringBot: stopped by user.");
    }

    /// <summary>Called every game tick. Drives the state machine on the main thread.</summary>
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (_state == BotState.Idle)
            return;

        if (!Context.IsWorldReady)
        {
            ForceStop();
            return;
        }

        // Safety: check tool hasn't changed
        if (Game1.player.CurrentTool is not WateringCan)
        {
            ForceStop("WateringBot: tool changed, stopping.");
            return;
        }

        // Drive the mover each tick when walking
        if (_state is BotState.Walking or BotState.Refilling)
            _mover.Update();

        switch (_state)
        {
            case BotState.WaitingForAnimation:
                if (_delayTicksRemaining > 0)
                {
                    _delayTicksRemaining--;
                    return;
                }
                _state = BotState.Watering;
                ProcessCurrentAction();
                break;

            case BotState.WaitingAfterRefill:
                if (_delayTicksRemaining > 0)
                {
                    _delayTicksRemaining--;
                    return;
                }
                AfterRefill();
                break;
        }
    }

    /// <summary>Stop bot if player warps to another location.</summary>
    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (IsActive)
            ForceStop("WateringBot: player warped, stopping.");
    }

    /// <summary>Callback when FarmerMover reaches its destination.</summary>
    private void OnArrived(Character c, GameLocation location)
    {
        if (!IsActive) return;

        if (_state == BotState.Refilling)
        {
            PerformRefill();
            return;
        }

        _state = BotState.Watering;
        ProcessCurrentAction();
    }

    /// <summary>Process the next tile in the current watering action.</summary>
    private void ProcessCurrentAction()
    {
        if (!IsActive) return;

        // Stamina check
        if (Game1.player.Stamina <= 2f)
        {
            _state = BotState.Idle;
            _mover.Stop();
            ShowMessage("process.exhausted", HUDMessage.error_type);
            Logger.Info("WateringBot: exhausted.");
            return;
        }

        // Water level check
        if (GetWateringCan() is WateringCan can && can.WaterLeft <= 0)
        {
            BeginRefill();
            return;
        }

        // Get current action
        if (_currentActionIndex >= _currentActions.Count)
        {
            AdvanceToNextGroup();
            return;
        }

        var action = _currentActions[_currentActionIndex];
        var target = action.TryDequeueTarget();

        if (target is Point tile)
        {
            int duration = WateringAnimator.AnimateWatering(Game1.player, tile);
            _delayTicksRemaining = Math.Max(1, duration * 60 / 1000);
            _state = BotState.WaitingForAnimation;
        }
        else
        {
            _currentActionIndex++;
            if (_currentActionIndex < _currentActions.Count)
            {
                _state = BotState.Walking;
                WalkToCurrentAction();
            }
            else
            {
                AdvanceToNextGroup();
            }
        }
    }

    /// <summary>Move to the next group, or finish if all groups are done.</summary>
    private void AdvanceToNextGroup()
    {
        _currentGroupIndex++;

        if (_currentGroupIndex >= _groupRoute.Count)
        {
            if (_config.RefillOnFinish && ShouldRefill())
            {
                BeginRefill();
                return;
            }

            _state = BotState.Idle;
            _mover.Stop();
            ShowMessage("process.end", HUDMessage.achievement_type);
            Logger.Info("WateringBot: finished watering all crops.");
            return;
        }

        _currentActions = RoutePlanner.PlanWateringPath(
            _groupRoute[_currentGroupIndex],
            Game1.player.TilePoint,
            _grid);
        _currentActionIndex = 0;

        if (_currentActions.Count > 0)
        {
            _state = BotState.Walking;
            WalkToCurrentAction();
        }
        else
        {
            AdvanceToNextGroup();
        }
    }

    /// <summary>Begin walking to a water source to refill.</summary>
    private void BeginRefill()
    {
        _refillAction = WaterSourceFinder.FindNearest(Game1.player.TilePoint, _grid);

        if (_refillAction == null)
        {
            _state = BotState.Idle;
            _mover.Stop();
            ShowMessage("process.waterless", HUDMessage.error_type);
            Logger.Warn("WateringBot: no water source found.");
            return;
        }

        _state = BotState.Refilling;
        _mover.StartPath(_refillAction.StandPosition, Game1.currentLocation, OnArrived);
    }

    /// <summary>Perform the refill animation at the water source.</summary>
    private void PerformRefill()
    {
        if (_refillAction?.TryDequeueTarget() is Point refillTile)
        {
            int duration = WateringAnimator.AnimateWatering(Game1.player, refillTile);
            _delayTicksRemaining = Math.Max(1, duration * 60 / 1000);
            _state = BotState.WaitingAfterRefill;
        }
        else
        {
            AfterRefill();
        }
    }

    /// <summary>After refilling, either recalculate the route or resume.</summary>
    private void AfterRefill()
    {
        if (_config.RedoPathOnRefill)
        {
            Logger.Debug("WateringBot: recalculating route after refill.");
            _state = BotState.Idle;
            Start();
        }
        else
        {
            _state = BotState.Walking;
            if (_currentActionIndex < _currentActions.Count)
                WalkToCurrentAction();
            else
                AdvanceToNextGroup();
        }
    }

    /// <summary>Walk the player to the current action's standing position.</summary>
    private void WalkToCurrentAction()
    {
        if (_currentActionIndex >= _currentActions.Count)
        {
            AdvanceToNextGroup();
            return;
        }

        var target = _currentActions[_currentActionIndex].StandPosition;
        _mover.StartPath(target, Game1.currentLocation, OnArrived);
    }

    private bool ShouldRefill()
    {
        if (GetWateringCan() is not WateringCan can)
            return false;

        float pct = (float)can.WaterLeft / can.waterCanMax;
        return pct < _config.RefillIfLower * 0.01f;
    }

    private static WateringCan? GetWateringCan()
    {
        return Game1.player.CurrentTool as WateringCan;
    }

    private void ForceStop(string? logMessage = null)
    {
        _state = BotState.Idle;
        _mover.Stop();
        if (logMessage != null)
            Logger.Warn(logMessage);
    }

    private void ShowMessage(string translationKey, int type)
    {
        string text = _helper.Translation.Get(translationKey);
        Game1.addHUDMessage(new HUDMessage(text, type));
    }
}
