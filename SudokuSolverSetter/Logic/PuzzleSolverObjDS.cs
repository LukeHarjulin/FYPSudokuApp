using System.Collections.Generic;
using System.Linq;

namespace SudokuSolverSetter
{
    /// <summary>
    /// This class handles Sudoku solving using objects (Cell/SudokuGrid)
    /// </summary>
    public class PuzzleSolverObjDS
    {
        //backtracking required?                change made?            moderate?           advanced?
        public bool g_BacktrackingReq = false, g_changeMade = false, g_moderate = false, g_advanced = false;
        public int g_Rating = 0;//puzzle rating score
        public string g_Difficulty = "Beginner", g_strategy = "";//puzzle difficulty
        private readonly PuzzleGenerator g_Gen = new PuzzleGenerator();
        public List<string> g_SolvePath = new List<string>();//solve path using strategies
        public List<string> g_BacktrackingPath = new List<string>();//solve path using backtracker
        public int[] g_StrategyCount = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="method">method '1' is human-strategy solver. '2' is Backtracking solver. </param>
        /// <returns>returns true if the puzzle is solved, false if not</returns>
        public bool Solver(SudokuGrid grid, int method)//Once called, the solver will attempt to entirely solve the puzzle, making decisions based off of the current state of the puzzle.
        {
            g_Rating = 0;
            g_SolvePath = new List<string>();
            g_BacktrackingPath = new List<string>();
            g_changeMade = false; g_moderate = false; g_advanced = false;
            g_BacktrackingReq = false;
            /*
             *  This do...while is necessary for repeating these methods for solving until no changes are made (which it assumes that the puzzle is complete or it could not complete it)
             *  The if and elses are to make the process faster of solving faster, 
                as it ensures that it tries the easiest less computationally heavy methods first before the more complex methods.
            */
            if (method == 1)
            {
                string separator = "|>---------------------------------------------------------------------------------------------------------------------->|";
                do
                {
                    if (CleanCandidateLists(grid))
                    {
                        g_SolvePath.Add(separator);
                    }
                    if (FindNakedSingles(grid))//Naked singles contributes +10 to the rating for each naked single found
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        
                    }
                    else if (FindHiddenSingles(grid))//Naked singles contributes +15 to the rating for each hidden single found
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                       
                    }
                    else if (FindNakedPair(grid))//A Naked Pair contributes +100 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_Rating += 100;
                        g_moderate = true;
                        g_StrategyCount[3]++;
                    }
                    else if (FindHiddenPair(grid))//A Hidden Pair contributes +150 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_Rating += 150;
                        g_moderate = true;
                        g_StrategyCount[4]++;
                    }
                    else if (FindPointingNumbers(grid))//A Point Pair/Triple contributes +250 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_Rating += 250;
                        g_moderate = true;
                        g_StrategyCount[5]++;
                    }
                    else if (FindBlockLineReduce(grid))//A Box-Line Reduction contributes +300 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_Rating += 300;
                        g_moderate = true;
                        g_StrategyCount[6]++;
                    }
                    else if (FindNakedTriple(grid))//A Naked Triple contributes +400 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_Rating += 400;
                        g_moderate = true;
                        g_StrategyCount[7]++;
                    }
                    else if (FindHiddenTriple(grid))//A Hidden Triple contributes +450 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_Rating += 450;
                        g_moderate = true;
                        g_StrategyCount[8]++;
                    }
                    else if (FindXWing(grid))//An X-Wing contributes +550 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_Rating += 550;
                        g_advanced = true;
                        g_StrategyCount[9]++;
                    }
                    else if (FindYWing(grid))//A Y-Wing contributes +600 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_Rating += 600;
                        g_advanced = true;
                        g_StrategyCount[10]++;
                    }
                    else if (FindXYZWing(grid))//An XYZ-Wing contributes +650 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_Rating += 650;
                        g_advanced = true;
                        g_StrategyCount[11]++;
                    }
                    else if (FindSingleChains(grid))//A successful Single Chain contributes +700 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_advanced = true;
                        g_StrategyCount[12]++;
                    }
                    else if (FindUniqueRectangleType1(grid))//A Unique Rectangle contributes +750 to the rating.
                    {
                        g_changeMade = true;
                        g_SolvePath.Add(separator);
                        g_Rating += 750;
                        g_advanced = true;
                        g_StrategyCount[13]++;
                    }
                    //More methods to add
                    else
                    {
                        g_changeMade = false;
                        g_SolvePath.Add(separator);
                    }
                } while (g_changeMade);
            }
            else if (method == 2)
            {
                BacktrackingSolver(grid, 0, 0, 3);
            }
            if (!g_Gen.CheckIfSolved(grid))
            {
                BacktrackingSolver(grid, 0, 0, 0);
                g_BacktrackingReq = true;
                g_Difficulty = "Extreme";
                g_SolvePath.Add("BACKTRACKING/TRIAL-AND-ERROR USED TO FINISH PUZZLE - UNABLE TO FINISH WITH IMPLEMENTED STRATEGIES");
                g_StrategyCount[0]++;
                return false;
            }
            if (g_moderate)
                g_Difficulty = "Moderate";
            if (g_advanced)
                g_Difficulty = "Advanced";
            return g_Gen.CheckIfSolved(grid);
        }
        /// <summary>
        /// Looks at all cells and removes numbers from candidate lists that can't exist in that cell. 
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
#region Strategy-Usage Solver method
        
        private bool CleanCandidateLists(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Rows[i][j].Num == '0')
                    {
                        for (int n = 0; n < 8; n++)
                        {
                            if (grid.Rows[i][j].Candidates.Contains(grid.Rows[i][j].NeighbourCells[0][n].Num))
                            {
                                grid.Rows[i][j].Candidates.Remove(grid.Rows[i][j].NeighbourCells[0][n].Num);
                                g_SolvePath.Add("---Candidate number " + grid.Rows[i][j].NeighbourCells[0][n].Num + " removed from cell [" + i + "," + j + "] - NUMBER EXISTS IN ROW");
                                changeMade = true;
                            }
                            if (grid.Rows[i][j].Candidates.Contains(grid.Rows[i][j].NeighbourCells[1][n].Num))
                            {
                                grid.Rows[i][j].Candidates.Remove(grid.Rows[i][j].NeighbourCells[1][n].Num);
                                g_SolvePath.Add("---Candidate number " + grid.Rows[i][j].NeighbourCells[1][n].Num + " removed from cell [" + i + "," + j + "] - NUMBER EXISTS IN COLUMN");
                                changeMade = true;
                            }
                            if (grid.Rows[i][j].Candidates.Contains(grid.Rows[i][j].NeighbourCells[2][n].Num))
                            {
                                grid.Rows[i][j].Candidates.Remove(grid.Rows[i][j].NeighbourCells[2][n].Num);
                                g_SolvePath.Add("---Candidate number " + grid.Rows[i][j].NeighbourCells[2][n].Num + " removed from cell [" + i + "," + j + "] - NUMBER EXISTS IN BLOCK");
                                changeMade = true;
                            }
                        }
                    }
                }
            }
            return changeMade;
        }
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
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Rows[i][j].Num == '0' && grid.Rows[i][j].Candidates.Count == 1)
                    {
                        grid.Rows[i][j].Num = grid.Rows[i][j].Candidates[0];
                        grid.Rows[i][j].Candidates.Clear();
                        changeMade = true;
                        g_SolvePath.Add("Number " + grid.Rows[i][j].Num + " placed in cell [" + i + "," + j + "] - NAKED SINGLE");
                        g_Rating += 10;
                        g_StrategyCount[1]++;
                    }
                }
            }
            return changeMade;
        }
        /// <summary>
        /// Hidden Singles
        /// Searches through all cells, if a cell is empty  - call it cellA, 
        /// for each group related to cellA,
        /// iterate from 1-9 and check each cell (call the cell cellB) in the group if it contains the current number.
        /// If cellB contains the number, remove it from the 1-9 list.
        /// If the loop reaches the end of a group and the list contains only 1 candidate number, cellA's value must be that last candidate number
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindHiddenSingles(SudokuGrid grid)
        {
            bool changeMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Rows[i][j].Num == '0')
                    {
                        List<char> numList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                        for (int index = 0; index < 3 && grid.Rows[i][j].Num == '0'; index++)
                        {
                            foreach (char candidate in grid.Rows[i][j].Candidates)
                            {
                                int counter = 0;
                                foreach (Cell neighbour in grid.Rows[i][j].NeighbourCells[index])
                                {
                                    if (neighbour.Num == candidate || neighbour.Num == '0' && neighbour.Candidates.Contains(candidate))
                                    {
                                        if (++counter > 0)
                                            break;
                                    }
                                }
                                if (counter == 0)
                                {
                                    grid.Rows[i][j].Num = candidate;
                                    grid.Rows[i][j].Candidates.Clear();
                                    changeMade = true;
                                    g_Rating += 15;
                                    g_StrategyCount[2]++;
                                    g_SolvePath.Add("Number " + candidate + " placed in cell [" + i + "," + j + "] - HIDDEN SINGLE");
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return changeMade;
        }
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
                    if (grid.Rows[i][j].Num == '0' && grid.Rows[i][j].Candidates.Count == 2)
                    {
                        for (int index = 0; index < 3; index++)
                        {
                            foreach (Cell neighbour in grid.Rows[i][j].NeighbourCells[index])
                            {
                                if (neighbour.Num == '0' && neighbour.Candidates.SequenceEqual(grid.Rows[i][j].Candidates))
                                {
                                    foreach (Cell cell in grid.Rows[i][j].NeighbourCells[index])
                                    {
                                        if (cell != neighbour && cell.Candidates.Contains(neighbour.Candidates[0]) && cell.Num == '0')
                                        {
                                            cell.Candidates.Remove(neighbour.Candidates[0]);
                                            changeMade = true;
                                            g_SolvePath.Add("Candidate number " + neighbour.Candidates[0] + " removed from cell [" + cell.XLocation + "," + cell.YLocation + "] - NAKED PAIR");
                                        }
                                        if (cell != neighbour && cell.Candidates.Contains(neighbour.Candidates[1]) && cell.Num == '0')
                                        {
                                            cell.Candidates.Remove(neighbour.Candidates[1]);
                                            changeMade = true;
                                            g_SolvePath.Add("Candidate number " + neighbour.Candidates[1] + " removed from cell [" + cell.XLocation + "," + cell.YLocation + "] - NAKED PAIR");
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
                    if (grid.Rows[i][j].Num == '0' && grid.Rows[i][j].Candidates.Count > 2)
                    {
                        for (int index = 0; index < 3 && grid.Rows[i][j].Candidates.Count > 2; index++)
                        {
                            List<Cell> cellList = new List<Cell>(9);//Stores all cells that contain candidates
                            List<char> candis = new List<char>(9);//If only two candidates exist in the group, add the candidate to this list
                            foreach (char candidate in grid.Rows[i][j].Candidates)
                            {
                                int counter = 0;
                                foreach (Cell neighbour in grid.Rows[i][j].NeighbourCells[index])
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
                                            removeList1.AddRange(grid.Rows[i][j].Candidates);
                                            removeList2.AddRange(cellList[b].Candidates);
                                            removeList1.RemoveAll(candidate => candidate == candis[a] || candidate == candis[b]);
                                            removeList2.RemoveAll(candidate => candidate == candis[a] || candidate == candis[b]);

                                            grid.Rows[i][j].Candidates.RemoveAll(candidate => candidate != candis[a] && candidate != candis[b]);//Removes all candidates but the common candidates
                                            cellList[b].Candidates.RemoveAll(candidate => candidate != candis[a] && candidate != candis[b]);
                                            
                                            if (removeList1.Count != 0)
                                            {
                                                changeMade = true;
                                                g_SolvePath.Add("Candidate numbers " + new string(removeList1.ToArray()) + " removed from cell [" + i + "," + j + "] - HIDDEN PAIR");
                                            }
                                            if (removeList2.Count != 0)
                                            {
                                                changeMade = true;
                                                g_SolvePath.Add("Candidate numbers " + new string(removeList2.ToArray()) + " removed from cell [" + cellList[b].XLocation + "," + cellList[b].YLocation + "] - HIDDEN PAIR");
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
        /// <summary>
        /// A naked triple is where any three cells that share a group contain, in total, three candidate numbers.
        /// The combinations of candidates for a naked triple are:
        /// [x,y,z] [x,y,z] [x,y,z]
        /// [x,y,z] [x,y,z] [x,y] 
        /// [x,y,z] [x,y]   [x,z] 
        /// [x,y]   [x,z]   [y,z]
        /// Find Naked Triple in 3,3,3 | 3,3,2 | 3,2,2 format:
        /// -Iterate over cells till an empty cell is found containing two/three candidates, call that cell 'CellA'
        /// -Iterate over each cell in each group associated with CellA, call it cellB
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
                    if (grid.Rows[i][j].Num == '0' && grid.Rows[i][j].Candidates.Count == 3)
                    {
                        for (int index = 0; index < 3; index++)
                        {
                            List<Cell> triples = new List<Cell>(3)
                            {
                                grid.Rows[i][j]
                            };
                            foreach (Cell neighbour in grid.Rows[i][j].NeighbourCells[index])//Foreach neighbour to cellA, find out how many candidates they have in common
                            {
                                int candiCount = 0;
                                foreach (char candidate in grid.Rows[i][j].Candidates)
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
                                    g_SolvePath.Add("Something has gone terribly wrong with naked triple...");//emergency eject - should never occur
                                    return false;
                                }
                            }
                            if (triples.Count == 3)
                            {
                                for (int n = 0; n < 8; n++)
                                {
                                    if (grid.Rows[i][j].NeighbourCells[index][n].Num == '0' && grid.Rows[i][j].NeighbourCells[index][n] != triples[1] && grid.Rows[i][j].NeighbourCells[index][n] != triples[2])
                                    {
                                        string removed = "";
                                        if (grid.Rows[i][j].NeighbourCells[index][n].Candidates.Remove(triples[0].Candidates[0]))
                                        {
                                            changeMade = true;
                                            removed += triples[0].Candidates[0];
                                        }
                                        if (grid.Rows[i][j].NeighbourCells[index][n].Candidates.Remove(triples[0].Candidates[1]))
                                        {
                                            changeMade = true;
                                            removed += triples[0].Candidates[1];
                                        }
                                        if (grid.Rows[i][j].NeighbourCells[index][n].Candidates.Remove(triples[0].Candidates[2]))
                                        {
                                            changeMade = true;
                                            removed += triples[0].Candidates[2];
                                        }
                                        if (removed.Length > 0)
                                        {
                                            g_SolvePath.Add("Candidate number " + removed + " removed from cell [" + grid.Rows[i][j].NeighbourCells[index][n].XLocation + "," + grid.Rows[i][j].NeighbourCells[index][n].YLocation + "] - NAKED TRIPLE");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (grid.Rows[i][j].Num == '0' && grid.Rows[i][j].Candidates.Count == 2)//Finds naked triples that are in the 2/2/2 format, i.e. [1,2] [2,3] [1,3]
                    {
                        for (int index = 0; index < 3; index++)
                        {
                            bool foundTriple = false;
                            foreach (Cell neighbour in grid.Rows[i][j].NeighbourCells[index])
                            {
                                if (neighbour.Num == '0' && neighbour.Candidates.Count == 2 
                                    && (neighbour.Candidates.Contains(grid.Rows[i][j].Candidates[0]) || neighbour.Candidates.Contains(grid.Rows[i][j].Candidates[1])))//Try find cellC with this cellB to make a triple
                                {
                                    char candidateA = neighbour.Candidates.Contains(grid.Rows[i][j].Candidates[0]) ? grid.Rows[i][j].Candidates[1] : grid.Rows[i][j].Candidates[0];
                                    char candidateB = grid.Rows[i][j].Candidates.Contains(neighbour.Candidates[0]) ? neighbour.Candidates[1] : neighbour.Candidates[0];
                                    char candidateC = grid.Rows[i][j].Candidates.Contains(neighbour.Candidates[0]) ? neighbour.Candidates[0] : neighbour.Candidates[1];
                                    foreach (Cell neighbourB in grid.Rows[i][j].NeighbourCells[index])
                                    {
                                        if (neighbourB.Num == '0' && neighbourB.Candidates.Count == 2 && neighbourB.Candidates.Contains(candidateA) 
                                            && neighbourB.Candidates.Contains(candidateB))//Found 2/2/2 naked triple
                                        {
                                            foundTriple = true;
                                            for (int n = 0; n < grid.Rows[i][j].NeighbourCells[index].Count; n++)
                                            {
                                                if (grid.Rows[i][j].NeighbourCells[index][n].Num == '0' && grid.Rows[i][j].NeighbourCells[index][n] != neighbour 
                                                    && grid.Rows[i][j].NeighbourCells[index][n] != neighbourB)//Finds next empty cell in group that isn't apart of the triple
                                                {
                                                    string removed = "";
                                                    if (grid.Rows[i][j].NeighbourCells[index][n].Candidates.Remove(candidateA))//Tries to remove candidateA from the cell
                                                    {
                                                        changeMade = true;
                                                        removed += candidateA;
                                                    }
                                                    if (grid.Rows[i][j].NeighbourCells[index][n].Candidates.Remove(candidateB))//Tries to remove candidateB from the cell
                                                    {
                                                        changeMade = true;
                                                        removed += candidateB;
                                                    }
                                                    if (grid.Rows[i][j].NeighbourCells[index][n].Candidates.Remove(candidateC))//Tries to remove candidateC from the cell
                                                    {
                                                        changeMade = true;
                                                        removed += candidateC;
                                                    }
                                                    if (removed.Length > 0)
                                                    {
                                                        g_SolvePath.Add("Candidate number " + removed + " removed from cell [" + grid.Rows[i][j].NeighbourCells[index][n].XLocation + "," + grid.Rows[i][j].NeighbourCells[index][n].YLocation + "] - NAKED TRIPLE");
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
        /// <summary>
        /// This is ridiculous and complicated. So no code will be explained :(
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
                    if (grid.Rows[i][j].Num == '0' && grid.Rows[i][j].Candidates.Count > 2)
                    {
                        for (int index = 0; index < 3 && grid.Rows[i][j].Candidates.Count > 2; index++)
                        {
                            Dictionary<char, int> candidateFreq = new Dictionary<char, int>();
                            char[] fullCandiList = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                            foreach (char candidate in fullCandiList)
                            {
                                if (grid.Rows[i][j].Candidates.Contains(candidate))
                                {
                                    candidateFreq.Add(candidate, 1);
                                }
                                foreach (Cell cell in grid.Rows[i][j].NeighbourCells[index])
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
                                if (grid.Rows[i][j].Candidates.Contains(candidateA))
                                {
                                    Cell cellA = grid.Rows[i][j];
                                    foreach (Cell cellB in cellA.NeighbourCells[index])
                                    {
                                        if (cellB.Candidates.Contains(candidateA))
                                        {
                                            foreach (char candidateB in candidateFreq.Keys)
                                            {
                                                if (cellB.Candidates.Contains(candidateB) && candidateB != candidateA && (candidateFreq[candidateB] == 3 && !cellA.Candidates.Contains(candidateB))==false)
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
                                                                            g_SolvePath.Add("Candidate numbers " + new string(removeList.ToArray()) + " removed from cell [" + cellA.XLocation + "," + cellA.YLocation + "] - HIDDEN TRIPLE");
                                                                            
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
                                                                            g_SolvePath.Add("Candidate numbers " + new string(removeList.ToArray()) + " removed from cell [" + cellB.XLocation + "," + cellB.YLocation + "] - HIDDEN TRIPLE");
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
                                                                            g_SolvePath.Add("Candidate numbers " + new string(removeList.ToArray()) + " removed from cell [" + cellC.XLocation + "," + cellC.YLocation + "] - HIDDEN TRIPLE");
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
                    if (grid.Rows[i][j].Num == '0')
                    {
                        foreach (char candidate in grid.Rows[i][j].Candidates)
                        {
                            List<Cell> cellSave = new List<Cell>(2);
                            foreach (Cell neighbour in grid.Rows[i][j].NeighbourCells[2])
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
                                    if (grid.Rows[i][j].NeighbourCells[index][n] != cellSave[0] && grid.Rows[i][j].NeighbourCells[index][n].Candidates.Contains(candidate))
                                    {
                                        grid.Rows[i][j].NeighbourCells[index][n].Candidates.Remove(candidate);
                                        changeMade = true;
                                        g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + grid.Rows[i][j].NeighbourCells[index][n].XLocation + "," + grid.Rows[i][j].NeighbourCells[index][n].YLocation + "] - POINTING PAIR");
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
                                        if (grid.Rows[i][j].NeighbourCells[index][n] != cellSave[0] && grid.Rows[i][j].NeighbourCells[index][n] != cellSave[1] && grid.Rows[i][j].NeighbourCells[index][n].Candidates.Contains(candidate))
                                        {
                                            grid.Rows[i][j].NeighbourCells[index][n].Candidates.Remove(candidate);
                                            changeMade = true;
                                            g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + grid.Rows[i][j].NeighbourCells[index][n].XLocation + "," + grid.Rows[i][j].NeighbourCells[index][n].YLocation + "] - POINTING TRIPLE");
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
                    if (grid.Rows[i][j].Num == '0')
                    {
                        foreach (char candidate in grid.Rows[i][j].Candidates)
                        {
                            List<Cell> cellSave = new List<Cell>(2);
                            byte counterRow = 0, validCellRow = 0, counterCol = 0, validCellCol = 0;
                            for (int n = 0; n < 8; n++)
                            {
                                if (grid.Rows[i][j].NeighbourCells[0][n].Num == '0' && grid.Rows[i][j].NeighbourCells[0][n].Candidates.Contains(candidate))
                                {
                                    counterRow++;
                                    if (grid.Rows[i][j].NeighbourCells[0][n].BlockLoc == grid.Rows[i][j].BlockLoc)
                                    {
                                        validCellRow++;
                                    }
                                }
                                if (grid.Rows[i][j].NeighbourCells[1][n].Num == '0' && grid.Rows[i][j].NeighbourCells[1][n].Candidates.Contains(candidate))
                                {
                                    counterCol++;
                                    if (grid.Rows[i][j].NeighbourCells[1][n].BlockLoc == grid.Rows[i][j].BlockLoc)
                                    {
                                        validCellCol++;
                                    }
                                }
                            }
                            if (counterRow == validCellRow)
                            {
                                foreach (Cell blockNB in grid.Rows[i][j].NeighbourCells[2])
                                {
                                    if (blockNB.XLocation != grid.Rows[i][j].XLocation && blockNB.Candidates.Contains(candidate))
                                    {
                                        blockNB.Candidates.Remove(candidate);
                                        changeMade = true;
                                        g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + blockNB.XLocation + "," + blockNB.YLocation + "] - BOX-LINE REDUCTION");
                                    }
                                }
                            }
                            else if (counterCol == validCellCol)
                            {
                                foreach (Cell blockNB in grid.Rows[i][j].NeighbourCells[2])
                                {
                                    if (blockNB.YLocation != grid.Rows[i][j].YLocation && blockNB.Candidates.Contains(candidate))
                                    {
                                        blockNB.Candidates.Remove(candidate);
                                        changeMade = true;
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
        /// <summary>
        /// Find YWing:
        /// Two types: Block-Row/Column and Row-Column (Remember Groups: Index = 0 = Row, Group Index = 1 = Column, Group Index = 2 = Block)
        /// Block-Column/Row is where the hinge shares a block with another cell in y-wing and the 3rd cell is in the same column/row as the hinge
        /// -Iterate over cells till an empty cell with two candidates is found, call that cell 'CellA'/'Hinge'
        /// -Iterate over each cell in the row/column (Group Index 0/1) of the hinge, call that cell 'CellB'
        /// -If cellB is also empty and contains two candidates and shares ONLY one of it's candidates with the CellA, start searching for the third cell to the wing
        /// -Start iterating over the cells in the next group associated with CellA (e.g. if CellB was found in a row, start iterating over a column. If it was found in a column, iterate through a block),
        /// call this cell 'CellC'
        /// -If CellC is empty and ONLY contains the other candidate from CellA and the other candidate from CellB, 
        /// and none of the three Cells share more than one group(row/column/block), a Y-Wing is formed.
        /// -The candidate that can start being removed from places is the candidate that CellB and CellC have in common. (i.e. CellA {1,2}, CellB {1,3}, CellC {2,3})
        /// -This candidate can be removed from all cells where the groups from CellB and CellC overlap
        /// 
        /// The diagrams below show three different formats of YWings, the astericks(*) show where 'X' cannot exist. 
        ///     1   2   3   4   5   6   7   8   9      -|-        1   2   3   4   5   6   7   8   9     -|-        1   2   3   4   5   6   7   8   9 
        ///   _____________________________________    -|-     _____________________________________    -|-     _____________________________________
        /// A |   ¦   ¦   |   ¦ * ¦   |   ¦   ¦   |    -|-   A |XZ ¦   ¦   | * ¦ * ¦ * |   ¦   ¦   |    -|-   A |XZ ¦   ¦   |   ¦   ¦ * |   ¦   ¦   |
        /// B |   ¦   ¦   |   ¦ * ¦XZ |   ¦   ¦   |    -|-   B |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   B |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// C |   ¦   ¦   |   ¦YZ ¦   |   ¦   ¦   |    -|-   C |YZ ¦ * ¦ * |   ¦   ¦XY |   ¦   ¦   |    -|-   C |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        ///   |___________|___________|___________|    -|-     |___________|___________|___________|    -|-     |___________|___________|___________|
        /// D |   ¦   ¦   |   ¦   ¦ * |   ¦   ¦   |    -|-   D |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   D |YZ ¦   ¦   |   ¦   ¦XY |   ¦   ¦   |
        /// E |   ¦   ¦   |   ¦XY ¦ * |   ¦   ¦   |    -|-   E |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   E |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// F |   ¦   ¦   |   ¦   ¦ * |   ¦   ¦   |    -|-   F |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   F |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |
        /// G |___________________________________|    -|-   G |___________________________________|    -|-   G |___________________________________|
        /// -This is because no matter how you place X,Y,Z in those cells, X will always be ruled out of the cells marked as '*'
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
                    if (grid.Rows[i][j].Num == '0' && grid.Rows[i][j].Candidates.Count == 2)
                    {
                        Cell cellA = grid.Rows[i][j];
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
                                                            if (grid.Rows[row][col].Candidates.Contains(thirdCandi))
                                                            {
                                                                grid.Rows[row][col].Candidates.Remove(thirdCandi);
                                                                changeMade = true;
                                                                g_SolvePath.Add("Candidate number " + thirdCandi + " removed from cell [" + row + "," + col + "] - Y-WING, ROW-COLUMN - HINGE [" + cellA.XLocation + "," + cellA.YLocation + "] - WINGS [" + cellB.XLocation + "," + cellB.YLocation + "] [" + cellC.XLocation + "," + cellC.YLocation + "]");
                                                            }
                                                        }
                                                        else if (index_n == 2 && cellC.XLocation != cellA.XLocation && cellC.YLocation != cellA.YLocation )//block-column/row y-wing
                                                        {
                                                            for (int n = 0; n < 8; n++)
                                                            {
                                                                if (cellC.NeighbourCells[index][n].Candidates.Contains(thirdCandi) && cellC.NeighbourCells[index][n].BlockLoc == cellB.BlockLoc)
                                                                {
                                                                    cellC.NeighbourCells[index][n].Candidates.Remove(thirdCandi);
                                                                    changeMade = true;
                                                                    g_SolvePath.Add("Candidate number " + thirdCandi + " removed from cell [" + cellC.NeighbourCells[index][n].XLocation + "," + cellC.NeighbourCells[index][n].YLocation + "] - Y-WING, BLOCK-ROW/COLUMN - HINGE [" + cellA.XLocation + "," + cellA.YLocation + "] - WINGS [" + cellB.XLocation + "," + cellB.YLocation + "] [" + cellC.XLocation + "," + cellC.YLocation + "]");
                                                                }
                                                                if (cellB.NeighbourCells[index][n].Candidates.Contains(thirdCandi) && cellB.NeighbourCells[index][n].BlockLoc == cellC.BlockLoc)
                                                                {
                                                                    cellB.NeighbourCells[index][n].Candidates.Remove(thirdCandi);
                                                                    changeMade = true;
                                                                    g_SolvePath.Add("Candidate number " + thirdCandi + " removed from cell [" + cellC.NeighbourCells[index][n].XLocation + "," + cellC.NeighbourCells[index][n].YLocation + "] - Y-WING, BLOCK-ROW/COLUMN - HINGE [" + cellA.XLocation + "," + cellA.YLocation + "] - WINGS [" + cellB.XLocation + "," + cellB.YLocation + "] [" + cellC.XLocation + "," + cellC.YLocation + "]");
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
        /// G |___________________________________|    -|-   G |___________________________________|    -|-   G |___________________________________|   -|-   G |___________________________________|
        /// A |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   A |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   A |   ¦XYZ¦   |   ¦   ¦   |   ¦   ¦   |   -|-   A |   ¦YZ ¦   |   ¦   ¦   |   ¦   ¦   |
        /// B |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   B |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   B |   ¦XYZ¦   |   ¦   ¦   |   ¦   ¦   |   -|-   B |   ¦YZ ¦   |   ¦   ¦   |   ¦   ¦   |
        /// C |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   C |   ¦   ¦   |   ¦   ¦   |   ¦   ¦   |    -|-   C |   ¦XYZ¦   |   ¦   ¦   |   ¦   ¦   |   -|-   C |   ¦YZ ¦   |   ¦   ¦   |   ¦   ¦   |
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
                    if (grid.Rows[i][j].Num == '0')
                    {
                        foreach (char candidate in grid.Rows[i][j].Candidates)
                        {
                            Cell cellA = grid.Rows[i][j];
                            Cell cellB = CheckRowColumn(grid.Rows[i][j], candidate, 0);//check row
                            Cell cellC;
                            Cell cellD;
                            int axis = 0;
                            if (cellB != null)
                            {
                                axis = 1;//look in row
                            }
                            else
                            {
                                cellB = CheckRowColumn(grid.Rows[i][j], candidate, 1);//checks column
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
                                                    g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + cellC.NeighbourCells[axis][n].XLocation + "," + cellC.NeighbourCells[axis][n].YLocation + "] - X-WING " + type + " formed in [" + cellA.XLocation+","+cellA.YLocation+ "] [" + cellB.XLocation + "," + cellB.YLocation + "] [" + cellC.XLocation + "," + cellC.YLocation + "] ["+cellD.XLocation+","+cellD.YLocation+"]");
                                                }
                                                if (cellD.NeighbourCells[axis][n].Candidates.Contains(candidate) && cellC.NeighbourCells[axis][n] != cellA && cellC.NeighbourCells[axis][n] != cellB)
                                                {
                                                    cellD.NeighbourCells[axis][n].Candidates.Remove(candidate);
                                                    changeMade = true;
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
        /// <summary>
        /// Finding an XYZ-Wing
        /// -Very similar to a Y-Wing in the regards of a hinge and two cells named the wings.
        /// -The difference between the two is that a Y-Wing can only contain cells with 2 candidates, and each cell only has one candidate in common.
        /// -The hinge and wing system is still the same in the sense that the hinge can see the other two cells where as the other two cells can't see each other.
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
        /// G |___________________________________|    -|-   G |___________________________________|
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
                    if (grid.Rows[i][j].Num == '0' && grid.Rows[i][j].Candidates.Count == 3)
                    {
                        List<Cell> XYZWing = new List<Cell>(3)
                            {
                                grid.Rows[i][j]
                            };
                        char commonCandidate = ' ';
                        for (int index = 0; index < 3; index++)
                        {
                            foreach (Cell neighbour in XYZWing[0].NeighbourCells[index])//Foreach neighbour to first cell, find out how many candidates they have in common
                            {
                                int candiCount = 0;
                                foreach (char candidate in grid.Rows[i][j].Candidates)
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
                                    for (int index2 = index+1; index2 < 3; index2++)
                                    {
                                        foreach (Cell neighbour2 in XYZWing[0].NeighbourCells[index2])//Foreach neighbour to first cell, find out how many candidates they have in common
                                        {
                                            int candiCount2 = 0;
                                            foreach (char candidate in grid.Rows[i][j].Candidates)
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
        
        /// <summary>
        /// Find Single Chains(aka Simple Colouring):
        /// A chain is a collection of cells that are linked together because of common a candidate(s).
        /// Two cells can only have a link if the common candidate doesn't exist within any other cell in the group (row/column/block).
        /// This way, if one of the two cells' value becomes the shared candidate number, the other cell cannot possibly be that number.
        /// If either of those cells are linked with another cell through the same candidate number, 
        /// it would cause a CHAIN reaction of preventing and forcing certain cells to be the candidate number.
        /// Within the code below, if two cells are linked, one cell is labelled as true and the other as false.
        /// This method replicates the colouring technique, true representing one colour, false representing another.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindSingleChains(SudokuGrid grid)
        {
            bool changeMade = false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Rows[i][j].Num == '0')
                    {
                        foreach (char candidate in grid.Rows[i][j].Candidates)
                        {
                            List<Cell> chain = new List<Cell>(8) { grid.Rows[i][j] };
                            List<bool> indicatorForCell= new List<bool>(8) { true };
                            int c = 0;//Iterator for cellsContainingCandi for recursion
                            
                            if (ChainRecursion(grid, candidate, c, chain, indicatorForCell))//If true, chain has been built
                            {
                                for (int l = 0; l < chain.Count; l++)//Iterate through cells in chain (cellA)
                                {
                                    for (int index = 0; index < 3; index++)//iterates through groups (row/column/block) to look at
                                    {
                                        foreach (Cell neighbour in chain[l].NeighbourCells[index])//iterate through the cells in the row/column/block associated with 'cellA', (neighbourA)
                                        {
                                            if (neighbour.Candidates.Contains(candidate) && !chain.Contains(neighbour))//if neighbourA contains the common candidate and isn't in the chain...
                                            {
                                                for (int k = l + 1; k < chain.Count; k++)//iterate through the cells in the chain, start one cell ahead in the chain list (cellB)
                                                {
                                                    if (indicatorForCell[l] != indicatorForCell[k])//if the indicator (colour) of cellA is different to that of cellB...
                                                    {
                                                        for (int index2 = 0; index2 < 3; index2++)//iterates through groups (row/column/block) to look at
                                                        {
                                                            foreach (Cell neighbour2 in chain[k].NeighbourCells[index2])//iterate through the cells in the row/column/block associated with 'cellB', (neighbourB)
                                                            {
                                                                if (neighbour == neighbour2 && neighbour2.Candidates.Contains(candidate))//if two cells in the chain with different colours can see a cell outside of the chain with the common candidate...
                                                                {
                                                                    neighbour.Candidates.Remove(candidate);//...the candidate can be removed from that cell
                                                                    changeMade = true;
                                                                    g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + neighbour.XLocation + "," + neighbour.YLocation + "] - SINGLE CHAINS / SIMPLE COLOURING - RULE 1");
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
                                    g_SolvePath.Add("Chain: [" + chain[0].XLocation + ", " + chain[0].YLocation + "]");
                                    for (int z = 1; z < chain.Count; z++)
                                    {
                                        g_SolvePath[g_SolvePath.Count-1] += " - ["+chain[z].XLocation+","+ chain[z].YLocation + "]";
                                    }
                                    g_Rating += chain.Count * 100;
                                    return true;
                                }
                                else
                                {
                                    bool? contradiction = null;
                                    for (int c1 = 0; c1 < chain.Count && contradiction == null; c1++)//iterate through all cells in chain (cellA), also whilst a contradiction hasn't been found
                                    {
                                        for (int c2 = c1 + 1; c2 < chain.Count && contradiction == null; c2++)//start iterating one cell ahead of 'cellA' (cellB), also whilst a contradiction hasn't been found
                                        {
                                            if (indicatorForCell[c1] == indicatorForCell[c2])//if cellA and cellB are coloured the same...
                                            {
                                                if (chain[c1].XLocation == chain[c2].XLocation || chain[c1].YLocation == chain[c2].YLocation || chain[c1].BlockLoc == chain[c2].BlockLoc)//if c2 shares a group (row/column/block) with c1, contradiction occurs.
                                                {
                                                    contradiction = indicatorForCell[c1];
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (contradiction != null)//if a contradiction has occured, 'contradiction' will not be null
                                    {
                                        for (int d = 0; d < chain.Count; d++)//loop over cells in chain...
                                        {
                                            if (indicatorForCell[d] == contradiction)//if the cell is coloured the same as the indicated contradiction, remove the candidate from the cell
                                            {
                                                chain[d].Candidates.Remove(candidate);
                                                changeMade = true;
                                                g_SolvePath.Add("Candidate number " + candidate + " removed from cell [" + chain[d].XLocation + "," + chain[d].YLocation + "] - SINGLE CHAINS / SIMPLE COLOURING - RULE 2");
                                            }
                                        }
                                    }
                                }
                                if (changeMade)
                                {
                                    g_Rating += chain.Count * 100;
                                    return true;
                                }
                            }
                            
                        }
                    }
                }
            }
            return changeMade;
        }
        /// <summary>
        /// Code breakdown:
        /// -if the current iteration doesn't exists, end of chain has been met. (iteration is increased within the ChainRecursion method call)
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
        private bool ChainRecursion(SudokuGrid grid, char candidate, int c, List<Cell> chain, List<bool> indicatorForCell)
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
            if (ChainRecursion(grid, candidate, ++c, chain, indicatorForCell))
                return true;
            return false;
        }
        
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
                    if (grid.Rows[i][j].Candidates.Count == 2 && grid.Rows[i][j].Num == '0')
                    {
                        Cell cellA = grid.Rows[i][j];
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
                                        Cell cellD = grid.Rows[xAxis][yAxis];
                                        if (cellD.Candidates.Contains(cellC.Candidates[0])
                                            && cellD.Candidates.Contains(cellC.Candidates[1]))//Searches for if a Cell D exists
                                        {
                                            cellD.Candidates.Remove(cellC.Candidates[0]);
                                            cellD.Candidates.Remove(cellC.Candidates[1]);
                                            changeMade = true;
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
                if (grid.Rows[row][col].Candidates.Contains(grid.Rows[row][col].NeighbourCells[0][n].Num))
                {
                    grid.Rows[row][col].Candidates.Remove(grid.Rows[row][col].NeighbourCells[0][n].Num);
                }
                if (grid.Rows[row][col].Candidates.Contains(grid.Rows[row][col].NeighbourCells[1][n].Num))
                {
                    grid.Rows[row][col].Candidates.Remove(grid.Rows[row][col].NeighbourCells[1][n].Num);
                }
                if (grid.Rows[row][col].Candidates.Contains(grid.Rows[row][col].NeighbourCells[2][n].Num))
                {
                    grid.Rows[row][col].Candidates.Remove(grid.Rows[row][col].NeighbourCells[2][n].Num);
                }
                if (grid.Rows[row][col].Candidates.Count == 0)
                {
                    return false;
                }
            }

            if (grid.Rows[row][col].Candidates.Count == 1)
            {
                grid.Rows[row][col].Num = grid.Rows[row][col].Candidates[0];
                if (variator == 3)
                    g_BacktrackingPath.Add(row.ToString() + col.ToString() + grid.Rows[row][col].Candidates[0].ToString());//Add to solve path
            }
            return true;
        }
        /// <summary>
        /// This Backtracking solver uses heavy recursion to reach a solution, iterating through cells attempting to place each possible number in each cell till the valid solution is found.
        /// It initially starts from the top left cell and, with each recursive instance, looks at the next cell over in the current row.
        /// Once the last cell in the row is reached, the column counter is incremented and the row counter is set back to 0. 
        /// For example, if [i,j] is a cell, when looking at cell [0,8], the next cell to be looked at is [1,0].
        /// </summary>
        /// <param name="grid">Sudoku grid that is passed into and mutated in the method</param>
        /// <param name="row">Current row number being examined in this instance of the method</param>
        /// <param name="col">Current column number being examined in this instance of the method</param>
        /// <param name="variator">Variator changes how the solver functions. 
        /// '0' = normal ordered candidate list, '1' = reversed candidate list, '2' = shuffled candidate list, '3' = no use of naked single strategy
        /// </param>
        /// <returns>Returns true if solver completes puzzle with all values in the correct place. 
        /// Returns false if solver finds contradiction within a cell, i.e. no candidate numbers in a cell</returns>
        public bool BacktrackingSolver(SudokuGrid grid, int row, int col, byte variator)
        {
            if (col == 9 && row == 9)//If somehow the method tries to look at this non-existent cell, this catches the exception
            {
                if (g_Gen.CheckIfSolved(grid))
                {
                    return true;
                }
                else
                {
                    grid.Rows[--row][--col].Num = '0';
                    if (variator == 3)
                        g_BacktrackingPath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
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
                            if (g_Gen.CheckIfSolved(grid))
                            {
                                return true;
                            }
                            else
                            {
                                grid.Rows[--row][--col].Num = '0';
                                if (variator == 3)
                                    g_BacktrackingPath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
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
            if (variator == 0 || variator == 3)
                grid.Rows[row][col].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            else if (variator == 1)
                grid.Rows[row][col].Candidates = new List<char> { '9', '8', '7', '6', '5', '4', '3', '2', '1' };
            else if (variator == 2)
                grid.Rows[row][col].Candidates = g_Gen.Shuffler(new List<char> { '9', '8', '7', '6', '5', '4', '3', '2', '1' });
            //Reversed list is for checking for multiple solutions or shuffle if given the generating

            if (!RemoveCands(grid, row, col, variator))//if it returns false, candidates count must be 0 so a contradiction is found
            {
                grid.Rows[row][col].Num = '0';
                if (variator == 3)
                    g_BacktrackingPath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
                return false;
            }


            int nextRow = row, nextCol = col;
            if (++nextCol == 9)//increments the nextCol value which is used in conjunction with nextRow to look at the next cell in the sequence. If it is 9, it must be reset to 0
            {
                if (++nextRow == 9)//Currently looking at cell 81 in grid
                {
                    grid.Rows[row][col].Num = grid.Rows[row][col].Candidates[0];//Sets the last cell to be the only value possible, then the solution is checked.
                    if (variator == 3)
                        g_BacktrackingPath.Add(row.ToString() + col.ToString() + grid.Rows[row][col].Candidates[0].ToString());//Add to solve path
                    if (g_Gen.CheckIfSolved(grid))
                    {
                        return true;
                    }
                    else
                    {
                        grid.Rows[row][col].Num = '0';//cell value must be set to 0 to backtrack
                        if (variator == 3)
                            g_BacktrackingPath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
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
                    if (variator == 3)
                        g_BacktrackingPath.Add(row.ToString() + col.ToString() + candidate.ToString());//Add to solve path
                    //A new instance of BacktrackingSolver is called using the grid with an updated value of a cell and is provided with the next cell coordinates
                    if (BacktrackingSolver(grid, nextRow, nextCol, variator))
                        return true;
                }
            }
            else
            {
                //If the current cell contains a number found from a naked single strategy,
                //then it is dismissed and a new instance of BacktrackingSolver is called and is provided with the next cell coordinates
                if (BacktrackingSolver(grid, nextRow, nextCol, variator))
                    return true;
                else
                {//if it returns false, then the number for the cell is set back to 0, and then cycles backwards through the recursions by returning false.
                    grid.Rows[row][col].Num = '0';
                    if (variator == 3)
                        g_BacktrackingPath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
                    return false;
                }
            }
            grid.Rows[row][col].Num = '0';//cell value must be set to 0 to backtrack
            if (variator == 3)
                g_BacktrackingPath.Add(row.ToString() + col.ToString() + "0");//Add to solve path
            return false;//gets hit if each attempt with each 'candidate' returns false in the foreach
        }
#endregion


        //Non-functional/Incomplete - IGNORE REGION "SOLVER FOR SOLVING CELL BY CELL BUTTON"
#region Solver for Solving Cell by Cell button
        public bool SolveNextStep(SudokuGrid grid)
        {
            if (g_Gen.CheckIfSolved(grid))
            {
                return true;
            }
            g_changeMade = false; g_moderate = false; g_advanced = false;
            string separator = "|>---------------------------------------------------------------------------------------------------------------------->|";

            if (CleanCandidateLists(grid))
            {
                g_SolvePath.Add(separator);
                g_strategy = "Clean Candidates";
            }
            else if (FindNakedSingles(grid))//Naked singles contributes +10 to the rating for each naked single found
            {
                g_SolvePath.Add(separator);
                g_strategy = "Naked Singles";
            }
            else if (FindHiddenSingles(grid))//Naked singles contributes +15 to the rating for each hidden single found
            {
                g_SolvePath.Add(separator);
                g_strategy = "Hidden Singles";
            }
            else if (FindNakedPair(grid))//A Naked Pair contributes +100 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 100;
                g_moderate = true;
                g_strategy = "Naked Pair";
            }
            else if (FindHiddenPair(grid))//A Hidden Pair contributes +150 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 150;
                g_moderate = true;
                g_strategy = "Hidden Pair";
            }
            else if (FindPointingNumbers(grid))//A Point Pair/Triple contributes +250 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 250;
                g_moderate = true;
                g_strategy = "Pointing Pair/Triple";
            }
            else if (FindBlockLineReduce(grid))//A Box-Line Reduction contributes +300 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 300;
                g_moderate = true;
                g_strategy = "Block-Line Reduction";
            }
            else if (FindNakedTriple(grid))//A Naked Triple contributes +400 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 400;
                g_moderate = true;
                g_strategy = "Naked Triple";
            }
            else if (FindHiddenTriple(grid))//A Hidden Triple contributes +450 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 450;
                g_moderate = true;
                g_strategy = "Hidden Triple";
            }
            else if (FindXWing(grid))//An X-Wing contributes +550 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 550;
                g_advanced = true;
                g_strategy = "X-Wing";
            }
            else if (FindYWing(grid))//A Y-Wing contributes +600 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 600;
                g_advanced = true;
                g_strategy = "Y-Wing";
            }
            else if (FindXYZWing(grid))//An XYZ-Wing contributes +650 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 650;
                g_advanced = true;
                g_strategy = "XYZ-Wing";
            }
            else if (FindSingleChains(grid))//A successful Single Chain contributes +700 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 700;
                g_advanced = true;
                g_strategy = "Single Chains/Simple Colouring";
            }
            else if (FindUniqueRectangleType1(grid))//A Unique Rectangle contributes +750 to the rating.
            {
                g_SolvePath.Add(separator);
                g_Rating += 750;
                g_advanced = true;
                g_strategy = "Unique Rectangle Type 1";
            }
            //More methods to add
            else
            {
                BacktrackingSolver(grid, 0, 0, 0);
                g_BacktrackingReq = true;
                g_Difficulty = "Extreme";
                g_SolvePath.Add("BACKTRACKING/TRIAL-AND-ERROR USED TO FINISH PUZZLE - UNABLE TO FINISH WITH IMPLEMENTED STRATEGIES");
                g_SolvePath.Add(separator);
                g_strategy = "Trial-and-error/Brute-force";
            }
            if (g_Gen.CheckIfSolved(grid))
            {
                return true;
            }
            if (g_moderate)
                g_Difficulty = "Moderate";
            if (g_advanced)
                g_Difficulty = "Advanced";
            return false;
        }
#endregion
    }
}
