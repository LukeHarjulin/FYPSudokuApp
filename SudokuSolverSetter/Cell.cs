using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverSetter
{
    public class Cell
    {
        public Cell()
        {

        }
        public Cell(int num, int xLocation, int yLocation, int blockLoc, List<int> candidates, bool readOnly)
        {
            Num = num;
            XLocation = xLocation;
            YLocation = yLocation;
            BlockLoc = blockLoc;
            Candidates = candidates;
            ReadOnly = readOnly;
        }
        public int Num { get; set; }
        public int XLocation { get; set; }
        public int YLocation { get; set; }
        public int BlockLoc { get; set; }
        public List<int> Candidates { get; set; }
        public bool ReadOnly { get; set; }

    }
}
