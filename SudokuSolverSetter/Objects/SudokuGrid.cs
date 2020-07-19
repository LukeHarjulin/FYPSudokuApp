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

        public SudokuGrid(Cell[][] cells, string difficulty)
        {
            Cells = cells;
            Difficulty = difficulty;
        }

        public Cell[][] Cells { get; set; }
        public string Difficulty { get; set; }

    }
}
