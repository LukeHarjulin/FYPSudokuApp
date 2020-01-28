using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverSetter
{
    public class Grid
    {
        public Grid()
        {

        }

        public Grid(Cell[][] rows, int puzzleID)
        {
            Rows = rows;
            PuzzleID = puzzleID;
        }

        public Cell[][] Rows { get; set; }
        public int PuzzleID { get; set; }
    }
}
