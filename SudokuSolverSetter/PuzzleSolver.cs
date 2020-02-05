using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverSetter
{
    public class PuzzleSolver
    {
        #region Full Solver method
        public SudokuGrid Solver(SudokuGrid grid)//Once called, the solver will attempt to entirely solve the puzzle, making decisions based off the the scenarios provided.
        {
            bool changeMade = false;
            /*
             *  This do...while is necessary for repeating these methods for solving until no changes are made (which it assumes that the puzzle is complete or it could not complete it)
             *  The if and elses are to make the process faster of solving faster, 
                as it ensures that it tries the easiest less computationally heavy methods first before the more complex methods.
            */
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
                else
                {
                    changeMade = false;
                }
            } while (changeMade);
            
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
                                    changeMade = true;
                                }
                            }
                        }
                        if (grid.Rows[i][j].Candidates.Count == 1)
                        {
                            changeMade = true;
                            grid.Rows[i][j].Num = grid.Rows[i][j].Candidates[0];
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
                                { curCell.Candidates.Remove(cellWithPair.Candidates[0]); changeMade = true; }
                                if (!cellWithPair.Equals(curCell) && !curCell.Equals(grid.Rows[i][j]) && curCell.Candidates.Contains(cellWithPair.Candidates[1]))
                                { curCell.Candidates.Remove(cellWithPair.Candidates[1]); changeMade = true; }
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
                                { grid.Rows[k][j].Candidates.Remove(cellWithPair.Candidates[0]); changeMade = true; }
                                if (!cellWithPair.Equals(grid.Rows[k][j]) && !grid.Rows[k][j].Equals(grid.Rows[i][j]) && grid.Rows[k][j].Candidates.Contains(cellWithPair.Candidates[1]))
                                { grid.Rows[k][j].Candidates.Remove(cellWithPair.Candidates[1]); changeMade = true; }
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
                                    { grid.Rows[x][y].Candidates.Remove(cellWithPair.Candidates[0]); changeMade = true; }
                                    if (!cellWithPair.Equals(grid.Rows[x][y]) && !grid.Rows[x][y].Equals(grid.Rows[i][j]) && grid.Rows[x][y].Candidates.Contains(cellWithPair.Candidates[1]))
                                    { grid.Rows[x][y].Candidates.Remove(cellWithPair.Candidates[1]); changeMade = true; }
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
                            if (grid.Rows[i][h].Candidates.Contains(n + 1))
                            {
                                grid.Rows[i][h].Num = n + 1;
                                grid.Rows[i][h].Candidates.Clear();
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
                            if (grid.Rows[h][colu].Candidates.Contains(n + 1))
                            {
                                grid.Rows[h][colu].Num = n + 1;
                                grid.Rows[h][colu].Candidates.Clear();
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
                                if (grid.Rows[x][y].Candidates.Contains(n + 1))
                                {
                                    grid.Rows[x][y].Num = n + 1;//Assigns the number of the cell with the hidden single
                                    grid.Rows[x][y].Candidates.Clear();
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
                                if (grid.Rows[x][y].Candidates.Contains(n + 1))
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
                                if (grid.Rows[xLoc][c].Candidates.Contains(n + 1) && cell1.YLocation != c && cell2.YLocation != c)
                                { grid.Rows[xLoc][c].Candidates.Remove(n + 1); changeMade = true; }
                            }
                        }
                        else if (cell1.YLocation == cell2.YLocation)
                        {
                            int yLoc = cell1.YLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[c][yLoc].Candidates.Contains(n + 1) && cell1.XLocation != c && cell2.XLocation != c)
                                { grid.Rows[c][yLoc].Candidates.Remove(n + 1); changeMade = true; }
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
                                if (grid.Rows[x][y].Candidates.Contains(n + 1))
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
                                if (grid.Rows[xLoc][c].Candidates.Contains(n + 1) && cell1.YLocation != c && cell2.YLocation != c && cell3.YLocation != c)
                                { grid.Rows[xLoc][c].Candidates.Remove(n + 1); changeMade = true; }
                            }
                        }
                        else if (cell1.YLocation == cell2.YLocation && cell1.YLocation == cell3.YLocation)
                        {
                            int yLoc = cell1.YLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[c][yLoc].Candidates.Contains(n + 1) && cell1.XLocation != c && cell2.XLocation != c && cell3.XLocation != c)
                                { grid.Rows[c][yLoc].Candidates.Remove(n + 1); changeMade = true; }
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



            return changeMade;
        }
        /*Chaining:
         * 
        */
        /*X-Wing Strategy:
         * 
         */
        #endregion
        #region Temporary Solver used in puzzle generator
        public int SolveACell(int[] position, SudokuGrid grid)//Used in the generator - unfinshied!
        {
            int cellNum = 0;
            List<int> numberList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (Cell[] item in grid.Rows)
            {
                foreach (Cell cell in item)
                {
                    if ((grid.Rows[position[0]][position[1]].XLocation == cell.XLocation || grid.Rows[position[0]][position[1]].YLocation == cell.YLocation || grid.Rows[position[0]][position[1]].BlockLoc == cell.BlockLoc) && numberList.Contains(cell.Num))
                    {
                        numberList.Remove(cell.Num);
                    }
                }
            }
            grid.Rows[position[0]][position[1]].Candidates = numberList;
            if (numberList.Count == 1)
            {
                grid.Rows[position[0]][position[1]].Num = numberList[0];
                cellNum = numberList[0];
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
                            if (grid.Rows[i][h].Candidates.Contains(n + 1))
                            {
                                grid.Rows[i][h].Num = n + 1;
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
                            if (grid.Rows[h][colu].Candidates.Contains(n + 1))
                            {
                                grid.Rows[h][colu].Num = n + 1;
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
                                if (grid.Rows[x][y].Candidates.Contains(n + 1))
                                {
                                    grid.Rows[x][y].Num = n + 1;
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
                                if (grid.Rows[x][y].Candidates.Contains(n + 1))
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
                                if (grid.Rows[xLoc][c].Candidates.Contains(n + 1) && cell1.YLocation != c && cell2.YLocation != c)
                                { grid.Rows[xLoc][c].Candidates.Remove(n + 1); return true; }
                            }
                        }
                        else if (cell1.YLocation == cell2.YLocation)
                        {
                            int yLoc = cell1.YLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[c][yLoc].Candidates.Contains(n + 1) && cell1.XLocation != c && cell2.XLocation != c)
                                { grid.Rows[c][yLoc].Candidates.Remove(n + 1); return true; }
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
                                if (grid.Rows[x][y].Candidates.Contains(n + 1))
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
                                if (grid.Rows[xLoc][c].Candidates.Contains(n + 1) && cell1.YLocation != c && cell2.YLocation != c && cell3.YLocation != c)
                                { grid.Rows[xLoc][c].Candidates.Remove(n + 1); return true; }
                            }
                        }
                        else if (cell1.YLocation == cell2.YLocation && cell1.YLocation == cell3.YLocation)
                        {
                            int yLoc = cell1.YLocation;
                            for (int c = 0; c < 9; c++)
                            {
                                if (grid.Rows[c][yLoc].Candidates.Contains(n + 1) && cell1.XLocation != c && cell2.XLocation != c && cell3.XLocation != c)
                                { grid.Rows[c][yLoc].Candidates.Remove(n + 1); return true; }
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
    }
}
