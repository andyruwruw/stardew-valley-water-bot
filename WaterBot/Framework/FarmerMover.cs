using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Pathfinding;
using WaterBot.Framework.Models;

namespace WaterBot.Framework;

/// <summary>
/// Moves the farmer along a pre-computed path using <see cref="PathFindController"/>,
/// the game's built-in character movement system. PathFindController manages movement
/// directions and calls MovePosition each frame, avoiding timing issues with the
/// game's input processing pipeline.
/// </summary>
internal sealed class FarmerMover
{
    /// <summary>Max ticks without position change before declaring stuck.</summary>
    private const int StuckThresholdTicks = 120; // ~2 seconds at 60fps

    private Action<Character, GameLocation>? _onArrival;
    private Action? _onPathFailed;
    private Point _lastPosition;
    private int _stuckTicks;
    private bool _active;

    /// <summary>Whether the mover is currently guiding the farmer along a path.</summary>
    public bool IsMoving => _active;

    /// <summary>Whether the arrival callback fired during the current game tick.</summary>
    public bool ArrivedThisTick { get; private set; }

    /// <summary>
    /// Compute a path from the farmer's current position to the target tile and start following it.
    /// Calls <paramref name="onArrival"/> when the farmer reaches the target (or immediately if already there).
    /// Calls <paramref name="onPathFailed"/> if no path exists to the target.
    /// </summary>
    public void StartPath(Point target, GameLocation location,
        Action<Character, GameLocation>? onArrival,
        Action? onPathFailed = null)
    {
        _onArrival = onArrival;
        _onPathFailed = onPathFailed;
        _stuckTicks = 0;
        ArrivedThisTick = false;

        var farmer = Game1.player;
        var start = farmer.TilePoint;

        if (start == target)
        {
            _active = false;
            Arrive();
            return;
        }

        var grid = new LiveTileGrid(location);
        var bfsPath = Pathfinder.FindPath(start, target, grid);

        if (bfsPath == null)
        {
            Logger.Warn($"FarmerMover: no path from ({start.X},{start.Y}) to ({target.X},{target.Y}).");
            _active = false;
            var failCallback = _onPathFailed;
            _onPathFailed = null;
            _onArrival = null;
            failCallback?.Invoke();
            return;
        }

        // Convert Queue<Point> (BFS order) to Stack<Point> for PathFindController.
        // PathFindController.Peek() reads from top, so first waypoint must be on top.
        // new Stack(enumerable) pushes each element, so the last becomes the top.
        // Reversing first ensures the first BFS waypoint ends up on top.
        var stack = new Stack<Point>(bfsPath.Reverse());

        _lastPosition = start;
        _active = true;

        farmer.controller = new PathFindController(
            stack, location, farmer, target)
        {
            endBehaviorFunction = (c, loc) => Arrive()
        };
    }

    /// <summary>
    /// Called each tick from the bot's UpdateTicked handler.
    /// Monitors for stuck conditions — actual movement is handled by PathFindController.
    /// </summary>
    public void Update()
    {
        ArrivedThisTick = false;

        if (!_active)
            return;

        // If the controller was cleared externally (e.g. cutscene), treat as path failed
        if (Game1.player.controller == null)
        {
            Logger.Warn("FarmerMover: controller cleared externally.");
            var failCallback = _onPathFailed;
            _active = false;
            _onPathFailed = null;
            _onArrival = null;
            failCallback?.Invoke();
            return;
        }

        var current = Game1.player.TilePoint;

        // Stuck detection: if position hasn't changed for too long, give up
        if (current == _lastPosition)
        {
            _stuckTicks++;
            if (_stuckTicks >= StuckThresholdTicks)
            {
                Logger.Warn($"FarmerMover: stuck at ({current.X},{current.Y}) for {_stuckTicks} ticks, stopping.");
                var failCallback = _onPathFailed;
                Stop();
                failCallback?.Invoke();
                return;
            }
        }
        else
        {
            _stuckTicks = 0;
            _lastPosition = current;
        }
    }

    /// <summary>Stop movement immediately and clear the controller.</summary>
    public void Stop()
    {
        _active = false;
        _onArrival = null;
        _onPathFailed = null;
        _stuckTicks = 0;
        Game1.player.controller = null;
        Game1.player.Halt();
    }

    private void Arrive()
    {
        ArrivedThisTick = true;
        _active = false;
        var callback = _onArrival;
        _onArrival = null;
        _onPathFailed = null;
        _stuckTicks = 0;

        // Controller is cleared by the game after update() returns true,
        // but clear it here too in case Arrive was called from StartPath (same-tile).
        Game1.player.controller = null;
        Game1.player.Halt();
        callback?.Invoke(Game1.player, Game1.currentLocation);
    }

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
        private readonly Dictionary<Point, TileInfo> _cache = new();

        public int Width { get; }
        public int Height { get; }

        public LiveTileGrid(GameLocation location)
        {
            _location = location;
            Width = location.map.Layers[0].LayerWidth;
            Height = location.map.Layers[0].LayerHeight;
        }

        /// <inheritdoc/>
        public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        /// <inheritdoc/>
        public TileInfo? GetTile(Point p) => _cache.GetValueOrDefault(p);

        /// <summary>
        /// Query the game's collision API for tile passability and cache the result.
        /// </summary>
        public TileInfo GetOrQuery(int x, int y)
        {
            var key = new Point(x, y);
            if (_cache.TryGetValue(key, out var cached))
                return cached;

            bool blocked = _location.isCollidingPosition(
                new Rectangle(x * 64 + 1, y * 64 + 1, 62, 62),
                Game1.viewport, isFarmer: true, -1, glider: false, Game1.player);

            var tile = new TileInfo(x, y, blocked, isWaterSource: false, needsWatering: false);
            _cache[key] = tile;
            return tile;
        }

        /// <inheritdoc/>
        public IEnumerable<TileInfo> GetOrthogonalNeighbors(Point p)
        {
            foreach (var off in OrthOffsets)
            {
                int nx = p.X + off.X, ny = p.Y + off.Y;
                if (IsInBounds(nx, ny))
                    yield return GetOrQuery(nx, ny);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TileInfo> GetAllNeighbors(Point p)
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
