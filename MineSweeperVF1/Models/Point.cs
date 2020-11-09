using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MineSweeperVF1.Models
{
    public class Point
    {
        public Point() { }

        public Point(int y, int x) { this.X = x; this.Y = y; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
