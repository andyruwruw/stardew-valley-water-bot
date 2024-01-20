using System;
using Microsoft.Xna.Framework;

namespace BotFramework.Helpers
{
    /// <summary>
    /// Home of various distance formulas
    /// </summary>
    public class Distance
    {
        /// <summary>
        /// Finds distance between two tiles using <see href="https://en.wikipedia.org/wiki/Taxicab_geometry">manhattan distance</see>.
        /// </summary>
        /// <remarks>
        /// Shortest and least accurate.
        /// </remarks>
        /// 
        /// <param name="tile">First tile</param>
        /// <param name="tile2">Second tile</param>
        /// <returns>Distance between</returns>
        //public static double Manhattan(
        //    Tile tile,
        //    Tile tile2
        //) {
        //    return Distance.Manhattan(
        //        tile.GetTileX(),
        //        tile.GetTileY(),
        //        tile2.GetTileX(),
        //        tile2.GetTileY());
        //}

        /// <summary>
        /// Finds distance between two points using <see href="https://en.wikipedia.org/wiki/Taxicab_geometry">manhattan distance</see>.
        /// </summary>
        /// <remarks>
        /// Shortest and least accurate.
        /// </remarks>
        /// 
        /// <param name="point">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>Distance between</returns>
        public static double Manhattan(
            Point point,
            Point point2
        )
        {
            return Distance.Manhattan(
                point.X,
                point.Y,
                point2.X,
                point2.Y);
        }

        /// <summary>
        /// Finds distance between two coordinates using <see href="https://en.wikipedia.org/wiki/Taxicab_geometry">manhattan distance</see>.
        /// </summary>
        /// <remarks>
        /// Shortest and least accurate.
        /// </remarks>
        /// 
        /// <param name="x">X of first point</param>
        /// <param name="y">Y of first point</param>
        /// <param name="x2">X of second point</param>
        /// <param name="y2">Y of second point</param>
        /// <returns>Distance between</returns>
        public static double Manhattan(
            int x,
            int y,
            int x2,
            int y2
        )
        {
            return Math.Abs(x - x2) + Math.Abs(y - y2);
        }

        /// <summary>
        /// Finds the distance bewteen two tiles using traditional Euclidean distance.
        /// </summary>
        /// <remarks>
        /// Short and somewhat accurate.
        /// </remarks>
        /// 
        /// <param name="tile">First tile</param>
        /// <param name="tile2">Second tile</param>
        /// <returns>Distance between</returns>
        //public static double Euclidean(
        //    Tile tile,
        //    Tile tile2
        //) {
        //    return Distance.Euclidean(
        //        tile.GetTileX(),
        //        tile.GetTileY(),
        //        tile2.GetTileX(),
        //        tile2.GetTileY());
        //}

        /// <summary>
        /// Finds the distance bewteen two points using traditional Euclidean distance.
        /// </summary>
        /// <remarks>
        /// Short and somewhat accurate.
        /// </remarks>
        /// 
        /// <param name="point">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>Distance between</returns>
        public static double Euclidean(
            Point point,
            Point point2
        )
        {
            return Distance.Euclidean(
                point.X,
                point.Y,
                point2.X,
                point2.Y);
        }

        /// <summary>
        /// Finds the distance bewteen two coordinates using traditional Euclidean distance.
        /// </summary>
        /// <remarks>
        /// Short and somewhat accurate.
        /// </remarks>
        /// 
        /// <param name="x">X of first point</param>
        /// <param name="y">Y of first point</param>
        /// <param name="x2">X of second point</param>
        /// <param name="y2">Y of second point</param>
        /// <returns>Distance between</returns>
        public static double Euclidean(
            int x,
            int y,
            int x2,
            int y2
        )
        {
            return Math.Sqrt(Math.Pow(x2 - x, 2) + Math.Pow(y2 - y, 2));
        }

        /// <summary>
        /// Finds the distance between two tiles using A* pathfinding.
        /// </summary>
        /// <remarks>
        /// Longest but most accurate.
        /// </remarks>
        /// 
        /// <param name="tile"></param>
        /// <param name="tile2"></param>
        /// <returns></returns>
        //public static double AStar(
        //    Tile tile,
        //    Tile tile2
        //)
        //{
        //    return 0.0;
        //}
    }
}
