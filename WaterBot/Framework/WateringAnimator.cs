using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

namespace WaterBot.Framework;

/// <summary>
/// Encapsulates watering can tool logic.
/// Uses the game's real animation system (<see cref="FarmerSprite.animateOnce"/>)
/// with <see cref="Farmer.toolOverrideFunction"/> to target exact tile coordinates
/// while playing the full 4-frame watering animation.
/// </summary>
internal static class WateringAnimator
{
    /// <summary>Animation indices for watering by facing direction (down, right, up, left).</summary>
    private static readonly int[] WateringAnimIndices = { 180, 172, 164, 188 };

    /// <summary>
    /// Start the watering animation targeting a specific tile.
    /// Uses <see cref="Farmer.toolOverrideFunction"/> so the animation's
    /// <see cref="Farmer.useTool"/> callback waters the exact target tile.
    /// The animation completes when <see cref="Farmer.canMoveNow"/> fires
    /// (~675ms later), which sets <see cref="Farmer.UsingTool"/> to false.
    /// </summary>
    public static void AnimateWatering(Farmer farmer, Point targetTile)
    {
        if (farmer.CurrentTool is not WateringCan wateringCan)
        {
            Logger.Warn("WateringAnimator: player is not holding a watering can.");
            return;
        }

        farmer.FacingDirection = GetFacingDirection(farmer.TilePoint, targetTile);
        farmer.Halt();
        farmer.UsingTool = true;
        farmer.CanMove = false;
        wateringCan.Update(farmer.FacingDirection, 0, farmer);

        // Redirect useTool callback to water the exact target tile.
        // The lambda clears itself after firing to avoid leaking into later tool uses.
        int pixelX = targetTile.X * 64;
        int pixelY = targetTile.Y * 64;
        farmer.toolOverrideFunction = who =>
        {
            who.toolOverrideFunction = null;
            wateringCan.DoFunction(who.currentLocation, pixelX, pixelY, 0, who);
        };

        int animIndex = WateringAnimIndices[farmer.FacingDirection];
        farmer.FarmerSprite.animateOnce(animIndex, 125f, 3);
    }

    /// <summary>
    /// Whether the watering animation has finished playing.
    /// <see cref="Farmer.canMoveNow"/> sets <see cref="Farmer.UsingTool"/> to false
    /// at the end of the animation's final frame.
    /// </summary>
    public static bool IsAnimationComplete(Farmer farmer)
    {
        return !farmer.UsingTool;
    }

    /// <summary>
    /// Reset the farmer's tool-use state so they can move freely.
    /// Call this before starting PathFindController movement or on bot stop.
    /// </summary>
    public static void ResetFarmerState(Farmer farmer)
    {
        farmer.toolOverrideFunction = null;
        farmer.UsingTool = false;
        farmer.CanMove = true;
        farmer.FarmerSprite.PauseForSingleAnimation = false;
        farmer.forceCanMove();
    }

    /// <summary>
    /// Get the facing direction from the player's position toward a target tile.
    /// 0=up, 1=right, 2=down, 3=left.
    /// </summary>
    private static int GetFacingDirection(Point from, Point to)
    {
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;

        // Prefer vertical facing when diagonal
        if (dy < 0) return 0; // up
        if (dy > 0) return 2; // down
        if (dx < 0) return 3; // left
        if (dx > 0) return 1; // right
        return 2; // same tile — default down
    }
}
