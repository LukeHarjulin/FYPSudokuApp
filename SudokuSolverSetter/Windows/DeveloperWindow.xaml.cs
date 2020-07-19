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
        private string g_PrevCellContents = "";
        private string g_PuzzleString = "";
        private bool g_validPuzzle = true;
        private SudokuGrid g_grid = null;
        private readonly MainWindow g_homePage = new MainWindow();
        private readonly PuzzleGenerator g_gen = new PuzzleGenerator();
        private PuzzleSolverAdvDS g_solve = new PuzzleSolverAdvDS();
        private string g_currentTime = "";
        private DispatcherTimer G_DT { get; set; }
        private Stopwatch G_Timer { get; set; }
        private readonly List<string> g_BacktrackingSolvePath = new List<string>();
        private int g_PathCounter = 0;
        private readonly List<int> g_ratingList = new List<int>();
        private readonly List<string> g_puzzleStrList = new List<string>(), g_IDList = new List<string>();
        private readonly SolidColorBrush focusCell = new SolidColorBrush(Color.FromArgb(255, 176, 231, 233));
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
                txtbx.LostFocus += new RoutedEventHandler(Cell_LostFocus);
                txtbx.GotFocus += new RoutedEventHandler(Cell_GotFocus);
                txtbx.PreviewKeyDown += new KeyEventHandler(Cell_PreviewKeyDown);
                DataObject.AddPastingHandler(txtbx, OnCancelCommand);
                SudokuPuzzle.Children.Add(txtbx);
                if (col == 8)
                {
                    col = -1;
                    row++;
                }
            }
            PuzzleDifficulty_combo.SelectedIndex = 0;
        }
        #region Functions/Methods
        /// <summary>
        /// Updates the combo box with all the puzzles from the XML file, displayed and ordered by their rating
        /// </summary>
        public void AddPuzzlesToCombo()
        {
            if (PuzzlesByRating_combo.Items.Count > 0)
            {
                PuzzlesByRating_combo.Items.Clear();
                g_ratingList.Clear();
                g_puzzleStrList.Clear();
            }
            string fileName = @"Puzzles/SudokuPuzzles.xml";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(fileName);
                XmlNode sudokuPuzzles = doc.DocumentElement.SelectSingleNode("/SudokuPuzzles");
                XmlNodeList puzzleLabels = sudokuPuzzles.ChildNodes;
                XmlNode label = puzzleLabels[0];
                XmlNodeList difficulty = label.SelectSingleNode(((ComboBoxItem)PuzzleDifficulty_combo.SelectedItem).Content.ToString()).ChildNodes;
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
                                g_IDList.Insert(i, puzzle["ID"].InnerText);
                                g_puzzleStrList.Insert(i, puzzle["SudokuString"].InnerText);
                                added = true;
                                break;
                            }
                        }
                        if (!added)
                        {
                            g_ratingList.Add(rating);
                            g_puzzleStrList.Add(puzzle["SudokuString"].InnerText);
                            g_IDList.Add(puzzle["ID"].InnerText);
                        }
                    }
                    else
                    { 
                        g_ratingList.Add(rating);
                        g_puzzleStrList.Insert(i, puzzle["SudokuString"].InnerText);
                        g_IDList.Insert(i, puzzle["ID"].InnerText);
                    }
                }


                for (int i = 0; i < g_ratingList.Count; i++)
                {
                    PuzzlesByRating_combo.Items.Add(g_IDList[i] + ", " + g_ratingList[i]);
                }
            }
            catch (Exception)
            {

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
            g_solve = new PuzzleSolverAdvDS();
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
                if (grid.Cells[x][y].Num == '0') //0's are placeholders for when there is no value, so any 0's are turned into textboxes containing the candidate values.
                {
                    ((TextBox)SudokuPuzzle.Children[i]).FontSize = 12;
                    ((TextBox)SudokuPuzzle.Children[i]).Text = "";
                    for (int c = 0; c < grid.Cells[x][y].Candidates.Count; c++)
                    {
                        ((TextBox)SudokuPuzzle.Children[i]).Text += grid.Cells[x][y].Candidates[c] + " ";
                    }
                }
                else
                {
                    ((TextBox)SudokuPuzzle.Children[i]).FontSize = 36;
                    ((TextBox)SudokuPuzzle.Children[i]).Text = grid.Cells[x][y].Num.ToString();
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
        private void TimeTestSolvers(PuzzleSolverAdvDS puzzleSolver, SudokuGrid grid, int iterations, string puzzleString)
        {
            if (g_validPuzzle)
            {
                int rating = 0;
                bool solved = false;
                string[] times = new string[4];
                double averageTime = 0;
                for (int m = 1; m < 3; m++)//Tests Advanced data structure solvers
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
                                    grid.Cells[x][y].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                                grid.Cells[x][y].Num = puzzleString[counter];
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
                    outputStr += solved ? "Strategy Solver using an Advanced Data Structure: " + times[0] + "\r\n" : "Strategy Solver using an Advanced Data Structure(trial-and-error required): " + times[0] + "\r\n";
                    times[1] = times[1].Remove(0, 2);
                    outputStr += "Backtracking Solver using an Advanced Data Structure: " + times[1] + "\r\n";
                    times[2] = times[2].Remove(0, 2);
                    outputStr += "Backtracking Solver using a Simple Data Structure: " + times[2] + "\r\n";

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
        private void B_Solve_Click(object sender, RoutedEventArgs e)//This button on the interface is used to solve the g_grid that it is presented
        {
            if (g_validPuzzle)
            {
                int cellNum = 0;
                g_solve = new PuzzleSolverAdvDS();
                //Maps the textbox values onto the g_grid
                for (int r = 0; r < g_grid.Cells.Length; r++)
                {
                    for (int c = 0; c < g_grid.Cells[r].Length; c++)
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
                            g_grid.Cells[r][c].Candidates = candiList;
                            g_grid.Cells[r][c].Num = '0';
                        }
                        else if (((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length == 1 && ((TextBox)SudokuPuzzle.Children[cellNum]).FontSize == 36)
                        {
                            g_grid.Cells[r][c].Candidates = new List<char> { };
                            g_grid.Cells[r][c].Num = ((TextBox)SudokuPuzzle.Children[cellNum]).Text[0];
                        }
                        else if (((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length == 0)
                        {
                            g_grid.Cells[r][c].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                            g_grid.Cells[r][c].Num = '0';
                        }
                        cellNum++;
                    }
                }
                int method = 1;
                if ((Button)sender == Backtracking_Solve_Obj)
                {
                    method = 2;
                }
                int iterations = int.Parse(Number_List_combo.SelectedItem.ToString());
                string puzzleString = g_gen.GridToString(g_grid);
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
                                        g_grid.Cells[x][y].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                                    g_grid.Cells[x][y].Num = puzzleString[counter];
                                    counter++;
                                }
                            }
                        }
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        solved = g_solve.Solver(g_grid, method);
                        if (i == 0)
                        {
                            rating = g_solve.g_Rating;
                            difficulty_lbl.Content = "Difficulty: " + g_solve.g_Difficulty;
                        }
                        watch.Stop();
                        averageTime += watch.ElapsedMilliseconds;

                    }
                    averageTime /= iterations;

                    g_currentTime = iterations > 1 ? "Average time taken to solve: " + averageTime / 1000 : "Time taken to solve: " + averageTime / 1000;
                    if (method == 1)
                    {
                        if (rating != 0)
                        {
                            PopulateGrid(g_grid);
                            if (solved && !g_solve.g_BacktrackingReq)
                            {
                                MessageBox.Show("SOLVED\r\n" + g_currentTime + "\r\nMeasured Puzzle Difficulty Level(experimental): " + g_solve.g_Difficulty + "\r\nMeasured Puzzle Rating(experimental): " + rating);
                            }
                            else if (solved && g_solve.g_BacktrackingReq)
                            {
                                MessageBox.Show("FAILED\r\n" + g_currentTime + "\r\nFinished with trial and error, unable to finish using implemented strategies." + "\r\nMeasured Puzzle Difficulty Level(experimental): " + g_solve.g_Difficulty + "\r\nMeasured Puzzle Rating(experimental): " + rating);
                            }
                            else if (!solved)
                            {
                                MessageBox.Show("FAILED\r\n" + g_currentTime + "\r\nFailed with trial and error, puzzle must be invalid!." + "\r\nMeasured Puzzle Difficulty Level(experimental): " + g_solve.g_Difficulty + "\r\nMeasured Puzzle Rating(experimental): " + rating);
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
                            if (solved)
                            {
                                MessageBoxResult result = MessageBox.Show("SOLVED\r\n" + g_currentTime + "\r\n\r\nDo you want to see a simulation of the Backtracking algorithm solving the puzzle?\r\nTotal # of Digit Changes: " + g_solve.g_BacktrackingPath.Count + "\r\nEstimated Duration of Simulation: " + estimateSimulationTime + " seconds\r\n(Warning: The simulation can take a very long time to finish, ~17ms per digit change)", "Backtracking Simulation Confirmation", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.Yes)
                                {
                                    g_BacktrackingSolvePath.AddRange(g_solve.g_BacktrackingPath);
                                    StartTimer();
                                }
                                else
                                {
                                    PopulateGrid(g_grid);
                                }
                            }
                            else
                            {
                                MessageBox.Show("FAILED\r\n" + g_currentTime + "\r\nFailed with trial and error, puzzle must be invalid!." + "\r\nMeasured Puzzle Difficulty Level(experimental): " + g_solve.g_Difficulty + "\r\nMeasured Puzzle Rating(experimental): " + rating);
                                PopulateGrid(g_grid);
                            }


                        }
                    }
                }
                else
                {
                    TimeTestSolvers(g_solve, g_grid, iterations, puzzleString);
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
        private void B_SolveStep_Click(object sender, RoutedEventArgs e)//This button on the interface is used to solve in increments/steps
        {
            if (g_validPuzzle)
            {
                try
                {
                    List<Cell> prevFilledCells = new List<Cell>();
                    int cellNum = 0;
                    //Maps the textbox values onto the g_grid
                    for (int r = 0; r < 9; r++)
                    {
                        for (int c = 0; c < 9; c++)
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
                                g_grid.Cells[r][c].Candidates = candiList;
                                g_grid.Cells[r][c].Num = '0';
                                g_grid.Cells[r][c].ReadOnly = false;
                            }
                            else if (((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length == 1 && ((TextBox)SudokuPuzzle.Children[cellNum]).FontSize == 36)
                            {
                                g_grid.Cells[r][c].Candidates = new List<char> { };
                                g_grid.Cells[r][c].Num = ((TextBox)SudokuPuzzle.Children[cellNum]).Text[0];
                                if (!prevFilledCells.Contains(g_grid.Cells[r][c]) && !g_grid.Cells[r][c].ReadOnly)
                                    prevFilledCells.Add(g_grid.Cells[r][c]);
                                
                            }
                            else if (((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length == 0)
                            {
                                g_grid.Cells[r][c].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                                g_grid.Cells[r][c].Num = '0';
                            }
                            cellNum++;
                        }
                    }
                    if (!g_gen.CheckIfSolved(g_grid))
                    {
                        if (g_solve.SolveNextStep(g_grid))
                        {
                            if (g_solve.g_Rating != 0)
                            {
                                PopulateGrid(g_grid);
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
                        else if (g_solve.g_BacktrackingReq)
                        {
                            MessageBox.Show("FAILED\r\n" + g_currentTime + "\r\nFailed with trial and error, puzzle must be invalid!.");
                        }
                        PopulateGrid(g_grid);
                        strategy_lbl.Content = "Strategy/cleaning from Step " + g_solve.g_StepCounter + ":\r\n" + g_solve.g_strategy;
                        for (int c = 0; c < prevFilledCells.Count; c++)
                        {
                            ((TextBox)SudokuPuzzle.Children[(prevFilledCells[c].XLocation * 9) + prevFilledCells[c].YLocation]).Background = Brushes.LightGreen;
                        }
                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                if (!g_grid.Cells[i][j].ReadOnly && g_grid.Cells[i][j].Num != '0' && !prevFilledCells.Contains(g_grid.Cells[i][j]))
                                {
                                    ((TextBox)SudokuPuzzle.Children[(i*9)+j]).Background = Brushes.LawnGreen;
                                }
                            }
                        }
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
        private void Create_Store_Puzzles_btn_Click(object sender, RoutedEventArgs e)
        {
            CreatePuzzles createPuzzles = new CreatePuzzles(1000)
            {
                Owner = this
            };
            createPuzzles.ShowDialog();
            AddPuzzlesToCombo();
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
                g_solve = new PuzzleSolverAdvDS();
                strategy_lbl.Content = "";
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
            g_grid = g_gen.Setter();//Calling the automated puzzle generator method to create a puzzle
            int givensCounter = 0;
            for (int i = 0; i < 9; i++)//fill in candidate values for each cell with a full candidate list
            {
                for (int j = 0; j < 9; j++)
                {
                    if (g_grid.Cells[i][j].Num != '0')
                    {
                        givensCounter++;
                    }
                    g_grid.Cells[i][j].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                }
            }
            g_validPuzzle = true;
            givenNums_lbl.Content = "Given Numbers: " + givensCounter;
            difficulty_lbl.Content = "Difficulty: Unknown";
            strategy_lbl.Content = "";
            PopulateGrid(g_grid);
            g_PuzzleString = g_gen.GridToString(g_grid);
            Clipboard.SetText(g_PuzzleString);
            g_solve = new PuzzleSolverAdvDS();
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
                averageTime /= iterations;
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
            if (PuzzlesByRating_combo.Items.Count > 0)
            {
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
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (g_grid.Cells[i][j].Num == 0)
                                g_grid.Cells[i][j].ReadOnly = false;
                            else
                                g_grid.Cells[i][j].ReadOnly = true;
                        }
                    }
                    givenNums_lbl.Content = "Given Numbers: " + givensCounter;
                    difficulty_lbl.Content = "Difficulty: " + ((ComboBoxItem)PuzzleDifficulty_combo.SelectedItem).Content.ToString();
                    g_solve = new PuzzleSolverAdvDS();
                    strategy_lbl.Content = "";
                }
            }

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
            AddPuzzlesToCombo();
            if (PuzzlesByRating_combo.Items.Count > 0)
            {
                PuzzlesByRating_combo.SelectedIndex = 0;
            }
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
            g_solve = new PuzzleSolverAdvDS();
            PopulateGridString(g_PuzzleString);
            g_gen.StringToGrid(g_grid, g_PuzzleString);
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (g_grid.Cells[i][j].Num == 0)
                        g_grid.Cells[i][j].ReadOnly = false;
                    else
                        g_grid.Cells[i][j].ReadOnly = true;
                }
            }
            strategy_lbl.Content = "";
            int givensCounter = 0;
            for (int i = 0; i < g_PuzzleString.Length; i++)
            {
                if (g_PuzzleString[i] != '0')
                    givensCounter++;
            }
            givenNums_lbl.Content = "Given Numbers: " + givensCounter;

        }
        private void ReGradePuzzles_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to update the rating of each and every puzzle in storage?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            string fileName = @"Puzzles/SudokuPuzzles.xml";
            XmlDocument doc = new XmlDocument();
            try
            {
                StreamWriter ratingWrite = new StreamWriter(@"Puzzles/ratings.txt", false);
                StreamWriter difficWrite = new StreamWriter(@"Puzzles/difficulties.txt", false);
                StreamWriter givensWrite = new StreamWriter(@"Puzzles/givens.txt", false);
                StreamWriter IDsWrite = new StreamWriter(@"Puzzles/IDs.txt", false);
                StreamWriter stepsWrite = new StreamWriter(@"Puzzles/steps.txt", false);
                #region Strategy Files
                StreamWriter NS = new StreamWriter(@"Puzzles/StratsCounts/nakedsingles.txt", false), HS = new StreamWriter(@"Puzzles/StratsCounts/hiddensingles.txt", false), NP = new StreamWriter(@"Puzzles/StratsCounts/nakedpair.txt", false),
                HP = new StreamWriter(@"Puzzles/StratsCounts/hiddenpair.txt", false), PP = new StreamWriter(@"Puzzles/StratsCounts/pointline.txt", false), BLR = new StreamWriter(@"Puzzles/StratsCounts/blocklinereduc.txt", false), NT = new StreamWriter(@"Puzzles/StratsCounts/nakedtriple.txt", false),
                HT = new StreamWriter(@"Puzzles/StratsCounts/hiddentriple.txt", false), XW = new StreamWriter(@"Puzzles/StratsCounts/xwing.txt", false), YW = new StreamWriter(@"Puzzles/StratsCounts/ywing.txt", false), XYZ = new StreamWriter(@"Puzzles/StratsCounts/xyzwing.txt", false),
                SC = new StreamWriter(@"Puzzles/StratsCounts/singlechains.txt", false), UR1 = new StreamWriter(@"Puzzles/StratsCounts/uniquerecttyp1.txt", false), BT = new StreamWriter(@"Puzzles/StratsCounts/backtrack.txt", false);
                #endregion
                Stopwatch Timer = new Stopwatch();
                Timer.Start();
                doc.Load(fileName);
                XmlNode sudokuPuzzles = doc.DocumentElement.SelectSingleNode("/SudokuPuzzles");
                string sudokuString = "";
                int counter = 0;
                foreach (XmlNode label in sudokuPuzzles.ChildNodes)
                {
                    foreach (XmlNode difficulty in label.ChildNodes)
                    {
                        List<List<string>> allPuzzles = new List<List<string>>();
                        foreach (XmlNode puzzle in difficulty.ChildNodes)
                        {
                            PuzzleSolverAdvDS solver = new PuzzleSolverAdvDS();
                            counter++;
                            if (label.Name == "Started")
                                sudokuString = puzzle["OriginalSudokuString"].InnerText;
                            else if (label.Name == "Completed")
                                sudokuString = puzzle["SudokuString"].InnerText.Remove(0,1);
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
                                ratingWrite.WriteLine(solver.g_Rating);
                                givensWrite.WriteLine(givens);
                                IDsWrite.WriteLine(puzzle["ID"].InnerText);
                                stepsWrite.WriteLine(solver.g_StepCounter);
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
                IDsWrite.Close();
                stepsWrite.Close();
                NS.Close(); HS.Close(); NP.Close(); HP.Close(); PP.Close(); BLR.Close(); NT.Close();
                HT.Close(); XW.Close(); YW.Close(); XYZ.Close(); SC.Close(); UR1.Close(); BT.Close();
                doc.Save(fileName);
                Timer.Stop();
                MessageBox.Show("Successfully re-graded all puzzles in storage\r\n" + "Elapsed Time: " + Timer.Elapsed + "\r\nNumber of puzzles re-graded: " + counter);
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
            MessageBox.Show("Copied current puzzle string to clipboard!");
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
                int givensCounter = 0;
                for (int i = 0, index = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++, index++)
                    {
                        if (((TextBox)SudokuPuzzle.Children[index]).Text == "")
                        {
                            g_grid.Cells[i][j].Num = '0';
                        }
                        else
                        {
                            g_grid.Cells[i][j].Num = ((TextBox)SudokuPuzzle.Children[index]).Text[0];
                            givensCounter++;
                        }
                    }
                }
                if (g_gen.ValidateInput(SudokuPuzzle, g_grid, (TextBox)sender))
                {
                    if (g_gen.CheckValidity(g_grid))
                    {
                        g_validPuzzle = true;
                        g_PuzzleString = g_gen.GridToString(g_grid);
                    }
                    else
                    {
                        g_validPuzzle = false;
                    }
                    givenNums_lbl.Content = "Given Numbers: " + givensCounter;
                    g_PrevCellContents = ((TextBox)sender).Text;
                }
                else
                {
                    g_validPuzzle = false;
                    if (g_PrevCellContents != "")
                    {
                        string givensContent = givenNums_lbl.Content.ToString();
                        int prevGivens = int.Parse(givensContent.Substring(15, givensContent.Length - 15));
                        givenNums_lbl.Content = "Given Numbers: " + --prevGivens;
                    }
                    ((TextBox)sender).Text = "";
                    g_PrevCellContents = ((TextBox)sender).Text;
                }
            }
        }
        /// <summary>
        /// Clears all textboxes of all values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (G_Timer != null)
            {
                if (G_Timer.IsRunning)
                {
                    StopTimer();
                }
            }
            g_validPuzzle = false;
            givenNums_lbl.Content = "Given Numbers: 0";
            difficulty_lbl.Content = "Difficulty: Unknown";
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
        /// <summary>
        /// Handles when a cell in the grid is selected, changing the background colour of the selected cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Background = focusCell;//sets the current focused cell to a more prominent colour
            g_PrevCellContents = ((TextBox)sender).Text;
        }
        private void Cell_LostFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Background = Brushes.White;//sets the current focused cell to default colour
        }
        private void Cell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (SudokuPuzzle.Children.IndexOf((TextBox)sender) - 1 >= 0)
                    {
                        SudokuPuzzle.Children[SudokuPuzzle.Children.IndexOf((TextBox)sender) - 1].Focus();
                    }
                    break;
                case Key.Up:
                    if (SudokuPuzzle.Children.IndexOf((TextBox)sender) - 9 >= 0)
                    {
                        SudokuPuzzle.Children[SudokuPuzzle.Children.IndexOf(((TextBox)sender)) - 9].Focus();
                    }
                    break;
                case Key.Right:
                    if (SudokuPuzzle.Children.IndexOf((TextBox)sender) + 1 <= 80)
                    {
                        SudokuPuzzle.Children[SudokuPuzzle.Children.IndexOf((TextBox)sender) + 1].Focus();
                    }
                    break;
                case Key.Down:
                    if (SudokuPuzzle.Children.IndexOf((TextBox)sender) + 9 <= 80)
                    {
                        SudokuPuzzle.Children[SudokuPuzzle.Children.IndexOf((TextBox)sender) + 9].Focus();
                    }
                    break;
                case Key.Delete:
                    if (!((TextBox)sender).IsReadOnly && ((TextBox)sender).Text.Length > 0)
                    {
                        string givensContent = givenNums_lbl.Content.ToString();
                        int prevGivens = int.Parse(givensContent.Substring(15, givensContent.Length - 15));
                        givenNums_lbl.Content = "Given Numbers: " + --prevGivens;
                        ((TextBox)sender).Text = "";
                    }
                    break;
                case Key.Back:
                    if (!((TextBox)sender).IsReadOnly && ((TextBox)sender).Text.Length > 0)
                    {
                        string givensContent = givenNums_lbl.Content.ToString();
                        int prevGivens = int.Parse(givensContent.Substring(15, givensContent.Length - 15));
                        givenNums_lbl.Content = "Given Numbers: " + --prevGivens;
                        ((TextBox)sender).Text = "";
                    }
                    break;
                default:
                    break;
            }
            #endregion
        }
    }
}