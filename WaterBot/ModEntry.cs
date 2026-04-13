using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using WaterBot.Framework;

namespace WaterBot;

/// <summary>The mod entry point. Wires SMAPI events and config menu.</summary>
public class ModEntry : Mod
{
    private ModConfig _config = null!;
    private WateringBot _bot = null!;

    public override void Entry(IModHelper helper)
    {
        _config = helper.ReadConfig<ModConfig>();
        Logger.SetLogAction((msg, level) => Monitor.Log(msg, (LogLevel)level));

        _bot = new WateringBot(helper, _config);

        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.GameLoop.GameLaunched += (_, _) => SetUpConfigMenu();
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        if (_bot.IsActive)
        {
            _bot.Stop();
            return;
        }

        if (e.Button.IsActionButton() && IsPlayerWateringCrop())
        {
            Logger.Debug("Trigger: player right-clicked crop with watering can.");
            _bot.Start();
        }
    }

    /// <summary>
    /// Check whether the player is holding a watering can and the cursor is over
    /// a tile with a living crop that needs watering. No reflection needed — just
    /// check the tile under the tool location directly.
    /// </summary>
    private static bool IsPlayerWateringCrop()
    {
        var player = Game1.player;
        if (player?.CurrentTool is not WateringCan)
            return false;

        // Get the tile the player is targeting
        var mousePos = Utility.PointToVector2(Game1.getMousePosition())
            + new Vector2(Game1.viewport.X, Game1.viewport.Y);
        var toolLocation = player.GetToolLocation(mousePos);
        int tileX = (int)(toolLocation.X / 64);
        int tileY = (int)(toolLocation.Y / 64);

        var tileVec = new Vector2(tileX, tileY);

        if (Game1.currentLocation?.terrainFeatures.TryGetValue(tileVec, out var feature) != true)
            return false;

        if (feature is not HoeDirt { crop: Crop crop } || crop.dead.Value)
            return false;

        // Needs watering: still growing, or fully grown with days remaining, or regrows
        return (crop.fullyGrown.Value && crop.dayOfCurrentPhase.Value > 0)
            || (crop.currentPhase.Value < crop.phaseDays.Count - 1)
            || crop.RegrowsAfterHarvest();
    }

    private void SetUpConfigMenu()
    {
        var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (api is null)
            return;

        api.Register(
            mod: ModManifest,
            reset: _config.ResetToDefaults,
            save: () => Helper.WriteConfig(_config)
        );

        api.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.use_small_grouping.name"),
            tooltip: () => Helper.Translation.Get("config.use_small_grouping.desc"),
            getValue: () => _config.UseSmallGrouping,
            setValue: value => _config.UseSmallGrouping = value
        );

        api.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.refill_on_finish.name"),
            tooltip: () => Helper.Translation.Get("config.refill_on_finish.desc"),
            getValue: () => _config.RefillOnFinish,
            setValue: value => _config.RefillOnFinish = value
        );

        api.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.refill_if_lower.name"),
            tooltip: () => Helper.Translation.Get("config.refill_if_lower.desc"),
            getValue: () => _config.RefillIfLower,
            setValue: value => _config.RefillIfLower = value,
            min: 0,
            max: 100
        );

        api.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.redo_path_on_refill.name"),
            tooltip: () => Helper.Translation.Get("config.redo_path_on_refill.desc"),
            getValue: () => _config.RedoPathOnRefill,
            setValue: value => _config.RedoPathOnRefill = value
        );
    }
}
