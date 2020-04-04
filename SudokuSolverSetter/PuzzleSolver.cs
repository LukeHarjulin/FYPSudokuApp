using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SudokuSolverSetter
{
    public class PuzzleSolver
    {
        #region Full Solver method
        public SudokuGrid Solver(SudokuGrid grid, bool bruteForce)//Once called, the solver will attempt to entirely solve the puzzle, making decisions based off the the scenarios provided.
        {
            DeveloperWindow mainWindow = new DeveloperWindow();
            bool changeMade = false;
            /*
             *  This do...while is necessary for repeating these methods for solving until no changes are made (which it assumes that the puzzle is complete or it could not complete it)
             *  The if and elses are to make the process faster of solving faster, 
                as it ensures that it tries the easiest less computationally heavy methods first before the more complex methods.
            */
            if (!bruteForce)
            {
                do
                {
                    if (FindNakedNumbers(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindHiddenNumbers(grid))
                    {
                        changeMade = true;
                    }
                    //More methods to add
                    else if (FindXWing(grid))
                    {
                        changeMade = true;
                    }
                    else
                    {
                        changeMade = false;
                    }
                } while (changeMade);
            }
            else
            {
                BruteForceSolve(grid, 0, 0, 0);
            }


            return grid;
        }


        private bool FindNakedNumbers(SudokuGrid grid)//This method searches through all empty cells and revaluates the candidates for each cell. If there is only one candidate for a cell, it must be that number.
        {
            bool changeMade = false;//Used to flag if a number has been discovered, so the grid can be checked again

            for (int i = 0; i < grid.Rows.Length; i++)
            {
                for (int j = 0; j < grid.Rows[i].Length; j++)
                {
                    if (grid.Rows[i][j].Num == 0)
                    {
                        //Start checking the rows, columns or block, eliminating numbers from the candidate list
                        //If only one candidate remains, it must the answer. If multiple candidates remain, move on for now.
                        foreach (Cell[] item in grid.Rows)
                        {
                            foreach (Cell cell in item)
                            {
                                if (grid.Rows[i][j].Candidates.Contains(cell.Num) &&
                                    (grid.Rows[i][j].XLocation == cell.XLocation || grid.Rows[i][j].YLocation == cell.YLocation || grid.Rows[i][j].BlockLoc == cell.BlockLoc))
                                {
                                    grid.Rows[i][j].Candidates.Remove(cell.Num);
                                    //mainWindow.PopulateGrid(grid);
                                    changeMade = true;
                                }
                            }
                        }
                        if (grid.Rows[i][j].Candidates.Count == 1)
                        {
                            changeMade = true;
                            grid.Rows[i][j].Num = grid.Rows[i][j].Candidates[0];
                            //mainWindow.PopulateGrid(grid);
                        }
                        //If two candidates remain, then it attempts to find a naked pair (aka a Conjugate Pair) by calling the FindNakedPair() method
                        else if (grid.Rows[i][j].Candidates.Count == 2)
                        {
                            if (FindNakedPair(grid, grid.Rows[i][j]))
                            {
                                changeMade = true;
                            }
                        }
                    }

                }
            }

            return changeMade;
        }

        private bool FindNakedPair(SudokuGrid grid, Cell cellWithPair)
        {
            bool changeMade = false;
            int nakedPairCount = 0;
            for (int i = 0; i < grid.Rows.Length; i++)
            {
                for (int j = 0; j < grid.Rows[i].Length; j++)
                {
                    if (cellWithPair.Candidates.SequenceEqual(grid.Rows[i][j].Candidates) && !cellWithPair.Equals(grid.Rows[i][j]))
                    {
                        if (cellWithPair.XLocation == grid.Rows[i][j].XLocation && cellWithPair != grid.Rows[i][j])
                        {
                            nakedPairCount++;
                            foreach (Cell curCell in grid.Rows[i])
                            {
                                if (!cellWithPair.Equals(curCell) && !curCell.Equals(grid.Rows[i][j]) && curCell.Candidates.Contains(cellWithPair.Candidates[0]))
                                {
                                    curCell.Candidates.Remove(cellWithPair.Candidates[0]);
                                    changeMade = true;
                                    //mainWindow.PopulateGrid(grid);
                                }
                                if (!cellWithPair.Equals(curCell) && !curCell.Equals(grid.Rows[i][j]) && curCell.Candidates.Contains(cellWithPair.Candidates[1]))
                                {
                                    curCell.Candidates.Remove(cellWithPair.Candidates[1]);
                                    changeMade = true;
                                    //mainWindow.PopulateGrid(grid);
                                }
                            }
                            //if (!changeMade)
                            //{
                            //    FindUniqueRectangle(grid, cellWithPair, grid.Rows[i][j]);
                            //}
                            if (nakedPairCount > 1 && changeMade == false)
                            {
                                nakedPairCount = nakedPairCount;
                            }
                        }
                        if (cellWithPair.YLocation == grid.Rows[i][j].YLocation && cellWithPair != grid.Rows[i][j])
                        {
                            nakedPairCount++;
                            for (int k = 0; k < grid.Rows.Length; k++)
                            {
                                if (!cellWithPair.Equals(grid.Rows[k][j]) && !grid.Rows[k][j].Equals(grid.Rows[i][j]) && grid.Rows[k][j].Candidates.Contains(cellWithPair.Candidates[0]))
                                {
                                    grid.Rows[k][j].Candidates.Remove(cellWithPair.Candidates[0]);
                                    changeMade = true;
                                    //mainWindow.PopulateGrid(grid);
                                }
                                if (!cellWithPair.Equals(grid.Rows[k][j]) && !grid.Rows[k][j].Equals(grid.Rows[i][j]) && grid.Rows[k][j].Candidates.Contains(cellWithPair.Candidates[1]))
                                {
                                    grid.Rows[k][j].Candidates.Remove(cellWithPair.Candidates[1]);
                                    changeMade = true;
                                    //mainWindow.PopulateGrid(grid);
                                }
                            }
                            //if (!changeMade)
                            //{
                            //    FindUniqueRectangle(grid, cellWithPair, grid.Rows[i][j]);
                            //}
                            if (nakedPairCount > 1 && changeMade == false)
                            {
                                nakedPairCount = nakedPairCount;
                            }
                        }
                        if (cellWithPair.BlockLoc == grid.Rows[i][j].BlockLoc && cellWithPair != grid.Rows[i][j])
                        {
                            nakedPairCount++;
                            int xStart = 0, yStart = 0;
                            switch (cellWithPair.BlockLoc)
                            {
                                case 1:
                                    xStart = 0;
                                    yStart = 0;
                                    break;
                                case 2:
                                    xStart = 0;
                                    yStart = 3;
                                    break;
                                case 3:
                                    xStart = 0;
                                    yStart = 6;
                                    break;
                                case 4:
                                    xStart = 3;
                                    yStart = 0;
                                    break;
                                case 5:
                                    xStart = 3;
                                    yStart = 3;
                                    break;
                                case 6:
                                    xStart = 3;
                                    yStart = 6;
                                    break;
                                case 7:
                                    xStart = 6;
                                    yStart = 0;
                                    break;
                                case 8:
                                    xStart = 6;
                                    yStart = 3;
                                    break;
                                case 9:
                                    xStart = 6;
                                    yStart = 6;
                                    break;
                                default:
                                    break;
                            }
                            for (int x = xStart; x < xStart + 3; x++)
                            {
                                for (int y = yStart; y < yStart + 3; y++)
                                {
                                    if (!cellWithPair.Equals(grid.Rows[x][y]) && !grid.Rows[x][y].Equals(grid.Rows[i][j]) && grid.Rows[x][y].Candidates.Contains(cellWithPair.Candidates[0]))
                                    {
                                        grid.Rows[x][y].Candidates.Remove(cellWithPair.Candidates[0]);
                                        changeMade = true;
                                        //mainWindow.PopulateGrid(grid);
                                    }
                                    if (!cellWithPair.Equals(grid.Rows[x][y]) && !grid.Rows[x][y].Equals(grid.Rows[i][j]) && grid.Rows[x][y].Candidates.Contains(cellWithPair.Candidates[1]))
                                    {
                                        grid.Rows[x][y].Candidates.Remove(cellWithPair.Candidates[1]);
                                        changeMade = true;
                                        //mainWindow.PopulateGrid(grid);
                                    }
                                }
                            }
                            if (nakedPairCount > 1 && changeMade == false)
                            {
                                nakedPairCount = nakedPairCount;
                            }
                            //if (!changeMade)
                            //{
                            //    FindUniqueRectangle(grid, cellWithPair, grid.Rows[i][j]);
                            //}
                        }
                    }


                }
            }

            return changeMade;
        }
        private bool FindHiddenNumbers(SudokuGrid grid)
        {
            bool changeMade = false;

            for (int i = 0; i < grid.Rows.Length; i++)//Finding Hidden Singles in rows
            {
                int[] freqOfEachCandi = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int j = 0; j < grid.Rows[i].Length; j++)
                {

                    for (int l = 0; l < grid.Rows[i][j].Candidates.Count; l++)
                    {
                        if (grid.Rows[i][j].Num == 0)
                        {
                            freqOfEachCandi[grid.Rows[i][j].Candidates[l] - 1]++;
                        }
                    }

                }
                for (int n = 0; n < freqOfEachCandi.Length; n++)
                {
                    if (freqOfEachCandi[n] == 1)
                    {
                        for (int h = 0; h < grid.Rows[i].Length; h++)
                        {
                            if (grid.Rows[i][h].Candidates.Contains((n + 1).ToString()[0]))
                            {
                                grid.Rows[i][h].Num = (n + 1).ToString()[0];
                                grid.Rows[i][h].Candidates.Clear();
                                //mainWindow.PopulateGrid(grid);
                                changeMade = true;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            for (int colu = 0; colu < 9; colu++)//Finding Hidden Singles in columns
            {
                int[] freqOfEachCandi = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int row = 0; row < 9; row++)
                {
                    for (int l = 0; l < grid.Rows[row][colu].Candidates.Count; l++)
                    {
                        if (grid.Rows[row][colu].Num == 0)
                        {
                            freqOfEachCandi[grid.Rows[row][colu].Candidates[l] - 1]++;
                        }
                    }

                }
                for (int n = 0; n < 9; n++)
                {
                    if (freqOfEachCandi[n] == 1)
                    {
                        for (int h = 0; h < 9; h++)
                        {
                            if (grid.Rows[h][colu].Candidates.Contains((n + 1).ToString()[0]))
                            {
                                grid.Rows[h][colu].Num = (n + 1).ToString()[0];
                                grid.Rows[h][colu].Candidates.Clear();
                                //mainWindow.PopulateGrid(grid);
                                changeMade = true;
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            int xStart = 0, yStart = 0;
            for (int sg = 0; sg < 9; sg++)//Finding Hidden Singles in blocks
            {
                int[] freqOfEachCandi = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int x = xStart; x < xStart + 3; x++)//Iterates through the blocks rows
                {
                    for (int y = yStart; y < yStart + 3; y++)//Iterates through the blocks columns
                    {
                        for (int l = 0; l < grid.Rows[x][y].Candidates.Count; l++)
                        {
                            if (grid.Rows[x][y].Num == 0)
                            {
                                freqOfEachCandi[grid.Rows[x][y].Candidates[l] - 1]++;//Increments the frequency of the value that corresponds with the candidate number - 1 (arrays index at 0)
                            }
                        }
                    }
                }
                for (int n = 0; n < 9; n++)
                {
                    if (freqOfEachCandi[n] == 1)//Hidden singles obivously should only frequent once in 
                    {
                        for (int x = xStart; x < xStart + 3; x++)//Iterates through the blocks rows
                        {
                            for (int y = yStart; y < yStart + 3; y++)//Iterates through the blocks columns
                            {
                                if (grid.Rows[x][y].Candidates.Contains((n + 1).ToString()[0]))
                                {
                                    grid.Rows[x][y].Num = (n + 1).ToString()[0];//Assigns the number of the cell with the hidden single
                                    grid.Rows[x][y].Candidates.Clear();
                                    //mainWindow.PopulateGrid(grid);
                                    changeMade = true;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    if (freqOfEachCandi[n] == 2)//Hidden Pairs
                    {
                        //Storage for the pair of cells
                        Cell cell1 = new Cell();
                        Cell cell2 = new Cell();
                        int counter = 0;//Counter used to assign cell1 with the first cell and cell2 with the second.

                        for (int x = xStart; x < xStart + 3; x++)//Iterates through the blocks rows
                        {
                            for (int y = yStart; y < yStart + 3; y++)//Iterates through the blocks columns
                            {
                                if (grid.Rows[x][y].Candidates.Contains((n + 1).ToString()[0]))
                                {
                                    counter++;
                                    if (counter == 1)
                                    {
                                        cell1 = grid.Rows[x][y];
                                    }
                                    else
                                    {
                                        cell2 = grid.Rows[x][y];
                                    }
                                }
                            }
                        }
                        if (cell1.XLocation == cell2.XLocation)
                        {
                            int xLoc = cell1.XLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[xLoc][c].Candidates.Contains((n + 1).ToString()[0]) && cell1.YLocation != c && cell2.YLocation != c)
                                {
                                    grid.Rows[xLoc][c].Candidates.Remove((n + 1).ToString()[0]);
                                    changeMade = true;
                                    //mainWindow.PopulateGrid(grid);
                                }
                            }
                        }
                        else if (cell1.YLocation == cell2.YLocation)
                        {
                            int yLoc = cell1.YLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[c][yLoc].Candidates.Contains((n + 1).ToString()[0]) && cell1.XLocation != c && cell2.XLocation != c)
                                {
                                    grid.Rows[c][yLoc].Candidates.Remove((n + 1).ToString()[0]);
                                    changeMade = true;
                                    //mainWindow.PopulateGrid(grid);
                                }
                            }
                        }
                    }
                    if (freqOfEachCandi[n] == 3)//Hidden Triples
                    {
                        Cell cell1 = new Cell();
                        Cell cell2 = new Cell();
                        Cell cell3 = new Cell();
                        int counter = 0;
                        for (int x = xStart; x < xStart + 3; x++)
                        {
                            for (int y = yStart; y < yStart + 3; y++)
                            {
                                if (grid.Rows[x][y].Candidates.Contains((n + 1).ToString()[0]))
                                {
                                    counter++;
                                    if (counter == 1)
                                    { cell1 = grid.Rows[x][y]; }
                                    else if (counter == 2)
                                    { cell2 = grid.Rows[x][y]; }
                                    else
                                    { cell3 = grid.Rows[x][y]; }
                                }
                            }
                        }
                        if (cell1.XLocation == cell2.XLocation && cell1.XLocation == cell3.XLocation)
                        {
                            int xLoc = cell1.XLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[xLoc][c].Candidates.Contains((n + 1).ToString()[0]) && cell1.YLocation != c && cell2.YLocation != c && cell3.YLocation != c)
                                {
                                    grid.Rows[xLoc][c].Candidates.Remove((n + 1).ToString()[0]);
                                    changeMade = true;
                                    //mainWindow.PopulateGrid(grid);
                                }
                            }
                        }
                        else if (cell1.YLocation == cell2.YLocation && cell1.YLocation == cell3.YLocation)
                        {
                            int yLoc = cell1.YLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[c][yLoc].Candidates.Contains((n + 1).ToString()[0]) && cell1.XLocation != c && cell2.XLocation != c && cell3.XLocation != c)
                                {
                                    grid.Rows[c][yLoc].Candidates.Remove((n + 1).ToString()[0]);
                                    changeMade = true;
                                    //mainWindow.PopulateGrid(grid);
                                }
                            }
                        }
                    }
                }
                yStart += 3;
                if (yStart == 9)
                {
                    yStart = 0;
                    xStart += 3;
                }
            }

            return changeMade;
        }

        /*Rule for Type1 Unique Rectangle:
         *There must four pairs in a rectangle that spans two blocks.
         *All of the pairs must be naked, apart from one.
         *The cell that contains more than just the pair is able to get rid of the pair from the candidate number list.
         * Reason: A sudoku puzzle cannot contain four conjugate pairs as there can only be one solution to the puzzle. 
         * If numbers are interchangeable, that means there is more than one solution
        */
        public bool FindUniqueRectangle(SudokuGrid grid, Cell pair1, Cell pair2)
        {
            bool changeMade = false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (true)
                    {

                    }
                }
            }

            return changeMade;
        }
        /*Chaining:
         * 
        */

        /// <summary>
        /// X-Wing Strategy: The X-Wing strategy looks at candidate numbers that in rows and columns. 
        /// The X-Wing strategy focuses on finding a single common candidate in four different cells that do not share a block between any of the cells.
        /// Each of the four cells must share a row with one of the cells and a column with a different cell.
        /// .......
        /// </summary>
        /// <param name="grid">Passing through the SudokuGrid to examine for Xwings</param>
        /// <returns></returns>
        public bool FindXWing(SudokuGrid grid)
        {            
            bool changeMade = false;
            List<Cell> cL = new List<Cell>(2);//cell locations
            int axis = -1;//initialise at -1 because it is incremented to 0 as the while loop starts
            char[] candiList = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            while (++axis < 2)//axis so that it can switch and check for finding x-wings in the opposite axis
            {
                foreach (char i in candiList)//iterates through numbers
                {
                    for (int x = 0; x < 9; x++)//iterates through rows
                    {
                        byte numCounter = 0, addedCounter = 0;//'numCounter' used to count how many occurences of a candidate number there are in a row/column. Added counts added numbers to cL in the current row
                        for (int y = 0; y < 9; y++)//iterates through columns
                        {                            
                            if (axis == 1 && grid.Rows[y][x].Num == 0 && grid.Rows[y][x].Candidates.Contains(i))//switch for finding x-wings in the opposite axis
                            {
                                if (++numCounter > 2)
                                {
                                    cL.RemoveRange(cL.Count - addedCounter, addedCounter);
                                    break;
                                }
                                if (!GroupShare(cL, grid, y, x, axis))
                                {
                                    addedCounter++;
                                    cL.Add(grid.Rows[y][x]);
                                }
                            }
                            else if (grid.Rows[x][y].Num == 0 && grid.Rows[x][y].Candidates.Contains(i))
                            {
                                if (++numCounter > 2)
                                {
                                    cL.RemoveRange(cL.Count - addedCounter, addedCounter);
                                    break;
                                }
                                if (!GroupShare(cL, grid, x, y, axis))
                                {
                                    addedCounter++;
                                    cL.Add(grid.Rows[x][y]);
                                }
                            }
                        }
                        if (cL.Count == 2)
                        {
                            axis = axis == 0 ? 1 : 0;//need to swap axis values to access row/column instead of column/row
                            for (int k = 0; k < 8; k++)
                            {
                                if (cL[0].NeighbourCells[axis][k].Candidates.Contains(i) && cL[1].NeighbourCells[axis][k].Candidates.Contains(i)) 
                                {
                                    int m = 0;
                                    if (axis == 1)
                                    {
                                        for (int l = 0; l < 8; l++)
                                        {
                                            if (cL[0].NeighbourCells[1][k].NeighbourCells[0][l].Candidates.Contains(i))
                                            {
                                                if (++m > 1)
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int l = 0; l < 8; l++)
                                        {
                                            if (cL[0].NeighbourCells[0][k].NeighbourCells[1][l].Candidates.Contains(i))
                                            {
                                                if (++m > 1)
                                                    break;
                                            }
                                        }
                                    }
                                    
                                    if (m==1)//if only 1 other cell's candidate values contain 'i' then attempt to remove any candidate 'i's from cells in the same column/row (depending on the orientation of the xwing)
                                    {
                                        cL.Add(cL[0].NeighbourCells[axis][k]);
                                        cL.Add(cL[1].NeighbourCells[axis][k]);
                                        //looks at the columns/rows and removes any of 'i's from the cell's candidate list
                                        for (int n = 0; n < 8; n++)
                                        {

                                            if (cL[0].NeighbourCells[axis][n].Candidates.Contains(i) && cL[2] != cL[0].NeighbourCells[axis][n])//checks column of first cell in first row of xwing
                                            {
                                                cL[0].NeighbourCells[axis][n].Candidates.Remove(i);
                                                changeMade = true;
                                            }
                                            if (cL[1].NeighbourCells[axis][n].Candidates.Contains(i) && cL[3] != cL[1].NeighbourCells[axis][n])//checks column of second cell in first row of xwing and check whether 
                                            {
                                                cL[1].NeighbourCells[axis][n].Candidates.Remove(i);
                                                changeMade = true;
                                            }
                                            
                                        }
                                        break;
                                    }
                                }
                            }
                            axis = axis == 0 ? 1 : 0;
                        }
                        
                    }
                    cL = new List<Cell>();
                    
                }
            }


            return changeMade;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cL"></param>
        /// <param name="grid"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool GroupShare(List<Cell> cL, SudokuGrid grid, int x, int y, int axis)
        {
            for (int n = 0; n < cL.Count; n++)
            {
                if (grid.Rows[x][y].BlockLoc == cL[n].BlockLoc)
                    return true;                
            }
            if (cL.Count > 1)
            {
                if (axis ==0 && y != cL[cL.Count-2].YLocation)
                    return true;
                if (axis == 1 && y != cL[cL.Count - 2].XLocation)
                    return true;
            }
            return false;
        }
        #endregion
        #region Temporary Solver used in puzzle generator
        public int SolveACell(int[] position, SudokuGrid grid)//Used in the generator - unfinshied!
        {
            int cellNum = 0;
            List<char> candidateList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            foreach (Cell[] item in grid.Rows)
            {
                foreach (Cell cell in item)
                {
                    if ((grid.Rows[position[0]][position[1]].XLocation == cell.XLocation || grid.Rows[position[0]][position[1]].YLocation == cell.YLocation || grid.Rows[position[0]][position[1]].BlockLoc == cell.BlockLoc) && candidateList.Contains(cell.Num))
                    {
                        candidateList.Remove(cell.Num);
                    }
                }
            }
            grid.Rows[position[0]][position[1]].Candidates = candidateList;
            if (candidateList.Count == 1)
            {
                grid.Rows[position[0]][position[1]].Num = candidateList[0];
                cellNum = candidateList[0];
            }
            return cellNum;
        }
        #endregion
        #region Solver for Solving Cell by Cell button
        public SudokuGrid SolveCellByCell(SudokuGrid grid)
        {
            bool changeMade = false;

            if (!FindNakedNumbers1by1(grid))
            {
                FindHiddenNumbers1by1(grid);
            }


            return grid;
        }

        private bool FindNakedNumbers1by1(SudokuGrid grid)//This method searches through all empty cells and revaluates the candidates for each cell. If there is only one candidate for a cell, it must be that number.
        {
            bool changeMade = false;//Used to flag if a number has been discovered, so the grid can be checked again

            for (int i = 0; i < grid.Rows.Length; i++)
            {
                for (int j = 0; j < grid.Rows[i].Length; j++)
                {
                    if (grid.Rows[i][j].Num == 0)
                    {
                        //Start checking the rows, columns or block, eliminating numbers from the candidate list
                        //If only one candidate remains, it must the answer. If multiple candidates remain, move on for now.
                        foreach (Cell[] item in grid.Rows)
                        {
                            foreach (Cell cell in item)
                            {
                                if (grid.Rows[i][j].Candidates.Contains(cell.Num) &&
                                    (grid.Rows[i][j].XLocation == cell.XLocation || grid.Rows[i][j].YLocation == cell.YLocation || grid.Rows[i][j].BlockLoc == cell.BlockLoc))
                                {
                                    grid.Rows[i][j].Candidates.Remove(cell.Num);

                                    changeMade = true;
                                }
                            }
                        }
                        if (grid.Rows[i][j].Candidates.Count == 1)
                        {
                            grid.Rows[i][j].Num = grid.Rows[i][j].Candidates[0];
                            changeMade = true;
                        }
                        else if (grid.Rows[i][j].Candidates.Count == 2)
                        {
                            if (FindNakedPair1by1(grid, grid.Rows[i][j]))
                            {
                                changeMade = true;
                            }
                        }
                    }
                }
            }

            return changeMade;
        }

        private bool FindNakedPair1by1(SudokuGrid grid, Cell cellWithPair)
        {
            bool changeMade = false;
            for (int i = 0; i < grid.Rows.Length; i++)
            {
                for (int j = 0; j < grid.Rows[i].Length; j++)
                {
                    if (cellWithPair.Candidates.SequenceEqual(grid.Rows[i][j].Candidates) && !cellWithPair.Equals(grid.Rows[i][j]))
                    {
                        if (cellWithPair.XLocation == grid.Rows[i][j].XLocation && cellWithPair != grid.Rows[i][j])
                        {
                            foreach (Cell curCell in grid.Rows[i])
                            {
                                if (!cellWithPair.Equals(curCell) && !curCell.Equals(grid.Rows[i][j]) && curCell.Candidates.Contains(cellWithPair.Candidates[0]))
                                { curCell.Candidates.Remove(cellWithPair.Candidates[0]); return true; }
                                if (!cellWithPair.Equals(curCell) && !curCell.Equals(grid.Rows[i][j]) && curCell.Candidates.Contains(cellWithPair.Candidates[1]))
                                { curCell.Candidates.Remove(cellWithPair.Candidates[1]); return true; }
                            }
                        }
                        if (cellWithPair.YLocation == grid.Rows[i][j].YLocation && cellWithPair != grid.Rows[i][j])
                        {
                            for (int k = 0; k < grid.Rows.Length; k++)
                            {
                                if (!cellWithPair.Equals(grid.Rows[k][j]) && !grid.Rows[k][j].Equals(grid.Rows[i][j]) && grid.Rows[k][j].Candidates.Contains(cellWithPair.Candidates[0]))
                                { grid.Rows[k][j].Candidates.Remove(cellWithPair.Candidates[0]); return true; }
                                if (!cellWithPair.Equals(grid.Rows[k][j]) && !grid.Rows[k][j].Equals(grid.Rows[i][j]) && grid.Rows[k][j].Candidates.Contains(cellWithPair.Candidates[1]))
                                { grid.Rows[k][j].Candidates.Remove(cellWithPair.Candidates[1]); return true; }
                            }
                        }
                        if (cellWithPair.BlockLoc == grid.Rows[i][j].BlockLoc)
                        {
                            int xStart = 0, yStart = 0;
                            switch (cellWithPair.BlockLoc)
                            {
                                case 1:
                                    xStart = 0;
                                    yStart = 0;
                                    break;
                                case 2:
                                    xStart = 0;
                                    yStart = 3;
                                    break;
                                case 3:
                                    xStart = 0;
                                    yStart = 6;
                                    break;
                                case 4:
                                    xStart = 3;
                                    yStart = 0;
                                    break;
                                case 5:
                                    xStart = 3;
                                    yStart = 3;
                                    break;
                                case 6:
                                    xStart = 3;
                                    yStart = 6;
                                    break;
                                case 7:
                                    xStart = 6;
                                    yStart = 0;
                                    break;
                                case 8:
                                    xStart = 6;
                                    yStart = 3;
                                    break;
                                case 9:
                                    xStart = 6;
                                    yStart = 6;
                                    break;
                                default:
                                    break;
                            }
                            for (int x = xStart; x < xStart + 3; x++)
                            {
                                for (int y = yStart; y < yStart + 3; y++)
                                {
                                    if (!cellWithPair.Equals(grid.Rows[x][y]) && !grid.Rows[x][y].Equals(grid.Rows[i][j]) && grid.Rows[x][y].Candidates.Contains(cellWithPair.Candidates[0]))
                                    { grid.Rows[x][y].Candidates.Remove(cellWithPair.Candidates[0]); return true; }
                                    if (!cellWithPair.Equals(grid.Rows[x][y]) && !grid.Rows[x][y].Equals(grid.Rows[i][j]) && grid.Rows[x][y].Candidates.Contains(cellWithPair.Candidates[1]))
                                    { grid.Rows[x][y].Candidates.Remove(cellWithPair.Candidates[1]); return true; }
                                }
                            }

                        }
                    }


                }
            }

            return changeMade;
        }
        private bool FindHiddenNumbers1by1(SudokuGrid grid)
        {
            bool changeMade = false;

            for (int i = 0; i < grid.Rows.Length; i++)//Finding Hidden Singles in rows
            {
                int[] freqOfEachCandi = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int j = 0; j < grid.Rows[i].Length; j++)
                {

                    for (int l = 0; l < grid.Rows[i][j].Candidates.Count; l++)
                    {
                        if (grid.Rows[i][j].Num == 0)
                        {
                            freqOfEachCandi[grid.Rows[i][j].Candidates[l] - 1]++;//Increments the frequency of the value that corresponds with the candidate number - 1 (arrays index at 0)
                        }
                    }

                }
                for (int n = 0; n < freqOfEachCandi.Length; n++)
                {
                    if (freqOfEachCandi[n] == 1)
                    {
                        for (int h = 0; h < grid.Rows[i].Length; h++)
                        {
                            if (grid.Rows[i][h].Candidates.Contains((n + 1).ToString()[0]))
                            {
                                grid.Rows[i][h].Num = (n + 1).ToString()[0];
                                grid.Rows[i][h].Candidates.Clear();
                                return true;
                            }
                        }
                        break;
                    }
                }
            }
            for (int colu = 0; colu < 9; colu++)//Finding Hidden Singles in columns
            {
                int[] freqOfEachCandi = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int row = 0; row < 9; row++)
                {
                    for (int l = 0; l < grid.Rows[row][colu].Candidates.Count; l++)
                    {
                        if (grid.Rows[row][colu].Num == 0)
                        {
                            freqOfEachCandi[grid.Rows[row][colu].Candidates[l] - 1]++;
                        }
                    }

                }
                for (int n = 0; n < 9; n++)
                {
                    if (freqOfEachCandi[n] == 1)
                    {
                        for (int h = 0; h < 9; h++)
                        {
                            if (grid.Rows[h][colu].Candidates.Contains((n + 1).ToString()[0]))
                            {
                                grid.Rows[h][colu].Num = (n + 1).ToString()[0];
                                grid.Rows[h][colu].Candidates.Clear();
                                return true;
                            }
                        }
                        break;
                    }
                }
            }

            int xStart = 0, yStart = 0;
            for (int sg = 0; sg < 9; sg++)//Finding Hidden Singles in blocks
            {
                int[] freqOfEachCandi = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int x = xStart; x < xStart + 3; x++)
                {
                    for (int y = yStart; y < yStart + 3; y++)
                    {
                        for (int l = 0; l < grid.Rows[x][y].Candidates.Count; l++)
                        {
                            if (grid.Rows[x][y].Num == 0)
                            {
                                freqOfEachCandi[grid.Rows[x][y].Candidates[l] - 1]++;
                            }
                        }
                    }
                }
                for (int n = 0; n < 9; n++)
                {
                    if (freqOfEachCandi[n] == 1)
                    {
                        for (int x = xStart; x < xStart + 3; x++)
                        {
                            for (int y = yStart; y < yStart + 3; y++)
                            {
                                if (grid.Rows[x][y].Candidates.Contains((n + 1).ToString()[0]))
                                {
                                    grid.Rows[x][y].Num = (n + 1).ToString()[0];
                                    grid.Rows[x][y].Candidates.Clear();
                                    return true;
                                }
                            }
                        }
                        break;
                    }
                    if (freqOfEachCandi[n] == 2)
                    {
                        Cell cell1 = new Cell();
                        Cell cell2 = new Cell();
                        int counter = 0;
                        for (int x = xStart; x < xStart + 3; x++)
                        {
                            for (int y = yStart; y < yStart + 3; y++)
                            {
                                if (grid.Rows[x][y].Candidates.Contains((n + 1).ToString()[0]))
                                {
                                    counter++;
                                    if (counter == 1)
                                    {
                                        cell1 = grid.Rows[x][y];
                                    }
                                    else
                                    {
                                        cell2 = grid.Rows[x][y];
                                    }
                                }
                            }
                        }
                        if (cell1.XLocation == cell2.XLocation)
                        {
                            int xLoc = cell1.XLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[xLoc][c].Candidates.Contains((n + 1).ToString()[0]) && cell1.YLocation != c && cell2.YLocation != c)
                                { grid.Rows[xLoc][c].Candidates.Remove((n + 1).ToString()[0]); return true; }
                            }
                        }
                        else if (cell1.YLocation == cell2.YLocation)
                        {
                            int yLoc = cell1.YLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[c][yLoc].Candidates.Contains((n + 1).ToString()[0]) && cell1.XLocation != c && cell2.XLocation != c)
                                { grid.Rows[c][yLoc].Candidates.Remove((n + 1).ToString()[0]); return true; }
                            }
                        }
                    }
                    if (freqOfEachCandi[n] == 3)
                    {
                        Cell cell1 = new Cell();
                        Cell cell2 = new Cell();
                        Cell cell3 = new Cell();
                        int counter = 0;
                        for (int x = xStart; x < xStart + 3; x++)
                        {
                            for (int y = yStart; y < yStart + 3; y++)
                            {
                                if (grid.Rows[x][y].Candidates.Contains((n + 1).ToString()[0]))
                                {
                                    counter++;
                                    if (counter == 1)
                                    { cell1 = grid.Rows[x][y]; }
                                    else if (counter == 2)
                                    { cell2 = grid.Rows[x][y]; }
                                    else
                                    { cell3 = grid.Rows[x][y]; }
                                }
                            }
                        }
                        if (cell1.XLocation == cell2.XLocation && cell1.XLocation == cell3.XLocation)
                        {
                            int xLoc = cell1.XLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[xLoc][c].Candidates.Contains((n + 1).ToString()[0]) && cell1.YLocation != c && cell2.YLocation != c && cell3.YLocation != c)
                                { grid.Rows[xLoc][c].Candidates.Remove((n + 1).ToString()[0]); return true; }
                            }
                        }
                        else if (cell1.YLocation == cell2.YLocation && cell1.YLocation == cell3.YLocation)
                        {
                            int yLoc = cell1.YLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[c][yLoc].Candidates.Contains((n + 1).ToString()[0]) && cell1.XLocation != c && cell2.XLocation != c && cell3.XLocation != c)
                                { grid.Rows[c][yLoc].Candidates.Remove((n + 1).ToString()[0]); return true; }
                            }
                        }
                    }
                }
                yStart += 3;
                if (yStart == 9)
                {
                    yStart = 0;
                    xStart += 3;
                }
            }

            return changeMade;
        }
        #endregion
        /// <summary>
        /// Removes all values from the current cell's candidate list that are also found within neighbouring cells
        /// I.e. Cells that are in the same groups as the cell in question.
        /// A group is a block/row/column
        /// </summary>
        /// <param name="grid">Sudoku grid that is passed into and mutated in the method</param>
        /// <param name="row">Current row number being examined in this instance of the method</param>
        /// <param name="col">Current column number being examined in this instance of the method</param>
        /// <returns></returns>
        public bool RemoveCands(SudokuGrid grid, int row, int col)
        {            
            for (int index = 0; index < 3; index++)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (grid.Rows[row][col].Candidates.Contains(grid.Rows[row][col].NeighbourCells[index][i].Num))
                    {
                        grid.Rows[row][col].Candidates.Remove(grid.Rows[row][col].NeighbourCells[index][i].Num);
                    }
                    if (grid.Rows[row][col].Candidates.Count == 0)
                    {
                        return false;
                    }
                }
            }
            if (grid.Rows[row][col].Candidates.Count == 1)
            {
                grid.Rows[row][col].Num = grid.Rows[row][col].Candidates[0];
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
        public bool BruteForceSolve(SudokuGrid grid, int row, int col, byte variator)
        {
            PuzzleGenerator gen = new PuzzleGenerator();

            if (col == 9 && row == 9)//If somehow the method tries to look at this non-existent cell, this catches the exception
            {
                if (gen.CheckIfSolved(grid))
                {
                    return true;
                }
                else
                {
                    grid.Rows[--row][--col].Num = '0';
                    return false;
                }
            }
            
            if (grid.Rows[row][col].Num != '0')
            {
                bool emptyCell = false;
                do//Searches for next empty cell
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
                                grid.Rows[--row][--col].Num = '0';
                                return false;
                            }
                        }
                        else
                            col = 0;

                    }
                    if (grid.Rows[row][col].Num == '0')
                        emptyCell = true;
                } while (!emptyCell);
            }
            if (variator == 0)
                grid.Rows[row][col].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            else if (variator == 1)
                grid.Rows[row][col].Candidates = new List<char> { '9', '8', '7', '6', '5', '4', '3', '2', '1' };
            else if (variator == 2)
                grid.Rows[row][col].Candidates = gen.Shuffler(new List<char> { '9', '8', '7', '6', '5', '4', '3', '2', '1' });
            //Reversed list is for checking for multiple solutions or shuffle if given the generating
            if (!RemoveCands(grid, row, col))//if it returns false, candidates count must be 0 so a contradiction is found
            {
                grid.Rows[row][col].Num = '0';                
                return false;
            }
           
            int nextRow = row, nextCol = col;
            if (++nextCol == 9)//increments the nextCol value which is used in conjunction with nextRow to look at the next cell in the sequence. If it is 9, it must be reset to 0
            {
                if (++nextRow == 9)//Currently looking at cell 81 in grid
                {
                    grid.Rows[row][col].Num = grid.Rows[row][col].Candidates[0];//Sets the last cell to be the only value possible, then the solution is checked.
                    if (gen.CheckIfSolved(grid))
                    {
                        return true;
                    }
                    else
                    {
                        grid.Rows[row][col].Num = '0';//cell value must be set to 0 to backtrack
                        return false;
                    }
                }
                else nextCol = 0;
            }
            if (grid.Rows[row][col].Num == '0')
            {
                foreach (char candidate in grid.Rows[row][col].Candidates)//iterates through each candidate value, assigning it to the current cell number.  
                {
                    grid.Rows[row][col].Num = candidate;
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
                    grid.Rows[row][col].Num = '0';
                    return false;
                }
            }
            grid.Rows[row][col].Num = '0';//cell value must be set to 0 to backtrack
            return false;//gets hit if each brute force attempt with each 'candidate' returns false in the foreach
        }
        
        public bool RemoveCands_array(char[][] grid, int row, int col, List<char> candidates)
        {
            PuzzleGenerator gen = new PuzzleGenerator();
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
        public bool BruteForceSolve_array(char[][] grid, int row, int col, byte variator)
        {
            PuzzleGenerator gen = new PuzzleGenerator();

            if (col == 9 && row == 9)//If somehow the method tries to look at this non-existent cell, this catches the exception
            {
                if (gen.CheckIfSolved_array(grid))
                {
                    return true;
                }
                else
                {
                    grid[--row][--col] = '0';
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
            if (!RemoveCands_array(grid, row, col, candidates))//if it returns false, candidates count must be 0 so a contradiction is found
            {
                grid[row][col] = '0';
                return false;
            }

            int nextRow = row, nextCol = col;
            if (++nextCol == 9)//increments the nextCol value which is used in conjunction with nextRow to look at the next cell in the sequence. If it is 9, it must be reset to 0
            {
                if (++nextRow == 9)//Currently looking at cell 81 in grid
                {
                    grid[row][col] = candidates[0];//Sets the last cell to be the only value possible, then the solution is checked.
                    if (gen.CheckIfSolved_array(grid))
                    {
                        return true;
                    }
                    else
                    {
                        grid[row][col] = '0';//cell value must be set to 0 to backtrack
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
                    //A new instance of BruteForceSolver is called using the grid with an updated value of a cell and is provided with the next cell coordinates
                    if (BruteForceSolve_array(grid, nextRow, nextCol, variator))
                        return true;
                }
            }
            else
            {
                //If the current cell contains a number found from a naked single strategy,
                //then it is dismissed and a new instance of BruteForceSolver is called and is provided with the next cell coordinates
                if (BruteForceSolve_array(grid, nextRow, nextCol, variator))
                    return true;
                else
                {//if it returns false, then the number for the cell is set back to 0, and then cycles backwards through the recursions by returning false.
                    grid[row][col] = '0';
                    return false;
                }
            }
            grid[row][col] = '0';//cell value must be set to 0 to backtrack
            return false;//gets hit if each brute force attempt with each 'candidate' returns false in the foreach
        }
    }
}
