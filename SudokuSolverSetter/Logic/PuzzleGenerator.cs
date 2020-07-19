using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SudokuSolverSetter
{
    public class PuzzleGenerator
    {
        //Initialising global objects/variables
        private readonly Random rand = new Random();
        private readonly int g_MaxRemoves = 63;
        /// <summary>
        /// 
        /// </summary>
        /// <returns>the puzzle in form of SudokuGrid</returns>
        public SudokuGrid Setter()
        {
            PuzzleSolverAdvDS solve = new PuzzleSolverAdvDS();
            SudokuGrid grid = ConstructGrid();
            //Using Backtracking Solver to fill in a blank grid to get a starting solution - possibly the part of the generator that causes performance issues
            bool testing = false;//testing condition
            if (!testing)
            {
                try
                {
                    if (!solve.CompileBacktracker(grid, 2))
                        MessageBox.Show("Generator did not find a puzzle to generate");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something has gone terribly wrong...\r\nError: " + ex);
                    return grid;
                }
            }
            else
            {
                #region Manual Testing Puzzle
                StringToGrid(grid, "600438079900705080800000032200004060000000000040600003180000005070806001560941008");
                #endregion
            }
            RemoveNumbers(grid, solve);
            return grid;
        }
        #region Public Functions
        public SudokuGrid ConstructGrid()
        {
            SudokuGrid grid = new SudokuGrid
            {
                Cells = new Cell[9][]
            };
            for (int i = 0; i < 9; i++)
            {
                grid.Cells[i] = new Cell[9];
                for (int j = 0; j < 9; j++)
                {
                    grid.Cells[i][j] = new Cell
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
            AddNeighbours(grid);
            return grid;
        }
        
        /// <summary>
        /// For each cell, add all neighbouring cells to the list of cells property
        /// </summary>
        /// <param name="grid"></param>
        public void AddNeighbours(SudokuGrid grid)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int nbCounter = 0;//nbCounter is neighbourcounter
                    grid.Cells[i][j].NeighbourCells = new Cell[3][]
                    {
                        new Cell[8],
                        new Cell[8],
                        new Cell[8]
                    };
                    for (int k = 0, p = 0; k < 9; k++)
                    {
                        if (j != k)
                        {
                            grid.Cells[i][j].NeighbourCells[0][p++] = grid.Cells[i][k];//add neighbour in i
                            nbCounter++;
                        }
                    }
                    nbCounter = 0;
                    for (int l = 0, p = 0; l < 9; l++)
                    {
                        if (l != i)
                        {
                            grid.Cells[i][j].NeighbourCells[1][p++] = grid.Cells[l][j];//add neighbour in column
                            nbCounter++;
                        }
                    }
                    nbCounter = 0;
                    int[] blockIndexes = BlockIndexGetter(grid.Cells[i][j].BlockLoc);

                    for (int x = blockIndexes[0], p = 0; x < blockIndexes[0] + 3; x++)
                    {
                        for (int y = blockIndexes[1]; y < blockIndexes[1] + 3; y++)
                        {
                            if (grid.Cells[x][y] != grid.Cells[i][j])
                            {
                                grid.Cells[i][j].NeighbourCells[2][p++] = grid.Cells[x][y];//add neighbour in block
                                nbCounter++;
                            }
                        }
                    }
                }
            }
        }
        public void RemoveNumbers(SudokuGrid grid, PuzzleSolverAdvDS solve)
        {
            ///This section consists of constantly removing parallel numbers, e.g. [0,0] and [8,8] or [2,5] and [5,2], and checking if the puzzle is still valid (i.e. still has only one solution)

            int removed = 0;
            List<string> cellsChecked = new List<string>(81);
            List<int> rowList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            List<int> colList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

            rowList = Shuffler_intList(rowList);
            colList = Shuffler_intList(colList);
            foreach (int row in rowList)
            {
                if (removed >= g_MaxRemoves)
                { break; }
                foreach (int col in colList)
                {
                    if (removed >= g_MaxRemoves)
                    { break; }
                    if (!cellsChecked.Contains(row.ToString() + col.ToString()))
                    {
                        int altRow = NumberSwitch(row);
                        int altCol = NumberSwitch(col);

                        if (grid.Cells[altRow][altCol].Num != '0' && grid.Cells[row][col].Num != '0')
                        {
                            bool valid = false;
                            List<char> numList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                            for (int x = 0; x < 9 && !valid; x++)//checks for if the puzzle is valid in the sense that is has more than 7 unique numbers at any point in the puzzle
                            {
                                for (int y = 0; y < 9 && !valid; y++)
                                {
                                    if (grid.Cells[x][y].Num != '0')
                                    {
                                        if ((x == altRow && y == altCol) == false && (x == row && y == col) == false)
                                        {
                                            numList.Remove(grid.Cells[x][y].Num);
                                        }
                                        if (numList.Count <= 1)
                                        {
                                            valid = true;
                                        }
                                    }
                                }
                            }
                            if (valid)
                            {
                                char[][] sudokuArray = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
                                for (int r = 0; r < 9; r++)
                                {
                                    for (int c = 0; c < 9; c++)
                                    {
                                        sudokuArray[r][c] = grid.Cells[r][c].Num;
                                    }
                                }
                                grid.Cells[row][col].Num = '0';
                                grid.Cells[altRow][altCol].Num = '0';
                                if (solve.CompileBacktracker(grid, 4))
                                {
                                    string firstSol = GridToString(grid);
                                    grid = RestartPuzzle(grid, sudokuArray);
                                    grid.Cells[row][col].Num = '0';
                                    grid.Cells[altRow][altCol].Num = '0';
                                    if (solve.CompileBacktracker(grid, 1))//tries Backtracking algorithm using reversed candidate lists so that if a solution that is different to the previous solution exists, it will be found, invalidating the puzzle
                                    {
                                        string secSol = GridToString(grid);
                                        if (firstSol == secSol)//valid puzzle
                                        {
                                            grid = RestartPuzzle(grid, sudokuArray);
                                            grid.Cells[row][col].Num = '0';
                                            grid.Cells[altRow][altCol].Num = '0';
                                            if (grid.Cells[row][col] == grid.Cells[altRow][altCol])
                                                removed++;
                                            else
                                                removed += 2;
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
                            if (grid.Cells[row][col] == grid.Cells[altRow][altCol])
                                cellsChecked.Add(row.ToString() + col.ToString());
                            else
                            {
                                cellsChecked.Add(row.ToString() + col.ToString());
                                cellsChecked.Add(altRow.ToString() + altCol.ToString());
                            }

                        }
                    }

                }
            }
            if (removed < g_MaxRemoves)
            {
                cellsChecked = new List<string>();
                foreach (int row in rowList)
                {
                    if (removed >= g_MaxRemoves)
                    { break; }
                    foreach (int col in colList)
                    {
                        if (removed >= g_MaxRemoves)
                        { break; }
                        if (!cellsChecked.Contains(row.ToString() + col.ToString()))
                        {
                            if (grid.Cells[row][col].Num != '0')
                            {
                                bool valid = false;
                                List<char> numList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                                for (int x = 0; x < 9 && !valid; x++)//checks for if the puzzle is valid in the sense that is has more than 7 unique numbers at any point in the puzzle
                                {
                                    for (int y = 0; y < 9 && !valid; y++)
                                    {
                                        if (grid.Cells[x][y].Num != '0')
                                        {
                                            if ((x == row && y == col) == false)
                                            {
                                                numList.Remove(grid.Cells[x][y].Num);
                                            }
                                            if (numList.Count <= 1)
                                            {
                                                valid = true;
                                            }
                                        }
                                    }
                                }
                                if (valid)
                                {
                                    char[][] sudokuArray = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
                                    for (int r = 0; r < 9; r++)
                                    {
                                        for (int c = 0; c < 9; c++)
                                        {
                                            sudokuArray[r][c] = grid.Cells[r][c].Num;
                                        }
                                    }
                                    grid.Cells[row][col].Num = '0';
                                    if (solve.CompileBacktracker(grid, 4))
                                    {
                                        string firstSol = GridToString(grid);
                                        grid = RestartPuzzle(grid, sudokuArray);
                                        grid.Cells[row][col].Num = '0';
                                        if (solve.CompileBacktracker(grid, 1))
                                        {
                                            string secSol = GridToString(grid);
                                            if (firstSol == secSol)//valid puzzle
                                            {
                                                grid = RestartPuzzle(grid, sudokuArray);
                                                grid.Cells[row][col].Num = '0';
                                                removed++;
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
                            }
                            cellsChecked.Add(row.ToString() + col.ToString());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// checks for if the puzzle is valid in the sense that is has more than 7 unique numbers at any point in the puzzle, more than 16 givens, and one solution
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool CheckValidity(SudokuGrid grid)
        {
            bool minOfEight = false;
            int givens = 0;
            List<char> numList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (grid.Cells[x][y].Num != '0')
                    {
                        numList.Remove(grid.Cells[x][y].Num);
                        if (numList.Count <= 1)
                        {
                            minOfEight = true;
                        }
                        givens++;
                    }
                }
            }
            if (!minOfEight || givens < 17)
                return false;
            char[][] sudokuArray = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    sudokuArray[r][c] = grid.Cells[r][c].Num;
                }
            }
            PuzzleSolverAdvDS solve = new PuzzleSolverAdvDS();
            if (solve.CompileBacktracker(grid, 4))
            {
                string firstSol = GridToString(grid);
                grid = RestartPuzzle(grid, sudokuArray);
                if (solve.CompileBacktracker(grid, 1))
                {
                    string secSol = GridToString(grid);
                    if (firstSol == secSol)//valid puzzle
                    {
                        grid = RestartPuzzle(grid, sudokuArray);
                        return true;
                    }
                    else
                    {
                        grid = RestartPuzzle(grid, sudokuArray);
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Reassigns the values in the puzzle to what they were prior to being solved
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sudokuArray">contains the original puzzle prior to solve</param>
        /// <returns></returns>
        public SudokuGrid RestartPuzzle(SudokuGrid grid, char[][] sudokuArray)
        {
            for (int x = 0; x < 9; x++)//O(n^2)
            {
                for (int y = 0; y < 9; y++)
                {
                    grid.Cells[x][y].Num = sudokuArray[x][y];
                }
            }
            return grid;
        }

        /// <summary>
        /// Shuffles the candidate values, used in Backtracking solvers
        /// </summary>
        /// <param name="rowNumbers"></param>
        /// <returns></returns>
        public List<char> Shuffler(List<char> rowNumbers)
        {
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
                    if (grid.Cells[row][col].Num == '0')
                    {
                        if (grid.Cells[row][col].Candidates.Count == 0)
                        {
                            MessageBox.Show("A strategy has malfunctioned and caused a contradiction or the puzzle is invalid");//Catches if a strategy makes a mistake by causing a cell to have no candidates
                        }
                        return false;
                    }
                    else if (numberList.Contains(grid.Cells[row][col].Num))
                    {
                        numberList.Remove(grid.Cells[row][col].Num);
                    }
                    for (int index = 0; index < 3; index++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            if (grid.Cells[row][col].NeighbourCells[index][i].Num == grid.Cells[row][col].Num)
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
        public string GridToString(SudokuGrid grid)
        {
            string sudokuExport = "";

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudokuExport += grid.Cells[i][j].Num;
                }
            }
            return sudokuExport;
        }
        public void StringToGrid(SudokuGrid puzzleGrid, string puzzleString)
        {
            int counter = 0;
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (puzzleString[counter] == '0')
                    {
                        puzzleGrid.Cells[x][y].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                    }
                    else
                    {
                        puzzleGrid.Cells[x][y].Candidates = new List<char> { };
                    }
                    puzzleGrid.Cells[x][y].Num = puzzleString[counter];
                    counter++;
                }
            }
        }
        public bool ValidateInput(UniformGrid SudokuPuzzle, SudokuGrid g_grid, TextBox g_selectedCell)
        {
            if (g_selectedCell.Text == "")
                return true;
            double index_ = SudokuPuzzle.Children.IndexOf(g_selectedCell);
            int row = (int)index_ / 9;
            int col = (int)index_ % 9;
            g_grid.Cells[row][col].Num = g_selectedCell.Text[0];
            if (g_grid.Cells[row][col].Num == '0')
                return true;
            bool valid = true;
            for (int n = 0; n < 8; n++)
            {
                if (g_grid.Cells[row][col].Num == g_grid.Cells[row][col].NeighbourCells[0][n].Num)
                {
                    int index = g_grid.Cells[row][col].NeighbourCells[0][n].XLocation * 9 + g_grid.Cells[row][col].NeighbourCells[0][n].YLocation;
                    if (((TextBox)SudokuPuzzle.Children[index]).Background != Brushes.Red)
                    {
                        g_grid.Cells[row][col].Num = '0';
                        valid = false; break;
                    }
                }
                if (g_grid.Cells[row][col].Num == g_grid.Cells[row][col].NeighbourCells[1][n].Num)
                {
                    int index = g_grid.Cells[row][col].NeighbourCells[1][n].XLocation * 9 + g_grid.Cells[row][col].NeighbourCells[1][n].YLocation;
                    if (((TextBox)SudokuPuzzle.Children[index]).Background != Brushes.Red)
                    {
                        g_grid.Cells[row][col].Num = '0';
                        valid = false; break;
                    }
                }
                if (g_grid.Cells[row][col].Num == g_grid.Cells[row][col].NeighbourCells[2][n].Num)
                {
                    int index = g_grid.Cells[row][col].NeighbourCells[2][n].XLocation * 9 + g_grid.Cells[row][col].NeighbourCells[2][n].YLocation;
                    if (((TextBox)SudokuPuzzle.Children[index]).Background != Brushes.Red)
                    {
                        g_grid.Cells[row][col].Num = '0';
                        valid = false; break;
                    }
                }
            }
            return valid;
        }
        #endregion
    }
}