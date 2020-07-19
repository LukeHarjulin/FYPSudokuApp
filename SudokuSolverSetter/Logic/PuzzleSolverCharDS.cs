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
            int blockNumber = row / 3 * 3 + col / 3 + 1;
            int[] indexes = g_Gen.BlockIndexGetter(blockNumber);
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
        /// basis pseudocode:
        /// FUNCTION
        ///     PASS IN:
        ///         VARIABLE G = 2-d array of characters representing cell values in puzzle grid
        ///     
        ///     FOR each cell in puzzle grid G
        ///         IF cell value is equal to '0'
        ///             Add cell to a list of empty cells, K
        ///         ENDIF
        ///     ENDFOR
        ///     call: Backtracker 
        ///         (PASS OUT: puzzle grid G, empty cells K, starting index of empty cells 0)
        /// ENDFUNCTION
        /// </summary>
        /// <param name="grid">Sudoku grid that is passed into and mutated in the method</param>
        /// <param name="variator">Variator changes how the solver functions. 
        /// '0' = normal ordered candidate list, '1' = reversed candidate list, '2' = shuffled candidate list, '3' = add to solve path, '4' = from generator method, requires order list of cells by cell index, not candidate count
        /// </param>
        /// <returns>Returns true if solver completes puzzle with all values in the correct place. 
        /// Returns false if solver finds contradiction within a cell, i.e. no candidate numbers in a cell</returns>
        public bool CompileBacktracker(char[][] grid, byte variator)
        {
            List<int[]> emptyCellsPositions = new List<int[]>();
            List<List<char>> emptyCellsCands = new List<List<char>>();
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
                            emptyCellsPositions.Add(new int[2] { i, j });
                            emptyCellsCands.Add(cands);
                        }
                        else
                        {
                            for (int k = 0; k < emptyCellsCands.Count; k++)
                            {
                                if (cands.Count < emptyCellsCands[k].Count)
                                {
                                    emptyCellsPositions.Insert(k, new int[2] { i, j });
                                    emptyCellsCands.Insert(k, cands);
                                    break;
                                }
                                else if (k == emptyCellsCands.Count - 1)
                                {
                                    emptyCellsPositions.Add(new int[2] { i, j });
                                    emptyCellsCands.Add(cands);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return Backtracker(grid, 0, emptyCellsPositions, emptyCellsCands);
        }
        /// <summary>
        /// Recursive backtracker method
        /// Pseudocode:
        /// BOOLEAN FUNCTION Backtracker
        ///     PASS IN:
        ///         VARIABLE G = 2-d array of characters representing cell values in puzzle grid
        ///         VARIABLE K = list of empty cells
        ///         VARIABLE i = current index in list of empty cells K
        ///         
        ///     IF no more empty cells exist THEN
        ///         IF puzzle is solved THEN
        ///             RETURN true
        ///         ELSE 
        ///             RETURN false
        ///         ENDIF
        ///     ENDIF
        ///     VARIABLE C = list of all candidates {1,..,9}
        ///     FOR each cell n within the same row/column/block as K[i] DO
        ///         IF the value of cell n exists in list of candidates C THEN
        ///             REMOVE value of n number from C
        ///         ENDIF
        ///     ENDFOR
        ///     FOR each candidate x in list of candidates C DO
        ///         ASSIGN value of empty cell K[i] with candidate x
        ///         IF Backtracker(PASS OUT: G, K, i+1) returns true THEN
        ///             RETURN true
        ///         ENDIF                  
        ///     ENDFOR
        ///     ASSIGN empty cell K[i] with candidate '0'
        ///     RETURN false
        /// ENDFUNCTION
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="k">current index of empty cells list each successive recursion is given k + 1</param>
        /// <param name="emptyCellsPositions">empty cells coordinates list that corresponds to list below</param>
        /// <param name="emptyCellsCands">empty cells candidate lists</param>
        /// <returns></returns>
        public bool Backtracker(char[][] grid, int k, List<int[]> emptyCellsPositions, List<List<char>> emptyCellsCands)
        {
            if (k >= emptyCellsPositions.Count)
            {
                return g_Gen.CheckIfSolved_array(grid);
            }
            List<char> oldCands = new List<char>() { };
            oldCands.AddRange(emptyCellsCands[k]);
            RemoveCands(grid, emptyCellsPositions[k][0], emptyCellsPositions[k][1], emptyCellsCands[k]);
            for (int c = 0; c < emptyCellsCands[k].Count; c++)
            {
                grid[emptyCellsPositions[k][0]][emptyCellsPositions[k][1]] = emptyCellsCands[k][c];
                solvePath.Add(emptyCellsPositions[k][0].ToString() + emptyCellsPositions[k][1].ToString() + emptyCellsCands[k][c]);//Add to solve path
                if (Backtracker(grid, k + 1, emptyCellsPositions, emptyCellsCands))
                    return true;
            }
            emptyCellsCands[k] = oldCands;
            grid[emptyCellsPositions[k][0]][emptyCellsPositions[k][1]] = '0';
            solvePath.Add(emptyCellsPositions[k][0].ToString() + emptyCellsPositions[k][1].ToString() + "0");//Add to solve path
            return false;
        }
    }
}
