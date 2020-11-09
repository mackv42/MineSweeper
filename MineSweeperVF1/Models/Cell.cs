using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MineSweeperVF1.Models
{
    public class Cell
    {
        public Cell()
        {
            this.Hidden = true;
        }
        public int Value { get; set; }
        public bool Hidden { get; set; }
        public bool Flagged { get; set; }
        public bool isBomb { get; set; }
    }
}
