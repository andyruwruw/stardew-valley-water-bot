using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBot.Framework
{
    class Tile
    {
        public bool block;

        public bool water;

        public bool waterable;

        public int x;

        public int y;

        public int f;

        public int g;

        public int h;

        public bool visited;

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;

            this.block = false;
            this.water = false;
            this.waterable = false;

            this.visited = false;
        }

        public Tile(int x, int y, bool block)
        {
            this.x = x;
            this.y = y;

            this.block = block;
            this.water = false;
            this.waterable = false;

            this.visited = false;
        }

        public Tile(int x, int y, bool block, bool water)
        {
            this.x = x;
            this.y = y;

            this.block = block;
            this.water = water;
            this.waterable = false;

            this.visited = false;
        }

        public Tile(int x, int y, bool block, bool water, bool waterable)
        {
            this.x = x;
            this.y = y;

            this.block = block;
            this.water = water;
            this.waterable = waterable;

            this.visited = false;
        }

        public int distanceTo(Tile other)
        {
            return Math.Abs(other.x - this.x) + Math.Abs(other.y - this.y);
        }

        public override string ToString()
        {
            if (this.water)
            {
                return "~";
            }
            if (this.waterable && this.block)
            {
                return "0";
            }
            if (this.waterable)
            {
                return "O";
            }
            if (this.block)
            {
                return "#";
            }
            return " ";
        }

        public void reset()
        {
            this.visited = false;
        }
    }
}
