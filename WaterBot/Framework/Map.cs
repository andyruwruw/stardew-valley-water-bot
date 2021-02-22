using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System;
using System.Linq;

namespace WaterBot.Framework
{
    public delegate void console(string message);

    /// <summary>
    /// Contains map data and relevant path finding / grouping algorithms.
    /// </summary>
    class Map
    {
        public List<List<Tile>> map;

        public List<Tile> waterableTiles;

        public List<Group> groupings;

        public int width;

        public int height;

        public Map() { }

        /// <summary>
        /// Converts coords to tile.
        /// </summary>
        /// 
        /// <param name="x">X of tile.</param>
        /// <param name="y">Y of tile.</param>
        /// <param name="width">Number of tiles horizontally.</param>
        /// <param name="height">Number of tiles vertically.</param>
        public static Point convertCoords(int x, int y, int width, int height)
        {
            return new Point(y, x);
        }

        /// <summary>
        /// Whether tile is passable.
        /// </summary>
        /// 
        /// <param name="x">X of tile.</param>
        /// <param name="y">Y of tile.</param>
        public static bool tileIsPassable(int x, int y)
        {
            return Game1.getFarm().isCollidingPosition(new Rectangle(y * 64 + 1, x * 64 + 1, 62, 62), Game1.viewport, isFarmer: true, -1, glider: false, Game1.player);
        }

        /// <summary>
        /// Whether tile can refill watering can.
        /// </summary>
        /// 
        /// <param name="x">X of tile.</param>
        /// <param name="y">Y of tile.</param>
        public static bool tileIsRefillable(int x, int y)
        {
            return Game1.getFarm().CanRefillWateringCanOnTile(y, x);
        }

