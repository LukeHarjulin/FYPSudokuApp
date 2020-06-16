using System.Collections.Generic;

namespace SudokuSolverSetter
{
    /// <summary>
    /// This class handles Sudoku solving using char[][] 
    /// </summary>
    public class PuzzleSolverCharDS
    {
        private readonly PuzzleGenerator g_Gen = new PuzzleGenerator();
        public List<string> solvePath = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="method">method '1' is human-strategy solver. '2' is Backtracking solver. '3' is Backtracking solver using char[][]</param>
        /// <returns></returns>
        public bool Solvers(char[][] grid, char method)
        {
            solvePath = new List<string>();
            bool changeMade = false;
            /*
             *  This do...while is necessary for repeating these methods for solving until no changes are made (which it assumes that the puzzle is complete or it could not complete it)
             *  The if and elses are to make the process of solving faster, 
                as it ensures that it tries the easiest less computationally heavy methods first before the more complex methods.
            */
            if (method == '1')
            {
                do
                {
                    ///Add functions/strategies to solve a puzzle using char[][]
                    changeMade = false;
                } while (changeMade);
            }
            else if (method == '2')
            {
                CompileBacktracker(grid, 0);
            }
            return g_Gen.CheckIfSolved_array(grid);
        }
        /// <summary>
        /// Removes all values from the current cell's candidate list that are also found within neighbouring cells
        /// I.e. Cells that are in the same groups as the cell in question.
        /// A group is a block/row/column
        /// </summary>
        /// <param name="grid">Sudoku grid of char[][] type that is passed into and mutated in the method</param>
        /// <param name="row">Current row number being examined in this instance of the method</param>
        /// <param name="col">Current column number being examined in this instance of the method</param>
        /// <param name="candidates">list of candidates that the current cell can contain</param>
        /// <returns>false if an error occurs and a candidate list contains 0 values | true if it is ok</returns>
        public bool RemoveCands(char[][] grid, int row, int col, List<char> candidates)
        {
            int blockNumber = 0;
            int[] indexes = new int[2];
            blockNumber = (row / 3) * 3 + (col / 3) + 1;
            indexes = g_Gen.BlockIndexGetter(blockNumber);
            for (int i = 0; i < 9; i++)
            {
                if (candidates.Contains(grid[row][i]) && i != col)
                {
                    candidates.Remove(grid[row][i]);
                }
                if (candidates.Contains(grid[i][col]) && i != row)
                {
                    candidates.Remove(grid[i][col]);
                }

                if (candidates.Contains(grid[indexes[0]][indexes[1]]) && indexes[0] != row && indexes[1] != col)
                {
                    candidates.Remove(grid[indexes[0]][indexes[1]]);
                }
                indexes[1]++;
                if (indexes[1] == 3 || indexes[1] == 6 || indexes[1] == 9)
                {
                    indexes[0]++;
                    indexes[1] -= 3;

                }
            }
            if (candidates.Count == 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// This Backtracking solver uses the Backtracker method to perform heavy recursion to reach a solution, iterating through a list of empty cells, order by their initial candidate count (smallest-highest), attempting to place each possible number in each cell till the valid solution is found.
        /// It initially starts from the cell with the least possible numbers to minimise worst-case-scenario and, with each recursive instance, looks at the next cell in the list.
        /// Once the last cell in the row is reached, the column counter is incremented and the row counter is set back to 0. 
        /// </summary>
        /// <param name="grid">Sudoku grid that is passed into and mutated in the method</param>
        /// <param name="variator">Variator changes how the solver functions. 
        /// '0' = normal ordered candidate list, '1' = reversed candidate list, '2' = shuffled candidate list, '3' = add to solve path, '4' = from generator method, requires order list of cells by cell index, not candidate count
        /// </param>
        /// <returns>Returns true if solver completes puzzle with all values in the correct place. 
        /// Returns false if solver finds contradiction within a cell, i.e. no candidate numbers in a cell</returns>
        public bool CompileBacktracker(char[][] grid, byte variator)
        {
            List<int[]> emptyCells = new List<int[]>();
            List<List<char>> emptyCellsOrderByCands = new List<List<char>>();
            bool first = true;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i][j] == '0')
                    {
                        List<char> cands = null;
                        if (variator == 0)
                            cands = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                        else if (variator == 1)
                            cands = new List<char> { '9', '8', '7', '6', '5', '4', '3', '2', '1' };
                        else if (variator == 2)
                            cands = g_Gen.Shuffler(new List<char> { '9', '8', '7', '6', '5', '4', '3', '2', '1' });
                        RemoveCands(grid, i, j, cands);
                        if (first)
                        {
                            first = false;
                            emptyCells.Add(new int[2] { i, j });
                            emptyCellsOrderByCands.Add(cands);
                        }
                        else
                        {
                            for (int k = 0; k < emptyCellsOrderByCands.Count; k++)
                            {
                                if (cands.Count < emptyCellsOrderByCands[k].Count)
                                {
                                    emptyCells.Insert(k, new int[2] { i, j });
                                    emptyCellsOrderByCands.Insert(k, cands);
                                    break;
                                }
                                else if (k == emptyCellsOrderByCands.Count - 1)
                                {
                                    emptyCells.Add(new int[2] { i, j });
                                    emptyCellsOrderByCands.Add(cands);
                                    break;
                                }
                            }
                        }
                        
                    }
                }
            }
            return Backtracker(grid, 0, variator, emptyCells, emptyCellsOrderByCands);
        }
        public bool Backtracker(char[][] grid, int k, byte variator, List<int[]> emptyCells, List<List<char>> emptyCellsOrderByCands)
        {
            if (k >= emptyCells.Count)
            {
                return g_Gen.CheckIfSolved_array(grid);
            }
            List<char> oldCands = new List<char>() { };
            oldCands.AddRange(emptyCellsOrderByCands[k]);
            RemoveCands(grid, emptyCells[k][0], emptyCells[k][1], emptyCellsOrderByCands[k]);
            for (int n = 0; n < emptyCellsOrderByCands[k].Count; n++)
            {
                grid[emptyCells[k][0]][emptyCells[k][1]] = emptyCellsOrderByCands[k][n];
                solvePath.Add(emptyCells[k][0].ToString() + emptyCells[k][1].ToString() + emptyCellsOrderByCands[k][n]);//Add to solve path
                if (Backtracker(grid, k + 1, variator, emptyCells, emptyCellsOrderByCands))
                    return true;
            }
            emptyCellsOrderByCands[k] = oldCands;
            grid[emptyCells[k][0]][emptyCells[k][1]] = '0';
            solvePath.Add(emptyCells[k][0].ToString() + emptyCells[k][1].ToString() + "0");//Add to solve path
            return false;
        }
    }
}
