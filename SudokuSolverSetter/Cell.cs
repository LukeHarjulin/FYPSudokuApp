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
        public Cell(char num, int xLocation, int yLocation, int blockLoc, List<char> candidates, bool readOnly, List<List<Cell>> neighbourCells)
        {
            Num = num;
            XLocation = xLocation;
            YLocation = yLocation;
            BlockLoc = blockLoc;
            Candidates = candidates;
            ReadOnly = readOnly;
            NeighbourCells = neighbourCells;
        }
        public char Num { get; set; }
        public int XLocation { get; set; }
        public int YLocation { get; set; }
        public int BlockLoc { get; set; }
        public List<char> Candidates { get; set; }
        public bool ReadOnly { get; set; }
        public List<List<Cell>> NeighbourCells { get; set; }

    }
}