        /// <summary>
        /// Whether tile needs watering.
        /// </summary>
        /// 
        /// <param name="x">X of tile.</param>
        /// <param name="y">Y of tile.</param>
        public static bool tileNeedsWatering(int x, int y)
        {
            Vector2 index = new Vector2(x, y);

            if (Game1.getFarm().isTileHoeDirt(index) && ((Game1.getFarm().terrainFeatures[index] as HoeDirt).state.Value) == 0 && ((Game1.getFarm().terrainFeatures[index] as HoeDirt).crop != null))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Loads farm data into array.
        /// </summary>
        /// 
        /// <param name="location">Farm location instance.</param>
        public void loadMap()
        {
            this.height = Game1.getFarm().map.Layers[0].LayerHeight;
            this.width = Game1.getFarm().map.Layers[0].LayerWidth;

            List<List<Tile>> map = new List<List<Tile>>();
            List<Tile> waterableTiles = new List<Tile>();

            for (int y = 0; y < this.height; y++)
            {
                List<Tile> row = new List<Tile>();

                for (int x = 0; x < this.width; x++)
                {
                    Point converted = Map.convertCoords(x, y, this.width, this.height);
                    bool needsWatering = Map.tileNeedsWatering(converted.Y, converted.X);

                    Tile tile = new Tile(
                        x,
                        y,
                        Map.tileIsPassable(converted.X, converted.Y),
                        Map.tileIsRefillable(converted.X, converted.Y),
                        needsWatering
                    );

                    if (needsWatering)
                    {
                        waterableTiles.Add(tile);
                    }
                    row.Add(tile);
                }

                map.Add(row);
            }

            this.map = map;
            this.waterableTiles = waterableTiles;
        }

        /// <summary>
        /// Returns a list of adjacent tiles.
        /// </summary>
        /// 
        /// <param name="tile">Tile to find neighbors of.</param>
        public List<Tile> getNeighbors(Tile tile, bool checkBlock = false, bool checkWaterable = false)
        {
            List<Tile> list = new List<Tile>();

            if (tile.x > 0)
            {
                Tile neighbor = this.map[tile.y][tile.x - 1];
                if ((!checkBlock || !neighbor.block) && (!checkWaterable || neighbor.waterable))
                {
                    list.Add(neighbor);
                }
            }
            if (tile.y > 0)
            {
                Tile neighbor = this.map[tile.y - 1][tile.x];
                if ((!checkBlock || !neighbor.block) && (!checkWaterable || neighbor.waterable))
                {
                    list.Add(neighbor);
                }
            }
            if (tile.x < this.width - 1)
            {
                Tile neighbor = this.map[tile.y][tile.x + 1];
                if ((!checkBlock || !neighbor.block) && (!checkWaterable || neighbor.waterable))
                {
                    list.Add(neighbor);
                }
            }
            if (tile.y < this.height - 1)
            {
                Tile neighbor = this.map[tile.y + 1][tile.x];
                if ((!checkBlock || !neighbor.block) && (!checkWaterable || neighbor.waterable))
                {
                    list.Add(neighbor);
                }
            }

            return list;
        }

        /// <summary>
        /// Displays farm in console.
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        public void displayMap(console console)
        {
            foreach (List<Tile> row in this.map)
            {
                string charRow = "";

                foreach (Tile tile in row)
                {
                    charRow += tile.ToString();
                }
                console(charRow);
            }
        }

        /// <summary>
        /// Displays group in console.
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        public void displayGroup(console console, Group group)
        {
            foreach (List<Tile> row in this.map)
            {
                string charRow = "";

                foreach (Tile tile in row)
                {
                    string character = tile.ToString();

                    switch (character)
                    {
                        case "0":
                        case "O":
                            if (group.Contains(tile))
                            {
                                charRow += character;
                            } else
                            {
                                charRow += "-";
                            }
                            break;
                        default:
                            charRow += character;
                            break;
                    } 
                }
                console(charRow);
            }
        }

        /// <summary>
        /// Find all groupings of waterable tiles
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        public void findGroupings(console console)
        {
            this.groupings = new List<Group>();

            foreach (Tile tile in this.waterableTiles)
            {
                bool grouped = false;

                if (tile.visited)
                {
                    continue;
                }

                int index = this.groupings.Count;

                if (tile.x <= this.width - 1)
                {
                    Tile neighbor = this.map[tile.y][tile.x + 1];
                    if (neighbor.waterable)
                    {
                        grouped = true;
                        Group group = new Group(index);
                        this.populateGroup(group, tile);
                        this.groupings.Add(group);
                    }
                }
                if (tile.y <= this.height - 1 && !grouped)
                {
                    Tile neighbor = this.map[tile.y + 1][tile.x];
                    if (neighbor.waterable)
                    {
                        grouped = true;
                        Group group = new Group(index);
                        this.populateGroup(group, tile);
                        this.groupings.Add(group);
                    }
                }
                if (tile.y <= this.height - 1 && tile.x <= this.width - 1 && !grouped)
                {
                    Tile neighbor = this.map[tile.y + 1][tile.x + 1];
                    if (neighbor.waterable)
                    {
                        grouped = true;
                        Group group = new Group(index);
                        this.populateGroup(group, tile);
                        this.groupings.Add(group);
                    }
                }
                if (!grouped)
                {
                    Group solo = new Group(index);

                    solo.Add(tile);
                    tile.visited = true;

                    this.groupings.Add(solo);
                }
            }
        }

        /// <summary>
        /// Recursively find connected waterable tiles (depth-first search)
        /// </summary>
        /// 
        /// <param name="group">List of tiles so far.</param>
        /// <param name="tile">New tile added.</param>
        public void populateGroup(Group group, Tile tile)
        {
            // Add the tile and mark it as visted
            group.Add(tile);
            tile.visited = true;

            // Check edge cases
            bool leftBorder = tile.x == 0;
            bool rightBorder = tile.x == this.width - 1;
            bool topBorder = tile.y == 0;
            bool bottomBorder = tile.y == this.height - 1;

            // Recursively check neighbors
            if (!leftBorder)
            {
                Tile neighbor = this.map[tile.y][tile.x - 1];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
            if (!leftBorder && !topBorder)
            {
                Tile neighbor = this.map[tile.y - 1][tile.x - 1];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
            if (!leftBorder && !bottomBorder)
            {
                Tile neighbor = this.map[tile.y + 1][tile.x - 1];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
            if (!rightBorder)
            {
                Tile neighbor = this.map[tile.y][tile.x + 1];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
            if (!rightBorder && !topBorder)
            {
                Tile neighbor = this.map[tile.y - 1][tile.x + 1];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
            if (!rightBorder && !bottomBorder)
            {
                Tile neighbor = this.map[tile.y + 1][tile.x + 1];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
            if (!topBorder)
            {
                Tile neighbor = this.map[tile.y - 1][tile.x];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
            if (!bottomBorder)
            {
                Tile neighbor = this.map[tile.y + 1][tile.x];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
        }

        /// <summary>
        /// Uses A* to find cost between different groupings.
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        public int[,] generateCostMatrix(console console)
        {
            int nodes = this.groupings.Count + 1;
            int[,] costMatrix = new int[nodes, nodes];

            // From
            for (int i = 0; i < nodes; i++)
            {
                // To
                for (int j = 0; j < nodes; j++)
                {
                    if (i == j)
                    {
                        costMatrix[i, j] = -1;
                    } else
                    {
                        if (costMatrix[j, i] > 0)
                        {
                            costMatrix[i, j] = costMatrix[j, i];
                        }
                        else if (j == 0 || i == 0)
                        {
                            Point start = Player.getPosition();
                            Point end = this.groupings[i == 0 ? j - 1 : i - 1].Centroid();

                            Tuple<List<Tile>, int> path = this.walkablePathBetweenPoints(console, start, end);

                            costMatrix[i, j] = path.Item2;
                        }
                        else
                        {
                            Point start = this.groupings[i - 1].Centroid();
                            Point end = this.groupings[j - 1].Centroid();

                            Tuple<List<Tile>, int> path = this.walkablePathBetweenPoints(console, start, end);

                            costMatrix[i, j] = path.Item2;
                        }
                    }
                }
            }

            List<int> safeGroups = new List<int>();
            List<int> deleteGroups = new List<int>();

            for (int i = 0; i < nodes; i++)
            {
                if (costMatrix[0, i] == int.MaxValue)
                {
                    deleteGroups.Add(i);
                } else
                {
                    safeGroups.Add(i);
                }
            }

            deleteGroups.Sort();

            for (int i = deleteGroups.Count - 1; i >= 0; i--)
            {
                this.groupings.RemoveAt(deleteGroups[i] - 1);
            }

            int[,] reachableCostMatrix = new int[safeGroups.Count, safeGroups.Count];

            for (int i = 0; i < safeGroups.Count; i++)
            {
                for (int j = 0; j < safeGroups.Count; j++)
                {
                    reachableCostMatrix[i, j] = costMatrix[safeGroups[i], safeGroups[j]];
                    reachableCostMatrix[j, i] = costMatrix[safeGroups[j], safeGroups[i]];
                }
            }

            return reachableCostMatrix;
        }

        /// <summary>
        /// Displays A*'s current progress and path.
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        /// <param name="start">Point to start at.</param>
        /// <param name="end">Point to end at.</param>
        /// <param name="cameFrom">Dictionary of path so far.</param>
        public void displayPath(console console, Point start, Point end, Dictionary<Tile, Tile> cameFrom)
        {
            console("Printing Path");
            foreach (List<Tile> row in this.map)
            {
                string charRow = "";

                foreach (Tile tile in row)
                {
                    if (tile.Equals(start))
                    {
                        charRow += "A";
                    } else if (tile.Equals(end))
                    {
                        charRow += "B";
                    } else
                    {
                        if (cameFrom.Keys.Contains(tile))
                        {
                            if (cameFrom[tile] != null)
                            {
                                charRow += "x";
                            }
                            else
                            {
                                charRow += "n";
                            }
                        } else
                        {
                            string character = tile.ToString();

                            switch (character)
                            {
                                case "0":
                                case "O":
                                    charRow += "-";
                                    break;
                                default:
                                    charRow += character;
                                    break;
                            }
                        }
                    }
                }
                console(charRow);
            }
        }

        /// <summary>
        /// Runs A* on map to find shortest path from start to end.
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        /// <param name="start">Point to start at.</param>
        /// <param name="end">Point to end at.</param>
        public Tuple<List<Tile>, int> walkablePathBetweenPoints(console console, Point start, Point end)
        {
            List<Tile> openSet = new List<Tile>();
            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
            Dictionary<Tile, int> gScore = new Dictionary<Tile, int>();
            Dictionary<Tile, int> fScore = new Dictionary<Tile, int>();

            Tile startTile = this.map[start.Y][start.X];

            openSet.Add(startTile);

            gScore.Add(startTile, 0);
            fScore.Add(startTile, startTile.distanceTo(end));

            while (openSet.Count > 0)
            {
                Tile current = openSet[0];
                int smallest = int.MaxValue;

                foreach (Tile tile in openSet)
                {
                    if (fScore[tile] < smallest)
                    {
                        smallest = fScore[tile];
                        current = tile;
                    }
                }

                if (current.Equals(end))
                {
                    List<Tile> path = new List<Tile>();
                    while (cameFrom.Keys.Contains(current))
                    {
                        current = cameFrom[current];
                        path.Add(current);
                    }
                    return new Tuple<List<Tile>, int>(path, path.Count);
                }

                openSet.Remove(current);

                List<Tile> neighbors = getNeighbors(current, true, false);

                foreach (Tile neighbor in neighbors)
                {
                    if (!gScore.Keys.Contains(neighbor))
                    {
                        gScore.Add(neighbor, int.MaxValue);
                    }

                    int tenativeGScore = gScore[current] + 1;
                    if (tenativeGScore < gScore[neighbor])
                    {
                        if (!cameFrom.Keys.Contains(neighbor))
                        {
                            cameFrom.Add(neighbor, null);
                        }
                        if (!fScore.Keys.Contains(neighbor))
                        {
                            fScore.Add(neighbor, int.MaxValue);
                        }

                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tenativeGScore;
                        fScore[neighbor] = tenativeGScore + neighbor.distanceTo(end);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }
            return new Tuple<List<Tile>, int>(null, int.MaxValue);
        }

        public List<Group> findGroupPath(console console)
        {
            List<Group> path = new List<Group>();

            int[,] costMatrix = this.generateCostMatrix(console);

            int rowLength = costMatrix.GetLength(0);
            int colLength = costMatrix.GetLength(1);

            for (int row = 0; row < rowLength; row++)
            {
                string matrix = "";
                for (int col = 0; col < colLength; col++)
                {
                    matrix += $" {costMatrix[row, col]} ";
                }
                console(matrix);
            }

            int sum = 0;
            int counter = 0;
            int j = 0;
            int i = 0;
            int min = int.MaxValue;

            List<int> visitedRouteList = new List<int>();

            visitedRouteList.Add(0);
            int[] route = new int[costMatrix.Length];

            int ranTimes = 0;

            while (i < costMatrix.GetLength(0) && j < costMatrix.GetLength(1))
            {
                ranTimes += 1;
                if (counter >= costMatrix.GetLength(0) - 1)
                {
                    break;
                }

                if (j != i && !(visitedRouteList.Contains(j)))
                {
                    if (costMatrix[i, j] < min)
                    {
                        min = costMatrix[i, j];
                        route[counter] = j + 1;
                    }
                }
                j++;

                if (j == costMatrix.GetLength(0))
                {
                    sum += min;
                    min = int.MaxValue;
                    visitedRouteList.Add(route[counter] - 1);

                    j = 0;
                    i = route[counter] - 1;
                    counter++;
                }
            }

            i = route[counter - 1] - 1;

            for (j = 0; j < costMatrix.GetLength(0); j++)
            {
                if ((i != j) && costMatrix[i, j] < min)
                {
                    min = costMatrix[i, j];
                    route[counter] = j + 1;
                }
            }
            sum += min;

            console($"Ran: {ranTimes} times");

            foreach (int index in visitedRouteList)
            {
                console($"Visited: {index}");
                if (index != 0)
                {
                    path.Add(this.groupings[index - 1]);
                }
            }

            console($"Minimum Cost is: {sum}");

            return path;
        }
    }
}
