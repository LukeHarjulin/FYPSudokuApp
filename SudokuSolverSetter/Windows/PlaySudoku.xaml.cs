using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Threading;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Windows.Ink;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for PlaySudoku.xaml
    /// </summary>
    public partial class PlaySudoku : Window
    {
        #region Initialisation
        #region Global variables
        private readonly MainWindow homePage = new MainWindow();
        private readonly PuzzleGenerator g_Gen = new PuzzleGenerator();
        private TextBox g_selectedCell;
        private readonly SudokuGrid g_grid = new SudokuGrid();
        private string g_currentTime = "";
        private readonly string g_rating, g_ID;
        private readonly string g_difficulty;
        private readonly string g_originalPuzzleString;
        private bool g_pencilMarker = false, penType = false, g_solved = false;
        private readonly SolidColorBrush focusCell = new SolidColorBrush(Color.FromArgb(150, 176, 231, 233));
        private readonly SolidColorBrush cellColour = new SolidColorBrush(Color.FromArgb(150, 255, 221, 192));
        private readonly SolidColorBrush hoverCell = new SolidColorBrush(Color.FromArgb(150, 255, 241, 230));
        private readonly SolidColorBrush altCellColour = new SolidColorBrush(Color.FromArgb(150, 255, 224, 233));
        private readonly SolidColorBrush pauseBlockBckGrd = new SolidColorBrush(Color.FromArgb(255, 238, 241, 255));
        private readonly SolidColorBrush darkPauseBlockBckGrd = new SolidColorBrush(Color.FromArgb(255, 238, 241, 255));
        private SolidColorBrush backgroundCol, darkFocusCell, darkHoverCell, darkerColour, darkColour, darkButtonColour, darkTextColour, buttonColour, altDarkCellColour;
        private readonly DrawingAttributes g_penDA;
        private readonly DrawingAttributes g_highlighterDA;
        private TimeSpan g_StartingTS = new TimeSpan();
        public DispatcherTimer DT { get; set; }
        public Stopwatch Timer { get; set; }
        #endregion
        public PlaySudoku() => InitializeComponent();
        /// <summary>
        /// Receives a difficulty setting from wherever it is called and loads that puzzle. If a puzzle in string from is provided, then that puzzle is loaded instead
        /// </summary>
        /// <param name="difficulty"></param>
        /// <param name="puzzleString"></param>
        public PlaySudoku(string difficulty, string puzzleString)//Initialize window
        {
            InitializeComponent();
            Panel.SetZIndex(PauseBlock, 2);
            g_penDA = new DrawingAttributes();
            g_penDA = new DrawingAttributes
            {
                Color = Colors.SpringGreen,
                Height = 3,
                Width = 3
            };
            g_highlighterDA = new DrawingAttributes
            {
                Color = Colors.SpringGreen,
                IsHighlighter = true,
                IgnorePressure = true,
                StylusTip = StylusTip.Rectangle,
                Height = 15,
                Width = 5
            };
            radio_Green.IsChecked = true;
            ColourCanvas.DefaultDrawingAttributes = g_highlighterDA;
            PuzzleGenerator gen = new PuzzleGenerator();
            timer_txtbx.Text = "0:00:00";
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
                txtbx.Opacity = 50;
                txtbx.BorderThickness = new Thickness(left, top, right, bottom);
                txtbx.CaretBrush = Brushes.Transparent;
                txtbx.BorderBrush = Brushes.Black;
                txtbx.Cursor = Cursors.Arrow;
                txtbx.TextWrapping = TextWrapping.Wrap;
                txtbx.FontSize = 36; txtbx.HorizontalContentAlignment = HorizontalAlignment.Center; txtbx.VerticalContentAlignment = VerticalAlignment.Center;
                txtbx.PreviewTextInput += new TextCompositionEventHandler(Cell_PreviewTextInput);
                txtbx.TextChanged += new TextChangedEventHandler(Cell_TextChanged);
                txtbx.LostFocus += new RoutedEventHandler(Cell_LostFocus);
                txtbx.PreviewKeyDown += new KeyEventHandler(Cell_PreviewKeyDown);
                txtbx.MouseEnter += new MouseEventHandler(MouseEnter_Cell);
                txtbx.MouseLeave += new MouseEventHandler(MouseLeave_Cell);
                txtbx.GotFocus += new RoutedEventHandler(Cell_GotFocus);
                DataObject.AddPastingHandler(txtbx, OnCancelCommand);

                SudokuPuzzle.Children.Add(txtbx); if (col == 8)
                {
                    col = -1;
                    row++;
                }
            }
            RecolourAllCells(false);
            string fileName = @"Puzzles/SudokuPuzzles.xml";
            g_difficulty = difficulty;
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(fileName);
                XmlNode sudokuPuzzles = doc.DocumentElement.SelectSingleNode("/SudokuPuzzles");
                if (puzzleString == "")//pulling a non-started puzzle
                {
                    XmlNode notStartedPuzzles = sudokuPuzzles.FirstChild;
                    XmlNodeList puzzleDifficulties = notStartedPuzzles.ChildNodes;
                    XmlNode puzzle = null;
                    Random rnd = new Random();
                    if (difficulty == "Beginner")//pull random beginner puzzle from xml file
                    {
                        Sudoku_Title.Content = "Beginner Sudoku Puzzle";
                        XmlNodeList beginnerPuzzles = puzzleDifficulties[0].ChildNodes;
                        puzzle = beginnerPuzzles[rnd.Next(0, beginnerPuzzles.Count)];
                    }
                    else if (difficulty == "Moderate")//pull random moderate puzzle from xml file
                    {
                        Sudoku_Title.Content = "Moderate Sudoku Puzzle";
                        XmlNodeList moderatePuzzles = puzzleDifficulties[1].ChildNodes;
                        puzzle = moderatePuzzles[rnd.Next(0, moderatePuzzles.Count)];
                    }
                    else if (difficulty == "Advanced")//pull random advanced puzzle from xml file
                    {
                        Sudoku_Title.Content = "Advanced Sudoku Puzzle";
                        XmlNodeList advancedPuzzles = puzzleDifficulties[2].ChildNodes;
                        puzzle = advancedPuzzles[rnd.Next(0, advancedPuzzles.Count)];
                    }
                    else if (difficulty == "Extreme")//pull random extreme puzzle from xml file
                    {
                        Sudoku_Title.Content = "Extreme Sudoku Puzzle";
                        XmlNodeList extremePuzzles = puzzleDifficulties[3].ChildNodes;
                        puzzle = extremePuzzles[rnd.Next(0, extremePuzzles.Count)];
                    }

                    puzzleString = puzzle.SelectSingleNode("SudokuString").InnerText;
                    g_rating = puzzle.SelectSingleNode("DifficultyRating").InnerText;
                    g_ID = puzzle.SelectSingleNode("ID").InnerText;
                }
                else if (puzzleString.Contains('_'))//puzzle that's been started
                {
                    Sudoku_Title.Content = difficulty + " Sudoku Puzzle";
                    XmlNode startedPuzzles = sudokuPuzzles.SelectSingleNode("Started");
                    XmlNodeList difficultyNode = startedPuzzles.SelectSingleNode(difficulty).ChildNodes;
                    foreach (XmlNode puzzle in difficultyNode)
                    {
                        if (puzzle.SelectSingleNode("SudokuString").InnerText == puzzleString)
                        { 
                            g_ID = puzzle.SelectSingleNode("ID").InnerText;
                            g_rating = puzzle.SelectSingleNode("DifficultyRating").InnerText;
                            g_originalPuzzleString = puzzle.SelectSingleNode("OriginalSudokuString").InnerText;
                            Rating_lbl.Content = g_rating;
                            string time = puzzle.SelectSingleNode("TimeTaken").InnerText;
                            string[] splitTime = time.Split(':');
                            if (splitTime.Length < 3)
                            {
                                g_StartingTS = TimeSpan.Parse("0:"+time);
                            }
                            else
                            {
                                g_StartingTS = TimeSpan.Parse(time);
                            }
                            
                            break;
                        }
                    }
                }
                else//puzzles that have not been started, or have been completed and are being restarted
                {
                    Sudoku_Title.Content = difficulty + " Sudoku Puzzle";
                    XmlNode startedPuzzles;
                    if (puzzleString.Contains('.'))
                        startedPuzzles = sudokuPuzzles.SelectSingleNode("Completed");
                    else
                        startedPuzzles = sudokuPuzzles.SelectSingleNode("NotStarted");
                    XmlNodeList difficultyNode = startedPuzzles.SelectSingleNode(difficulty).ChildNodes;
                    foreach (XmlNode puzzle in difficultyNode)
                    {
                        if (puzzle.SelectSingleNode("SudokuString").InnerText == puzzleString)
                        {
                            g_rating = puzzle.SelectSingleNode("DifficultyRating").InnerText;
                            g_ID = puzzle.SelectSingleNode("ID").InnerText;
                            g_originalPuzzleString = puzzleString;
                            break;
                        }
                    }
                }
                ///Populate grid with puzzle from xml doc
                if (puzzleString.Contains('_'))//puzzle that's been started
                {
                    for (int i = 0, counter = 0; counter < puzzleString.Length; counter++)
                    {
                        if (puzzleString[counter] == '|')
                        {
                            ((TextBox)SudokuPuzzle.Children[i]).IsReadOnly = true;
                            ((TextBox)SudokuPuzzle.Children[i]).FontWeight = FontWeights.Bold;
                            counter++;
                        }
                        else if (puzzleString[counter] == '-')
                        {
                            ResizeCellFont(((TextBox)SudokuPuzzle.Children[i]), 16);
                            counter++;
                        }
                        if (counter == puzzleString.Length)
                        {
                            break;
                        }
                        if (puzzleString[counter] == '_')
                        {
                            i++;
                            counter++;
                            ((TextBox)SudokuPuzzle.Children[i]).Text = "";
                        }
                        if (puzzleString[counter] != '0')
                        {
                            ((TextBox)SudokuPuzzle.Children[i]).Text += puzzleString[counter].ToString();
                            if (((TextBox)SudokuPuzzle.Children[i]).Text.Length > 1)
                            {
                                ResizeCellFont(((TextBox)SudokuPuzzle.Children[i]), 16);
                            }
                            else
                            {
                                ResizeCellFont(((TextBox)SudokuPuzzle.Children[i]), 36);
                            }
                        }
                        else
                        {
                            ((TextBox)SudokuPuzzle.Children[i]).Text = "";
                        }
                    }
                    g_grid = gen.ConstructGrid();
                    gen.StringToGrid(g_grid, g_originalPuzzleString);
                }
                else//populating grid with puzzle that has not been started, or has been completed and is being restarted
                {
                    if (puzzleString.Contains('.'))
                    {
                        puzzleString = puzzleString.Remove(0, 1);
                    }
                    for (int i = 0; i < 81; i++)
                    {
                        if (puzzleString[i] != '0')
                        {
                            ((TextBox)SudokuPuzzle.Children[i]).Text = puzzleString[i].ToString();
                            ((TextBox)SudokuPuzzle.Children[i]).IsReadOnly = true;
                            ((TextBox)SudokuPuzzle.Children[i]).FontWeight = FontWeights.Bold;
                        }
                    }
                    g_originalPuzzleString = puzzleString;
                    g_grid = gen.ConstructGrid();
                    gen.StringToGrid(g_grid, g_originalPuzzleString);
                }
                
            }
            catch (Exception)//Generates puzzle of random g_difficulty
            {
                MessageBox.Show("No puzzles exist... A puzzle of random difficulty will be generated after clicking 'OK'. \r\nGeneration of puzzle may take some time.");
                g_grid = gen.Setter();
                PopulateGrid(g_grid);
                g_originalPuzzleString = gen.GridToString(g_grid);
                CreatePuzzles createPuzzles = new CreatePuzzles();
                g_rating = createPuzzles.GetDifficulty(g_grid, g_originalPuzzleString, new PuzzleSolverAdvDS()).ToString();
                //Clipboard.SetText(g_gen.GridToString(grid));
                Sudoku_Title.Content = g_grid.Difficulty + " Sudoku Puzzle";
                g_difficulty = g_grid.Difficulty;
                g_ID = "1";
            }
            Rating_lbl.Content = "Rating: " + g_rating;
            ID_lbl.Content = "ID: " + g_ID;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    g_grid.Cells[i][j].Candidates = new List<char>(9) { };
                }
            }
            StartTimer();
            
        }
        #endregion
        #region Functions/Methods
        /// <summary>
        /// Starts timing the user on solving the puzzle when the window opens or a pause is resumed
        /// </summary>
        private void StartTimer()
        {
            Timer = new Stopwatch();
            DT = new DispatcherTimer();
            DT.Tick += new EventHandler(DT_Tick);
            DT.Interval = new TimeSpan(0,0,0,0,100);
            Timer.Start();
            DT.Start();
        }
        /// <summary>
        /// Pauses the timer if pause or help is requested (or if puzzle is complete)
        /// </summary>
        /// <param name="type">1 means to Pause, 2 means to Pause and bring up Help, 3 is Pause onced puzzle is finished</param>
        private void PauseTimer(int type)
        {
            if (Timer.IsRunning)
            {
                
                Timer.Stop();
                DT.Stop();
                if (type == 1 || type == 2)
                {
                    Pause_btn.Content = "Resume (P)";
                    RTB_LargeText.Inlines.Clear();
                    RTB_HelpText.Inlines.Clear();
                    PauseBlock.Visibility = Visibility.Visible;
                    PauseBlock.Background = nightmode_chkbx.IsChecked == true ? darkerColour : pauseBlockBckGrd;
                    btn1.IsEnabled = false; btn2.IsEnabled = false; btn3.IsEnabled = false; btn4.IsEnabled = false; btn5.IsEnabled = false; btn6.IsEnabled = false; btn7.IsEnabled = false; btn8.IsEnabled = false; btn9.IsEnabled = false;
                    del_btn.IsEnabled = false; TogglePencil.IsEnabled = false; ToggleDrawing.IsEnabled = false;
                }
                if (type == 1)
                {
                    RTB_LargeText.Inlines.Add("PAUSED");
                    RTB_LargeText.FontSize = 64;
                    RTB_LargeText.Padding = new Thickness(120, 250, 120, 0);
                }
                else if (type == 2)
                {
                    RTB_LargeText.Padding = new Thickness(0);
                    RTB_LargeText.Inlines.Add("HELP");
                    RTB_HelpText.Inlines.Add("Objective:\r\n\r\nFill in all the empty cells with a single number from 1-9 till the entire grid is full.\r\n" +
                        "However, a number cannot lie in a group – row, column, or block (the 3x3 mini grids) – more than once.\r\n" +
                        "Therefore, each group must contain every number from 1 - 9.\r\n" +
                        "A pop up will occur to notify you if you have completed the puzzle.\r\n\r\n\r\n" +
                        "Tips on How to Play:\r\n\r\n" +
                        "The most basic strategy is to look at any empty cell within the puzzle and then look at all the groups that it belongs to. Then, make a note of all the numbers that can go into that cell.\r\n" +
                        "To make note of what numbers can exist within a cell, toggle the \"Notes\" button by either clicking it or by pressing ‘N’ on your keyboard.\r\n\r\n" +
                        "Another basic strategy is to look at a group and identify which numbers are left to be placed in that group.\r\n" +
                        "Then, for each number that doesn't exist in that group, look at each cell within that group and check whether that possible number can be placed in that cell by looking at the other groups that the cell belongs to.\r\n\r\n" +
                        "Another tool at your disposal is the the \"Drawing\" tool toggle button. " +
                        "Whilst the Drawing tool is toggled 'ON', you can highlight and draw over the puzzle. " +
                        "There are 4 colours that can be used to aid the discovery/removal of a number from a cell.\r\n\r\n" +
                        "An example of how this tool can be used is to indicate individual links between numbers or to designate a set of numbers to meet certain conditions (i.e. Chaining/Cycles).\r\n" +
                        "There is a \"Clear Drawings\" button if you wish to erase drawings.\r\n" +
                        "Finally, the Drawing tool can be toggled 'OFF' to remove the overlay of drawings. " +
                        "However, your drawings will be saved for when you toggle the Drawing tool back 'ON'.\r\n\r\n\r\n" +
                        "\"What does the rating mean?\"\r\n\r\n" +
                        "The rating of the puzzle is based off the weighting and frequency of simplest strategies required to solve the puzzle.\r\n\r\n" +
                        "Dark Mode is available above the 'Help Menu' button, for those who prefer a darker colour scheme.\r\n\r\n\r\n" +
                        "Help Whilst Playing:\r\n\r\n" +
                        "If you enter a number into a cell that already exists in a corresponding row/column/block, the cell will be highlighted red to inform you of your mistake.\r\n\r\n" +
                        "The \"Update Candidates\" button is a helper tool that uses the current state of the puzzle to display, in each empty cell, the possible digits that can go into a cell - based off of the numbers missing from the row/column/block that the cell shares.\r\n\r\n" +
                        "The \"Show All Candidates\" button does the same as the Update Candidates button, however it ignores any updates to the notes in the cells that you have changed.\r\n\r\n" +
                        "The \"Clear Candidates\" button removes notes from all empty cells.\r\n\r\n" +
                        "The \"Show Clue\" button is another helper tool that is currently unimplemented.\r\n\r\n");
                    RTB_LargeText.FontSize = 42;
                    RTB_HelpText.FontSize = 18;
                }
                else if (type == 3)
                {
                    btn1.IsEnabled = false; btn2.IsEnabled = false; btn3.IsEnabled = false; btn4.IsEnabled = false; btn5.IsEnabled = false; btn6.IsEnabled = false; btn7.IsEnabled = false; btn8.IsEnabled = false; btn9.IsEnabled = false;
                    del_btn.IsEnabled = false; TogglePencil.IsEnabled = false; ToggleDrawing.IsEnabled = false; Pause_btn.IsEnabled = false; Help_btn.IsEnabled = false; nightmode_chkbx.IsEnabled = false;
                    SudokuPuzzle.IsEnabled = false; Update_Cands_btn.IsEnabled = false; Reset_Cands_btn.IsEnabled = false; Clear_Cands_btn.IsEnabled = false;
                    for (int i = 0; i < 81; i++)
                    {
                        ((TextBox)SudokuPuzzle.Children[i]).IsReadOnly = true;
                    }
                    RTB_LargeText.Inlines.Clear();
                    RTB_HelpText.Inlines.Clear();
                    PauseBlock.Visibility = Visibility.Visible;
                    PauseBlock.Background = nightmode_chkbx.IsChecked == true
                        ? new SolidColorBrush(Color.FromArgb(150, 45, 45, 45))
                        : new SolidColorBrush(Color.FromArgb(150, 238, 241, 255));
                    RTB_LargeText.Inlines.Add("Congratulations! Sudoku Complete!");
                    RTB_LargeText.FontSize = 48;
                    RTB_LargeText.Padding = new Thickness(120,250,120,0);
                    SavePuzzle(true);
                }
            }
            else
            {
                Pause_btn.Content = "Pause (P)";
                Timer.Start();
                DT.Start();
                PauseBlock.Visibility = Visibility.Hidden;
                btn1.IsEnabled = true; btn2.IsEnabled = true; btn3.IsEnabled = true; btn4.IsEnabled = true; btn5.IsEnabled = true; btn6.IsEnabled = true; btn7.IsEnabled = true; btn8.IsEnabled = true; btn9.IsEnabled = true;
                del_btn.IsEnabled = true; TogglePencil.IsEnabled = true; ToggleDrawing.IsEnabled = true;
            }
        }
        /// <summary>
        /// Pencil marking is a form of making notes on the puzzle grid.
        /// When activated, via either button click or 'N' key down, the numbers inputted into the cell will be smaller.
        /// When deactived, via the same methods, the numbers inputted into cells are normal size (36pt)
        /// </summary>
        /// <param name="source">the source is either button click or 'N' key down</param>
        private void PencilMarkONOFF(char source)
        {
            if (source == 'b')
            {
                if (TogglePencil.IsChecked == true)
                {
                    TogglePencil.Content = "Notes are ON (N)";
                    g_pencilMarker = true;
                    if (g_selectedCell != null)
                    {
                        g_selectedCell.Focus();
                    }
                }
                else if (TogglePencil.IsChecked == false)
                {
                    TogglePencil.Content = "Notes are OFF (N)";
                    g_pencilMarker = false;
                    if (g_selectedCell != null)
                    {
                        g_selectedCell.Focus();
                    }
                }
            }
            else if (source == 'k')
            {
                if (TogglePencil.IsChecked == false)
                {
                    TogglePencil.IsChecked = true;
                    TogglePencil.Content = "Notes are ON (N)";
                    g_pencilMarker = true;
                    if (g_selectedCell != null)
                    {
                        g_selectedCell.Focus();
                    }
                }
                else if (TogglePencil.IsChecked == true)
                {
                    TogglePencil.IsChecked = false;
                    TogglePencil.Content = "Notes are OFF (N)";
                    g_pencilMarker = false;
                    if (g_selectedCell != null)
                    {
                        g_selectedCell.Focus();
                    }
                }
            }
        }
        /// <summary>
        /// Drawing is a form of making notes and establishing strategies on the puzzle grid.
        /// When activated, via either button click or 'D' key down, the ink canvas will be displayed.
        /// When deactived, via the same methods, the ink canvas will remain in the background of the grid, but will still be visisble
        /// </summary>
        /// <param name="source">the source is either button click or 'N' key down</param>
        private void DrawingONOFF(char source)
        {
            if (source == 'b')
            {
                if (ToggleDrawing.IsChecked == true)
                {
                    ToggleDrawing.Content = "Drawing is ON (D)";
                    ColourCanvas.Visibility = Visibility.Visible;
                    DrawSelection_grid.Visibility = Visibility.Visible;
                    ColourCanvas.Focusable = true;
                    ColourCanvas.IsEnabled = true;
                    Panel.SetZIndex(ColourCanvas, 1);
                    Panel.SetZIndex(SudokuPuzzle, 0);
                }
                else if (ToggleDrawing.IsChecked == false)
                {
                    ToggleDrawing.Content = "Drawing is OFF (D)";
                    DrawSelection_grid.Visibility = Visibility.Hidden;
                    ColourCanvas.Focusable = false;
                    ColourCanvas.IsEnabled = false;
                    Panel.SetZIndex(ColourCanvas, 0);
                    Panel.SetZIndex(SudokuPuzzle, 1);
                    if (g_selectedCell != null)
                    {
                        g_selectedCell.Focus();
                    }
                }
            }
            else if (source == 'k')
            {
                if (ToggleDrawing.IsChecked == false)
                {
                    ToggleDrawing.IsChecked = true;
                    ToggleDrawing.Content = "Drawing is ON (D)";
                    ColourCanvas.Visibility = Visibility.Visible;
                    DrawSelection_grid.Visibility = Visibility.Visible;
                    ColourCanvas.Focusable = true;
                    ColourCanvas.IsEnabled = true;
                    Panel.SetZIndex(ColourCanvas, 1);
                    Panel.SetZIndex(SudokuPuzzle, 0);
                }
                else if (ToggleDrawing.IsChecked == true)
                {
                    ToggleDrawing.IsChecked = false;
                    ToggleDrawing.Content = "Drawing is OFF (D)";
                    DrawSelection_grid.Visibility = Visibility.Hidden;
                    ColourCanvas.Focusable = false;
                    ColourCanvas.IsEnabled = false;
                    Panel.SetZIndex(ColourCanvas, 0);
                    Panel.SetZIndex(SudokuPuzzle, 1);
                    if (g_selectedCell != null)
                    {
                        g_selectedCell.Focus();
                    }
                }
            }
        }
        /// <summary>
        /// This method populates the Uniform grid and its textboxes with all the given values from 'grid'.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="m_txtBxList"></param>
        public void PopulateGrid(SudokuGrid grid)
        {
            int x = 0;//row number
            int y = 0;//column number
            for (int i = 0; i < SudokuPuzzle.Children.Count; i++)
            {
                if (grid.Cells[x][y].Num != '0') //0's are placeholders for when there is no value, so any 0's are turned into textboxes containing the candidate values.
                {
                    ResizeCellFont(((TextBox)SudokuPuzzle.Children[i]), 36);
                    ((TextBox)SudokuPuzzle.Children[i]).Text = grid.Cells[x][y].Num.ToString();
                    if (grid.Cells[x][y].ReadOnly == true)//The readonly property ensures that the default given values of the sudoku puzzle remain readonly.
                    {
                        ((TextBox)SudokuPuzzle.Children[i]).FontWeight = FontWeights.Bold;
                        ((TextBox)SudokuPuzzle.Children[i]).IsReadOnly = true;
                    }
                }

                y++;
                if (y == 9)//row needs to increment and column needs to reset to 0 once it reaches the end of the row
                {
                    y = 0;
                    x++;
                }
            }
        }
        /// <summary>
        /// This function saves the original version of the current puzzle and the current state of the puzzle, if it is incomplete.
        /// </summary>
        /// <param name="completed">If puzzles is incomplete, all progress is saved including candidate values</param>
        private void SavePuzzle(bool completed)
        {
            ///Save Puzzle to Started/Completed
            XDocument doc;
            string filename = @"Puzzles/SudokuPuzzles.xml", candidatesInString = "";
            
            doc = File.Exists(filename)
                ? XDocument.Load(filename)
                : new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("Sudoku Puzzle Storage For SudokuSolverSetter App"),
                    new XElement("SudokuPuzzles",
                        new XElement("NotStarted",
                            new XElement("Beginner"),
                            new XElement("Moderate"),
                            new XElement("Advanced"),
                            new XElement("Extreme")
                            ),
                        new XElement("Started",
                            new XElement("Beginner"),
                            new XElement("Moderate"),
                            new XElement("Advanced"),
                            new XElement("Extreme")
                            ),
                        new XElement("Completed",
                            new XElement("Beginner"),
                            new XElement("Moderate"),
                            new XElement("Advanced"),
                            new XElement("Extreme")
                            )
                        )
                    );
            doc.Save(filename);
            if (!completed)
            {
                for (int i = 0; i < 81; i++)
                {
                    if (((TextBox)SudokuPuzzle.Children[i]).Text.Length > 1 || ((TextBox)SudokuPuzzle.Children[i]).FontSize == 16)
                    {
                        candidatesInString += ((TextBox)SudokuPuzzle.Children[i]).Text + "-";
                    }
                    else if (((TextBox)SudokuPuzzle.Children[i]).Text == "")
                    {
                        candidatesInString += "0";
                    }
                    else
                    {
                        candidatesInString += ((TextBox)SudokuPuzzle.Children[i]).Text;
                        if (((TextBox)SudokuPuzzle.Children[i]).IsReadOnly)
                        {
                            candidatesInString += "|";
                        }
                    }
                    if (i != 80)
                    {
                        candidatesInString += "_";
                    }
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filename);
                XmlNodeList sudokuPuzzles = xmlDoc.DocumentElement.SelectSingleNode("/SudokuPuzzles/Started").ChildNodes;
                bool puzzleExists = false;
                foreach (XmlNode difficulty in sudokuPuzzles)
                {
                    XmlNodeList puzzles = difficulty.ChildNodes;
                    foreach (XmlNode puzzle in puzzles)
                    {
                        if (puzzle.SelectSingleNode("ID").InnerText == g_ID)
                        {
                            string s = puzzle.SelectSingleNode("DifficultyRating").InnerText;
                            doc.Element("SudokuPuzzles").Element("Started").Element(g_difficulty).Add(
                            new XElement("puzzle",
                               new XElement("ID", g_ID),
                               new XElement("DifficultyRating", s),
                               new XElement("SudokuString", candidatesInString),
                               new XElement("OriginalSudokuString", g_originalPuzzleString),
                               new XElement("TimeTaken", g_currentTime),
                               new XElement("Date", DateTime.Today.Date.ToShortDateString()
                                    )
                                )
                            );
                            puzzleExists = true;
                            var childNode = doc.Element("SudokuPuzzles").Element("Started").Element(g_difficulty)
                            .Elements("puzzle")
                            .First(n => n.Element("ID").Value == g_ID);
                            childNode.Remove();
                            break;
                        }                        
                    }
                    if (puzzleExists)
                    {
                        break;
                    }
                }
                if (!puzzleExists)
                {
                    doc.Element("SudokuPuzzles").Element("Started").Element(g_difficulty).Add(
                           new XElement("puzzle",
                               new XElement("ID", g_ID),
                               new XElement("DifficultyRating", g_rating),
                               new XElement("SudokuString", candidatesInString),
                               new XElement("OriginalSudokuString", g_originalPuzzleString),
                               new XElement("TimeTaken", g_currentTime),
                               new XElement("Date", DateTime.Today.Date.ToShortDateString())
                               )
                           );
                }
            }
            else
            {
                doc.Element("SudokuPuzzles").Element("Completed").Element(g_difficulty).Add(
                           new XElement("puzzle",
                               new XElement("ID", g_ID),
                               new XElement("DifficultyRating", g_rating),
                               new XElement("SudokuString", "."+g_originalPuzzleString),
                               new XElement("TimeTaken", g_currentTime),
                               new XElement("Date", DateTime.Today.Date.ToShortDateString())
                               )
                           );
                try
                {
                    var childNode = doc.Element("SudokuPuzzles").Element("Started").Element(g_difficulty)
                            .Elements("puzzle")
                            .First(n => n.Element("ID").Value == g_ID);
                    childNode.Remove();
                }
                catch (Exception)
                {

                }
                
            }

            doc.Save(filename);
            MessageBox.Show("Successfully saved puzzle.");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        private void DarkModeChange(Grid grid)
        {
            for (int i = 0; i < grid.Children.Count; i++)
            {
                switch (grid.Children[i].GetType().Name)
                {
                    case "Label":
                        ((Label)grid.Children[i]).Foreground = darkTextColour;
                        break;
                    case "TextBox":
                        ((TextBox)grid.Children[i]).Foreground = darkTextColour;
                        break;
                    case "CheckBox":
                        ((CheckBox)grid.Children[i]).Foreground = darkTextColour;
                        break;
                    case "Button":
                        ((Button)grid.Children[i]).Foreground = darkTextColour; ((Button)grid.Children[i]).Background = darkButtonColour;
                        break;
                    case "ToggleButton":
                        ((ToggleButton)grid.Children[i]).Foreground = darkTextColour; ((ToggleButton)grid.Children[i]).Background = darkButtonColour;
                        break;
                    case "Rectangle":
                        ((Rectangle)grid.Children[i]).Stroke = darkTextColour;
                        break;
                    case "Grid":
                        DarkModeChange((Grid)grid.Children[i]);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        private void LightModeChange(Grid grid)
        {
            for (int i = 0; i < grid.Children.Count; i++)
            {
                switch (grid.Children[i].GetType().Name)
                {
                    case "Label":
                        ((Label)grid.Children[i]).Foreground = Brushes.Black;
                        break;
                    case "TextBox":
                        ((TextBox)grid.Children[i]).Foreground = Brushes.Black;
                        break;
                    case "CheckBox":
                        ((CheckBox)grid.Children[i]).Foreground = Brushes.Black;
                        break;
                    case "Button":
                        ((Button)grid.Children[i]).Foreground = Brushes.Black; ((Button)grid.Children[i]).Background = buttonColour;
                        break;
                    case "ToggleButton":
                        ((ToggleButton)grid.Children[i]).Foreground = Brushes.Black; ((ToggleButton)grid.Children[i]).Background = buttonColour;
                        break;
                    case "Rectangle":
                        ((Rectangle)grid.Children[i]).Stroke = Brushes.Black;
                        break;
                    case "RichTextBox":
                        ((RichTextBox)grid.Children[i]).Background = backgroundCol;
                        ((RichTextBox)grid.Children[i]).Foreground = Brushes.Black;
                        break;
                    case "Grid":
                        LightModeChange((Grid)grid.Children[i]);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// checks if recent input is valid within rules of placement
        /// </summary>
        private bool ValidInput()
        {
            if (g_selectedCell.Text == "" || g_selectedCell.Text.Length > 1 || g_selectedCell.FontSize == 16)
                return true;

            if (g_Gen.ValidateInput(SudokuPuzzle, g_grid, g_selectedCell))
            {
                if (nightmode_chkbx.IsChecked == true)
                    RecolourACell(true, g_selectedCell);
                else
                    RecolourACell(false, g_selectedCell);
                return true;
            }
            else
            {
                g_selectedCell.Background = Brushes.Red;
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="darkMode"></param>
        private void RecolourACell(bool darkMode, TextBox cell)
        {
            int i = SudokuPuzzle.Children.IndexOf(cell);
            cell.Foreground = darkMode ? darkTextColour : Brushes.Black;
            cell.BorderBrush = darkMode ? darkTextColour : Brushes.Black;
            cell.SelectionBrush = darkMode ? darkTextColour : Brushes.Black;
            if (i == 3 || i == 4 || i == 5 || i == 12 || i == 13 || i == 14
                || i == 21 || i == 22 || i == 23 || i == 27 || i == 28 || i == 29
                || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38
                || i == 42 || i == 43 || i == 44 || i == 45 || i == 46 || i == 47
                || i == 51 || i == 52 || i == 53 || i == 57 || i == 58 || i == 59
                || i == 66 || i == 67 || i == 68 || i == 75 || i == 76 || i == 77)
            {
                cell.Background = darkMode ? altDarkCellColour : altCellColour;
            }
            else
            {
                cell.Background = darkMode ? darkColour : cellColour;
            }
            if (g_selectedCell == cell)
            {
                cell.Background = darkMode ? darkFocusCell : focusCell;
            }
        }
        private void RecolourAllCells(bool darkMode)
        {
            for (int i = 0; i < SudokuPuzzle.Children.Count; i++)
            {
                ((TextBox)SudokuPuzzle.Children[i]).Foreground = darkMode ? darkTextColour : Brushes.Black;
                ((TextBox)SudokuPuzzle.Children[i]).BorderBrush = darkMode ? darkTextColour : Brushes.Black;
                ((TextBox)SudokuPuzzle.Children[i]).SelectionBrush = darkMode ? darkTextColour : Brushes.Black;

                if (i == 3 || i == 4 || i == 5 || i == 12 || i == 13 || i == 14
                    || i == 21 || i == 22 || i == 23 || i == 27 || i == 28 || i == 29
                    || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38
                    || i == 42 || i == 43 || i == 44 || i == 45 || i == 46 || i == 47
                    || i == 51 || i == 52 || i == 53 || i == 57 || i == 58 || i == 59
                    || i == 66 || i == 67 || i == 68 || i == 75 || i == 76 || i == 77)
                {
                    ((TextBox)SudokuPuzzle.Children[i]).Background = darkMode ? altDarkCellColour : altCellColour;
                }
                else
                {
                    ((TextBox)SudokuPuzzle.Children[i]).Background = darkMode ? darkColour : cellColour;
                }
            }
        }
        private void ResizeCellFont(TextBox cell, int fontsize)
        {
            cell.FontSize = fontsize;
            cell.CaretBrush = nightmode_chkbx.IsChecked == true ? (fontsize == 16 ? Brushes.White : Brushes.Transparent) : (fontsize == 16 ? Brushes.Black : Brushes.Transparent);
        }
        private void UpdateCandidates()
        {
            try
            {
                SudokuGrid grid = new SudokuGrid
                {
                    Cells = new Cell[9][]
                };
                int cellNum = 0;

                //This transforms the text in the boxes to a useable grid object. Resource heavy - alternative method may be developed in improvements
                for (int r = 0; r < grid.Cells.Length; r++)
                {
                    grid.Cells[r] = new Cell[9];
                    for (int c = 0; c < grid.Cells[r].Length; c++)
                    {
                        if (((TextBox)SudokuPuzzle.Children[cellNum]).Text == "" || ((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length > 1 || ((TextBox)SudokuPuzzle.Children[cellNum]).FontSize == 16 || ((TextBox)SudokuPuzzle.Children[cellNum]).Background == Brushes.Red)
                        {
                            List<char> candis = new List<char> { };
                            if (((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length > 1)
                            {

                                candis.AddRange(((TextBox)SudokuPuzzle.Children[cellNum]).Text.ToCharArray());
                            }
                            else
                            {
                                candis = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                            }
                            grid.Cells[r][c] = new Cell()
                            {
                                Candidates = candis,
                                Num = '0',
                                BlockLoc = (r / 3) * 3 + (c / 3) + 1,
                                XLocation = r,
                                YLocation = c,
                                ReadOnly = false
                            };
                        }
                        else
                        {
                            grid.Cells[r][c] = new Cell()
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
                g_Gen.AddNeighbours(grid);
                PuzzleSolverAdvDS solver = new PuzzleSolverAdvDS();
                if (solver.CleanCandidateLists(grid))
                {
                    int index = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (((TextBox)SudokuPuzzle.Children[index]).Text.Length > 1)
                            {
                                string candis = "";
                                for (int k = 0; k < grid.Cells[i][j].Candidates.Count; k++)
                                {
                                    candis += grid.Cells[i][j].Candidates[k];
                                }
                                ((TextBox)SudokuPuzzle.Children[index]).Text = candis;
                                g_grid.Cells[i][j].Candidates = grid.Cells[i][j].Candidates;
                                ResizeCellFont(((TextBox)SudokuPuzzle.Children[index]), 16);
                            }
                            index++;
                        }
                    }
                }
                if (g_selectedCell != null)
                {
                    g_selectedCell.Focus();
                }
            }
            catch (Exception)
            {

            }
        }
        private void ResetCandidates()
        {
            try
            {
                SudokuGrid grid = new SudokuGrid
                {
                    Cells = new Cell[9][]
                };
                int cellNum = 0;

                //This transforms the text in the boxes to a useable grid object. Resource heavy - alternative method may be developed in improvements
                for (int r = 0; r < grid.Cells.Length; r++)
                {
                    grid.Cells[r] = new Cell[9];
                    for (int c = 0; c < grid.Cells[r].Length; c++)
                    {
                        if (((TextBox)SudokuPuzzle.Children[cellNum]).Text == "" || ((TextBox)SudokuPuzzle.Children[cellNum]).Text.Length > 1 || ((TextBox)SudokuPuzzle.Children[cellNum]).FontSize == 16 || ((TextBox)SudokuPuzzle.Children[cellNum]).Background == Brushes.Red)
                        {
                            grid.Cells[r][c] = new Cell()
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
                            grid.Cells[r][c] = new Cell()
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
                g_Gen.AddNeighbours(grid);
                PuzzleSolverAdvDS solver = new PuzzleSolverAdvDS();
                if (solver.CleanCandidateLists(grid))
                {
                    int index = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (((TextBox)SudokuPuzzle.Children[index]).Text == "" || ((TextBox)SudokuPuzzle.Children[index]).Text.Length > 1)
                            {
                                string candis = "";
                                for (int k = 0; k < grid.Cells[i][j].Candidates.Count; k++)
                                {
                                    candis += grid.Cells[i][j].Candidates[k];
                                }
                                ((TextBox)SudokuPuzzle.Children[index]).Text = candis;
                                g_grid.Cells[i][j].Candidates = grid.Cells[i][j].Candidates;
                                ResizeCellFont(((TextBox)SudokuPuzzle.Children[index]), 16);
                            }
                            index++;
                        }
                    }
                }
                if (g_selectedCell != null)
                {
                    g_selectedCell.Focus();
                }
            }
            catch (Exception)
            {

            }
        }
        private void ClearCandidates()
        {
            for (int i = 0; i < 81; i++)
            {
                if (((TextBox)SudokuPuzzle.Children[i]).FontSize == 16)
                {
                    ((TextBox)SudokuPuzzle.Children[i]).Text = "";
                    ResizeCellFont(((TextBox)SudokuPuzzle.Children[i]), 36);
                }
            }
            if (g_selectedCell != null)
            {
                g_selectedCell.Focus();
            }
        }
        #endregion
        #region Event Handlers
        /// <summary>
        /// Event handler that updates the timer at the top of the window and display a congratulations message box if the puzzle is checked and is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DT_Tick(object sender, EventArgs e)
        {
            if (Timer.IsRunning)
            {
                TimeSpan ts = Timer.Elapsed.Add(g_StartingTS);
                g_currentTime = "";
                if (ts.Hours < 1)
                    g_currentTime += "0:";
                else
                    g_currentTime += ts.Hours + ":";

                if (ts.Minutes < 1)
                    g_currentTime += "00:";
                else if (ts.Minutes < 10)
                    g_currentTime += "0" + ts.Minutes + ":";
                else
                    g_currentTime += ts.Minutes + ":";

                if (ts.Seconds < 1)
                    g_currentTime += "00";
                else if (ts.Seconds < 10)
                    g_currentTime += "0" + ts.Seconds;
                else
                    g_currentTime += ts.Seconds;

                timer_txtbx.Text = g_currentTime;
            }
        }
        /// <summary>
        /// Event handler to handle pausing the timer, both the Help button and the Pause button use this event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pause_btn_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "Pause_btn")
            {
                PauseTimer(1);
            }
            else//Help button click
            {
                PauseTimer(2);
            }
        }
        /// <summary>
        /// If a number from the number pad on the right side of the window is clicked, the number is inserted into the selected cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Num_Button_Click(object sender, RoutedEventArgs e)
        {
            if (g_selectedCell != null)
            {
                if (!g_selectedCell.IsReadOnly)
                {
                    if (g_pencilMarker)
                    {
                        ResizeCellFont(g_selectedCell,16);
                        if (!g_selectedCell.Text.Contains(((Button)sender).Content.ToString()))
                        {
                            g_selectedCell.Text += ((Button)sender).Content.ToString();//Previously Selected Cell is given the number of the button
                        }
                    }
                    else
                    {
                        ResizeCellFont(g_selectedCell, 36);
                        g_selectedCell.Text = ((Button)sender).Content.ToString();//Previously Selected Cell is given the number of the button

                    }
                    g_selectedCell.Focus();
                }
                
            }
        } 
        /// <summary>
        /// Handles when a cell in the grid is selected, changing the background colour of the selected cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_GotFocus(object sender, RoutedEventArgs e)
        {
            if (g_selectedCell != null)//sets previously focused cell to default colour
            {
                if (g_selectedCell.Text == "" || ValidInput())
                {
                    bool darkMode = nightmode_chkbx.IsChecked == true ? true : false;
                    int i = SudokuPuzzle.Children.IndexOf(g_selectedCell);
                    if (i == 3 || i == 4 || i == 5 || i == 12 || i == 13 || i == 14
                        || i == 21 || i == 22 || i == 23 || i == 27 || i == 28 || i == 29
                        || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38
                        || i == 42 || i == 43 || i == 44 || i == 45 || i == 46 || i == 47
                        || i == 51 || i == 52 || i == 53 || i == 57 || i == 58 || i == 59
                        || i == 66 || i == 67 || i == 68 || i == 75 || i == 76 || i == 77)
                    {
                        g_selectedCell.Background = darkMode ? altDarkCellColour : altCellColour;
                    }
                    else
                    {
                        g_selectedCell.Background = darkMode ? darkColour : cellColour;
                    }
                    
                }
            }
            g_selectedCell = (TextBox)sender;
            if (((TextBox)sender).Background != Brushes.Red)
                ((TextBox)sender).Background = nightmode_chkbx.IsChecked == false ? focusCell : darkFocusCell;//sets the current focused cell to a more prominent colour
            g_selectedCell.CaretBrush = nightmode_chkbx.IsChecked == true ? (g_selectedCell.FontSize == 16 ? Brushes.White : Brushes.Transparent) : (g_selectedCell.FontSize == 16 ? Brushes.Black : Brushes.Transparent);
        }
        /// <summary>
        /// Asks the user if they are sure they want to quit, and ask if they want to save the progress. Closes the window properly if the 'X' is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!g_solved)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save your progress on this puzzle before you quit?", "Confirm", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    ///SavePuzzle
                    SavePuzzle(false);
                }
                else if (result == MessageBoxResult.No)
                {
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        private void ChangeDA_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!penType)
            {
                penType = true;
                changeDA_Button.Content = "Change To Highlighter";
                ColourCanvas.DefaultDrawingAttributes = g_penDA;
            }
            else
            {
                penType = false;
                changeDA_Button.Content = "Change To Pen";
                ColourCanvas.DefaultDrawingAttributes = g_highlighterDA;
            }
            
        }
        private void ClearDrawings_Button_Click(object sender, RoutedEventArgs e)
        {
            ColourCanvas.Strokes.Clear();
        }
        private void Update_Cands_btn_Click(object sender, RoutedEventArgs e)
        {
            UpdateCandidates();
        }
        private void Reset_Cands_btn_Click(object sender, RoutedEventArgs e)
        {
            ResetCandidates();
        }
        private void Clear_Cands_btn_Click(object sender, RoutedEventArgs e)
        {
            ClearCandidates();
        }
        private void Helper_btn_Click(object sender, RoutedEventArgs e)
        {
            if (g_selectedCell != null)
            {
                g_selectedCell.Focus();
            }
        }
        private void TogglePencil_Click(object sender, RoutedEventArgs e)
        {
            PencilMarkONOFF('b');
            if (g_selectedCell != null)
            {
                g_selectedCell.Focus();
            }
        }
        private void Radio_Colour_Checked(object sender, RoutedEventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "radio_Red":
                    g_penDA.Color = Colors.Red;
                    g_highlighterDA.Color = Colors.Red;
                    break;
                case "radio_Blue":
                    g_penDA.Color = Colors.Blue;
                    g_highlighterDA.Color = Colors.Blue;
                    break;
                case "radio_Green":
                    g_penDA.Color = Colors.SpringGreen;
                    g_highlighterDA.Color = Colors.SpringGreen;
                    break;
                case "radio_Orange":
                    g_penDA.Color = Colors.Gold;
                    g_highlighterDA.Color = Colors.Gold;
                    break;
                default:
                    break;
            }
                
        }
        /// <summary>
        /// Changes the colour of the selected cell back to normal when the cell is no longer in focus
        /// Also sorts the notes into ascending order for clarity and consistency in notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_LostFocus(object sender, RoutedEventArgs e)
        {
            if (g_selectedCell.Text.Length > 1)
            {
                List<int> sortNotes = new List<int>(g_selectedCell.Text.Length);
                for (int i = 0; i < g_selectedCell.Text.Length; i++)
                {
                    sortNotes.Add(int.Parse(g_selectedCell.Text[i].ToString()));
                }
                sortNotes.Sort();
                g_selectedCell.Text = string.Join("", sortNotes);
            }
            
        }
        private void Cell_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "P" || e.Text == "p")
            {
                if (!g_solved)
                    PauseTimer(1);
                e.Handled = true;
                return;
            }
            else if (e.Text == "N" || e.Text == "n")
            {
                PencilMarkONOFF('k');
                e.Handled = true;
                return;
            }
            else if (e.Text == "D" || e.Text == "d")
            {
                DrawingONOFF('k');
                e.Handled = true;
                return;
            }
            else if (e.Text == "H" || e.Text == "h")
            {
                if (!g_solved)
                    PauseTimer(2);
                e.Handled = true;
                return;
            }
            else if (e.Text == "A" || e.Text == "a")
            {
                ResetCandidates();
                e.Handled = true;
                return;
            }
            else if (e.Text == "U" || e.Text == "u")
            {
                UpdateCandidates();
                e.Handled = true;
                return;
            }
            else if (e.Text == "C" || e.Text == "c")
            {
                ClearCandidates();
                e.Handled = true;
                return;
            }
            if (!((TextBox)sender).IsReadOnly)
            {
                Regex rgx = new Regex("[1-9]");
                if (!rgx.IsMatch(e.Text) || ((TextBox)sender).Text.Contains(e.Text) && TogglePencil.IsChecked == true)
                {
                    e.Handled = true;
                }
                else if (TogglePencil.IsChecked == false || TogglePencil.IsChecked == null)
                {
                    ((TextBox)sender).Text = e.Text;
                    ((TextBox)sender).FontSize = 36;
                    e.Handled = true;
                }
                else if (TogglePencil.IsChecked == true)
                {
                    ((TextBox)sender).FontSize = 16;
                }
            }
        }
        /// <summary>
        /// Handles changes in cells
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (g_selectedCell == null)
            {
                return;
            }
            ((TextBox)sender).Text = ((TextBox)sender).Text.Trim();
            if (((TextBox)sender).Text == "")
            {
                if (!g_pencilMarker && ((TextBox)sender).FontSize == 36)
                {
                    double index = SudokuPuzzle.Children.IndexOf((TextBox)sender);
                    int i = (int)index / 9;
                    int j = (int)index % 9;
                    g_grid.Cells[i][j].Num = '0';
                    if (g_grid.Cells[i][j].Candidates.Count > 1)
                        ((TextBox)sender).Text = string.Join("", g_grid.Cells[i][j].Candidates.ToArray());
                    ResizeCellFont((TextBox)sender, 16);
                }
                if (g_selectedCell == (TextBox)sender)
                    ((TextBox)sender).Background = nightmode_chkbx.IsChecked == false ? focusCell : darkFocusCell;
                else
                {
                    if (nightmode_chkbx.IsChecked == true && ((TextBox)sender).Background == Brushes.Red)
                        RecolourACell(true, (TextBox)sender);
                    else if (nightmode_chkbx.IsChecked == false && ((TextBox)sender).Background == Brushes.Red)
                        RecolourACell(false, (TextBox)sender);
                }
                
            }
            if (!g_pencilMarker && ((TextBox)sender).Text.Length == 1)
            {
                if (((TextBox)sender).FontSize != 36) 
                    ResizeCellFont((TextBox)sender, 36);
                ValidInput();
                double index = SudokuPuzzle.Children.IndexOf((TextBox)sender);
                int i = (int)index / 9;
                int j = (int)index % 9;
                g_grid.Cells[i][j].Num = ((TextBox)sender).Text[0];
            }
            else if (g_pencilMarker)
            {
                double index = SudokuPuzzle.Children.IndexOf((TextBox)sender);
                int i = (int)index / 9;
                int j = (int)index % 9;
                g_grid.Cells[i][j].Num = '0';
                g_grid.Cells[i][j].Candidates.Clear();
                g_grid.Cells[i][j].Candidates.AddRange(((TextBox)sender).Text.ToCharArray());
                ResizeCellFont((TextBox)sender, 16);
                if (nightmode_chkbx.IsChecked == true && ((TextBox)sender).Background == Brushes.Red)
                    RecolourACell(true, (TextBox)sender);
                else if (nightmode_chkbx.IsChecked == false && ((TextBox)sender).Background == Brushes.Red)
                    RecolourACell(false, (TextBox)sender);
            }
            if (((TextBox)sender).Text.Length < 2 && !g_pencilMarker)
            {
                //Check if solved
                char[][] basicGrid = new char[9][];
                for (int i = 0; i < basicGrid.Length; i++)
                {
                    basicGrid[i] = new char[9];
                }
                int c = 0;
                int r = 0;
                for (int i = 0; i < SudokuPuzzle.Children.Count; i++, c++)//populate basic grid so it can be checked.
                {
                    if (c == 9)
                    {
                        c = 0;
                        r++;
                    }
                    if (((TextBox)SudokuPuzzle.Children[i]).Text == "" || ((TextBox)SudokuPuzzle.Children[i]).Text.Length > 1 || ((TextBox)SudokuPuzzle.Children[i]).FontSize == 16)
                    {
                        basicGrid[r][c] = '0';
                    }
                    else
                    {
                        basicGrid[r][c] = ((TextBox)SudokuPuzzle.Children[i]).Text[0];
                    }
                }
                if (g_Gen.CheckIfSolved_array(basicGrid))
                {
                    g_solved = true;
                    Back_btn.Content = "Quit";
                    PauseTimer(3);
                }
            }
            g_selectedCell.Focus();
            g_selectedCell.CaretIndex = g_selectedCell.Text.Length;
        }
        private void ToggleDrawing_Click(object sender, RoutedEventArgs e)
        {
            DrawingONOFF('b');
            if (g_selectedCell != null)
            {
                g_selectedCell.Focus();
            }
        }
        private void Preview_Drawing_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.P:
                    if (!g_solved)
                        PauseTimer(1);
                    e.Handled = true;
                    break;
                case Key.D:
                    DrawingONOFF('k');
                    e.Handled = true;
                    break;
                case Key.H:
                    if (!g_solved)
                        PauseTimer(2);
                    e.Handled = true;
                    break;
                case Key.A:
                    ResetCandidates();
                    e.Handled = true;
                    break;
                case Key.U:
                    UpdateCandidates();
                    e.Handled = true;
                    break;
                case Key.C:
                    ClearCandidates();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }
        private void OnCancelCommand(object sender, DataObjectEventArgs e)
        {
            e.CancelCommand();
        }
        /// <summary>
        /// Handles keydown events so that the user can: navigate through the grid with arrow keys, delete number(s) from a cell, 
        /// pause/unpause the puzzle, and activate/deactive notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (SudokuPuzzle.Children.IndexOf((TextBox)sender) - 1 >= 0)
                    {
                        SudokuPuzzle.Children[SudokuPuzzle.Children.IndexOf((TextBox)sender) - 1].Focus();
                    }
                    e.Handled = true;
                    break;
                case Key.Up:
                    if (SudokuPuzzle.Children.IndexOf((TextBox)sender) - 9 >= 0)
                    {
                        SudokuPuzzle.Children[SudokuPuzzle.Children.IndexOf(((TextBox)sender)) - 9].Focus();
                    }
                    e.Handled = true;
                    break;
                case Key.Right:
                    if (SudokuPuzzle.Children.IndexOf((TextBox)sender) + 1 <= 80)
                    {
                        SudokuPuzzle.Children[SudokuPuzzle.Children.IndexOf((TextBox)sender) + 1].Focus();
                    }
                    e.Handled = true;
                    break;
                case Key.Down:
                    if (SudokuPuzzle.Children.IndexOf((TextBox)sender) + 9 <= 80)
                    {
                        SudokuPuzzle.Children[SudokuPuzzle.Children.IndexOf((TextBox)sender) + 9].Focus();
                    }
                    e.Handled = true;
                    break;
                case Key.Delete:
                    if (!((TextBox)sender).IsReadOnly && ((TextBox)sender).Text.Length > 0)
                    {
                        if (((TextBox)sender).Background == Brushes.Red)
                        {
                            bool darkMode = nightmode_chkbx.IsChecked == true ? true : false;
                            RecolourACell(darkMode, (TextBox)sender);
                        }
                        ((TextBox)sender).Text = "";
                    }
                    e.Handled = true;
                    break;
                case Key.P:
                    if (!g_solved)
                        PauseTimer(1);
                    e.Handled = true;
                    break;
                case Key.N:                    
                    PencilMarkONOFF('k');
                    e.Handled = true;
                    break;
                case Key.D:                    
                    DrawingONOFF('k');
                    e.Handled = true;
                    break;
                case Key.H:                    
                    if (!g_solved)
                        PauseTimer(2);
                    e.Handled = true;
                    break;
                case Key.A:                    
                    ResetCandidates();
                    e.Handled = true;
                    break;
                case Key.U:
                    UpdateCandidates();
                    e.Handled = true;
                    break;
                case Key.C:
                    ClearCandidates();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
            
        }
        /// <summary>
        /// A messagebox result button has been implemented to prevent the user from accidentally exiting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveQuit_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// A messagebox result button has been implemented to prevent the user from accidentally generating a new puzzle, leaving the current one unsaved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to play a new puzzle?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                PlaySudoku playSudoku = new PlaySudoku(g_difficulty, "")
                {
                    Owner = Owner
                };
                Hide();//Unfortunately, this is the only way to get rid of the current puzzle and open a new one. Rather close, but that opens the main menu as well
                playSudoku.ShowDialog();
            }
        }       
        /// <summary>
        /// Dark mode activated, changes all Controls into a dark version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //DarkMode
            if (darkerColour == null)//reduces memory clutter by only ever assigning these colours once. Also, the objects are only created if the user activates darkmode.
            {
                darkerColour = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
                darkColour = new SolidColorBrush(Color.FromArgb(150, 45, 45, 45));
                altDarkCellColour = new SolidColorBrush(Color.FromArgb(150, 66, 66, 66));
                darkButtonColour = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
                darkTextColour = new SolidColorBrush(Color.FromArgb(200, 240, 240, 240));
                darkFocusCell = new SolidColorBrush(Color.FromArgb(150, 127, 127, 127));
                darkHoverCell = new SolidColorBrush(Color.FromArgb(150, 100, 100, 100));
            }
                
            this.Background = darkColour;
            cnvs.Background = darkerColour;
            DrawSelection_grid.Background = darkerColour;
            for (int i = 0; i < cnvs.Children.Count; i++)
            {
                if (cnvs.Children[i].GetType() == typeof(Grid))
                    DarkModeChange((Grid)cnvs.Children[i]);
                else if (cnvs.Children[i].GetType() == typeof(RichTextBox))
                {
                    ((RichTextBox)cnvs.Children[i]).Background = darkerColour;
                    ((RichTextBox)cnvs.Children[i]).Foreground = darkTextColour;
                }
            }
            for (int i = 0; i < Numbers_grid.Children.Count; i++)//1-9 button number grid
            {
                ((Button)Numbers_grid.Children[i]).Foreground = darkTextColour;
                ((Button)Numbers_grid.Children[i]).Background = darkButtonColour;
            }
            RecolourAllCells(true);
            if (g_selectedCell != null)
            {
                g_selectedCell.Focus();
            }
        }      
        /// <summary>
        /// Dark mode deactivated, changes all Controls back into light version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //LightMode
            //reduces overhead by only ever assigning these colours once. Also, the object is only created if the user deactivates darkmode.
            if (buttonColour == null)
            {
                buttonColour = new SolidColorBrush(Color.FromArgb(255, 255, 247, 230));
                backgroundCol = new SolidColorBrush(Color.FromArgb(255, 238, 241, 255));
            }
            this.Background = backgroundCol;
            cnvs.Background = backgroundCol;
            DrawSelection_grid.Background = backgroundCol;
            for (int i = 0; i < cnvs.Children.Count; i++)
            {
                if (cnvs.Children[i].GetType() == typeof(Grid))
                    LightModeChange((Grid)cnvs.Children[i]);
                else if (cnvs.Children[i].GetType() == typeof(RichTextBox))
                {
                    ((RichTextBox)cnvs.Children[i]).Background = backgroundCol;
                    ((RichTextBox)cnvs.Children[i]).Foreground = Brushes.Black;
                }
            }
            for (int i = 0; i < Numbers_grid.Children.Count; i++)//1-9 button number grid
            {
                ((Button)Numbers_grid.Children[i]).Foreground = Brushes.Black; 
                ((Button)Numbers_grid.Children[i]).Background = buttonColour;
            }
            RecolourAllCells(false);
            if (g_selectedCell != null)
            {
                g_selectedCell.Focus();
            }
        }
        /// <summary>
        /// Changes the colour of a button when mouse is hovering on the button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Mouse_Enter(object sender, MouseEventArgs e)
        {
            if (nightmode_chkbx.IsChecked == true)
            {
                if (sender.GetType().Name == "Button")
                {
                    ((Button)sender).Foreground = Brushes.Black;
                }
                else
                {
                    ((ToggleButton)sender).Foreground = Brushes.Black;
                }
            }
        }
        /// <summary>
        /// Changes the colour of a button when mouse leaves the button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Mouse_Leave(object sender, MouseEventArgs e)
        {
            if (nightmode_chkbx.IsChecked == true)
            {
                if (sender.GetType().Name == "Button")
                {
                    ((Button)sender).Foreground = darkTextColour;
                }
                else
                {
                    ((ToggleButton)sender).Foreground = darkTextColour;
                }
            }
        }
        /// <summary>
        /// Changes the colour of a cell when mouse is hovering on the cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseEnter_Cell(object sender, MouseEventArgs e)
        {
            if (g_selectedCell != (TextBox)sender && ((TextBox)sender).Background != Brushes.Red)
            {
                if (nightmode_chkbx.IsChecked == false)
                {
                    ((TextBox)sender).Background = hoverCell;
                }
                else
                {
                    ((TextBox)sender).Background = darkHoverCell;
                }
            }
        }
        /// <summary>
        /// Changes the colour of a cell when mouse leaves the cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeave_Cell(object sender, MouseEventArgs e)
        {
            if (g_selectedCell != (TextBox)sender && ((TextBox)sender).Background != Brushes.Red)
            {
                int i = SudokuPuzzle.Children.IndexOf((TextBox)sender);
                if (i == 3 || i == 4 || i == 5 || i == 12 || i == 13 || i == 14
                    || i == 21 || i == 22 || i == 23 || i == 27 || i == 28 || i == 29
                    || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38
                    || i == 42 || i == 43 || i == 44 || i == 45 || i == 46 || i == 47
                    || i == 51 || i == 52 || i == 53 || i == 57 || i == 58 || i == 59
                    || i == 66 || i == 67 || i == 68 || i == 75 || i == 76 || i == 77)
                {
                    ((TextBox)sender).Background = nightmode_chkbx.IsChecked == true ? altDarkCellColour : altCellColour;
                }
                else
                {
                    ((TextBox)sender).Background = nightmode_chkbx.IsChecked == true ? darkColour : cellColour;
                }
                
            }
        }
        /// <summary>
        /// Event handler that deletes the contents of a cell, if the cell is not read only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (g_selectedCell != null)
            {
                if (!g_selectedCell.IsReadOnly && g_selectedCell.Text.Length > 0)
                {
                    g_selectedCell.Text = "";
                    
                    g_selectedCell.Focus();
                }
            }
        }
        #endregion
    }
}
