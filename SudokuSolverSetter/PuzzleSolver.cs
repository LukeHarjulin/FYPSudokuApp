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
        public int difficulty = 0;
        #region Full Solver method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="method">method '1' is human-strategy solver. '2' is bruteforce solver. '3' is bruteforce solver using char[][]</param>
        /// <returns>returns true if the puzzle is solved, false if not</returns>
        public bool Solver(SudokuGrid grid, char method)//Once called, the solver will attempt to entirely solve the puzzle, making decisions based off the the scenarios provided.
        {
            difficulty = 0;
            bool changeMade = false;
            /*
             *  This do...while is necessary for repeating these methods for solving until no changes are made (which it assumes that the puzzle is complete or it could not complete it)
             *  The if and elses are to make the process faster of solving faster, 
                as it ensures that it tries the easiest less computationally heavy methods first before the more complex methods.
            */
            if (method == '1')
            {
                
                do
                {
                    if (FindNakedSingles(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindHiddenSingles(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindNakedPair(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindHiddenPair(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindNakedTriple(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindHiddenTriple(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindPointingNumbers(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindBlockLineReduce(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindXWing(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindYWing(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindSingleChains(grid))
                    {
                        changeMade = true;
                    }
                    else if (FindUniqueRectangleType1(grid))
                    {
                        changeMade = true;
                    }
                    //More methods to add
                    else
                    {
                        changeMade = false;
                    }
                } while (changeMade);
            }
            else if (method == '2')
            {
                BruteForceSolve(grid, 0, 0, 0);
            }
            
            PuzzleGenerator gen = new PuzzleGenerator();
            if (!gen.CheckIfSolved(grid))
            {
                BruteForceSolve(grid, 0, 0, 0);
                difficulty += 400;
            }
            return gen.CheckIfSolved(grid);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        private bool FindNakedSingles(SudokuGrid grid)
        {
            bool changeMade = false, escapeLoop = true;
            do
            {
                escapeLoop = true;
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
                                    changeMade = true;
                                    escapeLoop = false;
                                }
                                if (grid.Rows[i][j].Candidates.Contains(grid.Rows[i][j].NeighbourCells[1][n].Num))
                                {
                                    grid.Rows[i][j].Candidates.Remove(grid.Rows[i][j].NeighbourCells[1][n].Num);
                                    changeMade = true;
                                    escapeLoop = false;
                                }
                                if (grid.Rows[i][j].Candidates.Contains(grid.Rows[i][j].NeighbourCells[2][n].Num))
                                {
                                    grid.Rows[i][j].Candidates.Remove(grid.Rows[i][j].NeighbourCells[2][n].Num);
                                    changeMade = true;
                                    escapeLoop = false;
                                }
                            }
                            if (grid.Rows[i][j].Candidates.Count == 1)
                            {
                                grid.Rows[i][j].Num = grid.Rows[i][j].Candidates[0];
                                grid.Rows[i][j].Candidates.Clear();
                                changeMade = true;
                                escapeLoop = false;
                                difficulty++;
                            }

                        }
                    }
                }
            } while (!escapeLoop);
            
            return changeMade;
        }
        /// <summary>
        /// 
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
                                    difficulty += 2;
                                    break;
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
                                        if (cell != neighbour && cell.Candidates.Contains(neighbour.Candidates[0]))
                                        {
                                            cell.Candidates.Remove(neighbour.Candidates[0]);
                                            changeMade = true;
                                            difficulty += 3;
                                        }
                                        if (cell != neighbour && cell.Candidates.Contains(neighbour.Candidates[1]))
                                        {
                                            cell.Candidates.Remove(neighbour.Candidates[1]);
                                            changeMade = true;
                                            difficulty += 3;
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
                            List<Cell> cellList = new List<Cell>(9);
                            List<char> candis = new List<char>(9);
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
                                for (int c1 = 0; c1 < cellList.Count; c1++)
                                {
                                    for (int c2 = c1 + 1; c2 < cellList.Count; c2++)
                                    {
                                        if (cellList[c1] == cellList[c2])
                                        {
                                            grid.Rows[i][j].Candidates.RemoveAll(candidate => candidate != candis[c1] && candidate != candis[c2]);//Removes all candidates but then common candidates
                                            cellList[c2].Candidates.RemoveAll(candidate => candidate != candis[c1] && candidate != candis[c2]);
                                            changeMade = true;
                                            difficulty += 4;
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
        /// Find Naked Triple:
        /// -Iterate over cells till an empty cell is found containing two/three candidates, call that cell 'CellA'
        /// -Iterate over each cell in each group associated with CellA
        /// -
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

                }
            }

            return changeMade;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true of a change to a cell candidate list or value is made</returns>
        private bool FindHiddenTriple(SudokuGrid grid)
        {
            bool changeMade = false;


            return changeMade;
        }
        /// <summary>
        /// Find Hidden Triple:
        /// -Iterate over cells till an empty cell is found, call that cell 'CellA'
        /// -Iterate over each candidate from CellA's candidate list
        /// -Iterate over each cell in the block associated with CellA, counting how many empty cells within the block have 'candidate' in their candidate list.
        /// -If the counter is equal to one (or two), check if CellA shares a row or column with the cell that contains the common candidate (or both cells).
        /// -If they share a row/column, you can remove the candidate from any cells in that row/column.
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
                                        difficulty += 7;
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
                                            difficulty += 7;
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
        /// Find Block Line Reduce:
        /// -Iterate over cells till an empty cell is found, call that cell 'CellA'
        /// -For each candidate in CellA's candidate list, check the row/column and identify all cells in that row/column that contain candidate.
        /// -If none of the cells found in that row/column are are outside of the block, we can remove all 'candidate' from every other cell in the block.
        /// i.e.
        ///     1  2  3      4    5    6      7   8   9
        /// ------------------------------------------
        /// A |45  1 6   | 245  2459  7    | 8    49 3
        /// B |453 9 235 | 8    23456 3456 | 1247 47 1457
        /// C |8   7 235 | 2345 23459 1    | 24   6  459
        /// ------------------------------------------
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
                                        difficulty += 8;
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
                                        difficulty += 8;
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
                                                                difficulty += 15;
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
                                                                    difficulty += 10;
                                                                }
                                                                if (cellB.NeighbourCells[index][n].Candidates.Contains(thirdCandi) && cellB.NeighbourCells[index][n].BlockLoc == cellC.BlockLoc)
                                                                {
                                                                    cellB.NeighbourCells[index][n].Candidates.Remove(thirdCandi);
                                                                    changeMade = true;
                                                                    difficulty += 10;
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
        /// Rule for Type1 Unique Rectangle:
        /// -There must four pairs in a rectangle that spans two blocks.
        /// -All of the pairs must be naked, apart from one.
        /// -The cell that contains more than just the pair is able to get rid of the pair from the candidate number list.
        /// -Reason: A sudoku puzzle cannot contain four conjugate pairs as there can only be one solution to the puzzle.
        /// -If numbers are interchangeable, that means there is more than one solution
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
        public bool FindUniqueRectangleType1(SudokuGrid grid)
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
                                            difficulty += 20;
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
        /// The X-Wing strategy focuses on finding a single common candidate in four different cells that do not share a block between any of the cells.
        /// Each of the four cells must share a row with one of the cells and a column with a different cell.
        /// .......
        /// </summary>
        /// <param name="grid">Passing through the SudokuGrid to examine for Xwings</param>
        /// <returns>returns true if a change to a cell candidate list or value is made</returns>
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
                            if (axis == 1 && grid.Rows[y][x].Num == '0' && grid.Rows[y][x].Candidates.Contains(i))//switch for finding x-wings in the opposite axis
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
                            else if (grid.Rows[x][y].Num == '0' && grid.Rows[x][y].Candidates.Contains(i))
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
                                                difficulty += 20;
                                            }
                                            if (cL[1].NeighbourCells[axis][n].Candidates.Contains(i) && cL[3] != cL[1].NeighbourCells[axis][n])//checks column of second cell in first row of xwing and check whether 
                                            {
                                                cL[1].NeighbourCells[axis][n].Candidates.Remove(i);
                                                changeMade = true;
                                                difficulty += 20;
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
        /// <returns>returns true of a group is shared</returns>
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

        /// <summary>
        /// Find Single Chains(aka Simple Colouring):
        /// A chain is a collection of cells that are linked together because of common a candidate(s).
        /// Two cells can only have a link if the common candidate doesn't exist within any other cell in the group (row/column/block).
        /// This way, if one of the two cells' value becomes the shared candidate number, the other cell cannot possibly be that number.
        /// If either of those cells are linked with another cell through the same candidate number, 
        /// it would cause a CHAIN reaction of preventing and forcing certain cells to be the candidate number.
        /// Within the code below, if two cells are linked, one cell is labelled as true and the other as false.
        /// This method replicates the colouring technique, true representing one colour, false representing another.
        /// 
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
                            List<Cell> cellsContainingCandi = new List<Cell>(8) { grid.Rows[i][j] };
                            List<bool> indicatorForCell= new List<bool>(8) { true };
                            int c = 0;//Iterator for cellsContainingCandi for recursion
                            
                            if (ChainRecursion(grid, candidate, c, cellsContainingCandi, indicatorForCell))
                            {
                                for (int l = 0; l < cellsContainingCandi.Count; l++)
                                {
                                    for (int index = 0; index < 3; index++)
                                    {
                                        foreach (Cell neighbour in cellsContainingCandi[l].NeighbourCells[index])
                                        {
                                            if (neighbour.Candidates.Contains(candidate) && !cellsContainingCandi.Contains(neighbour))
                                            {
                                                for (int k = l + 1; k < cellsContainingCandi.Count; k++)
                                                {
                                                    if (indicatorForCell[l] != indicatorForCell[k])
                                                    {
                                                        for (int index2 = 0; index2 < 3; index2++)
                                                        {
                                                            foreach (Cell neighbour2 in cellsContainingCandi[k].NeighbourCells[index2])
                                                            {
                                                                if (neighbour == neighbour2 && neighbour2.Candidates.Contains(candidate))
                                                                {
                                                                    neighbour.Candidates.Remove(candidate);
                                                                    changeMade = true;
                                                                    difficulty += 20;
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
        /// Code breakdown:
        /// -if the current iteration doesn't exists, end of chain has been met. (iteration is increased within the ChainRecursion method call)
        /// -for each group, search through each cell in the group - call the cell 'Neighbour'
        /// -if neighbour contains the candidate and the cell value ies not [1-9],
        /// -increment a counter 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="candidate">current candidate being examined in chain</param>
        /// <param name="c">current iteration of cellsContainingCandi</param>
        /// <param name="cellsContainingCandi">cells that have been discovered in the chain</param>
        /// <param name="indicatorForCell">the true/false indicator for each cell discovered</param>
        /// <returns>returns true once a chain is made and cannot go any further, returns false if no chain is made</returns>
        private bool ChainRecursion(SudokuGrid grid, char candidate, int c, List<Cell> cellsContainingCandi, List<bool> indicatorForCell)
        {
            if (c == cellsContainingCandi.Count)
            {
                return true;
            }
            for (int index = 0; index < 3; index++)
            {
                int counter = 0;
                Cell saveCell = new Cell();
                foreach (Cell neighbour in cellsContainingCandi[c].NeighbourCells[index])
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
                    if (!cellsContainingCandi.Contains(saveCell))
                    {
                        cellsContainingCandi.Add(saveCell);
                        indicatorForCell.Add(!indicatorForCell[c]);
                    }
                }
            }
            if (ChainRecursion(grid, candidate, ++c, cellsContainingCandi, indicatorForCell))
                return true;
            return false;
        }

        //Non-functional
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
        
    }
}
