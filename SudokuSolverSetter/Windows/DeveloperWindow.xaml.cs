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
using System.IO;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for DeveloperWindow.xaml
    /// </summary>
    public partial class DeveloperWindow : Window
    {
        private string g_PuzzleString = "";
        private bool g_validPuzzle = true;
        private SudokuGrid g_grid = null;
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
            //Create all the cells so that they can be transformed
            for (int i = 0, row = 0, col = 0; i < 81; i++, col++)
            {
                
                TextBox txtbx = new TextBox();
                double left = 0.75, right = 0.75, top = 0.75, bottom = 0.75;
                if (row == 2 || row == 5 || row == 8)
                {
                    bottom = 2;
                }
                if (row == 0 || row == 3 || row == 6)
                {
                    top = 2;
                }
                if (col == 2 || col == 5 || col == 8)
                {
                    right = 2;
                }
                if (col == 0 || col == 3 || col == 6)
                {
                    left = 2;
                }
                txtbx.BorderThickness = new Thickness(left, top, right, bottom);
                txtbx.CaretBrush = Brushes.Transparent;
                txtbx.IsReadOnly = true;
                txtbx.FontWeight = FontWeights.SemiBold;
                txtbx.BorderBrush = Brushes.Black;
                txtbx.Cursor = Cursors.Arrow;
                txtbx.TextWrapping = TextWrapping.Wrap;
                txtbx.FontSize = 36; txtbx.HorizontalContentAlignment = HorizontalAlignment.Center; txtbx.VerticalContentAlignment = VerticalAlignment.Center;
                txtbx.PreviewTextInput += new TextCompositionEventHandler(Cell_PreviewTextInput);
                txtbx.TextChanged += new TextChangedEventHandler(Cell_TextChanged);
                DataObject.AddPastingHandler(txtbx, OnCancelCommand);
                SudokuPuzzle.Children.Add(txtbx); if (col == 8)
                {
                    col = -1;
                    row++;
                }
            }
            Symmetry_checkbox.IsChecked = true;
        }
        #region Functions/Methods
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
                ((TextBox)SudokuPuzzle.Children[i]).IsReadOnly = true;
                if (!rgx.IsMatch(grid[i].ToString()))
                {
                    ((TextBox)SudokuPuzzle.Children[i]).FontSize = 12;
                    ((TextBox)SudokuPuzzle.Children[i]).Text = "1 2 3 4 5 6 7 8 9";
                }
                else
                {
                    ((TextBox)SudokuPuzzle.Children[i]).FontSize = 36;
                    ((TextBox)SudokuPuzzle.Children[i]).Text = grid[i].ToString();
                    
                    givensCounter++;
                }
                
                ((TextBox)SudokuPuzzle.Children[i]).Background = Brushes.White;
            }
            givenNums_lbl.Content = "Given Numbers: " + givensCounter;
            g_solve = new PuzzleSolverObjDS();
        }
        /// <summary>
        /// This method populates the Uniform grid and it's textboxes with all the given values from 'grid'.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="SudokuPuzzle.Children">used by external classses, list needs to be passed in</param>
        /// <returns></returns>
        public UniformGrid PopulateGrid(SudokuGrid grid)
        {
            int x = 0;//row number
            int y = 0;//column number
            for (int i = 0; i < SudokuPuzzle.Children.Count; i++)
            {
                ((TextBox)SudokuPuzzle.Children[i]).IsReadOnly = true;
                if (grid.Rows[x][y].Num == '0') //0's are placeholders for when there is no value, so any 0's are turned into textboxes containing the candidate values.
                {
                    ((TextBox)SudokuPuzzle.Children[i]).FontSize = 12;
                    ((TextBox)SudokuPuzzle.Children[i]).Text = "";
                    for (int c = 0; c < grid.Rows[x][y].Candidates.Count; c++)
                    {
                        ((TextBox)SudokuPuzzle.Children[i]).Text += grid.Rows[x][y].Candidates[c] + " ";
                    }
                }
                else
                {
                    ((TextBox)SudokuPuzzle.Children[i]).FontSize = 36;
                    ((TextBox)SudokuPuzzle.Children[i]).Text = grid.Rows[x][y].Num.ToString();
                }
                
                y++;
                if (y == 9)//row needs to increment and column needs to reset to 0 once it reaches the end of the row
                {
                    y = 0;
                    x++;
                }
                ((TextBox)SudokuPuzzle.Children[i]).Background = Brushes.White;
            }
            return SudokuPuzzle;
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
            if (g_validPuzzle)
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

                    PopulateGrid(grid);
                    MessageBox.Show(outputStr);
                }
            }
            else
            {
                MessageBox.Show("Sudoku puzzle has more than one solution.\r\n\r\nAdd or change digits to create a valid Sudoku puzzle", "Error with puzzle");
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
                        int i1 = Convert.ToInt32(g_BacktrackingSolvePath[g_PathCounter][0].ToString());
                        int j1 = Convert.ToInt32(g_BacktrackingSolvePath[g_PathCounter][1].ToString());
                        int index = (i1 * 9) + j1;
                        ((TextBox)SudokuPuzzle.Children[index]).FontSize = 36;
                        if (g_BacktrackingSolvePath[g_PathCounter][2] == '0')
                        {
                            ((TextBox)SudokuPuzzle.Children[index]).Text = "";
                            ((TextBox)SudokuPuzzle.Children[index]).Background = Brushes.Yellow;
                        }
                        else
                        {
                            ((TextBox)SudokuPuzzle.Children[index]).Text = g_BacktrackingSolvePath[g_PathCounter][2].ToString();
                            ((TextBox)SudokuPuzzle.Children[index]).Background = Brushes.LightGreen;
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
        #endregion
        #region Events/Handlers
        /// <summary>
        /// In response to any of the Puzzle Solver buttons being click, 
        /// this event handler is called and handles the event slightly differently depending on which button was pressed.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_Solve_Click(object sender, RoutedEventArgs e)//This button on the interface is used to solve the grid that it is presented
        {
            if (g_validPuzzle)
            {
                //Initialising objects
                g_solve = new PuzzleSolverObjDS();
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
                        if (((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length > 1)
                        {
                            ((TextBox)SudokuPuzzle.Children[cellNum]).Text = "0";
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
                                Num = ((TextBox)SudokuPuzzle.Children[cellNum]).Text[0],
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
                        solved = g_solve.Solver(grid, method);
                        if (i == 0)
                        {
                            rating = g_solve.g_Rating;
                            difficulty_lbl.Content = "Difficulty: " + g_solve.g_Difficulty;
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
                            PopulateGrid(grid);
                            if (solved && !g_solve.g_BacktrackingReq)
                            {
                                MessageBox.Show("SOLVED\r\n" + g_currentTime + "\r\nMeasured Puzzle Difficulty Level(experimental): " + g_solve.g_Difficulty + "\r\nMeasured Puzzle Rating(experimental): " + rating);
                            }
                            else if (g_solve.g_BacktrackingReq)
                            {
                                MessageBox.Show("FAILED\r\n" + g_currentTime + "\r\nFinished with trial and error, unable to finish using implemented strategies." + "\r\nMeasured Puzzle Difficulty Level(experimental): " + g_solve.g_Difficulty + "\r\nMeasured Puzzle Rating(experimental): " + rating);
                            }
                            SolvePath path = new SolvePath();
                            path.PopulateTextBlock(rating, g_currentTime, g_solve);
                        }
                    }
                    else if (method == 2)
                    {
                        if (g_solve.g_BacktrackingPath.Count != 0)
                        {
                            int estimateSimulationTime = g_solve.g_BacktrackingPath.Count * 17 / 1000;
                            MessageBoxResult result = MessageBox.Show("SOLVED\r\n" + g_currentTime + "\r\n\r\nDo you want to see a simulation of the Backtracking algorithm solving the puzzle?\r\nTotal # of Digit Changes: " + g_solve.g_BacktrackingPath.Count + "\r\nEstimated Duration of Simulation: " + estimateSimulationTime + " seconds\r\n(Warning: The simulation can take a very long time to finish, ~17ms per digit change)", "Backtracking Simulation Confirmation", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                g_BacktrackingSolvePath.AddRange(g_solve.g_BacktrackingPath);
                                StartTimer();
                            }
                            else
                            {
                                PopulateGrid(grid);
                            }
                        }
                    }
                }
                else
                {
                    TimeTestSolvers(g_solve, grid, iterations, puzzleString);
                }
            }
            else
            {
                MessageBox.Show("Sudoku puzzle has more than one solution.\r\n\r\nAdd or change digits to create a valid Sudoku puzzle", "Error with puzzle");
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
            if (g_validPuzzle)
            {
                try
                {
                    //Initialising objects
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
                            if (((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length > 1)
                            {
                                List<char> candiList = new List<char>(((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length);
                                for (int i = 0; i < ((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length; i++)
                                {
                                    if (int.TryParse(((TextBox)SudokuPuzzle.Children[cellNum]).Text[i].ToString(), out int result))
                                    {
                                        candiList.Add(((TextBox)SudokuPuzzle.Children[cellNum]).Text[i]);
                                    }
                                }
                                ((TextBox)SudokuPuzzle.Children[cellNum]).Text = "0";
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
                                    Num = ((TextBox)SudokuPuzzle.Children[cellNum]).Text[0],
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
                                PopulateGrid(grid);
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
                        PopulateGrid(grid);
                        strategy_lbl.Content = "Strategy/cleaning just used:\r\n" + g_solve.g_strategy;
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Sudoku puzzle has more than one solution.\r\n\r\nAdd or change digits to create a valid Sudoku puzzle", "Error with puzzle");
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
                if (g_grid == null)
                {
                    g_grid = g_gen.ConstructGrid();
                }
                g_gen.StringToGrid(g_grid, importPuzzle.puzzleStr);
                g_PuzzleString = importPuzzle.puzzleStr;
                PopulateGridString(importPuzzle.puzzleStr);
                g_validPuzzle = true;
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
            g_grid = g_gen.Setter(symmetry);//Calling the automated puzzle generator method to create a puzzle
            int givensCounter = 0;
            for (int i = 0; i < 9; i++)//fill in candidate values for each cell with a full candidate list
            {
                for (int j = 0; j < 9; j++)
                {
                    if (g_grid.Rows[i][j].Num != '0')
                    {
                        givensCounter++;
                    }
                    g_grid.Rows[i][j].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                }
            }
            g_validPuzzle = true;
            givenNums_lbl.Content = "Given Numbers: " + givensCounter;
            difficulty_lbl.Content = "Difficulty: Unknown";
            strategy_lbl.Content = "Strategy just used:\r\n<strategy>";
            PopulateGrid(g_grid);
            g_PuzzleString = g_gen.SudokuToString(g_grid);
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
            if (g_validPuzzle)
            {
                PuzzleSolverCharDS solver = new PuzzleSolverCharDS();
                char[][] puzzle = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
                int counter = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (((TextBox)SudokuPuzzle.Children[counter]).Text.Length > 1 || ((TextBox)SudokuPuzzle.Children[counter]).Text == "0" || ((TextBox)SudokuPuzzle.Children[counter]).Text.Length == 0)
                        {
                            puzzle[i][j] = '0';
                        }
                        else
                        {
                            puzzle[i][j] = ((TextBox)SudokuPuzzle.Children[counter]).Text[0];
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
                                        ((TextBox)SudokuPuzzle.Children[counter]).FontSize = 36;
                                        ((TextBox)SudokuPuzzle.Children[counter]).Text = puzzleTemp[x][y].ToString();
                                    }
                                    else//should never really happpen
                                    {
                                        ((TextBox)SudokuPuzzle.Children[counter]).Text = "0";
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
            else
            {
                MessageBox.Show("Sudoku puzzle has more than one solution.\r\n\r\nAdd or change digits to create a valid Sudoku puzzle", "Error with puzzle");
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
                    ((TextBox)SudokuPuzzle.Children[i]).IsReadOnly = true;
                    if (g_PuzzleString[i] == '0')
                    {
                        ((TextBox)SudokuPuzzle.Children[i]).FontSize = 12;
                        ((TextBox)SudokuPuzzle.Children[i]).Text = "1 2 3 4 5 6 7 8 9";
                    }
                    else
                    {
                        ((TextBox)SudokuPuzzle.Children[i]).FontSize = 36;
                        ((TextBox)SudokuPuzzle.Children[i]).Text = g_PuzzleString[i].ToString();
                        givensCounter++;
                    }
                    ((TextBox)SudokuPuzzle.Children[i]).Background = Brushes.White;
                    
                }
                if (g_grid == null)
                {
                    g_grid = g_gen.ConstructGrid();
                }
                g_validPuzzle = true;
                g_gen.StringToGrid(g_grid, g_PuzzleString);
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
            g_validPuzzle = true;
            g_solve = new PuzzleSolverObjDS();
            PopulateGridString(g_PuzzleString);
            strategy_lbl.Content = "Strategy just used:\r\n<strategy>";
        }
        private void ReGradePuzzles_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to update the rating of each and every puzzle in storage?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            string symmetric;
            if (Symmetry_checkbox.IsChecked == true)
            {
                symmetric = @"Symmetric";
            }
            else
            {
                symmetric = @"NonSymmetric";
            }
            string fileName = symmetric + "/SudokuPuzzles.xml";
            XmlDocument doc = new XmlDocument();
            try
            {
                StreamWriter ratingWrite = new StreamWriter(symmetric + "/ratings.txt", false);
                StreamWriter difficWrite = new StreamWriter(symmetric + "/difficulties.txt", false);
                StreamWriter givensWrite = new StreamWriter(symmetric + "/givens.txt", false);
                #region Strategy Files
                StreamWriter NS = new StreamWriter(symmetric + "/StratsCounts/nakedsingles.txt", false), HS = new StreamWriter(symmetric + "/StratsCounts/hiddensingles.txt", false), NP = new StreamWriter(symmetric + "/StratsCounts/nakedpair.txt", false),
                HP = new StreamWriter(symmetric + "/StratsCounts/hiddenpair.txt", false), PP = new StreamWriter(symmetric + "/StratsCounts/pointline.txt", false), BLR = new StreamWriter(symmetric + "/StratsCounts/blocklinereduc.txt", false), NT = new StreamWriter(symmetric + "/StratsCounts/nakedtriple.txt", false),
                HT = new StreamWriter(symmetric + "/StratsCounts/hiddentriple.txt", false), XW = new StreamWriter(symmetric + "/StratsCounts/xwing.txt", false), YW = new StreamWriter(symmetric + "/StratsCounts/ywing.txt", false), XYZ = new StreamWriter(symmetric + "/StratsCounts/xyzwing.txt", false),
                SC = new StreamWriter(symmetric + "/StratsCounts/singlechains.txt", false), UR1 = new StreamWriter(symmetric + "/StratsCounts/uniquerecttyp1.txt", false), BT = new StreamWriter(symmetric + "/StratsCounts/backtrack.txt", false);
                #endregion
                Stopwatch Timer = new Stopwatch();
                Timer.Start();
                doc.Load(fileName);
                XmlNode sudokuPuzzles = doc.DocumentElement.SelectSingleNode("/SudokuPuzzles");
                XmlNodeList puzzleLabels = sudokuPuzzles.ChildNodes;
                string sudokuString = "";
                int counter = 0;
                foreach (XmlNode label in puzzleLabels)
                {
                    XmlNodeList difficulties = label.ChildNodes;
                    foreach (XmlNode difficulty in difficulties)
                    {
                        List<List<string>> allPuzzles = new List<List<string>>();
                        foreach (XmlNode puzzle in difficulty)
                        {
                            PuzzleSolverObjDS solver = new PuzzleSolverObjDS();
                            counter++;
                            if (label.Name == "Started")
                                sudokuString = puzzle["OriginalSudokuString"].InnerText;
                            else
                                sudokuString = puzzle["SudokuString"].InnerText;
                            SudokuGrid grid = g_gen.ConstructGrid();
                            g_gen.StringToGrid(grid, sudokuString);
                            solver.Solver(grid, 3);
                            puzzle["DifficultyRating"].InnerText = solver.g_Rating.ToString();
                            if (label.Name == "NotStarted")
                            {
                                int givens = 0;
                                for (int x = 0; x < sudokuString.Length; x++)
                                {
                                    if (sudokuString[x] != '0')
                                    {
                                        givens++;
                                    }
                                }
                                ratingWrite.WriteLine(solver.g_Rating);
                                switch (solver.g_Difficulty)
                                {
                                    case "Beginner":
                                        difficWrite.WriteLine("1");
                                        break;
                                    case "Moderate":
                                        difficWrite.WriteLine("2");
                                        break;
                                    case "Advanced":
                                        difficWrite.WriteLine("3");
                                        break;
                                    default:
                                        difficWrite.WriteLine("4");
                                        break;
                                }
                                givensWrite.WriteLine(givens);
                                NS.WriteLine(solver.g_StrategyCount[1]); HS.WriteLine(solver.g_StrategyCount[2]); NP.WriteLine(solver.g_StrategyCount[3]);
                                HP.WriteLine(solver.g_StrategyCount[4]); PP.WriteLine(solver.g_StrategyCount[5]); BLR.WriteLine(solver.g_StrategyCount[6]);
                                NT.WriteLine(solver.g_StrategyCount[7]); HT.WriteLine(solver.g_StrategyCount[8]); XW.WriteLine(solver.g_StrategyCount[9]);
                                YW.WriteLine(solver.g_StrategyCount[10]); XYZ.WriteLine(solver.g_StrategyCount[11]); SC.WriteLine(solver.g_StrategyCount[12]);
                                UR1.WriteLine(solver.g_StrategyCount[13]); BT.WriteLine(solver.g_StrategyCount[0]);
                            }
                        }
                    }
                }
                ratingWrite.Close();
                difficWrite.Close();
                givensWrite.Close();
                NS.Close(); HS.Close(); NP.Close(); HP.Close(); PP.Close(); BLR.Close(); NT.Close();
                HT.Close(); XW.Close(); YW.Close(); XYZ.Close(); SC.Close(); UR1.Close(); BT.Close();
                doc.Save(fileName);
                Timer.Stop();
                MessageBox.Show("Successfully re-graded all Symmetric/Non-Symmetric puzzles in storage\r\n" + "Elapsed Time: " + Timer.Elapsed + "\r\nNumber of puzzles re-graded: " + counter);
            }
            catch (Exception)
            {
                MessageBox.Show("Warning! No existing puzzles found in folder.");
            }
        }
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            string puzzleStr = "";
            for (int i = 0; i < 81; i++)
            {
                if (((TextBox)SudokuPuzzle.Children[i]).FontSize == 12 || ((TextBox)SudokuPuzzle.Children[i]).Text.Length > 1)
                {
                    puzzleStr += ".";
                }
                else
                {
                    puzzleStr += ((TextBox)SudokuPuzzle.Children[i]).Text;
                }
            }
            Clipboard.SetText(puzzleStr);
        }
        private void Cell_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!((TextBox)sender).IsReadOnly)
            { 
                Regex rgx = new Regex("[1-9]");
                if (!rgx.IsMatch(e.Text))
                {
                    e.Handled = true;
                }
                else
                {
                    ((TextBox)sender).Text = "";
                }
            }
        }
        private void Cell_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            if (!((TextBox)sender).IsReadOnly)
            {
                ((TextBox)sender).Text = ((TextBox)sender).Text.Trim();
                if (g_grid == null)
                {
                    g_grid = g_gen.ConstructGrid();
                }
                for (int i = 0, index = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++, index++)
                    {
                        if (((TextBox)SudokuPuzzle.Children[index]).Text == "")
                        {
                            g_grid.Rows[i][j].Num = '0';
                        }
                        else
                            g_grid.Rows[i][j].Num = ((TextBox)SudokuPuzzle.Children[index]).Text[0];
                    }
                }
                g_validPuzzle = g_gen.CheckValidity(g_grid);
                    
            }
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            g_validPuzzle = false;
            for (int i = 0; i < SudokuPuzzle.Children.Count; i++)
            {
                ((TextBox)SudokuPuzzle.Children[i]).Text = "";
                ((TextBox)SudokuPuzzle.Children[i]).FontSize = 36;
                ((TextBox)SudokuPuzzle.Children[i]).IsReadOnly = false;
            }
        }
        private void OnCancelCommand(object sender, DataObjectEventArgs e)
        {
            e.CancelCommand();
        }
        #endregion
    }
}