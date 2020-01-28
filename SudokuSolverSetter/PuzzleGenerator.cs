using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverSetter
{
    public class PuzzleGenerator//INCOMPLETE!!!!!!!!!
    {
        public Grid Setter()
        {
            PuzzleSolver solve = new PuzzleSolver();
            Random rand = new Random();

            Grid grid = new Grid
            {
                Rows = new Cell[9][]
            };
            grid.PuzzleID = 1;
            int value = 0;
            int vecPos = 0;
            bool newNum = true;
            short clearIncr = 0;
            int subGridCounter = 0;
            for (int i = 0; i < grid.Rows.Length; i++)
            {
                List<int> rowNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                grid.Rows[i] = new Cell[9];
                for (int j = 0; j < 9; j++)
                {
                    if (i < 3 && j < 3)
                    {
                        subGridCounter = 1;
                    }
                    else if (i < 3 && (j >= 3 && j < 6))
                    {
                        subGridCounter = 2;
                    }
                    else if (i < 3 && j >= 6)
                    {
                        subGridCounter = 3;
                    }
                    else if ((i >= 3 && i < 6) && j < 3)
                    {
                        subGridCounter = 4;
                    }
                    else if ((i >= 3 && i < 6) && (j >= 3 && j < 6))
                    {
                        subGridCounter = 5;
                    }
                    else if ((i >= 3 && i < 6) && j >= 6)
                    {
                        subGridCounter = 6;
                    }
                    else if (i >= 6 && j < 3)
                    {
                        subGridCounter = 7;
                    }
                    else if (i >= 6 && (j >= 3 && j < 6))
                    {
                        subGridCounter = 8;
                    }
                    else if (i >= 6 && j >= 6)
                    {
                        subGridCounter = 9;
                    }

                    grid.Rows[i][j] = new Cell
                    {
                        Candidates = new List<int>(9)
                    };
                    grid.Rows[i][j].XLocation = i;
                    grid.Rows[i][j].YLocation = j;
                    grid.Rows[i][j].SubGridLoc = subGridCounter;

                    int incr = 0;
                    int index = rowNumbers.Count;
                    while (index > 1)
                    {
                        index--;
                        int k = rand.Next(index + 1);
                        int tmp = rowNumbers[k];
                        rowNumbers[k] = rowNumbers[index];
                        rowNumbers[index] = tmp;
                    }
                    vecPos = rowNumbers.Count - 1;
                    do
                    {
                        if (incr > 0 && vecPos > 0)
                        {
                            vecPos--;
                        }
                        newNum = true;
                        value = rowNumbers[vecPos];


                        for (int k = 0; k < i; k++)
                        {
                            if (value == grid.Rows[k][j].Num)
                            {
                                newNum = false;
                                break;
                            }
                        }

                        for (int n = 0; n < j; n++)
                        {
                            if (!newNum)
                            {
                                break;
                            }
                            if (value == grid.Rows[i][n].Num)
                            {
                                newNum = false;
                                break;
                            }
                        }
                        for (int rowIndex = 0; rowIndex <= i; rowIndex++)
                        {
                            if (!newNum)
                            {
                                break;
                            }
                            if (rowIndex == i)
                            {
                                for (int l = 0; l < j; l++)
                                {
                                    if (grid.Rows[rowIndex][l].SubGridLoc == subGridCounter && grid.Rows[rowIndex][l].Num == value)
                                    {
                                        newNum = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int l = 0; l < 9; l++)
                                {
                                    if (grid.Rows[rowIndex][l].SubGridLoc == subGridCounter && grid.Rows[rowIndex][l].Num == value)
                                    {
                                        newNum = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (rowNumbers.Count == incr && !newNum)
                        {
                            break;
                        }
                        incr++;
                    } while (!newNum);
                    if (newNum)
                    {
                        grid.Rows[i][j].Num = value;
                        grid.Rows[i][j].ReadOnly = true;
                        rowNumbers.Remove(value);
                    }
                    else
                    {
                        //clear row, start again.
                        if (clearIncr > 7)
                        {
                            Array.Clear(grid.Rows, 0, grid.Rows.Length);
                            i = -1;
                            subGridCounter = 0;
                            clearIncr = 0;
                            break;
                        }
                        else
                        {
                            Array.Clear(grid.Rows[i], 0, grid.Rows[i].Length);
                            clearIncr++;
                            i--;
                            break;
                        }
                    }
                }
            }
            //Make sure there is only ONE solution
            int numbersToRemove = rand.Next(45, 65);

            List<int[]> positions = new List<int[]> { new int[] { 0,0 }, new int[] { 0,1 }, new int[] { 0,2 }, new int[] { 0,3 }, new int[] { 0,4 }, new int[] { 0,5 }, new int[] { 0,6 }, new int[] { 0,7 }, new int[] { 0,8 },
                                                      new int[] { 1,0 }, new int[] { 1,1 }, new int[] { 1,2 }, new int[] { 1,3 }, new int[] { 1,4 }, new int[] { 1,5 }, new int[] { 1,6 }, new int[] { 1,7 }, new int[] { 1,8 },
                                                      new int[] { 2,0 }, new int[] { 2,1 }, new int[] { 2,2 }, new int[] { 2,3 }, new int[] { 2,4 }, new int[] { 2,5 }, new int[] { 2,6 }, new int[] { 2,7 }, new int[] { 2,8 },
                                                      new int[] { 3,0 }, new int[] { 3,1 }, new int[] { 3,2 }, new int[] { 3,3 }, new int[] { 3,4 }, new int[] { 3,5 }, new int[] { 3,6 }, new int[] { 3,7 }, new int[] { 3,8 },
                                                      new int[] { 4,0 }, new int[] { 4,1 }, new int[] { 4,2 }, new int[] { 4,3 }, new int[] { 4,4 }, new int[] { 4,5 }, new int[] { 4,6 }, new int[] { 4,7 }, new int[] { 4,8 },
                                                      new int[] { 5,0 }, new int[] { 5,1 }, new int[] { 5,2 }, new int[] { 5,3 }, new int[] { 5,4 }, new int[] { 5,5 }, new int[] { 5,6 }, new int[] { 5,7 }, new int[] { 5,8 },
                                                      new int[] { 6,0 }, new int[] { 6,1 }, new int[] { 6,2 }, new int[] { 6,3 }, new int[] { 6,4 }, new int[] { 6,5 }, new int[] { 6,6 }, new int[] { 6,7 }, new int[] { 6,8 },
                                                      new int[] { 7,0 }, new int[] { 7,1 }, new int[] { 7,2 }, new int[] { 7,3 }, new int[] { 7,4 }, new int[] { 7,5 }, new int[] { 7,6 }, new int[] { 7,7 }, new int[] { 7,8 },
                                                      new int[] { 8,0 }, new int[] { 8,1 }, new int[] { 8,2 }, new int[] { 8,3 }, new int[] { 8,4 }, new int[] { 8,5 }, new int[] { 8,6 }, new int[] { 8,7 }, new int[] { 8,8 }
            };

            Grid gridSave = new Grid() { PuzzleID = grid.PuzzleID };//Passes by reference, changed to deep copy
            gridSave.Rows = new Cell[9][];
            for (int r = 0; r < gridSave.Rows.Length; r++)
            {
                gridSave.Rows[r] = new Cell[9];
                for (int c = 0; c < gridSave.Rows[r].Length; c++)
                {
                    gridSave.Rows[r][c] = new Cell()
                    {
                        Candidates = grid.Rows[r][c].Candidates,
                        Num = grid.Rows[r][c].Num,
                        SubGridLoc = grid.Rows[r][c].SubGridLoc,
                        XLocation = grid.Rows[r][c].XLocation,
                        YLocation = grid.Rows[r][c].YLocation,
                        ReadOnly = true
                    };
                }
            }

            //grid.Rows[0][0].Num = 0;
            //gridSave.PuzzleID = 2;
            int[] position;
            int prevNum = 0;
            int solvedNum = 0;
            for (int i = 0; i < numbersToRemove; i++)
            {
                position = positions[rand.Next(positions.Count)];
                positions.Remove(position);
                prevNum = grid.Rows[position[0]][position[1]].Num;
                grid.Rows[position[0]][position[1]].Num = 0;

                solvedNum = solve.SolveACell(position, grid);
                if (solvedNum == prevNum)
                {
                    grid.Rows[position[0]][position[1]].Num = 0;
                    grid.Rows[position[0]][position[1]].ReadOnly = false;
                }


            }


            return grid;
        }
    }
}
