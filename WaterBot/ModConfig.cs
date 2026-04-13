namespace WaterBot;

/// <summary>
/// Configuration options for the WaterBot mod.
/// </summary>
internal sealed class ModConfig
{
    /// <summary>Whether to use the minimal cover grouping algorithm instead of adjacency grouping.</summary>
    public bool UseSmallGrouping { get; set; } = false;

    /// <summary>Whether to automatically refill the watering can after finishing all crops.</summary>
    public bool RefillOnFinish { get; set; } = false;

    /// <summary>Only refill if remaining water percentage is below this threshold (0–100).</summary>
    public int RefillIfLower { get; set; } = 95;

    /// <summary>Whether to recalculate the full route after refilling instead of resuming.</summary>
    public bool RedoPathOnRefill { get; set; } = false;

    /// <summary>
    /// Reset all properties to their default values in place.
    /// Used by the GMCM reset callback to avoid replacing the object reference,
    /// which would desync from other classes holding a reference to this instance.
    /// </summary>
    public void ResetToDefaults()
    {
        UseSmallGrouping = false;
        RefillOnFinish = false;
        RefillIfLower = 95;
        RedoPathOnRefill = false;
    }
}
