using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverSetter
{
    public class SudokuGrid
    {
        public SudokuGrid()
        {

        }

        public SudokuGrid(Cell[][] rows, int puzzleID, char difficulty)
        {
            Rows = rows;
            PuzzleID = puzzleID;
            Difficulty = difficulty;
        }

        public Cell[][] Rows { get; set; }
        public int PuzzleID { get; set; }
        public char Difficulty { get; set; }

    }
}
