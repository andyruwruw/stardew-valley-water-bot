using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBot.Framework
{
    /// <summary>
    /// A group of waterable adjacent tiles.
    /// </summary>
    class Group
    {
        private List<Tile> list;

        public int index;

        public Group(int index)
        {
            this.index = index;
            this.list = new List<Tile>();
        }

        /// <summary>
        /// Add a tile to the group
        /// </summary>
        public void Add(Tile tile)
        {
            this.list.Add(tile);
        }

        /// <summary>
        /// Number of tiles in group
        /// </summary>
        public int Count()
        {
            return this.list.Count;
        }

        /// <summary>
        /// A group of waterable adjacent tiles.
        /// </summary>
        /// 
        /// <param name="tile"></param>
        public bool Contains(Tile tile)
        {
            return this.list.Contains(tile);
        }

        /// <summary>
        /// Finds the tile closes to the centroid of all the points.
        /// </summary>
        public Point Centroid()
        {
            int sumOfX = 0;
            int sumOfY = 0;

            foreach (Tile tile in this.list)
            {
                sumOfX += tile.x;
                sumOfY += tile.y;
            }

            Point centroid = new Point(sumOfX / this.list.Count, sumOfY / this.list.Count);

            int shortest = int.MaxValue;
            Tile centerWalkableTile = null;

            foreach (Tile tile in this.list)
            {
                if (!tile.block && tile.distanceTo(centroid) < shortest)
                {
                    shortest = tile.distanceTo(centroid);
                    centerWalkableTile = tile;
                }
            }

            return centerWalkableTile.getPoint();
        }

        public Tuple<Tile, double> findClosestTile(int x, int y)
        {
            Tile shortestTile = null;
            double shortest = float.MaxValue;

            foreach (Tile tile in this.list)
            {
                double distance = Math.Sqrt(Math.Pow(x - tile.x, 2) + Math.Pow(y - tile.y, 2));

                if (distance < shortest || shortestTile == null)
                {
                    shortest = distance;
                    shortestTile = tile;
                }
            }

            return Tuple.Create(shortestTile, shortest);
        }
    }
}
