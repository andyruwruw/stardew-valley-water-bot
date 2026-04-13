using Microsoft.Xna.Framework;
using StardewValley;

namespace WaterBot.Framework;

/// <summary>
/// Moves the farmer along a pre-computed waypoint path using <see cref="Farmer.setMoving(byte)"/>,
/// which feeds into the Farmer's normal input pipeline (animations, collision, running).
/// Replaces PathFindController to avoid jittery movement caused by per-frame direction clearing.
/// </summary>
internal sealed class FarmerMover
{
    // setMoving() direction codes (Farmer.cs:7014)
    private const byte MoveUp = 1;
    private const byte MoveRight = 2;
    private const byte MoveDown = 4;
    private const byte MoveLeft = 8;
    private const byte ReleaseUp = 33;
    private const byte ReleaseRight = 34;
    private const byte ReleaseDown = 36;
    private const byte ReleaseLeft = 40;
    private const byte StartRunning = 16;
    private const byte HaltCode = 64;

    private Queue<Point>? _waypoints;
    private Action<Character, GameLocation>? _onArrival;
    private byte _lastDirection;

    /// <summary>Whether the mover is currently guiding the farmer along a path.</summary>
    public bool IsMoving => _waypoints is { Count: > 0 };

    /// <summary>
    /// Compute a path from the farmer's current position to the target tile and start following it.
    /// Calls <paramref name="onArrival"/> when the farmer reaches the target (or immediately if already there).
    /// </summary>
    public void StartPath(Point target, GameLocation location, Action<Character, GameLocation>? onArrival)
    {
        _onArrival = onArrival;

        var start = Game1.player.TilePoint;
        if (start == target)
        {
            _waypoints = null;
            Arrive();
            return;
        }

        // Use a lightweight TileGrid for BFS pathfinding
        var grid = new LiveTileGrid(location);
        var path = Pathfinder.FindPath(start, target, grid);

        if (path == null)
        {
            Logger.Warn($"FarmerMover: no path from ({start.X},{start.Y}) to ({target.X},{target.Y}).");
            _waypoints = null;
            Arrive();
            return;
        }

        _waypoints = path;
        _lastDirection = 0;

        // Start running for faster movement
        Game1.player.setMoving(StartRunning);
    }

    /// <summary>
    /// Called each tick from the bot's UpdateTicked handler.
    /// Checks farmer position against the next waypoint and issues movement commands.
    /// </summary>
    public void Update()
    {
        if (_waypoints == null || _waypoints.Count == 0)
            return;

        var farmer = Game1.player;
        var current = farmer.TilePoint;
        var target = _waypoints.Peek();

        // If we've reached the current waypoint, advance
        if (current == target)
        {
            _waypoints.Dequeue();

            if (_waypoints.Count == 0)
            {
                farmer.Halt();
                Arrive();
                return;
            }

            target = _waypoints.Peek();
        }

        // Determine direction toward next waypoint
        int dx = target.X - current.X;
        int dy = target.Y - current.Y;

        // Move one axis at a time (prefer the axis with greater distance)
        byte direction;
        if (Math.Abs(dy) >= Math.Abs(dx))
            direction = dy > 0 ? MoveDown : MoveUp;
        else
            direction = dx > 0 ? MoveRight : MoveLeft;

        // Only change direction if it differs from last frame
        if (direction != _lastDirection)
        {
            if (_lastDirection != 0)
                farmer.setMoving(GetReleaseCode(_lastDirection));

            farmer.setMoving(direction);
            _lastDirection = direction;
        }
    }

    /// <summary>Stop movement immediately and clear the path.</summary>
    public void Stop()
    {
        _waypoints = null;
        _onArrival = null;
        _lastDirection = 0;
        Game1.player.Halt();
    }

    private void Arrive()
    {
        var callback = _onArrival;
        _onArrival = null;
        _lastDirection = 0;

        Game1.player.Halt();
        callback?.Invoke(Game1.player, Game1.currentLocation);
    }

    private static byte GetReleaseCode(byte moveCode) => moveCode switch
    {
        MoveUp => ReleaseUp,
        MoveRight => ReleaseRight,
        MoveDown => ReleaseDown,
        MoveLeft => ReleaseLeft,
        _ => HaltCode
    };

    /// <summary>
    /// Minimal ITileGrid adapter for live game queries during pathfinding.
    /// Only used by FarmerMover.StartPath to feed BFS.
    /// </summary>
    private sealed class LiveTileGrid : ITileGrid
    {
        private static readonly Point[] OrthOffsets = { new(0, -1), new(0, 1), new(-1, 0), new(1, 0) };
        private static readonly Point[] AllOffs =
        {
            new(0, -1), new(0, 1), new(-1, 0), new(1, 0),
            new(-1, -1), new(1, -1), new(-1, 1), new(1, 1)
        };

        private readonly GameLocation _location;
        private readonly Dictionary<Point, Models.TileInfo> _cache = new();

        public int Width { get; }
        public int Height { get; }

        public LiveTileGrid(GameLocation location)
        {
            _location = location;
            Width = location.map.Layers[0].LayerWidth;
            Height = location.map.Layers[0].LayerHeight;
        }

        public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        public Models.TileInfo? GetTile(Point p) => _cache.GetValueOrDefault(p);

        public Models.TileInfo GetOrQuery(int x, int y)
        {
            var key = new Point(x, y);
            if (_cache.TryGetValue(key, out var cached))
                return cached;

            bool blocked = _location.isCollidingPosition(
                new Rectangle(x * 64 + 1, y * 64 + 1, 62, 62),
                Game1.viewport, isFarmer: true, -1, glider: false, Game1.player);

            var tile = new Models.TileInfo(x, y, blocked, isWaterSource: false, needsWatering: false);
            _cache[key] = tile;
            return tile;
        }

        public IEnumerable<Models.TileInfo> GetOrthogonalNeighbors(Point p)
        {
            foreach (var off in OrthOffsets)
            {
                int nx = p.X + off.X, ny = p.Y + off.Y;
                if (IsInBounds(nx, ny))
                    yield return GetOrQuery(nx, ny);
            }
        }

        public IEnumerable<Models.TileInfo> GetAllNeighbors(Point p)
        {
            foreach (var off in AllOffs)
            {
                int nx = p.X + off.X, ny = p.Y + off.Y;
                if (IsInBounds(nx, ny))
                    yield return GetOrQuery(nx, ny);
            }
        }
    }
}
