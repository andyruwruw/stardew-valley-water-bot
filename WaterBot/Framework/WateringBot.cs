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
    /// <summary>The possible states of the watering bot's lifecycle.</summary>
    private enum BotState
    {
        /// <summary>Bot is not running. Waiting for user trigger.</summary>
        Idle,
        /// <summary>Farmer is walking toward the next watering action's standing position.</summary>
        Walking,
        /// <summary>Farmer is at the standing position and processing watering targets.</summary>
        Watering,
        /// <summary>Watering animation is playing; waiting for the tick delay to elapse.</summary>
        WaitingForAnimation,
        /// <summary>Farmer is walking toward a water source to refill the can.</summary>
        Refilling,
        /// <summary>Refill animation is playing; waiting for the tick delay to elapse.</summary>
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
    private WateringAction? _refillAction;
    private int _startTick;

    /// <summary>Whether the bot is currently running (any state other than Idle).</summary>
    public bool IsActive => _state != BotState.Idle;

    /// <summary>Whether the bot started recently enough that stop requests should be ignored.</summary>
    public bool InGracePeriod => IsActive && Game1.ticks - _startTick < 30;

    /// <summary>
    /// Create a new WateringBot and subscribe to SMAPI events.
    /// </summary>
    /// <param name="helper">SMAPI mod helper for event registration and translations.</param>
    /// <param name="config">Mod configuration (injected, not static).</param>
    public WateringBot(IModHelper helper, ModConfig config)
    {
        _helper = helper;
        _config = config;

        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Player.Warped += OnWarped;
    }

    /// <summary>
    /// Start the watering bot from the player's current position.
    /// Scans the map for crops, groups them, plans an optimal route, and begins
    /// walking to the first watering position. Does nothing if no crops need watering.
    /// </summary>
    public void Start()
    {
        var location = Game1.currentLocation;
        var player = Game1.player;

        _grid.Load(location);

        if (_grid.CropTiles.Count == 0)
        {
            Logger.Debug("WateringBot: no crops need watering.");
            return;
        }

        var groups = _config.UseSmallGrouping
            ? CropGrouper.GroupByMinimalCover(_grid.CropTiles, _grid)
            : CropGrouper.GroupByAdjacency(_grid.CropTiles, _grid);

        if (groups.Count == 0)
        {
            Logger.Debug("WateringBot: no groups produced.");
            return;
        }

        _groupRoute = RoutePlanner.OrderGroups(groups, player.TilePoint);
        _currentGroupIndex = 0;

        _currentActions = RoutePlanner.PlanWateringPath(_groupRoute[0], player.TilePoint, _grid);
        _currentActionIndex = 0;

        if (_currentActions.Count == 0)
        {
            Logger.Debug("WateringBot: first group produced no actions.");
            return;
        }

        _state = BotState.Walking;
        _startTick = Game1.ticks;
        ShowMessage("process.start", HUDMessage.newQuest_type);
        Logger.Info("WateringBot: started.");

        if (GetWateringCan() is WateringCan can && can.WaterLeft <= 0)
        {
            BeginRefill();
            return;
        }

        WalkToCurrentAction();
    }

    /// <summary>
    /// Stop the bot immediately in response to a user interrupt (any button press).
    /// Halts farmer movement and displays an interruption message.
    /// </summary>
    public void Stop()
    {
        if (!IsActive) return;

        _state = BotState.Idle;
        _mover.Stop();
        WateringAnimator.ResetFarmerState(Game1.player);
        ShowMessage("process.interrupt", HUDMessage.error_type);
        Logger.Info("WateringBot: stopped by user.");
    }

    /// <summary>
    /// Main tick handler. Drives the state machine on the main thread each game tick.
    /// Updates the farmer mover when walking, counts down animation delays, and
    /// performs safety checks (world ready, tool equipped).
    /// </summary>
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (_state == BotState.Idle)
            return;

        if (!Context.IsWorldReady)
        {
            ForceStop();
            return;
        }

        if (Game1.player.CurrentTool is not WateringCan)
        {
            ForceStop("WateringBot: tool changed, stopping.");
            return;
        }

        // Drive the mover each tick when walking
        if (_state is BotState.Walking or BotState.Refilling)
        {
            _mover.Update();

            // If the mover fired an arrival callback this tick, the state machine
            // already transitioned (e.g. to WaitingForAnimation). Don't process
            // the switch below on the same tick — it would decrement the delay
            // counter by 1 before the first real wait tick.
            if (_mover.ArrivedThisTick)
                return;
        }

        switch (_state)
        {
            case BotState.WaitingForAnimation:
                if (!WateringAnimator.IsAnimationComplete(Game1.player))
                    return;
                _state = BotState.Watering;
                ProcessCurrentAction();
                break;

            case BotState.WaitingAfterRefill:
                if (!WateringAnimator.IsAnimationComplete(Game1.player))
                    return;
                AfterRefill();
                break;
        }
    }

    /// <summary>
    /// Safety handler: stop the bot if the player warps to another location,
    /// since the map data and pathfinding are no longer valid.
    /// </summary>
    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (IsActive)
            ForceStop("WateringBot: player warped, stopping.");
    }

    /// <summary>
    /// Callback fired by <see cref="FarmerMover"/> when the farmer reaches the
    /// destination tile. Routes to either refill handling or crop watering
    /// depending on the current bot state.
    /// </summary>
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

    /// <summary>
    /// Process the next tile in the current watering action. Checks stamina and water
    /// level before each tile. When all targets in an action are done, advances to the
    /// next action or group. Triggers the watering animation and sets a tick delay.
    /// </summary>
    private void ProcessCurrentAction()
    {
        if (!IsActive) return;

        if (Game1.player.Stamina <= 2f)
        {
            _state = BotState.Idle;
            _mover.Stop();
            WateringAnimator.ResetFarmerState(Game1.player);
            ShowMessage("process.exhausted", HUDMessage.error_type);
            Logger.Info("WateringBot: exhausted.");
            return;
        }

        if (GetWateringCan() is WateringCan can && can.WaterLeft <= 0)
        {
            BeginRefill();
            return;
        }

        if (_currentActionIndex >= _currentActions.Count)
        {
            AdvanceToNextGroup();
            return;
        }

        var action = _currentActions[_currentActionIndex];
        var target = action.TryDequeueTarget();

        if (target is Point tile)
        {
            WateringAnimator.AnimateWatering(Game1.player, tile);
            _state = BotState.WaitingForAnimation;
        }
        else
        {
            // All targets in this action are done — advance to next action
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

    /// <summary>
    /// Advance to the next crop group in the route. Plans a new watering path for it.
    /// If all groups are done, optionally refills and then ends the bot session.
    /// </summary>
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
            WateringAnimator.ResetFarmerState(Game1.player);
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
            // Empty group (all tiles unreachable) — skip to next
            AdvanceToNextGroup();
        }
    }

    /// <summary>
    /// Initiate a refill sequence: find the nearest water source via BFS,
    /// then walk the farmer to its standing position. If no water source is
    /// reachable, stops the bot with an error message.
    /// </summary>
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
        WateringAnimator.ResetFarmerState(Game1.player);
        _mover.StartPath(_refillAction.StandPosition, Game1.currentLocation, OnArrived,
            onPathFailed: () =>
            {
                _state = BotState.Idle;
                _mover.Stop();
                ShowMessage("process.waterless", HUDMessage.error_type);
                Logger.Warn("WateringBot: can't reach water source.");
            });
    }

    /// <summary>
    /// Perform the refill animation at the water source tile.
    /// Dequeues the refill target and triggers the watering animation
    /// (which the game interprets as a refill when aimed at water).
    /// </summary>
    private void PerformRefill()
    {
        if (_refillAction?.TryDequeueTarget() is Point refillTile)
        {
            WateringAnimator.AnimateWatering(Game1.player, refillTile);
            _state = BotState.WaitingAfterRefill;
        }
        else
        {
            AfterRefill();
        }
    }

    /// <summary>
    /// Called after the refill animation completes. Either recalculates the entire
    /// route (if <see cref="ModConfig.RedoPathOnRefill"/> is enabled) or resumes
    /// the current path from where it left off.
    /// </summary>
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

    /// <summary>
    /// Start walking the farmer to the current watering action's standing position.
    /// If there are no more actions, advances to the next group instead.
    /// </summary>
    private void WalkToCurrentAction()
    {
        if (_currentActionIndex >= _currentActions.Count)
        {
            AdvanceToNextGroup();
            return;
        }

        WateringAnimator.ResetFarmerState(Game1.player);
        var target = _currentActions[_currentActionIndex].StandPosition;
        _mover.StartPath(target, Game1.currentLocation, OnArrived, onPathFailed: OnPathFailed);
    }

    /// <summary>
    /// Called when pathfinding to a watering action fails (unreachable or stuck).
    /// Skips the current action and tries the next one, or advances to the next group.
    /// </summary>
    private void OnPathFailed()
    {
        Logger.Warn("WateringBot: path failed, skipping to next action.");
        _currentActionIndex++;
        if (_currentActionIndex < _currentActions.Count)
        {
            WalkToCurrentAction();
        }
        else
        {
            AdvanceToNextGroup();
        }
    }

    /// <summary>
    /// Check whether the watering can should be refilled based on the
    /// <see cref="ModConfig.RefillIfLower"/> threshold percentage.
    /// </summary>
    /// <returns>True if water level is below the configured threshold.</returns>
    private bool ShouldRefill()
    {
        if (GetWateringCan() is not WateringCan can)
            return false;

        float pct = (float)can.WaterLeft / can.waterCanMax;
        return pct < _config.RefillIfLower * 0.01f;
    }

    /// <summary>
    /// Get the player's currently equipped watering can, or null if a different tool
    /// is selected. Uses safe cast to avoid <see cref="InvalidCastException"/>.
    /// </summary>
    private static WateringCan? GetWateringCan()
    {
        return Game1.player.CurrentTool as WateringCan;
    }

    /// <summary>
    /// Force-stop the bot from an internal safety check (warp, tool change, world unloaded).
    /// Does not show a user-facing message — only logs if a message is provided.
    /// </summary>
    /// <param name="logMessage">Optional warning message to log, or null for silent stop.</param>
    private void ForceStop(string? logMessage = null)
    {
        _state = BotState.Idle;
        _mover.Stop();
        WateringAnimator.ResetFarmerState(Game1.player);
        if (logMessage != null)
            Logger.Warn(logMessage);
    }

    /// <summary>
    /// Show a translated HUD banner message to the player.
    /// </summary>
    /// <param name="translationKey">The i18n key from default.json (e.g. "process.start").</param>
    /// <param name="type">HUD message type constant controlling the banner style/color.</param>
    private void ShowMessage(string translationKey, int type)
    {
        string text = _helper.Translation.Get(translationKey);
        Game1.addHUDMessage(new HUDMessage(text, type));
    }
}
