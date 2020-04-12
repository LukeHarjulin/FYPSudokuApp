using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Windows.Threading;
using System.Diagnostics;

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for DeveloperWindow.xaml
    /// </summary>
    public partial class DeveloperWindow : Window
    {
        private List<TextBox> g_txtBxList = new List<TextBox>();
        private MainWindow g_homePage = new MainWindow();
        private PuzzleGenerator g_gen = new PuzzleGenerator();
        private PuzzleSolver g_solve = new PuzzleSolver();
        private string g_currentTime = "";
        private DispatcherTimer g_DT { get; set; }
        private Stopwatch g_Timer { get; set; }
        private List<string> g_BruteSolvePath = new List<string>();
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
            AddPuzzlesToCombo();
        }
        public void AddPuzzlesToCombo()
        {
            if (PuzzlesByRating_combo.Items.Count > 0)
            {
                PuzzlesByRating_combo.Items.Clear();
                g_ratingList.Clear();
                g_puzzleStrList.Clear();
            }
            string fileName = @"SudokuPuzzles.xml";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(fileName);
                XmlNode sudokuPuzzles = doc.DocumentElement.SelectSingleNode("/SudokuPuzzles");
                XmlNodeList puzzleLabels = sudokuPuzzles.ChildNodes;
                foreach (XmlNode label in puzzleLabels)
                {
                    XmlNodeList difficulties = label.ChildNodes;
                    foreach (XmlNode difficulty in difficulties)
                    {
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
                            else
                            {
                                g_puzzleStrList.Insert(i, puzzle["SudokuString"].InnerText);
                            }
                        }
                    }
                }
                foreach (int s in g_ratingList)
                {
                    PuzzlesByRating_combo.Items.Add(s);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Warning! No existing XML file found in folder.");
            }
        }
        public List<TextBox> PopulateGrid(SudokuGrid grid, List<TextBox> m_txtBxList)
        {
            /*This method populates the Uniform grid and its textboxes with all the given values from 'grid'.
            */
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
            }
            return m_txtBxList;
        }
        /// <summary>
        /// Tests the three solvers: Strategy solver, brute force solver with object data structure, and brute force solver with char[][] data structure
        /// </summary>
        /// <param name="puzzleSolver">passing through so that it isn't redeclared</param>
        /// <param name="grid">grid used to pass to the solvers</param>
        /// <param name="iterations">number of times chosen to solve the puzzle by each solver</param>
        /// <param name="puzzleString">the puzzle grid in string form</param>
        private void TimeTestSolvers(PuzzleSolver puzzleSolver, SudokuGrid grid, int iterations, string puzzleString)
        {
            int difficulty = 0;
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
                            difficulty = puzzleSolver.g_Difficulty;
                        }
                    }
                    else
                        puzzleSolver.Solver(grid, m);
                    watch.Stop();
                    averageTime += watch.ElapsedMilliseconds;
                }
                times[m - 1] = m + "," + (averageTime / iterations / 1000).ToString();
                
            }
            PuzzleSolverCharVer solver = new PuzzleSolverCharVer();
            char[][] puzzleCharArr = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
            averageTime = 0;
            for (int n = 0; n < iterations; n++)//Test char[][] brute force solver
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
            times[2] = 4 + "," + (averageTime / iterations / 1000).ToString();
            string outputStr = "After " + iterations + " iteration(s), average times taken to solve\r\nDifficulty (WIP): " + difficulty + "\r\n";
            times[0] = times[0].Remove(0, 2);
            outputStr += solved ? "Strategy Solver using Objects: " + times[0] + "\r\n" : "Strategy Solver using Objects(Brute-Force required): " + times[0] + "\r\n";
            times[1] = times[1].Remove(0, 2);
            outputStr += "Brute-Force Solver using Objects: " + times[1] + "\r\n";
            times[2] = times[2].Remove(0, 2);
            outputStr += "Brute-Force Solver using char[][]: " + times[2] + "\r\n";

            PopulateGrid(grid, g_txtBxList);
            MessageBox.Show(outputStr);
        }
        private void StartTimer()
        {
            g_Timer = new Stopwatch();
            g_DT = new DispatcherTimer();
            g_DT.Tick += new EventHandler(DT_Tick);
            g_DT.Interval = new TimeSpan(0, 0, 0, 0, 1);
            g_Timer.Start();
            g_DT.Start();
            Brute_Solve_char.IsEnabled = false;
            Brute_Solve_Obj.IsEnabled = false;
            b_Solve.IsEnabled = false;
            TestAllThree.IsEnabled = false;
            b_Solve1by1.IsEnabled = false;
            Import_Puzzle.IsEnabled = false;
            Create_Store_Puzzles_btn.IsEnabled = false;
        }
        private void StopTimer()
        {
            g_Timer.Stop();
            g_DT.Stop();
            Brute_Solve_char.IsEnabled = true;
            Brute_Solve_Obj.IsEnabled = true;
            b_Solve.IsEnabled = true;
            TestAllThree.IsEnabled = true;
            b_Solve1by1.IsEnabled = true;
            Import_Puzzle.IsEnabled = true;
            Create_Store_Puzzles_btn.IsEnabled = true;
        }
        private void DT_Tick(object sender, EventArgs e)
        {
            if (g_Timer.IsRunning)
            {
                try
                {
                    if (g_PathCounter >= g_BruteSolvePath.Count)
                    {
                        StopTimer();
                    }
                    else
                    {
                        //Add number to cell
                        int i1 = Convert.ToInt32(g_BruteSolvePath[g_PathCounter][0]) + 1;
                        int j1 = Convert.ToInt32(g_BruteSolvePath[g_PathCounter][1]) + 1;
                        for (int i = 0; i < g_txtBxList.Count; i++)
                        {
                            if (g_txtBxList[i].Name[1] == (char)i1 && g_txtBxList[i].Name[3] == (char)j1)
                            {
                                g_txtBxList[i].FontSize = 36;
                                if (g_BruteSolvePath[g_PathCounter][2] == '0')
                                {
                                    g_txtBxList[i].Text = "";
                                }
                                else
                                {
                                    g_txtBxList[i].Text = g_BruteSolvePath[g_PathCounter][2].ToString();
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
        private void B_Solve_Click(object sender, RoutedEventArgs e)//This button on the interface is used to solve the grid that it is presented
        {
            //Initialising objects
            PuzzleSolver puzzleSolver = new PuzzleSolver();
            PuzzleGenerator gen = new PuzzleGenerator();
            SudokuGrid grid = new SudokuGrid();
            grid.Rows = new Cell[9][];
            int cellNum = 0;

            //This transforms the text in the boxes to a useable grid object. Resource heavy - alternative method may be developed in improvements
            for (int r = 0; r < grid.Rows.Length; r++)
            {
                grid.Rows[r] = new Cell[9];
                for (int c = 0; c < grid.Rows[r].Length; c++)
                {
                    string blockLoc = g_txtBxList[cellNum].Name[5].ToString();
                    if (g_txtBxList[cellNum].Text.Length > 1)
                    {
                        g_txtBxList[cellNum].Text = "0";
                        grid.Rows[r][c] = new Cell()
                        {
                            Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' },
                            Num = '0',
                            BlockLoc = Convert.ToInt32(blockLoc),
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
                            BlockLoc = Convert.ToInt32(blockLoc),
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
                    int[] blockIndexes = gen.BlockIndexGetter(grid.Rows[i][j].BlockLoc);

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
            int method = 1;
            if ((Button)sender == Brute_Solve_Obj)
            {
                method = 2;
            }
            int iterations = int.Parse(Number_List_combo.SelectedItem.ToString());
            string puzzleString = gen.SudokuToString(grid);
            if ((Button)sender != TestAllThree)
            {
                int difficulty = 0;
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
                        difficulty = puzzleSolver.g_Difficulty;
                    watch.Stop();
                    averageTime += watch.ElapsedMilliseconds;
                }
                averageTime = averageTime / iterations;
                
                g_currentTime = iterations > 1 ? "Average time taken to solve: " + averageTime / 1000 : "Time taken to solve: " + averageTime / 1000;

                
                if (method == 1)
                {
                    PopulateGrid(grid, g_txtBxList);
                    if (solved && !puzzleSolver.g_BruteForced)
                    {
                        MessageBox.Show("SOLVED\r\n" + g_currentTime + "\r\nMeasured Difficulty(WIP): " + difficulty);
                    }
                    else if (puzzleSolver.g_BruteForced)
                    {
                        MessageBox.Show("FAILED\r\n" + g_currentTime + "\r\nFinished with bruteforce, added +2000 to difficulty to compensate for unimplemented strategies.\r\nMeasured Difficulty(experimental): " + difficulty);
                    }
                    SolvePath path = new SolvePath();
                    path.PopulateTextBlock(difficulty, g_currentTime, puzzleSolver);
                    
                }
                else if (method == 2)
                {
                    MessageBoxResult result = MessageBox.Show("SOLVED\r\n" + g_currentTime+"\r\n\r\nDo you want to see the Brute-Force solver solve the puzzle?\r\n(Warning: The visual solving can take a very long time to finish)", "Brute-Force Visual Confirmation", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        g_BruteSolvePath.AddRange(puzzleSolver.g_BruteSolvePath);
                        StartTimer();
                    }
                    else
                    {
                        PopulateGrid(grid, g_txtBxList);
                    }
                }
            }
            else
            {
                TimeTestSolvers(puzzleSolver, grid, iterations, puzzleString);
            }

        }
        private void B_Solve1by1_Click(object sender, RoutedEventArgs e)//This button on the interface is used to solve in increments (e.g. once a value is placed into a cell, the solver stops)
        {
            //Initialising objects
            PuzzleSolver solve = new PuzzleSolver();
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
            SudokuGrid gridSolve = new SudokuGrid();//Passes by reference, changed to deep copy
            gridSolve.Rows = new Cell[9][];
            int cellNum = 0;

            //This transforms the text in the boxes to a useable grid object. Resource heavy - alternative method may be developed in improvements
            for (int r = 0; r < gridSolve.Rows.Length; r++)
            {
                gridSolve.Rows[r] = new Cell[9];
                for (int c = 0; c < gridSolve.Rows[r].Length; c++)
                {
                    string blockLoc = txtBxList[cellNum].Name[5].ToString();
                    if (txtBxList[cellNum].Text.Length > 1)
                    {
                        List<char> candiList = new List<char>();
                        candiList.AddRange(txtBxList[cellNum].Text.ToCharArray());
                        candiList.RemoveAll(item => item == ' ');
                        gridSolve.Rows[r][c] = new Cell()
                        {

                            Candidates = candiList,
                            Num = '0',
                            BlockLoc = Convert.ToInt32(blockLoc),
                            XLocation = r,
                            YLocation = c
                        };
                    }
                    else
                    {
                        gridSolve.Rows[r][c] = new Cell()
                        {
                            Candidates = new List<char> { },
                            Num = txtBxList[cellNum].Text[0],
                            BlockLoc = Convert.ToInt32(blockLoc),
                            XLocation = r,
                            YLocation = c
                        };
                    }

                    cellNum++;
                }
            }

            gridSolve = solve.SolveCellByCell(gridSolve);
            PopulateGrid(gridSolve, txtBxList);
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            if (g_Timer != null)
            {
                if (g_Timer.IsRunning)
                {
                    StopTimer();
                }
            }
            g_homePage = new MainWindow();
            g_homePage.Show();
        }

        private void Window_Close(object sender, EventArgs e)
        {
            if (g_Timer != null)
            {
                if (g_Timer.IsRunning)
                {
                    StopTimer();
                }
            }
            g_homePage = new MainWindow();
            g_homePage.Show();
        }

        private void Create_Store_Puzzles_btn_Click(object sender, RoutedEventArgs e)
        {
            CreatePuzzles createPuzzles = new CreatePuzzles();
            createPuzzles.ShowDialog();
            AddPuzzlesToCombo();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            ImportPuzzle importPuzzle = new ImportPuzzle();
            
            if (importPuzzle.ShowDialog() == true)
            {
                int givensCounter = 0;
                string importStr = importPuzzle.puzzleStr;
                Regex rgx = new Regex(@"[1-9]");
                for (int i = 0; i < 81; i++)
                {
                    if (!rgx.IsMatch(importStr[i].ToString()))
                    {
                        g_txtBxList[i].FontSize = 12;
                        g_txtBxList[i].Text = "1 2 3 4 5 6 7 8 9";
                    }
                    else
                    {
                        g_txtBxList[i].FontSize = 36;
                        g_txtBxList[i].Text = importStr[i].ToString();
                        givensCounter++;
                    }
                }
                givenNums_lbl.Content = "Given Numbers: " + givensCounter;
            }
            
        }

        private void GeneratePuzzle_Click(object sender, RoutedEventArgs e)
        {
            if (g_Timer != null)
            {
                if (g_Timer.IsRunning)
                {
                    StopTimer();
                }
            }
            SudokuGrid grid = g_gen.Setter();//Calling the automated puzzle generator method to create a puzzle
            int givensCounter = 0;
            for (int i = 0; i < 9; i++)
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
            g_txtBxList = PopulateGrid(grid, g_txtBxList);
            Clipboard.SetText(g_gen.SudokuToString(grid));
        }

        private void BruteSolve_char_Click(object sender, RoutedEventArgs e)
        {
            PuzzleSolverCharVer solver = new PuzzleSolverCharVer();
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
            char method = '1';
            if ((Button)sender == Brute_Solve_char)
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
                var watch = System.Diagnostics.Stopwatch.StartNew();
                solved = solver.Solvers(puzzleTemp, '2');
                watch.Stop();
                averageTime += watch.ElapsedMilliseconds;
            }
            averageTime = averageTime / iterations;
            g_currentTime = iterations > 1 ? "Average time taken to solve: " + averageTime / 1000 : "Time taken to solve: " + averageTime / 1000;
            counter = 0;
            
            if (solved)
            {
                MessageBoxResult result = MessageBox.Show("SOLVED\r\n" + g_currentTime + "\r\n\r\nDo you want to see the Brute-Force solver solve the puzzle?\r\n(Warning: The visual solving can take a very long time to finish)", "Brute-Force Visual Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    g_BruteSolvePath.AddRange(solver.solvePath);
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
                MessageBox.Show("FAILED\r\n" + g_currentTime);
            }
        }

        private void RatingCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (g_Timer != null)
            {
                if (g_Timer.IsRunning)
                {
                    StopTimer();
                }
            }
            if (PuzzlesByRating_combo.SelectedIndex >= 0)
            {
                int givensCounter = 0;
                for (int i = 0; i < 81; i++)
                {
                    if (g_puzzleStrList[PuzzlesByRating_combo.SelectedIndex][i] == '0')
                    {
                        g_txtBxList[i].FontSize = 12;
                        g_txtBxList[i].Text = "1 2 3 4 5 6 7 8 9";
                    }
                    else
                    {
                        g_txtBxList[i].FontSize = 36;
                        g_txtBxList[i].Text = g_puzzleStrList[PuzzlesByRating_combo.SelectedIndex][i].ToString();
                        givensCounter++;
                    }
                }
                givenNums_lbl.Content = "Given Numbers: " + givensCounter;
            }
        }
    }
}
