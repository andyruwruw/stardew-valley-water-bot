using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

namespace WaterBot.Framework;

/// <summary>
/// Encapsulates the watering can animation and tool-use logic.
/// Extracted from the old WaterBotController.water() method.
/// </summary>
internal static class WateringAnimator
{
    /// <summary>
    /// Animate the player watering a target tile. Faces the target, plays the animation
    /// and sound, and triggers the tool update so the crop actually gets watered.
    /// </summary>
    /// <returns>The animation duration in milliseconds.</returns>
    public static int AnimateWatering(Farmer farmer, Point targetTile)
    {
        if (farmer.CurrentTool is not WateringCan wateringCan)
        {
            Logger.Warn("WateringAnimator: player is not holding a watering can.");
            return 0;
        }

        // Face the target tile
        farmer.FacingDirection = GetFacingDirection(farmer.TilePoint, targetTile);

        if (farmer.isEmoteAnimating)
            farmer.EndEmoteAnimation();

        // Set up tool-use state
        farmer.FarmerSprite.SetOwner(farmer);
        farmer.CanMove = false;
        farmer.UsingTool = true;
        farmer.canReleaseTool = true;
        farmer.Halt();
        farmer.CurrentTool.Update(farmer.FacingDirection, 0, farmer);
        farmer.stopJittering();
        farmer.canReleaseTool = false;

        // Play watering sound
        if (wateringCan.WaterLeft > 0 && farmer.ShouldHandleAnimationSound())
            farmer.currentLocation.localSound("wateringCan");

        // Set the click target so the tool knows which tile to affect
        farmer.lastClick = new Vector2(targetTile.X * Game1.tileSize, targetTile.Y * Game1.tileSize);

        // Trigger the sprite animation for the facing direction
        int duration = GetAnimationDuration(farmer);
        int animationFrame = farmer.FacingDirection switch
        {
            0 => 180, // up
            1 => 172, // right
            2 => 164, // down
            3 => 188, // left
            _ => 164
        };

        ((FarmerSprite)farmer.Sprite).animateOnce(animationFrame, duration / 3f, 3);
        return duration;
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

    /// <summary>
    /// Animation duration in milliseconds. Doubled when the player has low stamina
    /// (matching vanilla behavior where exhausted animations are slower).
    /// </summary>
    private static int GetAnimationDuration(Farmer farmer)
    {
        int multiplier = farmer.Stamina > 0f ? 1 : 2;
        return 125 * multiplier * 3; // 375ms normal, 750ms exhausted
    }
}
