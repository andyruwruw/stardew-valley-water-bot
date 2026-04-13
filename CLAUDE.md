# WaterBot

Automates watering crops in Stardew Valley. Right-click a crop with a watering can → the bot takes over, waters all reachable crops, refills the can as needed, and stops when done or when any button is pressed.

**Author:** andyruwruw | **Version:** 1.4.1 | **UniqueID:** `andyruwruw.WaterBot`
**Nexus:** 8167 | **SMAPI:** 4.0.0+ | **.NET 6.0**

---

## File Map

```
WaterBot/
├── ModEntry.cs                         Entry point — input handling, GMCM config menu
├── ModConfig.cs                        Config class (4 properties)
├── IGenericModConfigMenuApi.cs         Trimmed GMCM interface (3 methods)
├── WaterBot.csproj                     SDK-style, Pathoschild.Stardew.ModBuildConfig 4.1.1
├── manifest.json                       Mod metadata
├── Framework/
│   ├── WateringBot.cs                  State machine — tick-based scheduling, safety guards
│   ├── FarmerMover.cs                  Waypoint-following movement via setMoving() (replaces PathFindController)
│   ├── Pathfinder.cs                   BFS pathfinding through ITileGrid (testable, no game dependency)
│   ├── WateringAnimator.cs             Watering/refill animation encapsulation
│   ├── TileGrid.cs                     Targeted map scanning, tile queries, neighbor lookups
│   ├── CropGrouper.cs                  Adjacency DFS grouping + greedy set cover
│   ├── RoutePlanner.cs                 Manhattan nearest-neighbor TSP + within-group fill paths
│   ├── WaterSourceFinder.cs            BFS for nearest water source
│   ├── Logger.cs                       Static SMAPI IMonitor wrapper
│   └── Models/
│       ├── TileInfo.cs                 Immutable tile data (x, y, flags as properties)
│       ├── CropGroup.cs               Tile group with O(1) Contains, precomputed centroid
│       └── WateringAction.cs          Stand position + target queue with nullable dequeue
└── i18n/
    ├── default.json                    English (12 keys)
    └── de, es, fr, hu, it, ja, ko, pt, ru, tr, zh translations
```

---

## Architecture

### Execution Flow

```
Right-click crop with watering can
  → ModEntry.OnButtonPressed()
    → IsPlayerWateringCrop() validates trigger
      → WateringBot.Start()
        1. TileGrid.Load()              — scan terrainFeatures for crops + neighbors
        2. CropGrouper.GroupBy*()       — cluster adjacent crops (DFS or set cover)
        3. RoutePlanner.OrderGroups()   — greedy nearest-neighbor by Manhattan distance
        4. RoutePlanner.PlanWateringPath() — DFS within first group → WateringAction list
        5. Check water level            — refill via WaterSourceFinder if empty
        6. PathFindController           — walk to first WateringAction
          → OnArrived() callback
            → ProcessCurrentAction()
              - WateringAnimator.AnimateWatering() + tick delay
              - dequeue next target or advance to next action/group
              - repeat until all groups done → end
```

### State Machine (WateringBot)

Driven by `UpdateTicked` — all game state mutation on the main thread.

- **Idle** → `Start()` → **Walking** (PathFindController to first action)
- **Walking** → arrival callback → **Watering** (dequeue targets)
- **Watering** → animate → **WaitingForAnimation** (tick countdown)
- **WaitingForAnimation** → ticks elapsed → **Watering** (next target)
- **Watering** → empty can → **Refilling** (BFS + walk to water)
- **Refilling** → arrival → **WaitingAfterRefill** (refill animation)
- **WaitingAfterRefill** → done → **Walking** (resume or recalculate)
- Any state → button press → **Idle** (Stop)
- Any state → warp/tool change → **Idle** (ForceStop)
- Any state → stamina ≤ 2 → **Idle** (exhausted)

### Key Algorithms

| Algorithm | Class | Method |
|---|---|---|
| Targeted map scan | `TileGrid` | `Load()` — iterates terrainFeatures, not all tiles |
| Iterative DFS grouping | `CropGrouper` | `GroupByAdjacency()` — all 4 directions, explicit stack |
| Greedy set cover | `CropGrouper` | `GroupByMinimalCover()` — pick best standing position per round |
| Nearest-neighbor TSP | `RoutePlanner` | `OrderGroups()` — Manhattan distance, no custom A* |
| Within-group fill | `RoutePlanner` | `PlanWateringPath()` — iterative DFS with standing selection |
| BFS refill search | `WaterSourceFinder` | `FindNearest()` — Queue + HashSet, nullable return |

---

## Configuration

| Option | Type | Default | Effect |
|---|---|---|---|
| `UseSmallGrouping` | bool | false | Use greedy set cover instead of adjacency DFS grouping |
| `RefillOnFinish` | bool | false | Auto-refill watering can after completing all crops |
| `RefillIfLower` | int | 95 | Only refill if water remaining below this percentage (0–100) |
| `RedoPathOnRefill` | bool | false | Recalculate full route after refilling vs resume current path |

---

## SMAPI Integration Points

| Hook | Where | What it does |
|---|---|---|
| `Input.ButtonPressed` | ModEntry | Start (action button on crop) or stop (any button while active) |
| `GameLoop.GameLaunched` | ModEntry | Sets up GMCM config menu |
| `GameLoop.UpdateTicked` | WateringBot | Tick-based state machine driver (main thread) |
| `Player.Warped` | WateringBot | Safety stop on location change |
| `Farmer.setMoving(byte)` | FarmerMover | Simulated input for smooth player movement (replaces PathFindController) |
| `HUDMessage` | WateringBot | Status banners (start, stop, exhausted, no water, done) |

---

## Testing Requirements

- **Always write unit tests** for new or modified logic. Target **90% code coverage** on all algorithm and model classes.
- **Always run tests** before considering any change complete: `dotnet test WaterBot.sln`
- Test project: `WaterBot.Tests/` — xUnit, targets net8.0, references the main project via `InternalsVisibleTo`.
- Use `MockTileGrid` (builds from a `char[,]` grid) to test algorithms without game dependencies.
- Logger is decoupled from SMAPI at the type level (uses `Action<string, int>` delegate) so algorithms can run in tests without loading game assemblies.
- If adding a new algorithm class, make it depend on `ITileGrid` (not `TileGrid`) so it's testable.

---

## Coordinate Convention

All code uses standard **(x = column, y = row)** matching the game's APIs:
- `Point(x, y)`, `Vector2(x, y)`, `Rectangle(x, y, w, h)`
- `TilePoint.X` = column, `TilePoint.Y` = row
- `isCollidingPosition(Rectangle(x*64, y*64, ...))` 
- `CanRefillWateringCanOnTile(x, y)`

No coordinate swapping anywhere.
