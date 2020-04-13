﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SudokuSolverSetter
{
    public class PuzzleGenerator//INCOMPLETE!!!!!!!!!
    {
        //Initialising global objects/variables
        private Random rand = new Random();
        public SudokuGrid Setter()
        {
            PuzzleSolver solve = new PuzzleSolver();


            SudokuGrid grid = new SudokuGrid
            {
                Rows = new Cell[9][]
            };
            for (int i = 0; i < 9; i++)
            {
                grid.Rows[i] = new Cell[9];
                for (int j = 0; j < 9; j++)
                {
                    grid.Rows[i][j] = new Cell
                    {
                        BlockLoc = (i / 3) * 3 + (j / 3) + 1,//Block number = row/3*3+column/3
                        Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' },
                        Num = '0',
                        ReadOnly = true,
                        XLocation = i,
                        YLocation = j
                    };
                }
            }
            
            //For each cell, add all neighbouring cells to the list of cells property
            #region Assigning Neighbouring Cells
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int nbCounter = 0;//nbCounter is neighbourcounter
                    grid.Rows[i][j].NeighbourCells = new List<Cell[]>
                    {
                        new Cell[8],
                        new Cell[8],
                        new Cell[8]
                    };
                    for (int k = 0; k < 9; k++)
                    {
                        if (j != k)
                        {
                            grid.Rows[i][j].NeighbourCells[0][nbCounter] = grid.Rows[i][k];//add neighbour in i
                            nbCounter++;
                        }
                    }
                    nbCounter = 0;
                    for (int l = 0; l < 9; l++)
                    {
                        if (l != i)
                        {
                            grid.Rows[i][j].NeighbourCells[1][nbCounter] = grid.Rows[l][j];//add neighbour in column
                            nbCounter++;
                        }
                    }
                    nbCounter = 0;
                    int[] blockIndexes = BlockIndexGetter(grid.Rows[i][j].BlockLoc);

                    for (int x = blockIndexes[0]; x < blockIndexes[0] + 3; x++)
                    {
                        for (int y = blockIndexes[1]; y < blockIndexes[1] + 3; y++)
                        {
                            if (grid.Rows[x][y] != grid.Rows[i][j])
                            {
                                grid.Rows[i][j].NeighbourCells[2][nbCounter] = grid.Rows[x][y];//add neighbour in block
                                nbCounter++;
                            }
                        }
                    }
                }
            }
            #endregion

            //Using BruteForceSolver to fill in a blank grid to get a starting solution - possibly the part of the generator that causes performance issues
            try
            {
                if(!solve.BruteForceSolve(grid, 0, 0, 2))
                {
                    MessageBox.Show("Generator did not find a puzzle to generate");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something has gone terribly wrong...");
                return grid;
            }
            
            ///This section consists of constantly removing parallel numbers, e.g. [0,0] and [8,8] or [2,5] and [5,2], and checking if the puzzle is still valid (i.e. still has only one solution)
            char[][] sudokuArray = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
            bool changeMade = false;
            int removed = 0;
            List<string> cellsChecked = new List<string>();
            List<int> rowList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            List<int> colList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            do
            {
                changeMade = false;
                rowList = Shuffler_intList(rowList);
                colList = Shuffler_intList(colList);
                foreach (int row in rowList)
                {
                    if (removed > 63)
                    { changeMade = true; break; }
                    foreach (int col in colList)
                    {
                        if (removed > 63)
                        { changeMade = true; break; }
                        if (!cellsChecked.Contains(row.ToString() + col.ToString()))
                        {
                            int altRow = NumberSwitch(row);
                            int altCol = NumberSwitch(col);
                            if (grid.Rows[altRow][altCol].Num != '0' && grid.Rows[row][col].Num != '0')
                            {
                                for (int r = 0; r < 9; r++)
                                {
                                    for (int c = 0; c < 9; c++)
                                    {
                                        sudokuArray[r][c] = grid.Rows[r][c].Num;
                                    }
                                }
                                grid.Rows[row][col].Num = '0';
                                grid.Rows[altRow][altCol].Num = '0';
                                if (solve.BruteForceSolve(grid, 0, 0, 0))
                                {
                                    string firstSol = SudokuToString(grid);
                                    grid = RestartPuzzle(grid, sudokuArray);
                                    grid.Rows[row][col].Num = '0';
                                    grid.Rows[altRow][altCol].Num = '0';
                                    if (solve.BruteForceSolve(grid, 0, 0, 1))
                                    {
                                        string secSol = SudokuToString(grid);
                                        if (firstSol == secSol)//valid puzzle
                                        {
                                            grid = RestartPuzzle(grid, sudokuArray);
                                            grid.Rows[row][col].Num = '0';
                                            grid.Rows[altRow][altCol].Num = '0';
                                            changeMade = true;
                                            removed += 2;
                                            cellsChecked.Add(row.ToString() + col.ToString());
                                            cellsChecked.Add(altRow.ToString() + altCol.ToString());
                                        }
                                        else//Multiple solutions
                                        {
                                            grid = RestartPuzzle(grid, sudokuArray);
                                        }
                                    }
                                }
                                else//Invalid puzzle, should never occur
                                {
                                    grid = RestartPuzzle(grid, sudokuArray);
                                }
                            }
                        }
                        
                    }
                }
                ///Tries to remove more numbers to toughen the puzzle
                #region Toughen the puzzle
                if (!changeMade)
                {
                    foreach (int row in rowList)
                    {
                        if (removed > 63)
                        { changeMade = true; break; }
                        foreach (int col in colList)
                        {
                            if (removed > 63)
                            { changeMade = true; break; }
                            if (!cellsChecked.Contains(row.ToString() + col.ToString()))
                            {
                                if (grid.Rows[row][col].Num != 0)
                                {
                                    for (int r = 0; r < 9; r++)
                                    {
                                        for (int c = 0; c < 9; c++)
                                        {
                                            sudokuArray[r][c] = grid.Rows[r][c].Num;
                                        }
                                    }
                                    grid.Rows[row][col].Num = '0';
                                    if (solve.BruteForceSolve(grid, 0, 0, 0))
                                    {
                                        string firstSol = SudokuToString(grid);
                                        grid = RestartPuzzle(grid, sudokuArray);
                                        grid.Rows[row][col].Num = '0';
                                        if (solve.BruteForceSolve(grid, 0, 0, 1))
                                        {
                                            string secSol = SudokuToString(grid);
                                            if (firstSol == secSol)//valid puzzle
                                            {
                                                grid = RestartPuzzle(grid, sudokuArray);
                                                grid.Rows[row][col].Num = '0';
                                                changeMade = true;
                                                removed++;
                                                cellsChecked.Add(row.ToString() + col.ToString());
                                            }
                                            else
                                            {
                                                grid = RestartPuzzle(grid, sudokuArray);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        grid = RestartPuzzle(grid, sudokuArray);
                                    }
                                }
                                cellsChecked.Add(row.ToString() + col.ToString());
                            }
                        }
                    }
                }
                #endregion


            } while (removed <= 45 && changeMade);
            return grid;
        }
        /// <summary>
        /// Reassigns the values in the puzzle to what they were prior to being solved
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sudokuArray">contains the original puzzle prior to solve</param>
        /// <returns></returns>
        private SudokuGrid RestartPuzzle(SudokuGrid grid, char[][] sudokuArray)
        {
            for (int x = 0; x < 9; x++)//O(n^2)
            {
                for (int y = 0; y < 9; y++)
                {
                    grid.Rows[x][y].Num = sudokuArray[x][y];
                }
            }
            return grid;
        }
        /// <summary>
        /// Shuffles the candidate values, used in brute-force solvers
        /// </summary>
        /// <param name="rowNumbers"></param>
        /// <returns></returns>
        public List<char> Shuffler(List<char> rowNumbers)
        {
            rand = new Random();
            int index = rowNumbers.Count;
            while (index > 1)//Shuffle the numbers that can go in the row.
            {
                index--;
                int k = rand.Next(index + 1);
                char tmp = rowNumbers[k];
                rowNumbers[k] = rowNumbers[index];
                rowNumbers[index] = tmp;
            }
            return rowNumbers;
        }
        /// <summary>
        /// Used for shuffling row/column numbers in the puzzle generator so that random numbers are removed from the puzzle
        /// </summary>
        /// <param name="rowNumbers"></param>
        /// <returns></returns>
        public List<int> Shuffler_intList(List<int> rowNumbers)
        {
            rand = new Random();
            int index = rowNumbers.Count;
            while (index > 1)//Shuffle the numbers that can go in the row.
            {
                index--;
                int k = rand.Next(index + 1);
                int tmp = rowNumbers[k];
                rowNumbers[k] = rowNumbers[index];
                rowNumbers[index] = tmp;
            }
            return rowNumbers;
        }
        /// <summary>
        /// Used to check if the puzzle is solved
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool CheckIfSolved(SudokuGrid grid)
        {
            for (int row = 0; row < 9; row++)
            {
                List<char> numberList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                for (int col = 0; col < 9; col++)
                {
                    if (grid.Rows[row][col].Num == 0)
                    {
                        return false;
                    }
                    else if (numberList.Contains(grid.Rows[row][col].Num))
                    {
                        numberList.Remove(grid.Rows[row][col].Num);
                    }
                    for (int index = 0; index < 3; index++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            if (grid.Rows[row][col].NeighbourCells[index][i].Num == grid.Rows[row][col].Num)
                            {
                                return false;
                            }
                        }
                    }
                }
                if (numberList.Count > 0)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Used to check if the puzzle is solved, but only used for the alternate data structure, char[][]
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool CheckIfSolved_array(char[][] grid)
        {
            List<char> numberList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (grid[row][col] == '0')
                    {
                        return false;
                    }
                    else if (numberList.Contains(grid[row][col]))
                    {
                        numberList.Remove(grid[row][col]);
                    }
                    int blockNumber = 0;
                    int[] indexes = new int[2];
                    for (int i = 0; i < 9; i++)
                    {
                        if (grid[row][col] == grid[row][i] && i != col)
                        {
                            return false;
                        }
                        if (grid[row][col] == grid[i][col] && i != row)
                        {
                            return false;
                        }
                        blockNumber = (row / 3) * 3 + (col / 3) + 1;
                        indexes = BlockIndexGetter(blockNumber);
                        if (grid[row][col] == grid[indexes[0]][indexes[1]] && indexes[0] != row && indexes[1] != col)
                        {
                            return false;
                        }
                        indexes[1]++;
                        if (indexes[1] == 3 || indexes[1] == 6 || indexes[1] == 9)
                        {
                            indexes[0]++;
                            indexes[1] -= 3;
                        }
                    }
                }
                if (numberList.Count > 0)
                {
                    return false;
                }
                else
                {
                    numberList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                }
            }

            return true;
        }
        /// <summary>
        /// Method to get the number of the cell that is rotationally symmetrical to the current cell
        /// </summary>
        /// <param name="origNum"></param>
        /// <returns></returns>
        public int NumberSwitch(int origNum)
        {
            int newNum = 0;
            switch (origNum)
            {
                case 0:
                    newNum = 8;
                    break;
                case 1:
                    newNum = 7;
                    break;
                case 2:
                    newNum = 6;
                    break;
                case 3:
                    newNum = 5;
                    break;
                case 5:
                    newNum = 3;
                    break;
                case 6:
                    newNum = 2;
                    break;
                case 7:
                    newNum = 1;
                    break;
                case 8:
                    newNum = 0;
                    break;
                default:
                    newNum = origNum;//catches when num is 4
                    break;
            }
            return newNum;
        }
        /// <summary>
        /// This function Gets the x and y starting coordinates of a cell's block. I.e. Cell [2,6] will have the starting coordinates of 0 and 6 as it is in the third block
        /// </summary>
        /// <param name="blockNum"></param>
        /// <returns></returns>
        public int[] BlockIndexGetter(int blockNum)
        {
            int[] blockIndexes = new int[2];
            switch (blockNum)
            {
                case 1:
                    blockIndexes[0] = 0;
                    blockIndexes[1] = 0;
                    break;
                case 2:
                    blockIndexes[0] = 0;
                    blockIndexes[1] = 3;
                    break;
                case 3:
                    blockIndexes[0] = 0;
                    blockIndexes[1] = 6;
                    break;
                case 4:
                    blockIndexes[0] = 3;
                    blockIndexes[1] = 0;
                    break;
                case 5:
                    blockIndexes[0] = 3;
                    blockIndexes[1] = 3;
                    break;
                case 6:
                    blockIndexes[0] = 3;
                    blockIndexes[1] = 6;
                    break;
                case 7:
                    blockIndexes[0] = 6;
                    blockIndexes[1] = 0;
                    break;
                case 8:
                    blockIndexes[0] = 6;
                    blockIndexes[1] = 3;
                    break;
                case 9:
                    blockIndexes[0] = 6;
                    blockIndexes[1] = 6;
                    break;
                default:
                    break;
            }
            return blockIndexes;
        }
        /// <summary>
        /// This converts a SudokuGrid into a string of numbers from 0-9, where 0 is an empty cell
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public string SudokuToString(SudokuGrid grid)
        {
            string sudokuExport = "";

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudokuExport += grid.Rows[i][j].Num;
                }
            }
            return sudokuExport;
        }
    }
}
