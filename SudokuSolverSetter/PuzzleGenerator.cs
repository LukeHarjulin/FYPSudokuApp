using System;
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
                Rows = new Cell[9][],
                PuzzleID = 000
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
            #region Old Solution Generator            
            //grid.PuzzleID = 1;//Temporary ID, will be random 6 digit number in future
            //int value = 0;//value to be inserted into cell
            //int vecPos = 0;//vector position, used for extracting a number out of the rowNumbers List
            //bool newNum = true;//flags if the value exists in the row, column, or block so that it isn't placed in the cell.
            ////Block #

            ///*  -Starts off by iterating through the rows, assigning an empty array of Cells to each row
            //    -Next, the cells associated with the current row are iterated through, assigning a block number, 
            //    a row and column number, and an empty candidate list to each Cell
            //    -Next, shuffle the numbers that can go in the row.
            //    -Then test the chosen value against the existing numbers within the column/row/block.
            //    -Once a grid is generated, begin removing numbers (WIP - unsure of how to do this part - need more research)
            // */
            //for (int row = 0; row < grid.Rows.Length; row++)
            //{
            //    List<int> rowNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            //    grid.Rows[row] = new Cell[9];
            //    for (int col = 0; col < 9; col++)
            //    {
            //        //Assigning Block Value (starts from 0)
            //        //Block number = row/3*3+column/3
            //        blockCounter = (row / 3) * 3 + (col / 3) + 1;

            //        grid.Rows[row][col] = new Cell { Candidates = new List<int>(9) };
            //        grid.Rows[row][col].XLocation = row;
            //        grid.Rows[row][col].YLocation = col;
            //        grid.Rows[row][col].BlockLoc = blockCounter;
            //        int incr = 0;

            //        rowNumbers = Shuffler(rowNumbers);

            //        vecPos = rowNumbers.Count - 1;
            //        do
            //        {
            //            if (incr > 0 && vecPos > 0)
            //            {
            //                vecPos--;
            //            }
            //            newNum = true;
            //            value = rowNumbers[vecPos];//assigns the value with a value from the shuffled row of numbers

            //            for (int k = 0; k < row; k++)//Checks through the values in the current column against 'value'
            //            {
            //                if (value == grid.Rows[k][col].Num)
            //                {
            //                    newNum = false;
            //                    break;
            //                }
            //            }

            //            //Don't need to check the numbers within the current row against 'value' as the rowNumbers contains unused numbers

            //            for (int rowIndex = 0; rowIndex <= row; rowIndex++)//Checks through the values in the current block against 'value'
            //            {
            //                if (!newNum) { break; }

            //                for (int colIndex = 0; colIndex < 9; colIndex++)
            //                {

            //                    if (grid.Rows[rowIndex][colIndex].BlockLoc == blockCounter && grid.Rows[rowIndex][colIndex].Num == value)
            //                    {
            //                        newNum = false;
            //                        break;
            //                    }
            //                    if (rowIndex == row && colIndex == col)
            //                    {
            //                        break;
            //                    }
            //                }

            //            }
            //            if (rowNumbers.Count == incr && !newNum)//If all possible numbers have been attempted for that Cell, break out, clear the row and start again
            //            {
            //                break;
            //            }
            //            incr++;
            //        } while (!newNum);
            //        if (newNum)//Assign the Cell value as the number if it passes through the loop as a new number
            //        {
            //            grid.Rows[row][col].Num = value;
            //            grid.Rows[row][col].ReadOnly = true;
            //            rowNumbers.Remove(value);
            //        }
            //        else//Restart row/column if a break occured within the loop
            //        {
            //            Array.Clear(grid.Rows[row], 0, grid.Rows[row].Length);
            //            row--;
            //            break;
            //        }


            //    }
            //}
            #endregion
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

            //Using BruteForceSolver to fill in a blank grid to get a starting solution
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
                                else//Invalid puzzle
                                {
                                    grid = RestartPuzzle(grid, sudokuArray);
                                }
                            }
                        }
                        
                    }
                }
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

        private SudokuGrid RestartPuzzle(SudokuGrid grid, char[][] sudokuArray)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    grid.Rows[x][y].Num = sudokuArray[x][y];
                }
            }
            return grid;
        }
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
        /*
                public SudokuGrid RemoveNumber(SudokuGrid grid)
                {
                    int randRow = 0, randCol = 0, saveNum = 0;
                    int altRow = 0, altCol = 0, altSaveNum = 0;
                    randRow = rand.Next(0, 9);
                    randCol = rand.Next(0, 9);
                    altRow = NumberSwitch(randRow);
                    altCol = NumberSwitch(randCol);

                    if (grid.Rows[randRow][randCol].Num != 0 && grid.Rows[altRow][altCol].Num != 0)
                    {
                        saveNum = grid.Rows[randRow][randCol].Num;
                        grid.Rows[randRow][randCol].Num = 0;
                        grid.Rows[randRow][randCol].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        altSaveNum = grid.Rows[altRow][altCol].Num;
                        grid.Rows[altRow][altCol].Num = 0;
                        grid.Rows[altRow][altCol].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                        //Create a copy of the grid after a number is removed
                        SudokuGrid gridSave = new SudokuGrid() { PuzzleID = grid.PuzzleID };
                        gridSave.Rows = new Cell[9][];
                        for (int r = 0; r < gridSave.Rows.Length; r++)
                        {
                            gridSave.Rows[r] = new Cell[9];
                            for (int c = 0; c < gridSave.Rows[r].Length; c++)
                            {
                                gridSave.Rows[r][c] = new Cell()
                                {
                                    Num = grid.Rows[r][c].Num,
                                    BlockLoc = grid.Rows[r][c].BlockLoc,
                                    XLocation = grid.Rows[r][c].XLocation,
                                    YLocation = grid.Rows[r][c].YLocation,
                                    ReadOnly = true,
                                    NeighbourCells = grid.Rows[r][c].NeighbourCells
                                };
                                if (gridSave.Rows[r][c].Num == 0)
                                { gridSave.Rows[r][c].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }; }
                                else
                                { gridSave.Rows[r][c].Candidates = grid.Rows[r][c].Candidates; }
                            }
                        }
                        reverseCandidates = false;
                        if (BruteForceSolve(grid, 0, 0))
                        {
                            string firstSolution = SudokuToString(grid);
                            reverseCandidates = true;
                            SudokuGrid grid2 = new SudokuGrid() { PuzzleID = gridSave.PuzzleID };
                            grid2.Rows = new Cell[9][];
                            for (int r = 0; r < grid2.Rows.Length; r++)
                            {
                                grid2.Rows[r] = new Cell[9];
                                for (int c = 0; c < grid2.Rows[r].Length; c++)
                                {
                                    grid2.Rows[r][c] = new Cell()
                                    {
                                        Num = gridSave.Rows[r][c].Num,
                                        BlockLoc = gridSave.Rows[r][c].BlockLoc,
                                        XLocation = gridSave.Rows[r][c].XLocation,
                                        YLocation = gridSave.Rows[r][c].YLocation,
                                        ReadOnly = true,
                                        NeighbourCells = gridSave.Rows[r][c].NeighbourCells
                                    };
                                    if (grid2.Rows[r][c].Num == 0)
                                    { grid2.Rows[r][c].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }; }
                                    else
                                    { grid2.Rows[r][c].Candidates = gridSave.Rows[r][c].Candidates; }
                                }
                            }
                            BruteForceSolve(grid2, 0, 0);
                            string secondSolution = SudokuToString(grid2);
                            if (firstSolution == secondSolution)//If puzzle is valid
                            {
                                numbersToRemove -= 2;
                                gridSave.Rows[randRow][randCol].Candidates = new List<int> { 1, 2, 3, 4, 5, 6 , 7, 8, 9 };
                                gridSave.Rows[altRow][altCol].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                                grid = gridSave;
                                removed += 2;
                                return grid;
                            }
                            else//If puzzle has multiple solutions
                            {
                                grid = gridSave;
                                grid.Rows[randRow][randCol].Num = saveNum;
                                grid.Rows[randRow][randCol].Candidates = new List<int> { };
                                grid.Rows[altRow][altCol].Num = altSaveNum;
                                grid.Rows[altRow][altCol].Candidates = new List<int> { };
                                escapeCounter++;
                                return grid;
                            }
                        }
                        else//If puzzle is invalid
                        {
                            grid = gridSave;
                            grid.Rows[randRow][randCol].Num = saveNum;
                            grid.Rows[randRow][randCol].Candidates = new List<int> { };
                            grid.Rows[altRow][altCol].Num = altSaveNum;
                            grid.Rows[altRow][altCol].Candidates = new List<int> { };
                            escapeCounter++;
                            return grid;
                        }

                    }
                    return grid;
                }
                /*public SudokuGrid ToughenPuzzle(SudokuGrid grid, int x, int y)
                {
                    int saveNum = 0;

                    saveNum = grid.Rows[x][y].Num;
                    grid.Rows[x][y].Num = 0;
                    grid.Rows[x][y].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                    //Create a copy of the grid after a number is removed
                    SudokuGrid gridSave = new SudokuGrid() { PuzzleID = grid.PuzzleID };
                    gridSave.Rows = new Cell[9][];
                    for (int r = 0; r < gridSave.Rows.Length; r++)
                    {
                        gridSave.Rows[r] = new Cell[9];
                        for (int c = 0; c < gridSave.Rows[r].Length; c++)
                        {
                            gridSave.Rows[r][c] = new Cell()
                            {
                                Num = grid.Rows[r][c].Num,
                                BlockLoc = grid.Rows[r][c].BlockLoc,
                                XLocation = grid.Rows[r][c].XLocation,
                                YLocation = grid.Rows[r][c].YLocation,
                                ReadOnly = true,
                                NeighbourCells = grid.Rows[r][c].NeighbourCells
                            };
                            if (gridSave.Rows[r][c].Num == 0)
                            { gridSave.Rows[r][c].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }; }
                            else
                            { gridSave.Rows[r][c].Candidates = grid.Rows[r][c].Candidates; }
                        }
                    }
                    reverseCandidates = false;
                    if (BruteForceSolve(grid, 0, 0))
                    {
                        string firstSolution = SudokuToString(grid);
                        reverseCandidates = true;
                        SudokuGrid grid2 = new SudokuGrid() { PuzzleID = gridSave.PuzzleID };
                        grid2.Rows = new Cell[9][];
                        for (int r = 0; r < grid2.Rows.Length; r++)
                        {
                            grid2.Rows[r] = new Cell[9];
                            for (int c = 0; c < grid2.Rows[r].Length; c++)
                            {
                                grid2.Rows[r][c] = new Cell()
                                {
                                    Num = gridSave.Rows[r][c].Num,
                                    BlockLoc = gridSave.Rows[r][c].BlockLoc,
                                    XLocation = gridSave.Rows[r][c].XLocation,
                                    YLocation = gridSave.Rows[r][c].YLocation,
                                    ReadOnly = true,
                                    NeighbourCells = gridSave.Rows[r][c].NeighbourCells
                                };
                                if (grid2.Rows[r][c].Num == 0)
                                { grid2.Rows[r][c].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }; }
                                else
                                { grid2.Rows[r][c].Candidates = gridSave.Rows[r][c].Candidates; }
                            }
                        }
                        BruteForceSolve(grid2, 0, 0);
                        string secondSolution = SudokuToString(grid2);
                        if (firstSolution == secondSolution)//If puzzle is valid
                        {
                            numbersToRemove--;
                            gridSave.Rows[x][y].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                            grid = gridSave;
                            removed++;
                            return grid;
                        }
                        else//If puzzle has multiple solutions
                        {
                            grid = gridSave;
                            grid.Rows[x][y].Num = saveNum;
                            grid.Rows[x][y].Candidates = new List<int> { };
                            return grid;
                        }
                    }
                    else//If puzzle is invalid
                    {
                        grid = gridSave;
                        grid.Rows[x][y].Num = saveNum;
                        grid.Rows[x][y].Candidates = new List<int> { };
                        return grid;
                    }
                }
                */
        /*
        public bool BruteForceSolve(SudokuGrid grid, int row, int col)
        {
            PuzzleGenerator gen = new PuzzleGenerator();
            if (col == 9 && row == 9)
            {
                if (gen.CheckIfSolved(grid))
                {
                    return true;
                }
                else
                {
                    grid.Rows[--row][--col].Num = 0;
                    return false;
                }
            }
            //Searches for next empty cell
            if (grid.Rows[row][col].Num != 0)
            {
                bool emptyCell = false;
                do
                {
                    if (++col == 9)
                    {
                        if (++row == 9)
                        {
                            if (gen.CheckIfSolved(grid))
                            {
                                return true;
                            }
                            else
                            {
                                grid.Rows[--row][--col].Num = 0;
                                return false;
                            }
                        }
                        else
                            col = 0;

                    }
                    if (grid.Rows[row][col].Num == 0)
                        emptyCell = true;
                } while (!emptyCell);
            }
            int nextRow = row, nextCol = col;
            if (++nextCol == 9)
            {
                if (++nextRow == 9)
                {

                    foreach (int candidate in grid.Rows[row][col].Candidates)
                    {
                        grid.Rows[row][col].Num = candidate;
                        if (gen.CheckIfSolved(grid))
                        {
                            return true;
                        }
                        else
                        {
                            grid.Rows[row][col].Num = 0;
                            return false;
                        }

                    }
                }
                else nextCol = 0;
            }
            if (grid.Rows[row][col].Num == 0)
            {
                if (reverseCandidates)
                {
                    grid.Rows[row][col].Candidates.Reverse();
                }
                foreach (int candidate in grid.Rows[row][col].Candidates)
                {
                    grid.Rows[row][col].Num = candidate;
                    if (BruteForceSolve(grid, nextRow, nextCol))
                        return true;
                }
            }
            else
            {
                if (BruteForceSolve(grid, nextRow, nextCol))
                    return true;
                else
                {
                    grid.Rows[row][col].Num = 0;
                    return false;
                }
            }
            grid.Rows[row][col].Num = 0;
            return false;//gets hit if each brute force attempt with each 'candidate' returns false in the foreach
        }
        */
        /*
        public bool BruteSolve(SudokuGrid grid, Random rand)
        {
            bool changeMade;
            do
            {
                changeMade = false;
                for (int r = 0; r < 9; r++)//Time Complexity: O(9^2+3^2)
                {
                    for (int c = 0; c < 9; c++)
                    {
                        if (grid.Rows[r][c].Num == 0)
                        {
                            //Start checking the rows, columns or block, eliminating numbers from the candidate list
                            //If only one candidate remains, it must the answer. If multiple candidates remain, move on for now.
                            //int numTest = 0;
                            
                            //Original
                            /*for (int n = 0; n < 9; n++)
                            {
                                if (grid.Rows[r][c].Candidates.Contains(grid.Rows[r][n].Num))
                                {
                                    grid.Rows[r][c].Candidates.Remove(grid.Rows[r][n].Num);
                                    changeMade = true;
                                }
                                if (grid.Rows[r][c].Candidates.Contains(grid.Rows[n][c].Num))
                                {
                                    grid.Rows[r][c].Candidates.Remove(grid.Rows[n][c].Num);
                                    changeMade = true;
                                }
                            }
                            if (grid.Rows[r][c].Candidates.Count > 1)
                            {

                                int[] blockIndexes = BlockIndexGetter(grid.Rows[r][c].BlockLoc);

                                for (int x = blockIndexes[0]; x < blockIndexes[0] + 3; x++)
                                {
                                    for (int y = blockIndexes[1]; y < blockIndexes[1] + 3; y++)
                                    {
                                        if (grid.Rows[r][c].Candidates.Contains(grid.Rows[x][y].Num))
                                        {
                                            grid.Rows[r][c].Candidates.Remove(grid.Rows[x][y].Num);
                                            changeMade = true;
                                        }
                                    }
                                }
                            }
                            if (grid.Rows[r][c].Candidates.Count == 1)
                            {
                                //grid.Rows[r][c].Num = grid.Rows[r][c].Candidates[0];
                                numTest = grid.Rows[r][c].Candidates[0];
                                grid.Rows[r][c].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                                changeMade = true;
                                //break;
                            }
                            //Alternative
                            for (int index = 0; index < 3; index++)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    if (grid.Rows[r][c].Candidates.Contains(grid.Rows[r][c].NeighbourCells[index][i].Num))
                                    {
                                        grid.Rows[r][c].Candidates.Remove(grid.Rows[r][c].NeighbourCells[index][i].Num);
                                        changeMade = true;
                                    }
                                    if (grid.Rows[r][c].Candidates.Count == 1)
                                    {
                                        grid.Rows[r][c].Num = grid.Rows[r][c].Candidates[0];
                                        changeMade = true;
                                        break;
                                    }
                                }
                            }
                        }

                    }
                }
            } while (changeMade);



            int solutionCount = 0;

            if (!CheckIfSolved(grid))
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (grid.Rows[i][j].Num == 0)
                        {
                            solutionCount = 0;
                            //grid.Rows[i][j].Candidates = Shuffler(grid.Rows[i][j].Candidates, rand);
                            for (int k = 0; k < grid.Rows[i][j].Candidates.Count; k++)
                            {
                                grid.Rows[i][j].Num = grid.Rows[i][j].Candidates[k];
                                if (BruteSolve(grid, rand) && ++solutionCount > 1)
                                {
                                    return false;
                                }
                                else if (k == grid.Rows[i][j].Candidates.Count - 1)
                                {
                                    grid.Rows[i][j].Num = 0;
                                    grid.Rows[i][j].Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return true;
            }

            if (CheckIfSolved(grid) && solutionCount < 2)
            {
                return true;
            }
            else if (solutionCount > 1)
            {
                return false;
            }
            else
            {
                return false;
            }
        }
        */
        public bool CheckIfSolved(SudokuGrid grid)
        {
            List<char> numberList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int row = 0; row < 9; row++)
            {
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
                else
                {
                    numberList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                }
            }

            return true;
        }
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
        public int NumberSwitch(int origNum)//method to get the number of the cell that is rotationally symmetrical to the current cell
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
        public string SudokuToString(SudokuGrid grid)//used for copying the generated puzzle to clipboard - can be imported to www.sudokuwiki.org/sudoku.htm
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
