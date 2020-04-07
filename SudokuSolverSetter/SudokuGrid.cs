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

        public SudokuGrid(Cell[][] rows, string difficulty)
        {
            Rows = rows;
            Difficulty = difficulty;
        }

        public Cell[][] Rows { get; set; }
        public string Difficulty { get; set; }

    }
}
