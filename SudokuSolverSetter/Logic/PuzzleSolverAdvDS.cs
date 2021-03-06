﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Windows.Input;

namespace SudokuSolverSetter
{
    /// <summary>
    /// This class handles Sudoku solving using an advanced data structure (Cell/SudokuGrid)
    /// </summary>
    public class PuzzleSolverAdvDS
    {
        #region Initialisation
        //backtracking required?                change made?            moderate?           advanced?           add to solve path?
        public bool g_BacktrackingReq = false, g_changeMade = false, g_moderate = false, g_advanced = false, g_PathTracking = false;
        public int g_Rating = 0;//puzzle rating score
        public int g_StepCounter = 0;//number of steps to solve the puzzle
        public string g_Difficulty = "Beginner", g_strategy = "";//puzzle difficulty
        private readonly PuzzleGenerator g_Gen = new PuzzleGenerator();
        public List<string> g_SolvePath = new List<string>();//solve path using strategies
        public List<string> g_BacktrackingPath = new List<string>();//solve path using backtracker
        public int[] g_StrategyCount = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        #endregion
        #region Solver Decisions Method
        /// <summary>
        /// Once called, the solver will attempt to entirely solve the puzzle, making decisions based off of the current state of the puzzle.
        /// A strictly ordered list of strategies are established. If a strategy fails, the next strategy is attempted
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="method">method '1' and '3' are human-strategy solver. '2' is Backtracking solver. '3' doesn't require solvepath additions</param>
        /// <returns>returns true if the puzzle is solved, false if not</returns>
        public bool Solver(SudokuGrid grid, int method)
        {
            g_Rating = 0;
            g_StepCounter = 0;
            if (method == 1)
            {
                g_SolvePath = new List<string>();
                g_PathTracking = true;
            }
            g_changeMade = false; g_moderate = false; g_advanced = false;
            g_BacktrackingReq = false;
            /*
             *  This do...while is necessary for repeating these methods for solving until no changes are made (which it assumes that the puzzle is complete or it could not complete it)
             *  The if and elses are to make the process faster of solving faster, 
                as it ensures that it tries the easiest less computationally heavy methods first before the more complex methods.
            */
            if (method == 1 || method == 3)
            {

                string separator = "|>---------------------------------------------------------------------------------------------------------------------->|";
                int index = 0;
                do
                {
                    ++g_StepCounter;
                    if (CleanCandidateLists(grid))
                    {
                        if (method == 1)
                            g_SolvePath.Add(separator);
                    }
                    if (method == 1)
                    {
                        g_SolvePath.Add("STEP " + g_StepCounter + ": ");
                        index = g_SolvePath.Count - 1;
                    }
                    if (FindNakedSingles(grid))
                    {
                        g_changeMade = true;
                        if (method == 1)
                            g_SolvePath[index] += "Naked Single(s)";
                    }
                    else if (FindHiddenSingles(grid))
                    {
                        g_changeMade = true;
                        if (method == 1)
                            g_SolvePath[index] += "Hidden Single(s)";
                    }
                    else if (FindNakedPair(grid))//A Naked Pair contributes +175 to the rating.
                    {
                        g_changeMade = true;
                        g_Rating += 150;
                        g_moderate = true;
                        g_StrategyCount[3]++;
                        if (method == 1)
                            g_SolvePath[index] += "Naked Pair";
                    }
                    else if (FindHiddenPair(grid))//A Hidden Pair contributes +225 to the rating.
                    {
                        g_changeMade = true;
                        g_Rating += 175;
                        g_moderate = true;
                        g_StrategyCount[4]++;
                        if (method == 1)
                            g_SolvePath[index] += "Hidden Pair";
                    }
                    else if (FindPointingNumbers(grid))//A Point Pair/Triple contributes +100 to the rating.
                    {
                        g_changeMade = true;
                        g_Rating += 200;
                        g_moderate = true;
                        g_StrategyCount[5]++;
                        if (method == 1)
                            g_SolvePath[index] += "Pointing Line";
                    }
                    else if (FindBlockLineReduce(grid))//A Box-Line Reduction contributes +150 to the rating.
                    {
                        g_changeMade = true;
                        g_Rating += 225;
                        g_moderate = true;
                        g_StrategyCount[6]++;
                        if (method == 1)
                            g_SolvePath[index] += "Block-Line Reduction";
                    }
                    else if (FindNakedTriple(grid))//A Naked Triple contributes +350 to the rating.
                    {
                        g_changeMade = true;
                        g_Rating += 300;
                        g_moderate = true;
                        g_StrategyCount[7]++;
                        if (method == 1)
                            g_SolvePath[index] += "Naked Triple";
                    }
                    else if (FindHiddenTriple(grid))//A Hidden Triple contributes +400 to the rating.
                    {
                        g_changeMade = true;
                        g_Rating += 400;
                        g_moderate = true;
                        g_StrategyCount[8]++;
                        if (method == 1)
                            g_SolvePath[index] += "Hidden Triple";
                    }
                    else if (FindXWing(grid))//An X-Wing contributes +500 to the rating.
                    {
                        g_changeMade = true;
                        g_Rating += 500;
                        g_advanced = true;
                        g_StrategyCount[9]++;
                        if (method == 1)
                            g_SolvePath[index] += "X-Wing";
                    }
                    else if (FindSinglesChain(grid))//A Single's Chain contributes + 600 to the rating.
                    {
                        g_changeMade = true;
                        g_advanced = true;
                        g_Rating += 600;
                        g_StrategyCount[12]++;
                        if (method == 1)
                            g_SolvePath[index] += "Single's Chain / Simple Colouring";
                    }
                    else if (FindYWing(grid))//A Y-Wing contributes +650 to the rating.
                    {
                        g_changeMade = true;
                        g_Rating += 650;
                        g_advanced = true;
                        g_StrategyCount[10]++;
                        if (method == 1)
                            g_SolvePath[index] += "Y-Wing";
                    }
                    else if (FindXYZWing(grid))//An XYZ-Wing contributes +700 to the rating.
                    {
                        g_changeMade = true;
                        g_Rating += 700;
                        g_advanced = true;
                        g_StrategyCount[11]++;
                        if (method == 1)
                            g_SolvePath[index] += "XYZ-Wing";
                    }
                    else if (FindUniqueRectangleType1(grid))//A Unique Rectangle contributes +750 to the rating.
                    {
                        g_changeMade = true;
                        g_Rating += 750;
                        g_advanced = true;
                        g_StrategyCount[13]++;
                        if (method == 1)
                            g_SolvePath[index] += "Unique Rectangle Type 1";
                    }
                    //More methods to add
                    else
                    {
                        if (method == 1)
                        {
                            g_SolvePath.RemoveAt(g_SolvePath.Count - 1);
                        }
                        g_StepCounter--;
                        g_changeMade = false;

                    }
                    if (g_changeMade)
                    {
                        LastCandidates(grid);
                    }
                    g_SolvePath.Add(separator);
                } while (g_changeMade);
            }
            else if (method == 2)
            {
                g_BacktrackingPath = new List<string>();
                CompileBacktracker(grid, 3);
            }
            if (!g_Gen.CheckIfSolved(grid))
            {
                CompileBacktracker(grid, 0);
                g_BacktrackingReq = true;
                g_Difficulty = "Extreme";
                g_SolvePath.Add("BACKTRACKING/TRIAL-AND-ERROR USED TO FINISH PUZZLE - UNABLE TO FINISH WITH IMPLEMENTED STRATEGIES");
                g_StrategyCount[0]++;
            }
            else if (method == 1)
                g_SolvePath.Add("|--------------------------------FINISHED--------------------------------|");
            if (!g_BacktrackingReq)
            {
                if (g_moderate)
                    g_Difficulty = "Moderate";
                if (g_advanced)
                    g_Difficulty = "Advanced";
            }
            g_Rating = (int)(g_Rating * (1 + ((double)g_StepCounter / 100)));
            return g_Gen.CheckIfSolved(grid);
        }
        #endregion
        #region Strategy-Usage Solver method
        #region Grid Cleaning Methods
        /// <summary>
        /// Looks at all cells and analyses the numbers in the neighbouring cells, removing those numbers from candidate lists. 
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        public bool CleanCandidateLists(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0')
                    {
                        for (int n = 0; n < 8; n++)
                        {
                            if (grid.Cells[i][j].Candidates.Contains(grid.Cells[i][j].NeighbourCells[0][n].Num))
                            {
                                grid.Cells[i][j].Candidates.Remove(grid.Cells[i][j].NeighbourCells[0][n].Num);
                                if (g_PathTracking)
                                    g_SolvePath.Add("---Candidate number " + grid.Cells[i][j].NeighbourCells[0][n].Num + " removed from cell [" + i + "," + j + "] - NUMBER EXISTS IN ROW");
                                changeMade = true;
                            }
                            if (grid.Cells[i][j].Candidates.Contains(grid.Cells[i][j].NeighbourCells[1][n].Num))
                            {
                                grid.Cells[i][j].Candidates.Remove(grid.Cells[i][j].NeighbourCells[1][n].Num);
                                if (g_PathTracking)
                                    g_SolvePath.Add("---Candidate number " + grid.Cells[i][j].NeighbourCells[1][n].Num + " removed from cell [" + i + "," + j + "] - NUMBER EXISTS IN COLUMN");
                                changeMade = true;
                            }
                            if (grid.Cells[i][j].Candidates.Contains(grid.Cells[i][j].NeighbourCells[2][n].Num))
                            {
                                grid.Cells[i][j].Candidates.Remove(grid.Cells[i][j].NeighbourCells[2][n].Num);
                                if (g_PathTracking)
                                    g_SolvePath.Add("---Candidate number " + grid.Cells[i][j].NeighbourCells[2][n].Num + " removed from cell [" + i + "," + j + "] - NUMBER EXISTS IN BLOCK");
                                changeMade = true;
                            }
                        }
                    }
                }
            }
            return changeMade;
        }
        /// <summary>
        /// Cleans last candidates in cells to define the value of the cell after a strategy has made changes
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private bool LastCandidates(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0' && grid.Cells[i][j].Candidates.Count == 1)
                    {
                        grid.Cells[i][j].Num = grid.Cells[i][j].Candidates[0];
                        grid.Cells[i][j].Candidates.Clear();
                        if (!changeMade && g_PathTracking)
                            g_SolvePath.Add("(IMMEDIATE SOLUTIONS ACQUIRED FROM STRATEGY)");
                        changeMade = true;
                        g_Rating += 30;
                        if (g_PathTracking)
                            g_SolvePath.Add("-Last candidate number in cell [" + i + "," + j + "] is " + grid.Cells[i][j].Num);
                        g_StrategyCount[1]++;
                    }
                }
            }
            return changeMade;
        }
        #endregion
        #region Singles Strategies
        #region Naked Singles
        /// <summary>
        /// Naked Singles
        /// Set Theory:
        /// 'A' is the list of possible values for single cell
        /// |A| = 1
        /// If a single candidate number is left in a cell's candidate list, the cell's value becomes that last number.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private bool FindNakedSingles(SudokuGrid grid)
        {
            bool changeMade = false;
            Dictionary<Cell, char> cellsToChange = new Dictionary<Cell, char>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0' && grid.Cells[i][j].Candidates.Count == 1)
                    {
                        cellsToChange.Add(grid.Cells[i][j], grid.Cells[i][j].Candidates[0]);
                        changeMade = true;
                        int rcVal = FindNakedSingleRC(grid.Cells[i][j]);
                        if (g_PathTracking)
                            g_SolvePath.Add("Number " + grid.Cells[i][j].Candidates[0] + " placed in cell [" + i + "," + j + "] - RC value = " + rcVal + " - NAKED SINGLE");
                        g_Rating += rcVal * 10;
                        g_StrategyCount[1]++;
                    }
                }
            }
            if (changeMade)
            {
                foreach (var item in cellsToChange.Keys)
                {
                    item.Num = cellsToChange[item];
                    item.Candidates.Clear();
                }
            }
            return changeMade;
        }
        /// <summary>
        /// This method grades the complexity of the Naked Single usage by relational complexity
        /// The Relational Complexity of a strategy is the minimum number of groups that are required to deduce the Naked Single
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="singleCell"></param>
        /// <returns></returns>
        private int FindNakedSingleRC(Cell singleCell)
        {
            char[] numsInRow = new char[8], numsInCol = new char[8], numsInBlk = new char[8];
            for (int n = 0; n < 8; n++)
            {
                numsInRow[n] = singleCell.NeighbourCells[0][n].Num;
                numsInCol[n] = singleCell.NeighbourCells[1][n].Num;
                numsInBlk[n] = singleCell.NeighbourCells[2][n].Num;
            }
            if (!numsInRow.Contains('0') || !numsInCol.Contains('0') || !numsInBlk.Contains('0'))
                return 1;
            List<char> numberList = new List<char>(9) { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            numberList.Remove(singleCell.Candidates[0]);
            for (int i = 0; i < 8; i++)
            {
                numberList.Remove(numsInRow[i]);
                numberList.Remove(numsInCol[i]);
            }
            if (numberList.Count == 0)
                return 2;
            numberList = new List<char>(9) { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            numberList.Remove(singleCell.Candidates[0]);
            for (int i = 0; i < 8; i++)
            {
                numberList.Remove(numsInRow[i]);
                numberList.Remove(numsInBlk[i]);
            }
            if (numberList.Count == 0)
                return 2;
            numberList = new List<char>(9) { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            numberList.Remove(singleCell.Candidates[0]);
            for (int i = 0; i < 8; i++)
            {
                numberList.Remove(numsInCol[i]);
                numberList.Remove(numsInBlk[i]);
            }
            if (numberList.Count == 0)
                return 2;
            return 3;
        }
        #endregion
        #region Hidden Singles
        /// <summary>
        /// Hidden Singles
        /// Searches through all cells, if a cell is empty  - call it cellA, 
        /// for each group related to cellA,
        /// iterate from 1-9 and check each cell (call the cell cellB) in the group if it contains the current number.
        /// If cellB contains the number, remove it from the 1-9 list.
        /// If the loop reaches the end of a group and the list contains only 1 candidate number, cellA's value must be that last candidate number
        /// pseudocode:
        /// FOR each empty cell, K
        ///     FOR each neighbour group
        ///         FOR each candidate in K, c
        ///             ASSERT BOOL s as true
        ///             FOR each neighbour in current neighbour group
        ///                 IF neighbour contains candidate number c
        ///                     ASSIGN s as false
        ///                     break out of loop
        ///                 ENDIF
        ///             ENDFOR
        ///             IF s equals true
        ///                 ASSIGN value of K to c
        ///             ENDIF
        ///         ENDFOR
        ///     ENDFOR
        /// ENDFOR
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindHiddenSingles(SudokuGrid grid)
        {
            bool changeMade = false;
            Dictionary<Cell, char> cellsToChange = new Dictionary<Cell, char>();
            List<int> rcValues = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0' && grid.Cells[i][j].Candidates.Count > 1)
                    {
                        int[] rcvalues = new int[3] { 0, 0, 0 };
                        for (int index = 0; index < 3; index++)
                        {
                            foreach (char candidate in grid.Cells[i][j].Candidates)
                            {
                                bool single = true;

                                foreach (Cell neighbour in grid.Cells[i][j].NeighbourCells[index])
                                {
                                    if (neighbour.Num == candidate || neighbour.Num == '0' && neighbour.Candidates.Contains(candidate))
                                    {
                                        single = false;
                                        break;
                                    }
                                }
                                if (single)
                                {
                                    Dictionary<Cell, List<Cell>> constraints = new Dictionary<Cell, List<Cell>>();//each entry (cell) has a weight that implies the number of cells that it sees in the group
                                    int emptycells = 0;
                                    foreach (Cell neighbour in grid.Cells[i][j].NeighbourCells[index])
                                    {
                                        if (neighbour.Num == '0' & !neighbour.Candidates.Contains(candidate))
                                        {
                                            emptycells++;
                                            for (int index2 = 0; index2 < 3; index2++)
                                            {
                                                if (index2 != index)
                                                {
                                                    foreach (Cell neighbour2 in neighbour.NeighbourCells[index2])
                                                    {
                                                        if (neighbour2.Num == candidate)
                                                        {
                                                            if (!constraints.ContainsKey(neighbour2))
                                                            {
                                                                constraints.Add(neighbour2, new List<Cell>() { neighbour });
                                                            }
                                                            else if (!constraints[neighbour2].Contains(neighbour))
                                                            {
                                                                constraints[neighbour2].Add(neighbour);
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    List<Cell> removables = new List<Cell>();
                                    foreach (var key in constraints.Keys)
                                    {
                                        int t = 0;
                                        foreach (var constraint in constraints[key])
                                        {
                                            foreach (var key2 in constraints.Keys)
                                            {
                                                if (key2 != key && !removables.Contains(key2) && constraints[key2].Contains(constraint))
                                                {
                                                    t++;
                                                    break;
                                                }
                                            }
                                        }
                                        if (t == constraints[key].Count)
                                        {
                                            removables.Add(key);
                                        }
                                    }
                                    for (int rmv = 0; rmv < removables.Count; rmv++)
                                    {
                                        constraints.Remove(removables[rmv]);
                                    }
                                    int sum = 0;
                                    foreach (var key in constraints.Keys)
                                    {
                                        foreach (var constraint in constraints[key])
                                        {
                                            foreach (var key2 in constraints.Keys)
                                            {
                                                if (key2 != key && constraints[key2].Contains(constraint))
                                                {
                                                    sum--;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (sum < 0)
                                        sum /= 2;
                                    foreach (var item in constraints.Keys)
                                    {
                                        sum += constraints[item].Count;
                                    }
                                    if (!cellsToChange.ContainsKey(grid.Cells[i][j]))
                                    {
                                        cellsToChange.Add(grid.Cells[i][j], candidate);
                                    }
                                    changeMade = true;
                                    rcvalues[index] = 1 + constraints.Count + (emptycells - sum);
                                    break;
                                }
                            }
                        }
                        int lowestRC = 99;
                        for (int rcIndx = 0; rcIndx < rcvalues.Length; rcIndx++)
                        {
                            if (rcvalues[rcIndx] != 0)
                            {
                                if (rcvalues[rcIndx] < lowestRC)
                                {
                                    lowestRC = rcvalues[rcIndx];
                                }
                            }
                        }
                        if (lowestRC != 99)
                        {
                            g_Rating += lowestRC * 10;
                            rcValues.Add(lowestRC);
                        }
                    }
                }
            }
            if (changeMade)
            {
                int counter = 0;
                foreach (var item in cellsToChange.Keys)
                {
                    item.Num = cellsToChange[item];
                    item.Candidates.Clear();
                    g_StrategyCount[2]++;
                    if (g_PathTracking)
                        g_SolvePath.Add("Number " + item.Num + " placed in cell [" + item.XLocation + "," + item.YLocation + "] - RC value = " + rcValues[counter++] + " - HIDDEN SINGLE");
                }
            }

            return changeMade;
        }
        #endregion
        #endregion
        #region Pair Subset Strategies
        #region Naked Pair
        /// <summary>
        /// FindNakedPair:
        /// -Find empty cell with two candidates, call it 'Cell'
        /// -Foreach group associated with 'Cell', look at each cell within the group, call these cells 'Neighbours'
        /// -If a neighbour's candidate list is identical to 'Cell's candidate list, you have found a naked pair.
        /// -Foreach cell in the same group that the pair was found in (call each of these cells 'outlier'), look at the candidate list 
        /// -If there exists any candidates in that list that also exist in the pair's candidate list, remove the candidates from the 'outlier's list.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindNakedPair(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0' && grid.Cells[i][j].Candidates.Count == 2)
                    {
                        for (int index = 0; index < 3; index++)
                        {
                            foreach (Cell neighbour in grid.Cells[i][j].NeighbourCells[index])
                            {
                                if (neighbour.Num == '0' && neighbour.Candidates.SequenceEqual(grid.Cells[i][j].Candidates))
                                {
                                    foreach (Cell cell in grid.Cells[i][j].NeighbourCells[index])
                                    {
                                        bool cellChange = false; // bool for stacking solve path
                                        if (cell != neighbour && cell.Candidates.Contains(neighbour.Candidates[0]) && cell.Num == '0')
                                        {
                                            cell.Candidates.Remove(neighbour.Candidates[0]);
                                            if (g_PathTracking)
                                                g_SolvePath.Add("Candidate number " + neighbour.Candidates[0] + " removed from cell [" + cell.XLocation + "," + cell.YLocation + "] - NAKED PAIR - " + neighbour.Candidates[0] + "/" + neighbour.Candidates[1]);
                                            changeMade = true;
                                            cellChange = true;
                                        }
                                        if (cell != neighbour && cell.Candidates.Contains(neighbour.Candidates[1]) && cell.Num == '0')
                                        {
                                            cell.Candidates.Remove(neighbour.Candidates[1]);
                                            if (g_PathTracking && cellChange)
                                            {
                                                g_SolvePath.RemoveAt(g_SolvePath.Count - 1);
                                                g_SolvePath.Add("Candidate numbers " + neighbour.Candidates[0] + neighbour.Candidates[1] + " removed from cell [" + cell.XLocation + "," + cell.YLocation + "] - NAKED PAIR - " + neighbour.Candidates[0] + "/" + neighbour.Candidates[1]);
                                            }
                                            else if (g_PathTracking)
                                                g_SolvePath.Add("Candidate number " + neighbour.Candidates[1] + " removed from cell [" + cell.XLocation + "," + cell.YLocation + "] - NAKED PAIR - " + neighbour.Candidates[0] + "/" + neighbour.Candidates[1]);
                                            changeMade = true;
                                        }
                                    }
                                    if (changeMade)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return changeMade;
        }
        #endregion
        #region Hidden Pair
        /// <summary>
        /// Hidden Pairs: 
        /// -Iterate through cells till an empty cell with a candidate list count of greater than 2 is found.
        /// -Search through each group (row/column/block) and iterate over each candidate of the focused cell
        /// -If a candidate is only found once in the focused cell's currently focused group, then add the neighbour to a list of cells and the candidate to list of candidates
        /// -If the cell list magnitude is > 1, iterate over the cell list.
        /// -If two cells in that list are the same, you can safely say a hidden pair is found. 
        /// -Then remove all candidates that the two cells do not have in common
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindHiddenPair(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0' && grid.Cells[i][j].Candidates.Count > 2)
                    {
                        for (int index = 0; index < 3 && grid.Cells[i][j].Candidates.Count > 2; index++)
                        {
                            List<Cell> cellList = new List<Cell>(9);//Stores all cells that contain candidates
                            List<char> candis = new List<char>(9);//If only two candidates exist in the group, add the candidate to this list
                            foreach (char candidate in grid.Cells[i][j].Candidates)
                            {
                                int counter = 0;
                                foreach (Cell neighbour in grid.Cells[i][j].NeighbourCells[index])
                                {
                                    if (neighbour.Candidates.Contains(candidate))
                                    {
                                        if (++counter > 1)
                                        {
                                            cellList.RemoveAt(cellList.Count - 1);
                                            break;
                                        }
                                        cellList.Add(neighbour);
                                    }
                                }
                                if (counter == 1)//find a pair
                                {
                                    candis.Add(candidate);
                                }
                            }
                            if (cellList.Count > 1)
                            {
                                for (int a = 0; a < cellList.Count; a++)
                                {
                                    for (int b = a + 1; b < cellList.Count; b++)
                                    {
                                        if (cellList[a] == cellList[b])
                                        {
                                            List<char> removeList1 = new List<char>();
                                            List<char> removeList2 = new List<char>();
                                            removeList1.AddRange(grid.Cells[i][j].Candidates);
                                            removeList2.AddRange(cellList[b].Candidates);
                                            removeList1.RemoveAll(candidate => candidate == candis[a] || candidate == candis[b]);
                                            removeList2.RemoveAll(candidate => candidate == candis[a] || candidate == candis[b]);

                                            grid.Cells[i][j].Candidates.RemoveAll(candidate => candidate != candis[a] && candidate != candis[b]);//Removes all candidates but the common candidates
                                            cellList[b].Candidates.RemoveAll(candidate => candidate != candis[a] && candidate != candis[b]);

                                            if (removeList1.Count != 0)
                                            {
                                                changeMade = true;
                                                if (g_PathTracking)
                                                    g_SolvePath.Add("Candidate numbers " + new string(removeList1.ToArray()) + " removed from cell [" + i + "," + j + "] - HIDDEN PAIR - " + candis[a] + "/" + candis[b]);
                                            }
                                            if (removeList2.Count != 0)
                                            {
                                                changeMade = true;
                                                if (g_PathTracking)
                                                    g_SolvePath.Add("Candidate numbers " + new string(removeList2.ToArray()) + " removed from cell [" + cellList[b].XLocation + "," + cellList[b].YLocation + "] - HIDDEN PAIR - " + candis[a] + "/" + candis[b]);
                                            }
                                            if (changeMade)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return changeMade;
        }
        #endregion
        #endregion
        #region Triples Subset Strategies
        #region Naked Triple
        /// <summary>
        /// A naked triple is where any three cells that share a group contain, in total, three candidate numbers.
        /// The combinations of candidates for a naked triple are:
        /// [x,y,z] [x,y,z] [x,y,z]
        /// [x,y,z] [x,y,z] [x,y] 
        /// [x,y,z] [x,y]   [x,z] 
        /// [x,y]   [x,z]   [y,z]
        /// Find Naked Triple in 3,3,3 | 3,3,2 | 3,2,2 format:
        /// -Iterate over cells till an empty cell is found containing two/three candidates, call that cell 'cellA'
        /// -Iterate over each cell in each group associated with cellA, call it cellB
        /// -If cellB contains only 2 or 3 candidates and those candidates are also located in cellA, add the cell to a list.
        /// -Now continue iterating over the group, but if another cell that matches the above description is found, call it cellC, add it to the list... you have found a triple
        /// -Start removing any that are values located in the candidates lists of cellA/B/C from any cells, that contain those candidate values, within the current group.
        /// Find Naked Triple in 2,2,2 format:
        /// -For this format, you only need to look for 3 cells, each with 2 candidates and each with 1 candidate in common with one another. They also have to lie within the same group.
        /// -If that is found, remove any of that values that are located in the candidate lists of the triple from any of the cells, that contain those candidate values, within the current group.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindNakedTriple(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0' && grid.Cells[i][j].Candidates.Count == 3)
                    {
                        for (int index = 0; index < 3; index++)
                        {
                            List<Cell> triples = new List<Cell>(3)
                            {
                                grid.Cells[i][j]
                            };
                            foreach (Cell neighbour in grid.Cells[i][j].NeighbourCells[index])//Foreach neighbour to cellA, find out how many candidates they have in common
                            {
                                int candiCount = 0;
                                foreach (char candidate in grid.Cells[i][j].Candidates)
                                {
                                    if (neighbour.Num == '0' && neighbour.Candidates.Count < 4 && neighbour.Candidates.Contains(candidate))
                                    {
                                        candiCount++;
                                    }
                                }
                                if (candiCount == neighbour.Candidates.Count)
                                {
                                    if (candiCount >= 2)
                                    {
                                        triples.Add(neighbour);
                                    }
                                }

                                if (triples.Count > 3)
                                {
                                    if (g_PathTracking)
                                        g_SolvePath.Add("Something has gone terribly wrong with naked triple...");//emergency eject - should never occur
                                    return false;
                                }
                            }
                            if (triples.Count == 3)
                            {
                                for (int n = 0; n < 8; n++)
                                {
                                    if (grid.Cells[i][j].NeighbourCells[index][n].Num == '0' && grid.Cells[i][j].NeighbourCells[index][n] != triples[1] && grid.Cells[i][j].NeighbourCells[index][n] != triples[2])
                                    {
                                        string removed = "";
                                        if (grid.Cells[i][j].NeighbourCells[index][n].Candidates.Remove(triples[0].Candidates[0]))
                                        {
                                            changeMade = true;
                                            removed += triples[0].Candidates[0];
                                        }
                                        if (grid.Cells[i][j].NeighbourCells[index][n].Candidates.Remove(triples[0].Candidates[1]))
                                        {
                                            changeMade = true;
                                            removed += triples[0].Candidates[1];
                                        }
                                        if (grid.Cells[i][j].NeighbourCells[index][n].Candidates.Remove(triples[0].Candidates[2]))
                                        {
                                            changeMade = true;
                                            removed += triples[0].Candidates[2];
                                        }
                                        if (removed.Length > 0)
                                        {
                                            if (g_PathTracking)
                                                g_SolvePath.Add("Candidate number " + removed + " removed from cell [" + grid.Cells[i][j].NeighbourCells[index][n].XLocation + "," + grid.Cells[i][j].NeighbourCells[index][n].YLocation + "] - NAKED TRIPLE - " + triples[0].Candidates[0] + "/" + triples[0].Candidates[1] + "/" + triples[0].Candidates[2]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (grid.Cells[i][j].Num == '0' && grid.Cells[i][j].Candidates.Count == 2)//Finds naked triples that are in the 2/2/2 format, i.e. [1,2] [2,3] [1,3]
                    {
                        for (int index = 0; index < 3; index++)
                        {
                            bool foundTriple = false;
                            foreach (Cell neighbour in grid.Cells[i][j].NeighbourCells[index])
                            {
                                if (neighbour.Num == '0' && neighbour.Candidates.Count == 2
                                    && (neighbour.Candidates.Contains(grid.Cells[i][j].Candidates[0]) || neighbour.Candidates.Contains(grid.Cells[i][j].Candidates[1])))//Try find cellC with this cellB to make a triple
                                {
                                    char candidateA = neighbour.Candidates.Contains(grid.Cells[i][j].Candidates[0]) ? grid.Cells[i][j].Candidates[1] : grid.Cells[i][j].Candidates[0];
                                    char candidateB = grid.Cells[i][j].Candidates.Contains(neighbour.Candidates[0]) ? neighbour.Candidates[1] : neighbour.Candidates[0];
                                    char candidateC = grid.Cells[i][j].Candidates.Contains(neighbour.Candidates[0]) ? neighbour.Candidates[0] : neighbour.Candidates[1];
                                    foreach (Cell neighbourB in grid.Cells[i][j].NeighbourCells[index])
                                    {
                                        if (neighbourB.Num == '0' && neighbourB.Candidates.Count == 2 && neighbourB.Candidates.Contains(candidateA)
                                            && neighbourB.Candidates.Contains(candidateB))//Found 2/2/2 naked triple
                                        {
                                            foundTriple = true;
                                            for (int n = 0; n < 8; n++)
                                            {
                                                if (grid.Cells[i][j].NeighbourCells[index][n].Num == '0' && grid.Cells[i][j].NeighbourCells[index][n] != neighbour
                                                    && grid.Cells[i][j].NeighbourCells[index][n] != neighbourB)//Finds next empty cell in group that isn't apart of the triple
                                                {
                                                    string removed = "";
                                                    if (grid.Cells[i][j].NeighbourCells[index][n].Candidates.Remove(candidateA))//Tries to remove candidateA from the cell
                                                    {
                                                        changeMade = true;
                                                        removed += candidateA;
                                                    }
                                                    if (grid.Cells[i][j].NeighbourCells[index][n].Candidates.Remove(candidateB))//Tries to remove candidateB from the cell
                                                    {
                                                        changeMade = true;
                                                        removed += candidateB;
                                                    }
                                                    if (grid.Cells[i][j].NeighbourCells[index][n].Candidates.Remove(candidateC))//Tries to remove candidateC from the cell
                                                    {
                                                        changeMade = true;
                                                        removed += candidateC;
                                                    }
                                                    if (removed.Length > 0 && g_PathTracking)
                                                    {
                                                        g_SolvePath.Add("Candidate number " + removed + " removed from cell [" + grid.Cells[i][j].NeighbourCells[index][n].XLocation + "," + grid.Cells[i][j].NeighbourCells[index][n].YLocation + "] - NAKED TRIPLE - " + candidateA + "/" + candidateB + "/" + candidateC);
                                                    }
                                                }
                                            }
                                            if (changeMade)
                                                return true;
                                        }
                                    }
                                }
                                if (foundTriple)
                                    break;
                            }
                        }
                    }
                }
            }

            return changeMade;
        }
        #endregion
        #region Hidden Triple
        /// <summary>
        /// This is ridiculously convoluted. So no code will be explained. :(
        /// A lot of time was sunk into this strategy to perfect it.
        /// A hidden triple is where the format below is found hidden in a group amongst other candidate numbers
        /// [x,y,z] [x,y,z] [x,y,z]
        /// [x,y,z] [x,y,z] [x,y] 
        /// [x,y,z] [x,y]   [x,z] 
        /// [x,y]   [x,z]   [y,z]
        /// 
        /// For example: If the follow are three cell's candidate lists... 
        /// [2,3,4,5] [3,4,8,9] [1,4,5,7] 
        /// ...and these cells are all in a group together where '3', '4', and'5' do not exist anywhere else in the group other than in those three cells,
        /// then these cells contain a hidden triple. This means that '3', '4', and '5' must go into go into these three cells, in whatever order.
        /// This means that we can remove any other candidates from within these three cells. This leaves the cells looking like...
        /// [3,4,5] [3,4] [4,5]
        /// This strategy is very beneficial if found due to the number of candidates in total that are usually removed when finding a hidden triple.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true of a change to a cell candidate list or value is made</returns>
        private bool FindHiddenTriple(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0' && grid.Cells[i][j].Candidates.Count > 2)
                    {
                        for (int index = 0; index < 3 && grid.Cells[i][j].Candidates.Count > 2; index++)
                        {
                            Dictionary<char, int> candidateFreq = new Dictionary<char, int>();
                            char[] fullCandiList = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                            foreach (char candidate in fullCandiList)
                            {
                                if (grid.Cells[i][j].Candidates.Contains(candidate))
                                {
                                    candidateFreq.Add(candidate, 1);
                                }
                                foreach (Cell cell in grid.Cells[i][j].NeighbourCells[index])
                                {
                                    if (cell.Candidates.Contains(candidate))
                                    {
                                        if (!candidateFreq.ContainsKey(candidate))
                                        {
                                            candidateFreq.Add(candidate, 1);
                                        }
                                        else
                                        {
                                            candidateFreq[candidate]++;
                                        }
                                    }
                                }
                                if (candidateFreq.ContainsKey(candidate))
                                {
                                    if (candidateFreq[candidate] > 3)
                                    {
                                        candidateFreq.Remove(candidate);
                                    }
                                }
                            }
                            foreach (char candidateA in candidateFreq.Keys)
                            {
                                if (grid.Cells[i][j].Candidates.Contains(candidateA))
                                {
                                    Cell cellA = grid.Cells[i][j];
                                    foreach (Cell cellB in cellA.NeighbourCells[index])
                                    {
                                        if (cellB.Candidates.Contains(candidateA))
                                        {
                                            foreach (char candidateB in candidateFreq.Keys)
                                            {
                                                if (cellB.Candidates.Contains(candidateB) && candidateB != candidateA && (candidateFreq[candidateB] == 3 && !cellA.Candidates.Contains(candidateB)) == false)
                                                {
                                                    foreach (Cell cellC in cellB.NeighbourCells[index])
                                                    {
                                                        if (cellC.Candidates.Contains(candidateB) && cellC != cellA && ((candidateFreq[candidateA] == 3 && !cellC.Candidates.Contains(candidateA)) == false))
                                                        {
                                                            foreach (char candidateC in candidateFreq.Keys)
                                                            {
                                                                if (cellC.Candidates.Contains(candidateC) && cellA.Candidates.Contains(candidateC) && candidateC != candidateB && candidateC != candidateA && (candidateFreq[candidateC] == 3 && !cellB.Candidates.Contains(candidateC)) == false)
                                                                {
                                                                    //hidden triple found
                                                                    List<char> removeList = new List<char>();
                                                                    if ((cellA.Candidates.Count == 3 && cellA.Candidates.Contains(candidateA) && cellA.Candidates.Contains(candidateB) && cellA.Candidates.Contains(candidateC)) == false && cellA.Candidates.Count > 2)
                                                                    {
                                                                        removeList.AddRange(cellA.Candidates);
                                                                        removeList.RemoveAll(candidate => candidate == candidateA || candidate == candidateB || candidate == candidateC);
                                                                        if (removeList.Count != 0)
                                                                        {
                                                                            if (g_PathTracking)
                                                                                g_SolvePath.Add("Candidate numbers " + new string(removeList.ToArray()) + " removed from cell [" + cellA.XLocation + "," + cellA.YLocation + "] - HIDDEN TRIPLE - " + candidateA + "/" + candidateB + "/" + candidateC);

                                                                            changeMade = true;
                                                                        }
                                                                    }
                                                                    if ((cellB.Candidates.Count == 3 && cellB.Candidates.Contains(candidateA) && cellB.Candidates.Contains(candidateB) && cellB.Candidates.Contains(candidateC)) == false)
                                                                    {
                                                                        removeList.Clear();
                                                                        removeList.AddRange(cellB.Candidates);
                                                                        removeList.RemoveAll(candidate => candidate == candidateA || candidate == candidateB || candidate == candidateC);
                                                                        if (removeList.Count != 0)
                                                                        {
                                                                            if (g_PathTracking)
                                                                                g_SolvePath.Add("Candidate numbers " + new string(removeList.ToArray()) + " removed from cell [" + cellB.XLocation + "," + cellB.YLocation + "] - HIDDEN TRIPLE - " + candidateA + "/" + candidateB + "/" + candidateC);
                                                                            changeMade = true;
                                                                        }
                                                                    }
                                                                    if ((cellC.Candidates.Count == 3 && cellC.Candidates.Contains(candidateA) && cellC.Candidates.Contains(candidateB) && cellC.Candidates.Contains(candidateC)) == false)
                                                                    {
                                                                        removeList.Clear();
                                                                        removeList.AddRange(cellC.Candidates);
                                                                        removeList.RemoveAll(candidate => candidate == candidateA || candidate == candidateB || candidate == candidateC);
                                                                        if (removeList.Count != 0)
                                                                        {
                                                                            if (g_PathTracking)
                                                                                g_SolvePath.Add("Candidate numbers " + new string(removeList.ToArray()) + " removed from cell [" + cellC.XLocation + "," + cellC.YLocation + "] - HIDDEN TRIPLE - " + candidateA + "/" + candidateB + "/" + candidateC);
                                                                            changeMade = true;
                                                                        }
                                                                    }
                                                                    cellB.Candidates.RemoveAll(candidate => candidate != candidateA && candidate != candidateB && candidate != candidateC);//Removes all candidates but the common candidates
                                                                    cellA.Candidates.RemoveAll(candidate => candidate != candidateA && candidate != candidateB && candidate != candidateC);//Removes all candidates but the common candidates
                                                                    cellC.Candidates.RemoveAll(candidate => candidate != candidateA && candidate != candidateB && candidate != candidateC);//Removes all candidates but the common candidates
                                                                    if (changeMade == true)
                                                                        return true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return changeMade;
        }
        #endregion
        #endregion
        #region Intersection Removal Strategies
        #region Pointing Numbers
        /// <summary>
        /// Find Pointing Numbers:
        /// -Iterate over cells till an empty cell is found, call that cell 'CellA'
        /// -For each 'candidate' in CellA's candidate list, check the block and identify all cells in that block that contain 'candidate'.
        /// -If only 2 or 3 cells contain 'candidate' and they all reside in the same row or all in the same column, we can remove 'candidate' from every other cell in the row/column.
        /// i.e.
        ///     1  2  3     4    5    6     7   8   9
        ///   __________________________________________
        /// A |245 24 6 | 245  2459  7   | 82   29  3  |
        /// B |453 9 35 | 8    3456 3456 | 1247 27 2157|
        /// C |8   7 35 | 345  3459 1    | 24   26 259 |
        ///   |---------|----------------|-------------|
        /// '2' can be eliminated from A1, A2, A7, and A8 as a '2' has to exist in either A4 or A5 because there are no other 2's in that block 
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindPointingNumbers(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0')
                    {
                        foreach (char candidate in grid.Cells[i][j].Candidates)
                        {
                            List<Cell> cellSave = new List<Cell>(2);
                            foreach (Cell neighbour in grid.Cells[i][j].NeighbourCells[2])
                            {
                                if (neighbour.Candidates.Contains(candidate))
                                {
                                    cellSave.Add(neighbour);
                                    if (cellSave.Count > 2)
                                    {
                                        break;
                                    }

                                }
                            }
                            if (cellSave.Count == 1 && (cellSave[0].XLocation == i || cellSave[0].YLocation == j))
                            {
                                int index = cellSave[0].XLocation == i ? 0 : 1;//Search through column/row and remove 'candidate' from any of the candidate lists in that column/row
                                for (int n = 0; n < 8; n++)
                                {
                                    if (grid.Cells[i][j].NeighbourCells[index][n] != cellSave[0] && grid.Cells[i][j].NeighbourCells[index][n].Candidates.Contains(candidate))
                                    {
                                        grid.Cells[i][j].NeighbourCells[index][n].Candidates.Remove(candidate);
                                        changeMade = true;
                                        if (g_PathTracking)
                                            g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + grid.Cells[i][j].NeighbourCells[index][n].XLocation + "," + grid.Cells[i][j].NeighbourCells[index][n].YLocation + "] - POINTING PAIR");
                                    }
                                }
                            }
                            else if (cellSave.Count == 2)
                            {
                                if ((cellSave[0].XLocation == i && cellSave[1].XLocation == i) || (cellSave[0].YLocation == j && cellSave[1].YLocation == j))
                                {
                                    int index = cellSave[0].XLocation == i ? 0 : 1;//Search through column/row and remove 'candidate' from any of the candidate lists in that column/row
                                    for (int n = 0; n < 8; n++)
                                    {
                                        if (grid.Cells[i][j].NeighbourCells[index][n] != cellSave[0] && grid.Cells[i][j].NeighbourCells[index][n] != cellSave[1] && grid.Cells[i][j].NeighbourCells[index][n].Candidates.Contains(candidate))
                                        {
                                            grid.Cells[i][j].NeighbourCells[index][n].Candidates.Remove(candidate);
                                            changeMade = true;
                                            if (g_PathTracking)
                                                g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + grid.Cells[i][j].NeighbourCells[index][n].XLocation + "," + grid.Cells[i][j].NeighbourCells[index][n].YLocation + "] - POINTING TRIPLE");
                                        }
                                    }
                                }
                            }
                            if (changeMade)
                                return true;
                        }

                    }
                }
            }

            return changeMade;
        }
        #endregion
        #region Block-Line Reduction
        /// <summary>
        /// Find Block Line Reduce:
        /// -Iterate over cells till an empty cell is found, call that cell 'CellA'
        /// -For each candidate in CellA's candidate list, check the row/column and identify all cells in that row/column that contain candidate.
        /// -If none of the cells found in that row/column are are outside of the block, we can remove all 'candidate' from every other cell in the block.
        /// i.e.
        ///     1  2  3      4    5    6      7   8   9
        ///   ____________________________________________
        /// A |45  1 6   | 245  2459  7    | 8    49 3   |
        /// B |453 9 235 | 8    23456 3456 | 1247 47 1457|
        /// C |8   7 235 | 2345 23459 1    | 24   6  459 |
        ///   |----------|-----------------|-------------|
        /// '2' can be eliminated from B5, C4, C5 as a '2' has to exist in either A4 and A5 because there are no other 2's in that row 
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindBlockLineReduce(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0')
                    {
                        foreach (char candidate in grid.Cells[i][j].Candidates)
                        {
                            List<Cell> cellSave = new List<Cell>(2);
                            byte counterRow = 0, validRowCount = 0, counterCol = 0, validColCount = 0;
                            for (int n = 0; n < 8; n++)
                            {
                                if (grid.Cells[i][j].NeighbourCells[0][n].Num == '0' && grid.Cells[i][j].NeighbourCells[0][n].Candidates.Contains(candidate))
                                {
                                    counterRow++;
                                    if (grid.Cells[i][j].NeighbourCells[0][n].BlockLoc == grid.Cells[i][j].BlockLoc)
                                    {
                                        validRowCount++;
                                    }
                                }
                                if (grid.Cells[i][j].NeighbourCells[1][n].Num == '0' && grid.Cells[i][j].NeighbourCells[1][n].Candidates.Contains(candidate))
                                {
                                    counterCol++;
                                    if (grid.Cells[i][j].NeighbourCells[1][n].BlockLoc == grid.Cells[i][j].BlockLoc)
                                    {
                                        validColCount++;
                                    }
                                }
                            }
                            if (counterRow == validRowCount)
                            {
                                foreach (Cell blockNB in grid.Cells[i][j].NeighbourCells[2])
                                {
                                    if (blockNB.XLocation != grid.Cells[i][j].XLocation && blockNB.Candidates.Contains(candidate))
                                    {
                                        blockNB.Candidates.Remove(candidate);
                                        changeMade = true;
                                        if (g_PathTracking)
                                            g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + blockNB.XLocation + "," + blockNB.YLocation + "] - BOX-LINE REDUCTION");
                                    }
                                }
                            }
                            else if (counterCol == validColCount)
                            {
                                foreach (Cell blockNB in grid.Cells[i][j].NeighbourCells[2])
                                {
                                    if (blockNB.YLocation != grid.Cells[i][j].YLocation && blockNB.Candidates.Contains(candidate))
                                    {
                                        blockNB.Candidates.Remove(candidate);
                                        changeMade = true;
                                        if (g_PathTracking)
                                            g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + blockNB.XLocation + "," + blockNB.YLocation + "] - BOX-LINE REDUCTION");
                                    }
                                }
                            }
                            if (changeMade)
                                return true;
                        }
                    }
                }
            }

            return changeMade;
        }
        #endregion
        #endregion
        #region Chaining/Linking/Colouring Strategies
        #region Y-wing
        /// <summary>
        /// Find YWing:
        /// Two types: Block-Row/Column and Row-Column (Remember Groups: Index = 0 = Row, Group Index = 1 = Column, Group Index = 2 = Block)
        /// Block-Column/Row is where the hinge shares a block with another cell in y-wing and the 3rd cell is in the same column/row as the hinge
        /// - Iterate over cells till an empty cell with two candidates is found, call that cell 'CellA'/'Hinge'
        /// - Iterate over each cell in the row/column (Group Index 0/1) of the hinge, call that cell 'CellB'
        /// - If cellB is also empty and contains two candidates and shares ONLY one of it's candidates with the CellA, 
        /// start searching for the third cell to the wing
        /// - Start iterating over the cells in the next group associated with CellA 
        /// (e.g. if CellB was found in a row, start iterating over a column. If it was found in a column, 
        /// iterate through a block), call this cell 'CellC'
        /// - If CellC is empty and ONLY contains the other candidate from CellA and the other candidate from CellB, 
        /// and none of the three Cells share more than one group(row/column/block), a Y-Wing is formed.
        /// - The shared candidate between CellB and CellC can start being removed from the cells seen by both CellB and CellC
        /// 
        /// The diagrams below show three different formats of YWings, the astericks(*) show where 'X' cannot exist...
        /// - In reference to the explanation above Cells marked with candidates 'XZ' and 'XY' in the examples below are the wings - Cell's B and C.
        ///            Block-Row/Column example                      Block-Row/Column example                           Row-Column example
        ///     1   2   3   4   5   6   7   8   9      -|-        1   2   3   4   5   6   7   8   9     -|-        1   2   3   4   5   6   7   8   9 
        ///   _____________________________________    -|-     _____________________________________    -|-     _____________________________________
        /// A |   ¦   ¦   |   ¦ * ¦   |   ¦   ¦   |    -|-   A |XZ ¦   ¦   | * ¦ * ¦ * |   ¦   ¦   |    -|-   A |XZ ¦   ¦   |   ¦   ¦ * |   ¦   ¦   |
        /// B |   ¦   ¦   |   ¦ * ¦XZ |   ¦   ¦   |    -|-   B |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   B |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// C |   ¦   ¦   |   ¦YZ ¦   |   ¦   ¦   |    -|-   C |YZ ¦ * ¦ * |   ¦   ¦XY |   ¦   ¦   |    -|-   C |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        ///   |___________|___________|___________|    -|-     |___________|___________|___________|    -|-     |___________|___________|___________|
        /// D |   ¦   ¦   |   ¦   ¦ * |   ¦   ¦   |    -|-   D |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   D |YZ ¦   ¦   |   ¦   ¦XY |   ¦   ¦   |
        /// E |   ¦   ¦   |   ¦XY ¦ * |   ¦   ¦   |    -|-   E |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   E |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// F |   ¦   ¦   |   ¦   ¦ * |   ¦   ¦   |    -|-   F |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   F |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        ///   |___________________________________|    -|-     |___________________________________|    -|-     |___________________________________|
        /// G |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   G |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   G |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        ///  ...this is because no matter how you place X,Y,Z in those cells, X will always be ruled out of the cells marked as '*'
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindYWing(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0' && grid.Cells[i][j].Candidates.Count == 2)
                    {
                        Cell cellA = grid.Cells[i][j];
                        char nxtCandi = 'u';//Represents the next candidate in cellA's candidate list that needs to be found in the next group(s) - 'u' represented unassigned
                        char thirdCandi = 'u';
                        for (int index = 0; index < 2; index++)//index < 2 because we only need to look at row/column - if index is ever 1 and a y-wing is found, the y-wing must be block-column/row, else it must be row/column
                        {
                            foreach (Cell cellB in cellA.NeighbourCells[index])
                            {
                                if (cellB.Num == '0' && cellB.Candidates.Count == 2 && !cellB.Candidates.SequenceEqual(cellA.Candidates) && cellB.BlockLoc != cellA.BlockLoc)
                                {
                                    if (cellB.Candidates.Contains(cellA.Candidates[0]) || cellB.Candidates.Contains(cellA.Candidates[1]))
                                    {
                                        if (cellB.Candidates.Contains(cellA.Candidates[0]))
                                        {
                                            nxtCandi = cellA.Candidates[1];
                                            thirdCandi = cellA.Candidates[0] == cellB.Candidates[0] ? cellB.Candidates[1] : cellB.Candidates[0];
                                        }
                                        else
                                        {
                                            nxtCandi = cellA.Candidates[0];
                                            thirdCandi = cellA.Candidates[1] == cellB.Candidates[0] ? cellB.Candidates[1] : cellB.Candidates[0];
                                        }
                                        for (int index_n = index + 1; index_n < 3; index_n++)
                                        {
                                            foreach (Cell cellC in cellA.NeighbourCells[index_n])
                                            {
                                                if (cellC.Num == '0' && cellC.Candidates.Count == 2 && !cellC.Candidates.SequenceEqual(cellA.Candidates))
                                                {
                                                    if (cellC.Candidates.Contains(nxtCandi) && cellC.Candidates.Contains(thirdCandi))//Finding cellC to complete the chain of 3 different candidates in 3 different cells
                                                    {
                                                        if (index_n == 1)//row/column y-wing
                                                        {
                                                            int row = cellB.YLocation == cellA.YLocation ? cellB.XLocation : cellC.XLocation;
                                                            int col = cellB.XLocation == cellA.XLocation ? cellB.YLocation : cellC.YLocation;
                                                            if (grid.Cells[row][col].Candidates.Contains(thirdCandi))
                                                            {
                                                                grid.Cells[row][col].Candidates.Remove(thirdCandi);
                                                                changeMade = true;
                                                                if (g_PathTracking)
                                                                    g_SolvePath.Add("Candidate number " + thirdCandi + " removed from cell [" + row + "," + col + "] - Y-WING, ROW-COLUMN - HINGE [" + cellA.XLocation + "," + cellA.YLocation + "] - WINGS [" + cellB.XLocation + "," + cellB.YLocation + "] [" + cellC.XLocation + "," + cellC.YLocation + "]");
                                                            }
                                                        }
                                                        else if (index_n == 2 && cellC.XLocation != cellA.XLocation && cellC.YLocation != cellA.YLocation)//block-column/row y-wing
                                                        {
                                                            for (int n = 0; n < 8; n++)
                                                            {
                                                                if (cellC.NeighbourCells[index][n].Candidates.Contains(thirdCandi) && cellC.NeighbourCells[index][n].BlockLoc == cellB.BlockLoc)
                                                                {
                                                                    cellC.NeighbourCells[index][n].Candidates.Remove(thirdCandi);
                                                                    changeMade = true;
                                                                    if (g_PathTracking)
                                                                        g_SolvePath.Add("Candidate number " + thirdCandi + " removed from cell [" + cellC.NeighbourCells[index][n].XLocation + "," + cellC.NeighbourCells[index][n].YLocation + "] - Y-WING, BLOCK-ROW/COLUMN - HINGE [" + cellA.XLocation + "," + cellA.YLocation + "] - WINGS [" + cellB.XLocation + "," + cellB.YLocation + "] [" + cellC.XLocation + "," + cellC.YLocation + "]");
                                                                }
                                                                if (cellB.NeighbourCells[index][n].Candidates.Contains(thirdCandi) && cellB.NeighbourCells[index][n].BlockLoc == cellC.BlockLoc)
                                                                {
                                                                    cellB.NeighbourCells[index][n].Candidates.Remove(thirdCandi);
                                                                    changeMade = true;
                                                                    if (g_PathTracking)
                                                                        g_SolvePath.Add("Candidate number " + thirdCandi + " removed from cell [" + cellB.NeighbourCells[index][n].XLocation + "," + cellB.NeighbourCells[index][n].YLocation + "] - Y-WING, BLOCK-ROW/COLUMN - HINGE [" + cellA.XLocation + "," + cellA.YLocation + "] - WINGS [" + cellB.XLocation + "," + cellB.YLocation + "] [" + cellC.XLocation + "," + cellC.YLocation + "]");
                                                                }
                                                            }
                                                        }
                                                        if (changeMade)
                                                            return true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return changeMade;
        }
        #endregion
        #region X-Wing
        /// <summary>
        /// X-Wing Strategy: The X-Wing strategy looks at candidate numbers that in rows and columns. 
        /// The X-Wing strategy focuses on finding a single common candidate in four different cells that do not all share the same block .
        /// Each of the four cells must share a row with one of the cells and a column with a different cell.
        ///*X-wing type 1: Only two cells can contain the candidate value in both rows of the X-wing. Candidates can be eliminated in the columns xwing
        ///**X-wing type 2: Only two cells can contain the candidate value in both columns of the X-wing. Candidates can be eliminated in the rows of the xwing
        /// Method:
        /// -Find empty cell, loop through candidates - (call this cell 'cellA')
        /// -Foreach candidate perform this method - (call this candidate 'candidate')
        /// --Iterate through the cells in the row of cellA, if more than one cell in that row contains the candidate number, try find X-wing type 2, if only one cell contains the candidate number proceed with trying type 1*
        /// --After finding a cell that row that isn't within the same block as cellA, (call it cellB) starting iterating through the cells in the column of cellA,
        /// ---If a cell contains candidate number, (call it cellC), starting iterate through the row of cellC
        /// ---If more than one cell or no cells contains 'candidate' in the row of cellC, continue iterating through the column of cellA 
        /// ---If only one cell contains the candidate in the row of cellC, call it 'cellD'
        /// ----Check if within the column of cellD there exists cellB.
        /// -----If true, remove candidate from all cells in the rows that the X-wing is apart of (excl. cellA, cellB, cellC, cellD)
        /// 
        /// Finding X-wing type 2 is exactly the same as the above, however replace row with colmun and column with row.
        /// 
        /// The diagrams below show three different formats of YWings, the small x's show where 'X' could also exist but can be ruled out because of the XWing. 
        /// Type2                                                                                        | Type1
        ///     1   2   3   4   5   6   7   8   9      -|-       1   2   3   4   5   6   7   8   9      -|-       1   2   3   4   5   6   7   8   9     -|-       1   2   3   4   5   6   7   8   9  
        ///   _____________________________________    -|-     _____________________________________    -|-     _____________________________________   -|-     _____________________________________
        /// A |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   A |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   A |   ¦   ¦   |   ¦XYZ¦   |   ¦   ¦   |   -|-   A |   ¦   ¦   |   ¦YZ ¦   |   ¦   ¦   |
        /// B |XYZ¦XYZ¦   |   ¦XYZ¦XYZ|XYZ¦XYZ¦XYZ|    -|-   B |YZ ¦XYZ¦   |   ¦XYZ¦YZ |YZ ¦YZ ¦YZ |    -|-   B |   ¦XYZ¦   |   ¦XYZ¦   |   ¦   ¦   |   -|-   B |   ¦XYZ¦   |   ¦XYZ¦   |   ¦   ¦   |
        /// C |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   C |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   C |   ¦XYZ¦   |   ¦   ¦   |   ¦   ¦   |   -|-   C |   ¦YZ ¦   |   ¦   ¦   |   ¦   ¦   |
        ///   |___________|___________|___________|    -|-     |___________|___________|___________|    -|-     |___________|___________|___________|   -|-     |___________|___________|___________|
        /// D |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   D |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   D |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |   -|-   D |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// E |   ¦XYZ¦XYZ|XYZ¦XYZ¦   |XYZ¦   ¦XYZ|    -|-   E |   ¦XYZ¦YZ |YZ ¦XYZ¦   |YZ ¦   ¦YZ |    -|-   E |   ¦XYZ¦   |   ¦XYZ¦   |   ¦   ¦   |   -|-   E |   ¦XYZ¦   |   ¦XYZ¦   |   ¦   ¦   |
        /// F |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   F |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   F |   ¦   ¦   |   ¦XYZ¦   |   ¦   ¦   |   -|-   F |   ¦   ¦   |   ¦YZ ¦   |   ¦   ¦   |
        ///   |___________________________________|    -|-     |___________________________________|    -|-     |___________________________________|   -|-     |___________________________________|
        /// G |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   G |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   G |   ¦XYZ¦   |   ¦   ¦   |   ¦   ¦   |   -|-   G |   ¦YZ ¦   |   ¦   ¦   |   ¦   ¦   |
        /// H |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   H |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   H |   ¦XYZ¦   |   ¦   ¦   |   ¦   ¦   |   -|-   H |   ¦YZ ¦   |   ¦   ¦   |   ¦   ¦   |
        /// J |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   J |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   J |   ¦XYZ¦   |   ¦   ¦   |   ¦   ¦   |   -|-   J |   ¦YZ ¦   |   ¦   ¦   |   ¦   ¦   |
        ///   |___________|___________|___________|    -|-     |___________|___________|___________|    -|-     |___________|___________|___________|   -|-     |___________|___________|___________|
        /// Focus on the cells B2, B5, E2, and E5 in Type2. 
        /// If X is placed in B2, then X has to be placed in E5. This is because either cell B5 or E5 has to contain X, and if X is placed in B2 then X cannot be placed in B5 or E2.
        /// Consequently, E5 has to contain X.
        /// The excess X's from rows B and E can be removed.
        /// 
        ///  Focus on the cells B2, B5, E2, and E5 in Type1. 
        /// If X is placed in E2, then X has to be placed in B5. This is because either cell B2 or B5 has to contain X, and if X is placed in E2 then X cannot be placed in E5 or B2.
        /// Consequently, E5 has to contain X.
        /// The excess X's from columns 2 and 5 can be removed.
        /// </summary>
        /// <param name="grid">Passing through the SudokuGrid to examine for Xwings</param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindXWing(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0')
                    {
                        foreach (char candidate in grid.Cells[i][j].Candidates)
                        {
                            Cell cellA = grid.Cells[i][j];
                            Cell cellB = CheckRowColumn(grid.Cells[i][j], candidate, 0);//check row
                            Cell cellC;
                            Cell cellD;
                            int axis = 0;
                            if (cellB != null)
                            {
                                axis = 1;//look in row
                            }
                            else
                            {
                                cellB = CheckRowColumn(grid.Cells[i][j], candidate, 1);//checks column
                                if (cellB != null)
                                {
                                    axis = 0;//look in column
                                }
                            }
                            foreach (Cell neighbourToA in cellA.NeighbourCells[axis])
                            {
                                if (neighbourToA.Candidates.Contains(candidate) && neighbourToA.Num == '0')
                                {
                                    axis = axis == 0 ? 1 : 0;//need to swap axis values to access row/column instead of column/row
                                    cellC = neighbourToA;
                                    cellD = CheckRowColumn(cellC, candidate, axis);
                                    axis = axis == 0 ? 1 : 0;
                                    if (cellD != null)
                                    {
                                        if (cellD.NeighbourCells[axis].Contains(cellB))
                                        {
                                            string type = axis == 0 ? "Type2" : "Type1";
                                            for (int n = 0; n < 8; n++)
                                            {
                                                if (cellC.NeighbourCells[axis][n].Candidates.Contains(candidate) && cellC.NeighbourCells[axis][n] != cellA && cellC.NeighbourCells[axis][n] != cellB)
                                                {
                                                    cellC.NeighbourCells[axis][n].Candidates.Remove(candidate);
                                                    changeMade = true;
                                                    if (g_PathTracking)
                                                        g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + cellC.NeighbourCells[axis][n].XLocation + "," + cellC.NeighbourCells[axis][n].YLocation + "] - X-WING " + type + " formed in [" + cellA.XLocation + "," + cellA.YLocation + "] [" + cellB.XLocation + "," + cellB.YLocation + "] [" + cellC.XLocation + "," + cellC.YLocation + "] [" + cellD.XLocation + "," + cellD.YLocation + "]");
                                                }
                                                if (cellD.NeighbourCells[axis][n].Candidates.Contains(candidate) && cellC.NeighbourCells[axis][n] != cellA && cellC.NeighbourCells[axis][n] != cellB)
                                                {
                                                    cellD.NeighbourCells[axis][n].Candidates.Remove(candidate);
                                                    changeMade = true;
                                                    if (g_PathTracking)
                                                        g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + cellD.NeighbourCells[axis][n].XLocation + "," + cellD.NeighbourCells[axis][n].YLocation + "] - X-WING " + type + " formed in [" + cellA.XLocation + "," + cellA.YLocation + "] [" + cellB.XLocation + "," + cellB.YLocation + "] [" + cellC.XLocation + "," + cellC.YLocation + "] [" + cellD.XLocation + "," + cellD.YLocation + "]");
                                                }
                                            }
                                            if (changeMade)
                                                return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return changeMade;
        }
        /// <summary>
        /// Method used in X-Wing to check if a row/column contains more than one of a candidate (excluding the candidate in the current "cell")
        /// </summary>
        /// <param name="grid">passing the sudoku grid through</param>
        /// <param name="cell">current cell that's row/column is being examined</param>
        /// <param name="axis">the axis that is going to be checked, i.e. row/column</param>
        /// <param name="candidate">current candidate that is being examined for an X-Wing</param>
        /// <returns>true if only one cell with candidate exists, false if > 1</returns>
        ///
        private Cell CheckRowColumn(Cell cell, char candidate, int axis)
        {
            int counter = 0;
            Cell saveCell = null;
            for (int n = 0; n < 8; n++)
            {
                if (cell.NeighbourCells[axis][n].Candidates.Contains(candidate) && cell.NeighbourCells[axis][n].Num == '0')
                {
                    if (++counter > 1)
                    {
                        return null;
                    }
                    if (cell.NeighbourCells[axis][n].BlockLoc != cell.BlockLoc)
                    {
                        saveCell = cell.NeighbourCells[axis][n];
                    }
                }
            }
            return saveCell ?? null;
        }
        #endregion
        #region XYZ-Wing
        /// <summary>
        /// Finding an XYZ-Wing
        /// -Very similar to a Y-Wing in the regards of to a hinge and wings/pincers system.
        /// -The difference between the two is that the hinge of a Y-Wing can only contain 2 candidates, where as the XYZ-Wing must contain all 3 of the candidates in the subset.
        /// The diagrams below show two different formats of XYZWings, the astericks(*) show where 'X' cannot exist. 
        ///     1   2   3   4   5   6   7   8   9      -|-        1   2   3   4   5   6   7   8   9 
        ///   _____________________________________    -|-     _____________________________________
        /// A |   ¦   ¦   |   ¦ * ¦   |   ¦   ¦   |    -|-   A |XZ ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// B |   ¦   ¦   |   ¦ * ¦XZ |   ¦   ¦   |    -|-   B |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// C |   ¦   ¦   |   ¦XYZ¦   |   ¦   ¦   |    -|-   C |XYZ¦ * ¦ * |   ¦   ¦XY |   ¦   ¦   |
        ///   |___________|___________|___________|    -|-     |___________|___________|___________|
        /// D |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   D |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// E |   ¦   ¦   |   ¦XY ¦   |   ¦   ¦   |    -|-   E |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// F |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   F |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        ///   |___________________________________|    -|-     |___________________________________|
        /// G |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   G |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// -This is because no matter how you place XYZ in those cells, X will always be ruled out of those cells
        /// -Another way of looking at XYZ-Wings are as naked triples that span multiple groups. Any location where each cell in the triple can see, cannot contain 'X'.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private bool FindXYZWing(SudokuGrid grid)
        {
            bool changeMade = false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0' && grid.Cells[i][j].Candidates.Count == 3)
                    {
                        List<Cell> XYZWing = new List<Cell>(3)
                            {
                                grid.Cells[i][j]
                            };
                        char commonCandidate = ' ';
                        for (int index = 0; index < 3; index++)
                        {
                            foreach (Cell neighbour in XYZWing[0].NeighbourCells[index])//Foreach neighbour to first cell, find out how many candidates they have in common
                            {
                                int candiCount = 0;
                                foreach (char candidate in grid.Cells[i][j].Candidates)
                                {
                                    if (neighbour.Num == '0' && neighbour.Candidates.Count == 2 && neighbour.Candidates.Contains(candidate))
                                    {
                                        candiCount++;
                                        if (XYZWing.Count > 1)
                                        {
                                            if (XYZWing[1].Candidates.Contains(candidate))
                                            {
                                                commonCandidate = candidate;
                                            }
                                        }
                                    }
                                }
                                if (candiCount == 2)
                                {
                                    XYZWing.Add(neighbour);
                                    for (int index2 = index + 1; index2 < 3; index2++)
                                    {
                                        foreach (Cell neighbour2 in XYZWing[0].NeighbourCells[index2])//Foreach neighbour to first cell, find out how many candidates they have in common
                                        {
                                            int candiCount2 = 0;
                                            foreach (char candidate in grid.Cells[i][j].Candidates)
                                            {
                                                if (neighbour2.Num == '0' && neighbour2.Candidates.Count == 2 && neighbour2.Candidates.Contains(candidate))
                                                {
                                                    candiCount2++;
                                                }
                                            }
                                            if (candiCount2 == 2)
                                            {
                                                if (XYZWing.Count == 2)
                                                {
                                                    if (!XYZWing[1].Candidates.SequenceEqual(neighbour2.Candidates) && neighbour2.XLocation != XYZWing[1].XLocation && neighbour2.YLocation != XYZWing[1].YLocation && neighbour2.BlockLoc != XYZWing[1].BlockLoc)
                                                    {
                                                        XYZWing.Add(neighbour2);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (XYZWing.Count == 3 && (XYZWing[1].BlockLoc == XYZWing[0].BlockLoc || XYZWing[2].BlockLoc == XYZWing[0].BlockLoc))
                                        {
                                            int outsiderCell = XYZWing[1].BlockLoc == XYZWing[0].BlockLoc ? 2 : 1;
                                            commonCandidate = XYZWing[1].Candidates.Contains(XYZWing[2].Candidates[0]) ? XYZWing[2].Candidates[0] : XYZWing[2].Candidates[1];
                                            foreach (Cell neighbour3 in XYZWing[0].NeighbourCells[2])
                                            {
                                                if (neighbour3.Num == '0' && (XYZWing[outsiderCell].XLocation == neighbour3.XLocation || XYZWing[outsiderCell].YLocation == neighbour3.YLocation))
                                                {
                                                    if (neighbour3.Candidates.Remove(commonCandidate))
                                                    {
                                                        changeMade = true;
                                                        if (g_PathTracking)
                                                            g_SolvePath.Add("Candidate number " + commonCandidate + " removed from cell [" + neighbour3.XLocation + "," + neighbour3.YLocation + "] - XYZ-WING - HINGE [" + XYZWing[0].XLocation + "," + XYZWing[0].YLocation + "] - WINGS [" + XYZWing[1].XLocation + "," + XYZWing[1].YLocation + "] [" + XYZWing[2].XLocation + "," + XYZWing[2].YLocation + "]");

                                                    }
                                                }
                                            }
                                            if (changeMade)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                    if (XYZWing.Count > 2)
                                        XYZWing.RemoveAt(2);
                                    XYZWing.RemoveAt(1);
                                }

                            }
                        }

                    }
                }
            }

            return changeMade;
        }
        #endregion
        #region Single Chains
        /// <summary>
        /// Find Single's Chain(aka Simple Colouring):
        /// A chain is a collection of cells that are linked together because of common a candidate(s).
        /// Two cells can only have a link if the common candidate doesn't exist within any other cell in the group (row/column/block).
        /// This way, if one of the two cells' value becomes the shared candidate number, the other cell cannot possibly be that number.
        /// If either of those cells are linked with another cell through the same candidate number, 
        /// it would cause a CHAIN reaction of preventing and forcing certain cells to be the candidate number.
        /// Within the code below, if two cells are linked, one cell is labelled as true and the other as false.
        /// This method replicates the colouring technique, true representing one colour, false representing another.
        /// Rule 1:
        /// If a cell, outside of the chain, containing the common candidate is 'seen' by two differently coloured cells in the chain, remove that candidate from that cell.
        /// Rule 2:
        /// If a cell within the chain can 'see' another cell within the chain that is of the same colour, a contradiction has been found; 
        /// thus, all cells in the chain coloured the same can remove the common candidate from their list.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindSinglesChain(SudokuGrid grid)
        {
            bool changeMade = false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0')
                    {
                        foreach (char candidate in grid.Cells[i][j].Candidates)
                        {
                            List<Cell> chain = new List<Cell>(8) { grid.Cells[i][j] };
                            List<bool> indicatorForCell = new List<bool>(8) { true };
                            int c = 0;//Iterator for cellsContainingCandi for recursion

                            if (GenerateStrongChain_Recursion(grid, candidate, c, chain, indicatorForCell))//If true, chain has been built
                            {
                                if (SinglesChainRule1(candidate, chain, indicatorForCell))
                                    return true;
                                else if (SinglesChainRule2(candidate, chain, indicatorForCell))
                                    return true;

                            }

                        }
                    }
                }
            }
            return changeMade;
        }
        /// <summary>
        /// Code breakdown:
        /// -if the current iteration doesn't exists, end of chain has been met. (iteration is increased within the GenerateStrongChain_Recursion method call)
        /// -for each group, search through each cell in the group - call the cell 'Neighbour'
        /// -if neighbour contains the candidate and the cell value is empty,
        /// -increment a counter
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="candidate">current candidate being examined in chain</param>
        /// <param name="c">current iteration of cellsContainingCandi</param>
        /// <param name="chain">cells that have been discovered in the chain</param>
        /// <param name="indicatorForCell">the true/false indicator for each cell discovered</param>
        /// <returns>returns true once a chain is made and cannot go any further, returns false if no chain is made</returns>
        private bool GenerateStrongChain_Recursion(SudokuGrid grid, char candidate, int c, List<Cell> chain, List<bool> indicatorForCell)
        {
            if (c == chain.Count)
            {
                return true;
            }
            for (int index = 0; index < 3; index++)
            {
                int counter = 0;
                Cell saveCell = new Cell();
                foreach (Cell neighbour in chain[c].NeighbourCells[index])
                {
                    if (neighbour.Candidates.Contains(candidate) && neighbour.Num == '0')
                    {
                        if (++counter > 1)
                        {
                            break;
                        }
                        saveCell = neighbour;

                    }
                }
                if (counter == 1 && saveCell.Candidates != null)
                {
                    if (!chain.Contains(saveCell))
                    {
                        chain.Add(saveCell);
                        indicatorForCell.Add(!indicatorForCell[c]);
                    }
                }
            }
            if (GenerateStrongChain_Recursion(grid, candidate, ++c, chain, indicatorForCell))
                return true;
            return false;
        }/// <summary>
         /// Rule 1 Pseudocode:
         /// FUNCTION SinglesChainRule1
         ///    PASS IN: 
         ///         VARIABLE common candidate number named 'CCN'
         ///         VARIABLE list of cells in chain named 'Chain'
         ///         VARIABLE list of bools corresponding to Chain 'CellColours'
         ///    FOR each cell in Chain, ASSIGN as CellA
         ///        FOR each neighbour group of CellA
         ///            FOR each neighbour in group, ASSIGN as nbrA
         ///                IF neighbour contains CCN and neighbour is not in Chain
         ///                    FOR each cell added after CellA in the list Chain, ASSIGN as CellB
         ///                        IF colour of CellA and CellB are different
         ///                            FOR each neighbour group of CellB
         ///                                FOR each neighbour in group, ASSIGN as nbrB
         ///                                    IF nbrA and nbrB are the same, i.e. the cell outside the chain contains the CCN and is a neighbour to two cells of alternate colour
         ///                                        REMOVE CCN from the candidate list of nbrA/nbrB
         ///                                    ENDIF
         ///                                ENDFOR
         ///                            ENDFOR
         ///                        ENDIF
         ///                    ENDFOR
         ///                ENDIF
         ///            ENDFOR
         ///        ENDFOR
         ///    ENDFOR
         /// ENDFUNCTION
         /// </summary>
         /// <param name="grid"></param>
         /// <param name="candidate"></param>
         /// <param name="chain"></param>
         /// <param name="indicatorForCell"></param>
         /// <returns></returns>
        private bool SinglesChainRule1(char candidate, List<Cell> chain, List<bool> indicatorForCell)
        {
            bool changeMade = false;
            for (int l = 0; l < chain.Count; l++)//Iterate through all cells in constructed chain (name current cell 'cellA')
            {
                for (int index = 0; index < 3; index++)//iterates over groups (row/column/block)
                {
                    foreach (Cell neighbour in chain[l].NeighbourCells[index])//iterate through the cells in the row/column/block associated with 'cellA', (neighbourA)
                    {
                        if (neighbour.Candidates.Contains(candidate) && !chain.Contains(neighbour))//if neighbourA contains the common candidate and isn't in the chain...
                        {
                            for (int k = l + 1; k < chain.Count; k++)//iterate through the cells in the chain, start one cell ahead in the chain list (cellB) - because the previous cells in the chain have already looked at chain[l]
                            {
                                if (indicatorForCell[l] != indicatorForCell[k])//if the indicator (colour) of cellA is different to that of cellB...
                                {
                                    for (int index2 = 0; index2 < 3; index2++)//iterates over neighbour groups of CellB(row/column/block)
                                    {
                                        foreach (Cell neighbour2 in chain[k].NeighbourCells[index2])//iterate through the cells in the row/column/block associated with 'cellB', (neighbourB)
                                        {
                                            if (neighbour == neighbour2 && neighbour2.Candidates.Contains(candidate))//if two cells alternately coloured cells in the chain can see the same cell that contains the common candidate...
                                            {
                                                neighbour.Candidates.Remove(candidate);//...the candidate can be removed from that cell

                                                changeMade = true;
                                                if (g_PathTracking)
                                                    g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + neighbour.XLocation + "," + neighbour.YLocation + "] - SINGLE'S CHAIN / SIMPLE COLOURING - Rule 1");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (changeMade)
            {
                if (g_PathTracking)
                    g_SolvePath.Add("Chain: Blue[" + chain[0].XLocation + "," + chain[0].YLocation + "]");
                string colour = "";
                for (int z = 1; z < chain.Count; z++)
                {
                    colour = indicatorForCell[z] ? "Blue" : "Red";
                    if (g_PathTracking)
                        g_SolvePath[g_SolvePath.Count - 1] += " - " + colour + "[" + chain[z].XLocation + "," + chain[z].YLocation + "]";
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Rule 2 Pseudocode:
        /// FUNCTION SinglesChainRule2
        ///     PASS IN: 
        ///         VARIABLE common candidate number named 'CCN'
        ///         VARIABLE list of cells in chain named 'Chain'
        ///         VARIABLE list of bools corresponding to Chain named 'CellColours'
        ///     FOR each cell in the Chain list, ASSIGN as CellA
        ///         FOR each cell in the Chain list added after CellA, ASSIGN as CellB
        ///             IF the colour of CellA and CellB are the same AND CellA and CellB share a group
        ///                 FOR each cell in the Chain list, ASSIGN as CellC
        ///                     IF CellC is the same colour as CellA/CellB
        ///                         REMOVE CCN from CellC
        ///                     ENDIF
        ///                 ENDFOR
        ///             ENDIF
        ///         ENDFOR
        ///     ENDFOR
        /// ENDFUNCTION
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="candidate"></param>
        /// <param name="chain"></param>
        /// <param name="indicatorForCell"></param>
        /// <returns></returns>
        private bool SinglesChainRule2(char candidate, List<Cell> chain, List<bool> indicatorForCell)
        {
            bool changeMade = false;
            bool? contradiction = null;
            for (int c1 = 0; c1 < chain.Count && contradiction == null; c1++)//iterate through all cells in chain (cellA), also whilst a contradiction hasn't been found
            {
                for (int c2 = c1 + 1; c2 < chain.Count && contradiction == null; c2++)//start iterating one cell ahead of 'cellA' (cellB), also whilst a contradiction hasn't been found
                {
                    if (indicatorForCell[c1] == indicatorForCell[c2] && (chain[c1].XLocation == chain[c2].XLocation || chain[c1].YLocation == chain[c2].YLocation || chain[c1].BlockLoc == chain[c2].BlockLoc))//if cellA and cellB are coloured the same...
                    {
                        contradiction = indicatorForCell[c1];
                        if (g_PathTracking)
                            g_SolvePath.Add("SINGLE'S CHAIN / SIMPLE COLOURING - Rule 2\r\n\r\nContradiction in cells coloured " + (indicatorForCell[c1] ? "Blue" : "Red") + " between cells [" + chain[c1].XLocation + "," + chain[c1].YLocation + "] and [" + chain[c2].XLocation + "," + chain[c2].YLocation + "]");
                        for (int d = 0; d < chain.Count; d++)//loop over cells in chain...
                        {
                            if (indicatorForCell[d] == contradiction)//if the cell is coloured the same as the indicated contradiction, remove the candidate from the cell
                            {
                                chain[d].Candidates.Remove(candidate);
                                changeMade = true;
                                if (g_PathTracking)
                                    g_SolvePath.Add("-Candidate number " + candidate + " removed from cell [" + chain[d].XLocation + "," + chain[d].YLocation + "]");
                            }
                        }
                    }
                }
            }

            if (changeMade)
            {
                if (g_PathTracking)
                    g_SolvePath.Add("Chain: Blue[" + chain[0].XLocation + "," + chain[0].YLocation + "]");
                string colour = "";
                for (int z = 1; z < chain.Count; z++)
                {
                    colour = indicatorForCell[z] ? "Blue" : "Red";
                    if (g_PathTracking)
                        g_SolvePath[g_SolvePath.Count - 1] += " - " + colour + "[" + chain[z].XLocation + "," + chain[z].YLocation + "]";
                }
                return true;
            }
            return false;
        }
        #endregion
        #endregion
        #region Uniqueness Strategies
        /// <summary>
        /// Rule for Type1 Unique Rectangle:
        /// -There must four pairs in a rectangle that spans two blocks.
        /// -All of the pairs must be naked, apart from one.
        /// -The cell that contains more than just the pair is able to get rid of the pair from the candidate number list.
        /// -Reason: A sudoku puzzle cannot contain four conjugate pairs as there can only be one solution to the puzzle.
        /// -If numbers are interchangeable, that means there is more than one solution
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindUniqueRectangleType1(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Candidates.Count == 2 && grid.Cells[i][j].Num == '0')
                    {
                        Cell cellA = grid.Cells[i][j];
                        for (int bn = 0; bn < 8; bn++)//bn=block neighbour
                        {//Find a pair in the block, identify if it shares an XLocation or YLocation
                            if (cellA.NeighbourCells[2][bn].Num == '0' && cellA.NeighbourCells[2][bn].Candidates.SequenceEqual(cellA.Candidates)
                                && (cellA.NeighbourCells[2][bn].YLocation == j || cellA.NeighbourCells[2][bn].XLocation == i))//if true, CellA shares an YLocation with CellB. start searching for UR's across from the cells
                            {
                                int index = 0;
                                if (cellA.NeighbourCells[2][bn].XLocation == i)
                                {
                                    index = 1;
                                }
                                Cell cellB = cellA.NeighbourCells[2][bn];
                                for (int rn = 0; rn < 8; rn++)//rn = row neighbour
                                {
                                    if (cellA.NeighbourCells[index][rn].Num == '0' && cellA.NeighbourCells[index][rn].Candidates.SequenceEqual(cellA.Candidates)
                                        && cellA.NeighbourCells[index][rn].BlockLoc != cellA.BlockLoc)//Finds if a cell C exists
                                    {
                                        Cell cellC = cellA.NeighbourCells[index][rn];
                                        int xAxis = 0;
                                        int yAxis = 0;
                                        if ((cellA.XLocation == cellB.XLocation || cellA.XLocation == cellC.XLocation)
                                            && (cellA.YLocation == cellB.YLocation || cellA.YLocation == cellC.YLocation))
                                        {//opposite cell is A
                                            if (index == 0)
                                            { xAxis = cellB.XLocation; yAxis = cellC.YLocation; }
                                            else
                                            { xAxis = cellC.XLocation; yAxis = cellB.YLocation; }
                                        }
                                        else if ((cellB.XLocation == cellA.XLocation || cellB.XLocation == cellC.XLocation)
                                            && (cellB.YLocation == cellA.YLocation || cellB.YLocation == cellC.YLocation))
                                        {//opposite cell is B
                                            if (index == 0)
                                            { xAxis = cellA.XLocation; yAxis = cellC.YLocation; }
                                            else
                                            { xAxis = cellC.XLocation; yAxis = cellA.YLocation; }
                                        }
                                        //CellC cannot be opposite to CellD
                                        Cell cellD = grid.Cells[xAxis][yAxis];
                                        if (cellD.Candidates.Contains(cellC.Candidates[0])
                                            && cellD.Candidates.Contains(cellC.Candidates[1]))//Searches for if a Cell D exists
                                        {
                                            cellD.Candidates.Remove(cellC.Candidates[0]);
                                            cellD.Candidates.Remove(cellC.Candidates[1]);
                                            changeMade = true;
                                            if (g_PathTracking)
                                                g_SolvePath.Add("Candidate number " + cellC.Candidates[0] + " and " + cellC.Candidates[1] + " removed from cell [" + cellD.XLocation + "," + cellD.YLocation + "] - UNIQUE RECTANGLE - OTHER CORNERS [" + cellA.XLocation + "," + cellA.YLocation + "] [" + cellB.XLocation + "," + cellB.YLocation + "] [" + cellC.XLocation + "," + cellC.YLocation + "]");
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return changeMade;
        }
        #endregion
        #endregion
        #region Backtracking Solver
        /// <summary>
        /// Removes all values from the current cell's candidate list that are also found within neighbouring cells
        /// I.e. Cells that are in the same groups as the cell in question.
        /// A group is a block/row/column
        /// </summary>
        /// <param name="grid">Sudoku grid that is passed into and mutated in the method</param>
        /// <param name="row">Current row number being examined in this instance of the method</param>
        /// <param name="col">Current column number being examined in this instance of the method</param>
        /// <returns>false if an error occurs and a candidate list contains 0 values | true if it is ok</returns>        
        public bool RemoveCands(SudokuGrid grid, int row, int col, byte variator)
        {
            for (int n = 0; n < 8; n++)
            {
                if (grid.Cells[row][col].Candidates.Contains(grid.Cells[row][col].NeighbourCells[0][n].Num))
                {
                    grid.Cells[row][col].Candidates.Remove(grid.Cells[row][col].NeighbourCells[0][n].Num);
                }
                if (grid.Cells[row][col].Candidates.Contains(grid.Cells[row][col].NeighbourCells[1][n].Num))
                {
                    grid.Cells[row][col].Candidates.Remove(grid.Cells[row][col].NeighbourCells[1][n].Num);
                }
                if (grid.Cells[row][col].Candidates.Contains(grid.Cells[row][col].NeighbourCells[2][n].Num))
                {
                    grid.Cells[row][col].Candidates.Remove(grid.Cells[row][col].NeighbourCells[2][n].Num);
                }
                if (grid.Cells[row][col].Candidates.Count == 0)
                {
                    return false;
                }
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
        public bool CompileBacktracker(SudokuGrid grid, byte variator)
        {
            List<Cell> emptyCells = new List<Cell>();
            bool first = true;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Cells[i][j].Num == '0')
                    {
                        if (variator == 0 || variator == 3 || variator == 4)
                            grid.Cells[i][j].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                        else if (variator == 1)
                            grid.Cells[i][j].Candidates = new List<char> { '9', '8', '7', '6', '5', '4', '3', '2', '1' };
                        else if (variator == 2)
                            grid.Cells[i][j].Candidates = g_Gen.Shuffler(new List<char> { '9', '8', '7', '6', '5', '4', '3', '2', '1' });
                        if (variator == 0 || variator == 3)
                        {
                            RemoveCands(grid, i, j, variator);
                            if (first)
                            {
                                first = false;
                                emptyCells.Add(grid.Cells[i][j]);
                            }
                            else
                            {
                                for (int k = 0; k < emptyCells.Count; k++)
                                {
                                    if (grid.Cells[i][j].Candidates.Count < emptyCells[k].Candidates.Count)
                                    {
                                        emptyCells.Insert(k, grid.Cells[i][j]);
                                        break;
                                    }
                                    else if (k == emptyCells.Count - 1)
                                    {
                                        emptyCells.Add(grid.Cells[i][j]);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                            emptyCells.Add(grid.Cells[i][j]);
                    }
                }
            }
            return Backtracker(grid, 0, variator, emptyCells);
        }
        public bool Backtracker(SudokuGrid grid, int k, byte variator, List<Cell> emptyCells)
        {
            if (k >= emptyCells.Count)
                return g_Gen.CheckIfSolved(grid);
            List<char> oldCands = new List<char>();
            oldCands.AddRange(emptyCells[k].Candidates);
            RemoveCands(grid, emptyCells[k].XLocation, emptyCells[k].YLocation, variator);
            for (int n = 0; n < emptyCells[k].Candidates.Count; n++)
            {
                emptyCells[k].Num = emptyCells[k].Candidates[n];
                if (variator == 3)
                    g_BacktrackingPath.Add(emptyCells[k].XLocation.ToString() + emptyCells[k].YLocation.ToString() + emptyCells[k].Candidates[n]);//Add to solve path
                if (Backtracker(grid, k + 1, variator, emptyCells))
                    return true;
            }
            emptyCells[k].Num = '0';
            emptyCells[k].Candidates = oldCands;
            if (variator == 3)
                g_BacktrackingPath.Add(emptyCells[k].XLocation.ToString() + emptyCells[k].YLocation.ToString() + "0");//Add to solve path
            return false;
        }
        #endregion
        #region Solver Decisions Method for Solving Cell by Cell button
        public bool SolveNextStep(SudokuGrid grid)
        {
            g_PathTracking = true;
            if (g_Gen.CheckIfSolved(grid))
                return true;
            int index;

            g_changeMade = false;
            string separator = "|>---------------------------------------------------------------------------------------------------------------------->|";

            g_SolvePath.Add("STEP " + ++g_StepCounter + ": ");
            index = g_SolvePath.Count - 1;
            if (g_strategy != "Clean Candidates" && LastCandidates(grid))
            {
                g_strategy = "Last Candidates filled in";
                g_SolvePath.RemoveRange(index - 1, 2);
                g_StepCounter--;
            }
            else if (CleanCandidateLists(grid))
            {
                g_strategy = "Clean Candidates";
                g_SolvePath.RemoveAt(index);
                g_StepCounter--;
            }
            else if (FindNakedSingles(grid))
            {
                g_strategy = "Naked Single(s)";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindHiddenSingles(grid))
            {
                g_strategy = "Hidden Single(s)";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindNakedPair(grid))//A Naked Pair contributes +150 to the rating.
            {
                g_Rating += 150;
                g_moderate = true;
                g_strategy = "Naked Pair";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindHiddenPair(grid))//A Hidden Pair contributes +175 to the rating.
            {
                g_Rating += 175;
                g_moderate = true;
                g_strategy = "Hidden Pair";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindPointingNumbers(grid))//A Point Pair/Triple contributes +200 to the rating.
            {
                g_Rating += 200;
                g_moderate = true;
                g_strategy = "Pointing Line";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindBlockLineReduce(grid))//A Box-Line Reduction contributes +200 to the rating.
            {
                g_Rating += 225;
                g_moderate = true;
                g_strategy = "Block-Line Reduction";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindNakedTriple(grid))//A Naked Triple contributes +300 to the rating.
            {
                g_Rating += 300;
                g_moderate = true;
                g_strategy = "Naked Triple";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindHiddenTriple(grid))//A Hidden Triple contributes +400 to the rating.
            {
                g_Rating += 400;
                g_moderate = true;
                g_strategy = "Hidden Triple";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindXWing(grid))//An X-Wing contributes +500 to the rating.
            {
                g_Rating += 500;
                g_advanced = true;
                g_strategy = "X-Wing";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindSinglesChain(grid))//A Single's Chain contributes +600 to the rating.
            {
                g_Rating += 600;
                g_advanced = true;
                g_strategy = "Single's Chain/Simple Colouring";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindYWing(grid))//A Y-Wing contributes +650 to the rating.
            {
                g_Rating += 650;
                g_advanced = true;
                g_strategy = "Y-Wing";
                g_SolvePath[index] += g_strategy;
            }
            else if (FindXYZWing(grid))//An XYZ-Wing contributes +700 to the rating.
            {
                g_Rating += 700;
                g_advanced = true;
                g_strategy = "XYZ-Wing";
                g_SolvePath[index] += g_strategy;
            }

            else if (FindUniqueRectangleType1(grid))//A Unique Rectangle contributes +750 to the rating.
            {
                g_Rating += 750;
                g_advanced = true;
                g_strategy = "Unique Rectangle Type 1";
                g_SolvePath[index] += g_strategy;
            }
            //More methods to add
            else
            {
                g_StepCounter--;
                g_SolvePath.RemoveAt(g_SolvePath.Count - 1);
                CompileBacktracker(grid, 0);
                g_BacktrackingReq = true;
                g_Difficulty = "Extreme";
                g_SolvePath.Add("BACKTRACKING/TRIAL-AND-ERROR USED TO FINISH PUZZLE - UNABLE TO FINISH WITH IMPLEMENTED STRATEGIES");
                g_SolvePath.Add(separator);
                g_strategy = "Trial-and-error/Brute-force";
            }
            if (!g_BacktrackingReq)
            {
                if (g_moderate && !g_advanced)
                    g_Difficulty = "Moderate";
                if (g_advanced)
                    g_Difficulty = "Advanced";
            }
            g_SolvePath.Add(separator);
            if (g_Gen.CheckIfSolved(grid))
            {
                g_Rating = (int)(g_Rating * (1 + ((double)g_StepCounter / 100)));
                g_SolvePath.Add("FINISHED");
                return true;
            }

            return false;
        }
        #endregion
    }
}
