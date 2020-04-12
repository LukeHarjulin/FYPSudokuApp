using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverSetter
{/// <summary>
/// 
/// </summary>
    public class PuzzleSolverCharVer
    {
        private PuzzleGenerator gen = new PuzzleGenerator();
        public List<string> solvePath = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="method">method '1' is human-strategy solver. '2' is bruteforce solver. '3' is bruteforce solver using char[][]</param>
        /// <returns></returns>
        public bool Solvers(char[][] grid, char method)
        {
            solvePath.Clear();
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
                BruteForceSolve(grid, 0, 0, 0);
            }
            return gen.CheckIfSolved_array(grid);
        }
        public bool RemoveCands(char[][] grid, int row, int col, List<char> candidates)
        {
            int blockNumber = 0;
            int[] indexes = new int[2];
            blockNumber = (row / 3) * 3 + (col / 3) + 1;
            indexes = gen.BlockIndexGetter(blockNumber);
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
            if (candidates.Count == 1)
            {
                grid[row][col] = candidates[0];
                solvePath.Add(row.ToString() + col.ToString() + candidates[0].ToString());//Add to solve path
            }
            return true;
        }
        /// <summary>
        /// This brute force solver uses heavy recursion to reach a solution, iterating through cells attempting to place each possible number in each cell till the valid solution is found.
        /// It initially starts from the top left cell and, with each recursive instance, looks at the next cell over in the current row.
        /// Once the last cell in the row is reached, the column counter is incremented and the row counter is set back to 0. 
        /// For example, if [i,j] is a cell, when looking at cell [0,8], the next cell to be looked at is [1,0].
        /// </summary>
        /// <param name="grid">Sudoku grid that is passed into and mutated in the method</param>
        /// <param name="row">Current row number being examined in this instance of the method</param>
        /// <param name="col">Current column number being examined in this instance of the method</param>
        /// <param name="variator">The value of this changes whether the candidate list is reversed ('1') or not ('0')</param>
        /// <returns>Returns true if solver completes puzzle with all values in the correct place. 
        /// Returns false if solver finds contradiction within a cell, i.e. no candidate numbers in a cell</returns>
        public bool BruteForceSolve(char[][] grid, int row, int col, byte variator)
        {

            if (col == 9 && row == 9)//If somehow the method tries to look at this non-existent cell, this catches the exception
            {
                if (gen.CheckIfSolved_array(grid))
                {
                    return true;
                }
                else
                {
                    grid[--row][--col] = '0';
                    solvePath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
                    return false;
                }
            }

            if (grid[row][col] != '0')
            {
                bool emptyCell = false;
                do//Searches for next empty cell
                {
                    if (++col == 9)
                    {
                        if (++row == 9)
                        {
                            if (gen.CheckIfSolved_array(grid))
                            {
                                return true;
                            }
                            else
                            {
                                grid[--row][--col] = '0';
                                solvePath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
                                return false;
                            }
                        }
                        else
                            col = 0;

                    }
                    if (grid[row][col] == '0')
                        emptyCell = true;
                } while (!emptyCell);
            }
            List<char> candidates = new List<char>(9);
            if (variator == 0)
                candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            else if (variator == 1)
                candidates = new List<char> { '9', '8', '7', '6', '5', '4', '3', '2', '1' };
            else if (variator == 2)
                candidates = gen.Shuffler(new List<char> { '9', '8', '7', '6', '5', '4', '3', '2', '1' });
            //Reversed list is for checking for multiple solutions or shuffle if given the generating
            if (!RemoveCands(grid, row, col, candidates))//if it returns false, candidates count must be 0 so a contradiction is found
            {
                grid[row][col] = '0';
                solvePath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
                return false;
            }

            int nextRow = row, nextCol = col;
            if (++nextCol == 9)//increments the nextCol value which is used in conjunction with nextRow to look at the next cell in the sequence. If it is 9, it must be reset to 0
            {
                if (++nextRow == 9)//Currently looking at cell 81 in grid
                {
                    grid[row][col] = candidates[0];//Sets the last cell to be the only value possible, then the solution is checked.
                    solvePath.Add(row.ToString() + col.ToString() + candidates[0].ToString());//Add to solve path
                    if (gen.CheckIfSolved_array(grid))
                    {
                        return true;
                    }
                    else
                    {
                        grid[row][col] = '0';//cell value must be set to 0 to backtrack
                        solvePath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
                        return false;
                    }
                }
                else nextCol = 0;
            }
            if (grid[row][col] == '0')
            {
                foreach (char candidate in candidates)//iterates through each candidate value, assigning it to the current cell number.  
                {
                    grid[row][col] = candidate;
                    solvePath.Add(row.ToString() + col.ToString() + candidate.ToString());//Add to solve path
                    //A new instance of BruteForceSolver is called using the grid with an updated value of a cell and is provided with the next cell coordinates
                    if (BruteForceSolve(grid, nextRow, nextCol, variator))
                        return true;
                }
            }
            else
            {
                //If the current cell contains a number found from a naked single strategy,
                //then it is dismissed and a new instance of BruteForceSolver is called and is provided with the next cell coordinates
                if (BruteForceSolve(grid, nextRow, nextCol, variator))
                    return true;
                else
                {//if it returns false, then the number for the cell is set back to 0, and then cycles backwards through the recursions by returning false.
                    grid[row][col] = '0';
                    solvePath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
                    return false;
                }
            }
            grid[row][col] = '0';//cell value must be set to 0 to backtrack
            solvePath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
            return false;//gets hit if each brute force attempt with each 'candidate' returns false in the foreach
        }
    }
}
