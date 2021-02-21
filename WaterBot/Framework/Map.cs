using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace WaterBot.Framework
{
    public delegate void console(string message);

    class Map
    {
        public List<List<Tile>> map;

        public List<Tile> waterableTiles;

        public List<List<Tile>> groupings;

        public int width;

        public int height;

        public Map() {}

        /// <summary>Loads farm data into array.</summary>
        /// <param name="location">Farm location instance.</param>
        public void loadMap(GameLocation location)
        {
            this.height = location.map.Layers[0].LayerHeight;
            this.width = location.map.Layers[0].LayerWidth;

            List<List<Tile>> map = new List<List<Tile>>();
            List<Tile> waterableTiles = new List<Tile>();

            Game1.addHUDMessage(new HUDMessage($"{this.height} {this.width}", 1));

            for (int y = 0; y < this.height; y++)
            {
                List<Tile> row = new List<Tile>();

                for (int x = 0; x < this.width; x++)
                {
                    Point converted = this.convertCoords(x, y, this.width, this.height);
                    bool needsWatering = this.tileNeedsWatering(location, converted.Y, converted.X);

                    Tile tile = new Tile(
                        x,
                        y,
                        location.isCollidingPosition(new Rectangle(converted.Y * 64 + 1, converted.X * 64 + 1, 62, 62), Game1.viewport, isFarmer: true, -1, glider: false, Game1.player),
                        location.CanRefillWateringCanOnTile(converted.Y, converted.X),
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

        public Point convertCoords(int x, int y, int width, int height)
        {
            return new Point(y, x);
        }

        /// <summary>Displays farm in console.</summary>
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

        /// <summary>Whether tile needs watering.</summary>
        /// <param name="location">Farm location instance.</param>
        /// <param name="x">X of tile.</param>
        /// <param name="y">Y of tile.</param>
        public bool tileNeedsWatering(GameLocation location, int x, int y)
        {
            Vector2 index = new Vector2(x, y);

            if (location.isTileHoeDirt(index) && ((location.terrainFeatures[index] as HoeDirt).state.Value) == 0 && ((location.terrainFeatures[index] as HoeDirt).crop != null)) {
                return true;
            }
            return false;
        }

        /// <summary>Find all groupings of waterable tiles</summary>
        public void findGroupings(console console)
        {
            this.groupings = new List<List<Tile>>();

            console($"{this.map.Count} {this.map[0].Count}");

            foreach (Tile tile in this.waterableTiles)
            {
                bool grouped = false;

                if (tile.visited)
                {
                    continue;
                }

                if (tile.x > 0)
                {
                    Tile neighbor = this.map[tile.y][tile.x - 1];
                    if (neighbor.waterable)
                    {
                        grouped = true;
                        List<Tile> group = new List<Tile>();
                        this.populateGroup(group, tile);
                        this.groupings.Add(group);
                    }
                }
                if (tile.y > 0 && !grouped)
                {
                    Tile neighbor = this.map[tile.y - 1][tile.x];
                    if (neighbor.waterable)
                    {
                        grouped = true;
                        List<Tile> group = new List<Tile>();
                        this.populateGroup(group, tile);
                        this.groupings.Add(group);
                    }
                }
                if (tile.y > 0 && tile.x > 0 && !grouped)
                {
                    Tile neighbor = this.map[tile.y - 1][tile.x - 1];
                    if (neighbor.waterable)
                    {
                        grouped = true;
                        List<Tile> group = new List<Tile>();
                        this.populateGroup(group, tile);
                        this.groupings.Add(group);
                    }
                } else
                {
                    List<Tile> solo = new List<Tile>();

                    solo.Add(tile);
                    tile.visited = true;

                    this.groupings.Add(solo);
                }
            }
        }

        /// <summary>Recursively find connected waterable tiles (depth-first search)</summary>
        /// <param name="group">List of tiles so far.</param>
        /// <param name="tile">New tile added.</param>
        public void populateGroup(List<Tile> group, Tile tile)
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
    }
}
