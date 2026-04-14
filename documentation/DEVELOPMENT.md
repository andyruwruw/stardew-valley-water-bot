# Development

## Building

```bash
dotnet build WaterBot.sln
```

## Running Tests

The project includes a unit test suite covering all algorithms (grouping, routing, pathfinding) and model classes. Tests use xUnit and run without any game dependency.

```bash
# Run all tests
dotnet test WaterBot.sln

# Run with verbose output
dotnet test WaterBot.sln --verbosity normal

# Run a specific test class
dotnet test WaterBot.sln --filter "FullyQualifiedName~CropGrouperTests"

# Run a specific test
dotnet test WaterBot.sln --filter "FullyQualifiedName~Adjacency_LShape_OneGroup"
```

Tests cover:
- **Models** — TileInfo equality/hashing, CropGroup centroid/contains, WateringAction queue behavior
- **CropGrouper** — adjacency DFS (connected components, L-shapes, diagonals), greedy set cover
- **RoutePlanner** — nearest-neighbor group ordering, within-group fill path coverage
- **WaterSourceFinder** — BFS water search, blocked water handling, null when unreachable

## Smoketest

Functional smoke test checklist (manual — requires running the game):

- Start game, load save with crops
- Right-click crop with watering can → bot starts, waters all crops
- Press any button during watering → bot stops immediately
- Bot auto-refills at water source when can is empty
- Bot stops when stamina is low
- Warp during bot operation → bot stops safely
- Switch tool during bot operation → bot stops safely
- Test with `UseSmallGrouping = true`
- Test with `RefillOnFinish = true`
- Test with `RedoPathOnRefill = true`
- Test in greenhouse (non-farm location)