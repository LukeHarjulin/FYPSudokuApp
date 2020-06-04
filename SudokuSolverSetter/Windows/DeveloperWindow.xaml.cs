using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Media;

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for DeveloperWindow.xaml
    /// </summary>
    public partial class DeveloperWindow : Window
    {
        private string g_PuzzleString = "";
        private List<TextBox> g_txtBxList = new List<TextBox>();
        private MainWindow g_homePage = new MainWindow();
        private PuzzleGenerator g_gen = new PuzzleGenerator();
        private PuzzleSolverObjDS g_solve = new PuzzleSolverObjDS();
        private string g_currentTime = "";
        private DispatcherTimer G_DT { get; set; }
        private Stopwatch G_Timer { get; set; }
        private List<string> g_BacktrackingSolvePath = new List<string>();
        private int g_PathCounter = 0;
        private List<int> g_ratingList = new List<int>();
        private List<string> g_puzzleStrList = new List<string>();
        public DeveloperWindow()
        {
            InitializeComponent();

            Number_List_combo.Items.Add(1);
            Number_List_combo.Items.Add(10);
            Number_List_combo.Items.Add(25);
            Number_List_combo.Items.Add(50);
            Number_List_combo.Items.Add(100);
            Number_List_combo.Items.Add(500);
            Number_List_combo.Items.Add(1000);
            //Create list of all the cells so that they can be transformed
            g_txtBxList = new List<TextBox> {  x1y1g1, x1y2g1, x1y3g1, x1y4g2, x1y5g2, x1y6g2, x1y7g3, x1y8g3, x1y9g3,
                                             x2y1g1, x2y2g1, x2y3g1, x2y4g2, x2y5g2, x2y6g2, x2y7g3, x2y8g3, x2y9g3,
                                             x3y1g1, x3y2g1, x3y3g1, x3y4g2, x3y5g2, x3y6g2, x3y7g3, x3y8g3, x3y9g3,
                                             x4y1g4, x4y2g4, x4y3g4, x4y4g5, x4y5g5, x4y6g5, x4y7g6, x4y8g6, x4y9g6,
                                             x5y1g4, x5y2g4, x5y3g4, x5y4g5, x5y5g5, x5y6g5, x5y7g6, x5y8g6, x5y9g6,
                                             x6y1g4, x6y2g4, x6y3g4, x6y4g5, x6y5g5, x6y6g5, x6y7g6, x6y8g6, x6y9g6,
                                             x7y1g7, x7y2g7, x7y3g7, x7y4g8, x7y5g8, x7y6g8, x7y7g9, x7y8g9, x7y9g9,
                                             x8y1g7, x8y2g7, x8y3g7, x8y4g8, x8y5g8, x8y6g8, x8y7g9, x8y8g9, x8y9g9,
                                             x9y1g7, x9y2g7, x9y3g7, x9y4g8, x9y5g8, x9y6g8, x9y7g9, x9y8g9, x9y9g9
            };
            Symmetry_checkbox.IsChecked = true;
        }
        /// <summary>
        /// Updates the combo box with all the puzzles from the XML file, displayed and ordered by their rating
        /// </summary>
        public void AddPuzzlesToCombo(bool symmetry)
        {
            if (PuzzlesByRating_combo.Items.Count > 0)
            {
                PuzzlesByRating_combo.Items.Clear();
                g_ratingList.Clear();
                g_puzzleStrList.Clear();
            }
            string symmetric = "";
            if (symmetry)
            {
                symmetric = @"Symmetric";
            }
            else
            {
                symmetric = @"NonSymmetric";
            }
            string fileName = symmetric+"/SudokuPuzzles.xml";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(fileName);
                XmlNode sudokuPuzzles = doc.DocumentElement.SelectSingleNode("/SudokuPuzzles");
                XmlNodeList puzzleLabels = sudokuPuzzles.ChildNodes;
                foreach (XmlNode label in puzzleLabels)
                {
                    XmlNode difficulty = label.SelectSingleNode(((ComboBoxItem)PuzzleDifficulty_combo.SelectedItem).Content.ToString());
                    foreach (XmlNode puzzle in difficulty)
                    {
                        int rating = int.Parse(puzzle["DifficultyRating"].InnerText), i = 0;
                        bool added = false;
                        if (g_ratingList.Count > 0)
                        {
                            for (i = 0; i < g_ratingList.Count; i++)
                            {
                                if (rating < g_ratingList[i])
                                {
                                    g_ratingList.Insert(i, rating);
                                    added = true;
                                    break;
                                }
                            }
                            if (!added)
                            {
                                g_ratingList.Add(rating);
                            }
                        }
                        else
                            g_ratingList.Add(rating);
                        if (label.Name == "Started")
                        {
                            g_puzzleStrList.Insert(i, puzzle["OriginalSudokuString"].InnerText);
                        }
                        else if (label.Name == "Completed")
                        {
                            string tempPuzzleStr = puzzle["SudokuString"].InnerText;
                            tempPuzzleStr = tempPuzzleStr.Remove(0, 1);
                            g_puzzleStrList.Insert(i, tempPuzzleStr);
                        }
                        else
                        {
                            g_puzzleStrList.Insert(i, puzzle["SudokuString"].InnerText);
                        }
                    }
                    
                }
                for (int i = 0; i < g_ratingList.Count; i++)
                {
                    PuzzlesByRating_combo.Items.Add(g_ratingList[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong with loading puzzles... \r\n\r\nError Message:" + ex, "Error");
            }
        }
        public void PopulateGridString(string grid)
        {
            int givensCounter = 0;
            Regex rgx = new Regex(@"[1-9]");
            for (int i = 0; i < 81; i++)
            {
                if (!rgx.IsMatch(grid[i].ToString()))
                {
                    g_txtBxList[i].FontSize = 12;
                    g_txtBxList[i].Text = "1 2 3 4 5 6 7 8 9";
                }
                else
                {
                    g_txtBxList[i].FontSize = 36;
                    g_txtBxList[i].Text = grid[i].ToString();
                    
                    givensCounter++;
                }
                g_txtBxList[i].Background = Brushes.White;
            }
            givenNums_lbl.Content = "Given Numbers: " + givensCounter;
            g_solve = new PuzzleSolverObjDS();
        }
        /// <summary>
        /// This method populates the Uniform grid and it's textboxes with all the given values from 'grid'.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="m_txtBxList"></param>
        /// <returns></returns>
        public List<TextBox> PopulateGrid(SudokuGrid grid, List<TextBox> m_txtBxList)
        {
            int x = 0;//row number
            int y = 0;//column number
            for (int i = 0; i < m_txtBxList.Count; i++)
            {
                if (grid.Rows[x][y].Num == '0') //0's are placeholders for when there is no value, so any 0's are turned into textboxes containing the candidate values.
                {
                    m_txtBxList[i].FontSize = 12;
                    m_txtBxList[i].Text = "";
                    for (int c = 0; c < grid.Rows[x][y].Candidates.Count; c++)
                    {
                        m_txtBxList[i].Text += grid.Rows[x][y].Candidates[c].ToString() + " ";
                    }
                }
                else
                {
                    m_txtBxList[i].FontSize = 36;
                    m_txtBxList[i].Text = grid.Rows[x][y].Num.ToString();
                }
                y++;
                if (y == 9)//row needs to increment and column needs to reset to 0 once it reaches the end of the row
                {
                    y = 0;
                    x++;
                }
                g_txtBxList[i].Background = Brushes.White;
            }
            return m_txtBxList;
        }
        /// <summary>
        /// Tests the three solvers: Strategy solver, backtracking solver with object data structure, and backtracking solver with char[][] data structure
        /// </summary>
        /// <param name="puzzleSolver">passing through so that it isn't redeclared</param>
        /// <param name="grid">grid used to pass to the solvers</param>
        /// <param name="iterations">number of times chosen to solve the puzzle by each solver</param>
        /// <param name="puzzleString">the puzzle grid in string form</param>
        private void TimeTestSolvers(PuzzleSolverObjDS puzzleSolver, SudokuGrid grid, int iterations, string puzzleString)
        {
            int rating = 0;
            bool solved = false;
            string[] times = new string[4];
            double averageTime = 0;
            for (int m = 1; m < 3; m++)//Tests Object data structure solvers
            {
                averageTime = 0;
                for (int i = 0; i < iterations; i++)
                {
                    
                    int counter = 0;
                    for (int x = 0; x < 9; x++)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            if (puzzleString[counter] == '0')
                                grid.Rows[x][y].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                            grid.Rows[x][y].Num = puzzleString[counter];
                            counter++;
                        }
                    }
                    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
                    if (m == 1)
                    {
                        solved = puzzleSolver.Solver(grid, m);
                        if (i == 0)
                        {
                            rating = puzzleSolver.g_Rating;
                            difficulty_lbl.Content = "Difficulty: " + puzzleSolver.g_Difficulty;
                        }
                    }
                    else
                        puzzleSolver.Solver(grid, m);
                    watch.Stop();
                    averageTime += watch.ElapsedMilliseconds;
                }
                times[m - 1] = m + "," + (averageTime / iterations / 1000).ToString();
                
            }
            PuzzleSolverCharDS solver = new PuzzleSolverCharDS();
            char[][] puzzleCharArr = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
            averageTime = 0;
            for (int n = 0; n < iterations; n++)//Test char[][] backtracking solver
            {
                int counter2 = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        puzzleCharArr[i][j] = puzzleString[counter2];
                        counter2++;
                    }
                }
                var watch = System.Diagnostics.Stopwatch.StartNew();
                solver.Solvers(puzzleCharArr, '2');
                watch.Stop();
                averageTime += watch.ElapsedMilliseconds;
            }
            if (rating != 0)
            {
                times[2] = 4 + "," + (averageTime / iterations / 1000).ToString();
                string outputStr = "After " + iterations + " iteration(s), average times taken to solve\r\nMeasured Puzzle Difficulty Level(experimental): " + puzzleSolver.g_Difficulty + "\r\nMeasured Rating (experimental): " + rating + "\r\n";
                times[0] = times[0].Remove(0, 2);
                outputStr += solved ? "Strategy Solver using Objects: " + times[0] + "\r\n" : "Strategy Solver using Objects(trial-and-error required): " + times[0] + "\r\n";
                times[1] = times[1].Remove(0, 2);
                outputStr += "Backtracking Solver using Objects: " + times[1] + "\r\n";
                times[2] = times[2].Remove(0, 2);
                outputStr += "Backtracking Solver using char[][]: " + times[2] + "\r\n";

                PopulateGrid(grid, g_txtBxList);
                MessageBox.Show(outputStr);
            }
        }
        /// <summary>
        /// Timer starts when the display of the backtracking solver is requested
        /// </summary>
        private void StartTimer()
        {
            G_Timer = new Stopwatch();
            G_DT = new DispatcherTimer();
            G_DT.Tick += new EventHandler(DT_Tick);
            G_DT.Interval = new TimeSpan(0, 0, 0, 0, 1);
            G_Timer.Start();
            G_DT.Start();
            Backtracking_Solve_char.IsEnabled = false;
            Backtracking_Solve_Obj.IsEnabled = false;
            b_Solve.IsEnabled = false;
            TestAllThree.IsEnabled = false;
            Import_Puzzle.IsEnabled = false;
            Create_Store_Puzzles_btn.IsEnabled = false;
            b_Solve1by1.IsEnabled = false;
        }
        /// <summary>
        /// Stops the timer when the backtracking solver display is finished or a certain button is click
        /// </summary>
        private void StopTimer()
        {
            G_Timer.Stop();
            G_DT.Stop();
            Backtracking_Solve_char.IsEnabled = true;
            Backtracking_Solve_Obj.IsEnabled = true;
            b_Solve.IsEnabled = true;
            TestAllThree.IsEnabled = true;
            Import_Puzzle.IsEnabled = true;
            Create_Store_Puzzles_btn.IsEnabled = true;
            b_Solve1by1.IsEnabled = true;
            g_PathCounter = 0;
            g_BacktrackingSolvePath.Clear();
        }
        /// <summary>
        /// Used to display Backtracking solver algorithm.
        /// When a tick occurs and a number is placed/removed within/from the grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DT_Tick(object sender, EventArgs e)
        {
            if (G_Timer.IsRunning)
            {
                try
                {
                    if (g_PathCounter >= g_BacktrackingSolvePath.Count)
                    {
                        StopTimer();
                    }
                    else
                    {
                        //Add number to cell
                        int i1 = Convert.ToInt32(g_BacktrackingSolvePath[g_PathCounter][0]) + 1;
                        int j1 = Convert.ToInt32(g_BacktrackingSolvePath[g_PathCounter][1]) + 1;
                        for (int i = 0; i < g_txtBxList.Count; i++)
                        {
                            if (g_txtBxList[i].Name[1] == (char)i1 && g_txtBxList[i].Name[3] == (char)j1)
                            {
                                g_txtBxList[i].FontSize = 36;
                                if (g_BacktrackingSolvePath[g_PathCounter][2] == '0')
                                {
                                    g_txtBxList[i].Text = "";
                                    g_txtBxList[i].Background = Brushes.Yellow;
                                }
                                else
                                {
                                    g_txtBxList[i].Text = g_BacktrackingSolvePath[g_PathCounter][2].ToString();
                                    g_txtBxList[i].Background = Brushes.LightGreen;
                                }
                            }
                        }
                        g_PathCounter++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong with the timer. \r\nError: " + ex);
                    throw;
                }
                
            }
        }
        /// <summary>
        /// In response to any of the Puzzle Solver buttons being click, 
        /// this event handler is called and handles the event slightly differently depending on which button was pressed.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_Solve_Click(object sender, RoutedEventArgs e)//This button on the interface is used to solve the grid that it is presented
        {
            //Initialising objects
            PuzzleSolverObjDS puzzleSolver = new PuzzleSolverObjDS();
            PuzzleGenerator gen = new PuzzleGenerator();
            SudokuGrid grid = new SudokuGrid
            {
                Rows = new Cell[9][]
            };
            int cellNum = 0;

            //This transforms the text in the boxes to a useable grid object. Resource heavy - alternative method may be developed in improvements
            for (int r = 0; r < grid.Rows.Length; r++)
            {
                grid.Rows[r] = new Cell[9];
                for (int c = 0; c < grid.Rows[r].Length; c++)
                {
                    if (g_txtBxList[cellNum].Text.Length > 1)
                    {
                        g_txtBxList[cellNum].Text = "0";
                        grid.Rows[r][c] = new Cell()
                        {
                            Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' },
                            Num = '0',
                            BlockLoc = (r / 3) * 3 + (c / 3) + 1,
                            XLocation = r,
                            YLocation = c,
                            ReadOnly = false
                        };
                    }
                    else
                    {
                        grid.Rows[r][c] = new Cell()
                        {
                            Candidates = new List<char> { },
                            Num = g_txtBxList[cellNum].Text[0],
                            BlockLoc = (r / 3) * 3 + (c / 3) + 1,
                            XLocation = r,
                            YLocation = c,
                            ReadOnly = true
                        };
                    }

                    cellNum++;
                }
            }
            gen.AddNeighbours(grid);
            int method = 1;
            if ((Button)sender == Backtracking_Solve_Obj)
            {
                method = 2;
            }
            int iterations = int.Parse(Number_List_combo.SelectedItem.ToString());
            string puzzleString = gen.SudokuToString(grid);
            if ((Button)sender != TestAllThree)
            {
                int rating = 0;
                double averageTime = 0;
                bool solved = false;
                for (int i = 0; i < iterations; i++)
                {
                    if (i != 0)
                    {
                        int counter = 0;
                        for (int x = 0; x < 9; x++)
                        {
                            for (int y = 0; y < 9; y++)
                            {
                                if (puzzleString[counter] == '0')
                                    grid.Rows[x][y].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                                grid.Rows[x][y].Num = puzzleString[counter];
                                counter++;
                            }
                        }
                    }
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    solved = puzzleSolver.Solver(grid, method);
                    if (i == 0)
                    {
                        rating = puzzleSolver.g_Rating;
                        difficulty_lbl.Content = "Difficulty: " + puzzleSolver.g_Difficulty;
                    }
                    watch.Stop();
                    averageTime += watch.ElapsedMilliseconds;
                    
                }
                averageTime = averageTime / iterations;
                
                g_currentTime = iterations > 1 ? "Average time taken to solve: " + averageTime / 1000 : "Time taken to solve: " + averageTime / 1000;
                if (method == 1)
                {
                    if (rating != 0)
                    {
                        PopulateGrid(grid, g_txtBxList);
                        if (solved && !puzzleSolver.g_BacktrackingReq)
                        {
                            MessageBox.Show("SOLVED\r\n" + g_currentTime + "\r\nMeasured Puzzle Difficulty Level(experimental): " + puzzleSolver.g_Difficulty + "\r\nMeasured Puzzle Rating(experimental): " + rating);
                        }
                        else if (puzzleSolver.g_BacktrackingReq)
                        {
                            MessageBox.Show("FAILED\r\n" + g_currentTime + "\r\nFinished with trial and error, unable to finish using implemented strategies." + "\r\nMeasured Puzzle Difficulty Level(experimental): " + puzzleSolver.g_Difficulty + "\r\nMeasured Puzzle Rating(experimental): " + rating);
                        }
                        SolvePath path = new SolvePath();
                        path.PopulateTextBlock(rating, g_currentTime, puzzleSolver);
                    }
                }
                else if (method == 2)
                {
                    if (puzzleSolver.g_BacktrackingPath.Count != 0)
                    {
                        int estimateSimulationTime = puzzleSolver.g_BacktrackingPath.Count * 17 / 1000;
                        MessageBoxResult result = MessageBox.Show("SOLVED\r\n" + g_currentTime + "\r\n\r\nDo you want to see a simulation of the Backtracking algorithm solving the puzzle?\r\nTotal # of Digit Changes: " + puzzleSolver.g_BacktrackingPath.Count + "\r\nEstimated Duration of Simulation: " + estimateSimulationTime + " seconds\r\n(Warning: The simulation can take a very long time to finish, ~17ms per digit change)", "Backtracking Simulation Confirmation", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            g_BacktrackingSolvePath.AddRange(puzzleSolver.g_BacktrackingPath);
                            StartTimer();
                        }
                        else
                        {
                            PopulateGrid(grid, g_txtBxList);
                        }
                    }
                }
            }
            else
            {
                TimeTestSolvers(puzzleSolver, grid, iterations, puzzleString);
            }

        }
        /// <summary>
        /// Unfinished
        /// Used to step through the solver for development
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_Solve1by1_Click(object sender, RoutedEventArgs e)//This button on the interface is used to solve in increments/steps
        {
            try
            {
                //Initialising objects
                PuzzleGenerator gen = new PuzzleGenerator();
                List<TextBox> txtBxList = new List<TextBox>
                { x1y1g1, x1y2g1, x1y3g1, x1y4g2, x1y5g2, x1y6g2, x1y7g3, x1y8g3, x1y9g3,
                  x2y1g1, x2y2g1, x2y3g1, x2y4g2, x2y5g2, x2y6g2, x2y7g3, x2y8g3, x2y9g3,
                  x3y1g1, x3y2g1, x3y3g1, x3y4g2, x3y5g2, x3y6g2, x3y7g3, x3y8g3, x3y9g3,
                  x4y1g4, x4y2g4, x4y3g4, x4y4g5, x4y5g5, x4y6g5, x4y7g6, x4y8g6, x4y9g6,
                  x5y1g4, x5y2g4, x5y3g4, x5y4g5, x5y5g5, x5y6g5, x5y7g6, x5y8g6, x5y9g6,
                  x6y1g4, x6y2g4, x6y3g4, x6y4g5, x6y5g5, x6y6g5, x6y7g6, x6y8g6, x6y9g6,
                  x7y1g7, x7y2g7, x7y3g7, x7y4g8, x7y5g8, x7y6g8, x7y7g9, x7y8g9, x7y9g9,
                  x8y1g7, x8y2g7, x8y3g7, x8y4g8, x8y5g8, x8y6g8, x8y7g9, x8y8g9, x8y9g9,
                  x9y1g7, x9y2g7, x9y3g7, x9y4g8, x9y5g8, x9y6g8, x9y7g9, x9y8g9, x9y9g9
                };
                SudokuGrid grid = new SudokuGrid
                {
                    Rows = new Cell[9][]
                };
                int cellNum = 0;

                //This transforms the text in the boxes to a useable grid object. Resource heavy - alternative method may be developed in improvements
                for (int r = 0; r < grid.Rows.Length; r++)
                {
                    grid.Rows[r] = new Cell[9];
                    for (int c = 0; c < grid.Rows[r].Length; c++)
                    {
                        if (g_txtBxList[cellNum].Text.Length > 1)
                        {
                            List<char> candiList = new List<char>(g_txtBxList[cellNum].Text.Length);
                            for (int i = 0; i < g_txtBxList[cellNum].Text.Length; i++)
                            {
                                if (int.TryParse(g_txtBxList[cellNum].Text[i].ToString(), out int result))
                                {
                                    candiList.Add(g_txtBxList[cellNum].Text[i]);
                                }
                            }
                            g_txtBxList[cellNum].Text = "0";
                            grid.Rows[r][c] = new Cell()
                            {
                                Candidates = candiList,
                                Num = '0',
                                BlockLoc = (r / 3) * 3 + (c / 3) + 1,
                                XLocation = r,
                                YLocation = c,
                                ReadOnly = false
                            };
                        }
                        else
                        {
                            grid.Rows[r][c] = new Cell()
                            {
                                Candidates = new List<char> { },
                                Num = g_txtBxList[cellNum].Text[0],
                                BlockLoc = (r / 3) * 3 + (c / 3) + 1,
                                XLocation = r,
                                YLocation = c,
                                ReadOnly = true
                            };
                        }

                        cellNum++;
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        int nbCounter = 0;//nbCounter is neighbourcounter
                        grid.Rows[i][j].NeighbourCells = new List<List<Cell>>(3)
                    {
                        new List<Cell>(8),
                        new List<Cell>(8),
                        new List<Cell>(8)
                    };
                        for (int k = 0; k < 9; k++)
                        {
                            if (j != k)
                            {
                                grid.Rows[i][j].NeighbourCells[0].Add(grid.Rows[i][k]);//add neighbour in i
                                nbCounter++;
                            }
                        }
                        nbCounter = 0;
                        for (int l = 0; l < 9; l++)
                        {
                            if (l != i)
                            {
                                grid.Rows[i][j].NeighbourCells[1].Add(grid.Rows[l][j]);//add neighbour in column
                                nbCounter++;
                            }
                        }
                        nbCounter = 0;
                        int[] blockIndexes = gen.BlockIndexGetter(grid.Rows[i][j].BlockLoc);

                        for (int x = blockIndexes[0]; x < blockIndexes[0] + 3; x++)
                        {
                            for (int y = blockIndexes[1]; y < blockIndexes[1] + 3; y++)
                            {
                                if (grid.Rows[x][y] != grid.Rows[i][j])
                                {
                                    grid.Rows[i][j].NeighbourCells[2].Add(grid.Rows[x][y]);//add neighbour in block
                                    nbCounter++;
                                }
                            }
                        }
                    }
                }
                if (!gen.CheckIfSolved(grid))
                {
                    if (g_solve.SolveNextStep(grid))
                    {
                        if (g_solve.g_Rating != 0)
                        {
                            PopulateGrid(grid, g_txtBxList);
                            if (!g_solve.g_BacktrackingReq)
                            {
                                MessageBox.Show("SOLVED\r\n" + g_currentTime + "\r\nMeasured Puzzle Difficulty Level(experimental): " + g_solve.g_Difficulty + "\r\nMeasured Puzzle Rating(experimental): " + g_solve.g_Rating);
                            }
                            else if (g_solve.g_BacktrackingReq)
                            {
                                MessageBox.Show("FAILED\r\n" + g_currentTime + "\r\nFinished with trial and error, unable to finish using implemented strategies." + "\r\nMeasured Puzzle Difficulty Level(experimental): " + g_solve.g_Difficulty + "\r\nMeasured Puzzle Rating(experimental): " + g_solve.g_Rating);
                            }
                            SolvePath path = new SolvePath();
                            path.PopulateTextBlock(g_solve.g_Rating, g_currentTime, g_solve);
                            g_solve.g_Rating = 0;
                        }
                    }
                    PopulateGrid(grid, txtBxList);
                    strategy_lbl.Content = "Strategy/cleaning just used:\r\n" + g_solve.g_strategy;
                }
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }
        /// <summary>
        /// Button click event to cause the user to go back to the main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            if (G_Timer != null)
            {
                if (G_Timer.IsRunning)
                {
                    StopTimer();
                }
            }
            Close();
        }
        /// <summary>
        /// Closes the window properly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// Brings up a window to generate and store puzzles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Create_Store_Puzzles_btn_Click(object sender, RoutedEventArgs e)
        {
            bool symmetry = Symmetry_checkbox.IsChecked == true ? true : false;
            CreatePuzzles createPuzzles = new CreatePuzzles(1000, symmetry)
            {
                Owner = this
            };
            createPuzzles.ShowDialog();
            AddPuzzlesToCombo(symmetry);
        }
        /// <summary>
        /// Brings up a window to import a puzzle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Import_Click(object sender, RoutedEventArgs e)
        {
            ImportPuzzle importPuzzle = new ImportPuzzle
            {
                Owner = this
            };
            if (importPuzzle.ShowDialog() == true)
            {
                g_PuzzleString = importPuzzle.puzzleStr;
                PopulateGridString(importPuzzle.puzzleStr);
                difficulty_lbl.Content = "Difficulty: Unknown";
                g_solve = new PuzzleSolverObjDS();
                strategy_lbl.Content = "Strategy just used:\r\n<strategy>";
            }
            
        }
        /// <summary>
        /// Generates a puzzle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GeneratePuzzle_Click(object sender, RoutedEventArgs e)
        {
            if (G_Timer != null)
            {
                if (G_Timer.IsRunning)
                {
                    StopTimer();
                }
            }
            bool symmetry = Symmetry_checkbox.IsChecked == true ? true : false;
            SudokuGrid grid = g_gen.Setter(symmetry);//Calling the automated puzzle generator method to create a puzzle
            int givensCounter = 0;
            for (int i = 0; i < 9; i++)//fill in candidate values for each cell with a full candidate list
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Rows[i][j].Num != '0')
                    {
                        givensCounter++;
                    }
                    grid.Rows[i][j].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                }
            }
            givenNums_lbl.Content = "Given Numbers: " + givensCounter;
            difficulty_lbl.Content = "Difficulty: Unknown";
            strategy_lbl.Content = "Strategy just used:\r\n<strategy>";
            g_txtBxList = PopulateGrid(grid, g_txtBxList);
            g_PuzzleString = g_gen.SudokuToString(grid);
            Clipboard.SetText(g_PuzzleString);
            g_solve = new PuzzleSolverObjDS();
        }
        /// <summary>
        /// Backtracking Solve button click event using a different data structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BacktrackingSolve_char_Click(object sender, RoutedEventArgs e)
        {
            PuzzleSolverCharDS solver = new PuzzleSolverCharDS();
            char[][] puzzle = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
            int counter = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (g_txtBxList[counter].Text.Length > 1 || g_txtBxList[counter].Text == "0" || g_txtBxList[counter].Text.Length == 0)
                    {
                        puzzle[i][j] = '0';
                    }
                    else
                    {
                        puzzle[i][j] = g_txtBxList[counter].Text[0];
                    }
                    counter++;
                }
            }
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            char method = '1';
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            if ((Button)sender == Backtracking_Solve_char)
            {
                method = '2';
            }
            int iterations = int.Parse(Number_List_combo.SelectedItem.ToString());
            char[][] puzzleTemp = new char[9][];
            double averageTime = 0;
            bool solved = false;
            for (int i = 0; i < iterations; i++)
            {
                puzzleTemp = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        puzzleTemp[x][y] = puzzle[x][y];
                    }
                }
                var watch = Stopwatch.StartNew();
                solved = solver.Solvers(puzzleTemp, '2');
                watch.Stop();
                averageTime += watch.ElapsedMilliseconds;
            }
            averageTime = averageTime / iterations;
            g_currentTime = iterations > 1 ? "Average time taken to solve: " + averageTime / 1000 : "Time taken to solve: " + averageTime / 1000;
            counter = 0;
            if (solver.solvePath.Count != 0)
            {
                if (solved)
                {
                    int estimateSimulationTime = (solver.solvePath.Count * 17) / 1000;
                    MessageBoxResult result = MessageBox.Show("SOLVED\r\n" + g_currentTime + "\r\n\r\nDo you want to see a simulation of the Backtracking algorithm solving the puzzle?\r\nTotal # of Digit Changes: " + solver.solvePath.Count + "\r\nEstimated Duration of Simulation: " + estimateSimulationTime + " seconds\r\n(Warning: The simulation can take a very long time to finish, ~17ms per digit change)", "Backtracking Simulation Confirmation", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        g_BacktrackingSolvePath.AddRange(solver.solvePath);
                        StartTimer();
                    }
                    else
                    {
                        for (int x = 0; x < 9; x++)
                        {
                            for (int y = 0; y < 9; y++)
                            {
                                if (puzzleTemp[x][y] != '0')
                                {
                                    g_txtBxList[counter].FontSize = 36;
                                    g_txtBxList[counter].Text = puzzleTemp[x][y].ToString();
                                }
                                else//should never really happpen
                                {
                                    g_txtBxList[counter].Text = "0";
                                }
                                counter++;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("FAILED\r\n" + g_currentTime + "\r\nPuzzle may be invalid.");
                }
            }
        }
        /// <summary>
        /// Updates the puzzle grid with the selected puzzle from the combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RatingCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (G_Timer != null)
            {
                if (G_Timer.IsRunning)
                {
                    StopTimer();
                }
            }
            if (PuzzlesByRating_combo.SelectedIndex >= 0)
            {
                g_PuzzleString = g_puzzleStrList[PuzzlesByRating_combo.SelectedIndex];
                int givensCounter = 0;
                for (int i = 0; i < 81; i++)
                {
                    if (g_PuzzleString[i] == '0')
                    {
                        g_txtBxList[i].FontSize = 12;
                        g_txtBxList[i].Text = "1 2 3 4 5 6 7 8 9";
                    }
                    else
                    {
                        g_txtBxList[i].FontSize = 36;
                        g_txtBxList[i].Text = g_PuzzleString[i].ToString();
                        givensCounter++;
                    }
                    g_txtBxList[i].Background = Brushes.White;
                }
                givenNums_lbl.Content = "Given Numbers: " + givensCounter;
                difficulty_lbl.Content = "Difficulty: " + ((ComboBoxItem)PuzzleDifficulty_combo.SelectedItem).Content.ToString();
                g_solve = new PuzzleSolverObjDS();
                strategy_lbl.Content = "Strategy just used:\r\n<strategy>";
            }
        }
        private void Symmetry_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (PuzzleDifficulty_combo.SelectedIndex == 0)
            {
                AddPuzzlesToCombo(true);
                PuzzlesByRating_combo.SelectedIndex = 0;
            }
            else
                PuzzleDifficulty_combo.SelectedIndex = 0;
        }
        private void Symmetry_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PuzzleDifficulty_combo.SelectedIndex == 0)
            {
                AddPuzzlesToCombo(false);
                PuzzlesByRating_combo.SelectedIndex = 0;
            }
            else
                PuzzleDifficulty_combo.SelectedIndex = 0;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (G_Timer != null)
            {
                if (G_Timer.IsRunning)
                {
                    StopTimer();
                }
            }
        }
        private void DifficultyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Symmetry_checkbox.IsChecked == true)
            {
                AddPuzzlesToCombo(true);
            }
            else
            {
                AddPuzzlesToCombo(false);
            }
            PuzzlesByRating_combo.SelectedIndex = 0;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (G_Timer != null)
            {
                if (G_Timer.IsRunning)
                {
                    StopTimer();
                }
            }
            g_solve = new PuzzleSolverObjDS();
            PopulateGridString(g_PuzzleString);
            strategy_lbl.Content = "Strategy just used:\r\n<strategy>";
        }
    }
}