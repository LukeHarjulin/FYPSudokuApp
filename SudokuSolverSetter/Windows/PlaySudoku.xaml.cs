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

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for PlaySudoku.xaml
    /// </summary>
    public partial class PlaySudoku : Window
    {
        #region Initialisation
        #region Global variables
        private MainWindow homePage = new MainWindow();
        private PuzzleGenerator g_Gen = new PuzzleGenerator();
        private List<TextBox> g_Cells = new List<TextBox>();
        private TextBox g_selectedCell;
        private SudokuGrid g_grid = new SudokuGrid();
        private string g_cellContents = "", g_currentTime = "", g_rating, g_difficulty, g_originalPuzzleString;
        private bool g_pencilMarker = false, g_toggleRecursion = true, penType = false, g_solved = false;

        private SolidColorBrush focusCell = new SolidColorBrush(Color.FromArgb(255, 176, 231, 233));
        private SolidColorBrush cellColour = new SolidColorBrush(Color.FromArgb(255, 255, 221, 192));
        private SolidColorBrush hoverCell = new SolidColorBrush(Color.FromArgb(255, 255, 241, 230));
        private SolidColorBrush altCellColour = new SolidColorBrush(Color.FromArgb(255, 255, 224, 233));
        private SolidColorBrush pauseBlockBckGrd = new SolidColorBrush(Color.FromArgb(255, 238, 241, 255));
        private SolidColorBrush darkPauseBlockBckGrd = new SolidColorBrush(Color.FromArgb(255, 238, 241, 255));
        private SolidColorBrush backgroundCol, darkFocusCell, darkHoverCell, darkerColour, darkColour, darkButtonColour, darkTextColour, buttonColour, altDarkCellColour;
        private DrawingAttributes g_penDA, g_highlighterDA;
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
            timer_txtbx.Text = "0:00";
            g_Cells = new List<TextBox> {
                bx1,bx2,bx3,bx4,bx5,bx6,bx7,bx8,bx9,
                bx10,bx11,bx12,bx13,bx14,bx15,bx16,bx17,bx18,bx19,
                bx20,bx21,bx22,bx23,bx24,bx25,bx26,bx27,bx28,bx29,
                bx30,bx31,bx32,bx33,bx34,bx35,bx36,bx37,bx38,bx39,
                bx40,bx41,bx42,bx43,bx44,bx45,bx46,bx47,bx48,bx49,
                bx50,bx51,bx52,bx53,bx54,bx55,bx56,bx57,bx58,bx59,
                bx60,bx61,bx62,bx63,bx64,bx65,bx66,bx67,bx68,bx69,
                bx70,bx71,bx72,bx73,bx74,bx75,bx76,bx77,bx78,bx79,
                bx80,bx81
            };
            string fileName = @"Symmetric/SudokuPuzzles.xml";
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

                    if (difficulty == "Beginner")//pull random beginner puzzle from xml file
                    {
                        Sudoku_Title.Content = "Beginner Sudoku Puzzle";
                        XmlNodeList beginnerPuzzles = puzzleDifficulties[0].ChildNodes;
                        Random rnd = new Random();
                        XmlNode puzzle = beginnerPuzzles[rnd.Next(0, beginnerPuzzles.Count)];
                        puzzleString = puzzle.SelectSingleNode("SudokuString").InnerText;
                        g_rating = puzzle.SelectSingleNode("DifficultyRating").InnerText;
                        //pull random easy puzzle from xml file/database
                    }
                    else if (difficulty == "Moderate")//pull random moderate puzzle from xml file
                    {
                        Sudoku_Title.Content = "Moderate Sudoku Puzzle";
                        XmlNodeList moderatePuzzles = puzzleDifficulties[1].ChildNodes;
                        Random rnd = new Random();
                        XmlNode puzzle = moderatePuzzles[rnd.Next(0, moderatePuzzles.Count)];
                        puzzleString = puzzle.SelectSingleNode("SudokuString").InnerText;
                        g_rating = puzzle.SelectSingleNode("DifficultyRating").InnerText;
                    }
                    else if (difficulty == "Advanced")//pull random advanced puzzle from xml file
                    {
                        Sudoku_Title.Content = "Advanced Sudoku Puzzle";
                        XmlNodeList advancedPuzzles = puzzleDifficulties[2].ChildNodes;
                        Random rnd = new Random();
                        XmlNode puzzle = advancedPuzzles[rnd.Next(0, advancedPuzzles.Count)];
                        puzzleString = puzzle.SelectSingleNode("SudokuString").InnerText;
                        g_rating = puzzle.SelectSingleNode("DifficultyRating").InnerText;
                        
                    }
                    else if (difficulty == "Extreme")//pull random extreme puzzle from xml file
                    {
                        Sudoku_Title.Content = "Extreme Sudoku Puzzle";
                        XmlNodeList extremePuzzles = puzzleDifficulties[3].ChildNodes;
                        Random rnd = new Random();
                        XmlNode puzzle = extremePuzzles[rnd.Next(0, extremePuzzles.Count)];
                        puzzleString = puzzle.SelectSingleNode("SudokuString").InnerText;
                        g_rating = puzzle.SelectSingleNode("DifficultyRating").InnerText;
                    }
                }
                else if (puzzleString.Contains('_'))
                {
                    Sudoku_Title.Content = difficulty + " Sudoku Puzzle";
                    XmlNode startedPuzzles = sudokuPuzzles.SelectSingleNode("Started");
                    XmlNodeList difficultyNode = startedPuzzles.SelectSingleNode(difficulty).ChildNodes;
                    foreach (XmlNode puzzle in difficultyNode)
                    {
                        if (puzzle.SelectSingleNode("SudokuString").InnerText == puzzleString)
                        {
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
                else 
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
                            g_originalPuzzleString = puzzleString;
                            Rating_lbl.Content = g_rating;
                            break;
                        }
                    }
                }
                ///Populate grid with puzzle from xml doc
                if (puzzleString.Contains('_'))
                {
                    for (int i = 0, counter = 0; counter < puzzleString.Length; counter++)
                    {
                        if (puzzleString[counter] == '|')
                        {
                            g_Cells[i].IsReadOnly = true;
                            g_Cells[i].FontWeight = FontWeights.Bold;
                            counter++;
                        }
                        else if (puzzleString[counter] == '-')
                        {
                            g_Cells[i].FontSize = 16;
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
                            g_Cells[i].Text = "";
                        }
                        if (puzzleString[counter] != '0')
                        {
                            g_Cells[i].Text += puzzleString[counter].ToString();
                            if (g_Cells[i].Text.Length > 1)
                            {
                                g_Cells[i].FontSize = 16;
                            }
                            else
                            {
                                g_Cells[i].FontSize = 36;
                            }
                        }
                        else
                        {
                            g_Cells[i].Text = "";
                        }
                        
                    }
                }
                else
                {
                    if (puzzleString.Contains('.'))
                    {
                        puzzleString = puzzleString.Remove(0, 1);
                    }
                    for (int i = 0; i < 81; i++)
                    {
                        if (puzzleString[i] != '0')
                        {
                            g_Cells[i].Text = puzzleString[i].ToString();
                            g_Cells[i].IsReadOnly = true;
                            g_Cells[i].FontWeight = FontWeights.Bold;
                        }
                    }
                    g_originalPuzzleString = puzzleString;
                }
                
            }
            catch (Exception)//Generates puzzle of random g_difficulty
            {
                MessageBox.Show("No puzzles exist... A puzzle of random difficulty will be generated after clicking 'OK'. \r\nGeneration of puzzle may take some time.");
                g_grid = gen.Setter(true);
                PopulateGrid(g_grid, g_Cells);
                g_originalPuzzleString = gen.SudokuToString(g_grid);
                CreatePuzzles createPuzzles = new CreatePuzzles();
                g_rating = createPuzzles.GetDifficulty(g_grid, g_originalPuzzleString, new PuzzleSolverObjDS()).ToString();
                //Clipboard.SetText(g_gen.SudokuToString(grid));
                Sudoku_Title.Content = g_grid.Difficulty + " Sudoku Puzzle";
                g_difficulty = g_grid.Difficulty;
            }
            Rating_lbl.Content = "Rating: " + g_rating;
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
        /// <param name="type"></param>
        private void PauseTimer(int type)
        {
            if (Timer.IsRunning)
            {
                
                Timer.Stop();
                DT.Stop();
                
                if (type == 1)
                {
                    RTB_LargeText.Inlines.Clear();
                    RTB_HelpText.Inlines.Clear();
                    PauseBlock.Visibility = Visibility.Visible;
                    if (nightmode_chkbx.IsChecked == false)
                        PauseBlock.Background = pauseBlockBckGrd;
                    else
                        PauseBlock.Background = darkerColour;
                    RTB_LargeText.Inlines.Add("PAUSED");
                    RTB_LargeText.FontSize = 64;
                    RTB_LargeText.Padding = new Thickness(120, 250, 120, 0);
                    btn1.IsEnabled = false; btn2.IsEnabled = false; btn3.IsEnabled = false; btn4.IsEnabled = false; btn5.IsEnabled = false; btn6.IsEnabled = false; btn7.IsEnabled = false; btn8.IsEnabled = false; btn9.IsEnabled = false;
                    del_btn.IsEnabled = false; TogglePencil.IsEnabled = false; ToggleDrawing.IsEnabled = false; Helper_btn.IsEnabled = false;
                }
                else if (type == 2)
                {
                    RTB_LargeText.Inlines.Clear();
                    RTB_HelpText.Inlines.Clear();
                    PauseBlock.Visibility = Visibility.Visible;
                    if (nightmode_chkbx.IsChecked == false)
                        PauseBlock.Background = pauseBlockBckGrd;
                    else
                        PauseBlock.Background = darkerColour;
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
                        "The \"Show Clue\" button is currently non-functional.\r\n\r\n");
                    RTB_LargeText.FontSize = 42;
                    RTB_HelpText.FontSize = 18;
                    btn1.IsEnabled = false; btn2.IsEnabled = false; btn3.IsEnabled = false; btn4.IsEnabled = false; btn5.IsEnabled = false; btn6.IsEnabled = false; btn7.IsEnabled = false; btn8.IsEnabled = false; btn9.IsEnabled = false;
                    del_btn.IsEnabled = false; TogglePencil.IsEnabled = false; ToggleDrawing.IsEnabled = false; Helper_btn.IsEnabled = false;
                }
                else if (type == 3)
                {
                    btn1.IsEnabled = false; btn2.IsEnabled = false; btn3.IsEnabled = false; btn4.IsEnabled = false; btn5.IsEnabled = false; btn6.IsEnabled = false; btn7.IsEnabled = false; btn8.IsEnabled = false; btn9.IsEnabled = false;
                    del_btn.IsEnabled = false; TogglePencil.IsEnabled = false; ToggleDrawing.IsEnabled = false; Pause_btn.IsEnabled = false; Help_btn.IsEnabled = false; nightmode_chkbx.IsEnabled = false; Helper_btn.IsEnabled = false;
                    SudokuGrid.IsEnabled = false;
                    for (int i = 0; i < 81; i++)
                    {
                        g_Cells[i].IsReadOnly = true;
                    }
                    RTB_LargeText.Inlines.Clear();
                    RTB_HelpText.Inlines.Clear();
                    PauseBlock.Visibility = Visibility.Visible;
                    if (nightmode_chkbx.IsChecked == false)
                        PauseBlock.Background = new SolidColorBrush(Color.FromArgb(150, 238, 241, 255));
                    else
                        PauseBlock.Background = new SolidColorBrush(Color.FromArgb(150, 45, 45, 45));
                    RTB_LargeText.Inlines.Add("Congratulations! Sudoku Complete!");
                    RTB_LargeText.FontSize = 48;
                    RTB_LargeText.Padding = new Thickness(120,250,120,0);
                    SavePuzzle(true);
                }
            }
            else
            {
                Timer.Start();
                DT.Start();
                PauseBlock.Visibility = Visibility.Hidden;
                btn1.IsEnabled = true; btn2.IsEnabled = true; btn3.IsEnabled = true; btn4.IsEnabled = true; btn5.IsEnabled = true; btn6.IsEnabled = true; btn7.IsEnabled = true; btn8.IsEnabled = true; btn9.IsEnabled = true;
                del_btn.IsEnabled = true; TogglePencil.IsEnabled = true; ToggleDrawing.IsEnabled = true; Helper_btn.IsEnabled = true;
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
        /// This method populates the Uniform grid and its textboxes with all the given values from 'grid'.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="m_txtBxList"></param>
        public void PopulateGrid(SudokuGrid grid, List<TextBox> m_txtBxList)
        {
            int x = 0;//row number
            int y = 0;//column number
            for (int i = 0; i < m_txtBxList.Count; i++)
            {
                if (grid.Rows[x][y].Num != '0') //0's are placeholders for when there is no value, so any 0's are turned into textboxes containing the candidate values.
                {
                    m_txtBxList[i].FontSize = 36;
                    m_txtBxList[i].Text = grid.Rows[x][y].Num.ToString();
                    if (grid.Rows[x][y].ReadOnly == true)//The readonly property ensures that the default given values of the sudoku puzzle remain readonly.
                    {
                        m_txtBxList[i].FontWeight = FontWeights.Bold;
                        m_txtBxList[i].IsReadOnly = true;
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
            string filename = @"Symmetric/SudokuPuzzles.xml", candidatesInString = "";
            
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
                    if (g_Cells[i].Text.Length > 1 || g_Cells[i].FontSize == 16)
                    {
                        candidatesInString += g_Cells[i].Text + "-";
                    }
                    else if (g_Cells[i].Text == "")
                    {
                        candidatesInString += "0";
                    }
                    else
                    {
                        candidatesInString += g_Cells[i].Text;
                        if (g_Cells[i].IsReadOnly)
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
                        if (puzzle.SelectSingleNode("OriginalSudokuString").InnerText == g_originalPuzzleString)
                        {
                            string s = puzzle.SelectSingleNode("DifficultyRating").InnerText;
                            doc.Element("SudokuPuzzles").Element("Started").Element(g_difficulty).Add(
                            new XElement("puzzle",
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
                            .First(n => n.Element("OriginalSudokuString").Value == g_originalPuzzleString);
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
                               new XElement("DifficultyRating", g_rating),
                               new XElement("SudokuString", "."+g_originalPuzzleString),
                               new XElement("TimeTaken", g_currentTime),
                               new XElement("Date", DateTime.Today.Date.ToShortDateString())
                               )
                           );
                var childNode = doc.Element("SudokuPuzzles").Element("Started").Element(g_difficulty)
                            .Elements("puzzle")
                            .First(n => n.Element("OriginalSudokuString").Value == g_originalPuzzleString);
                childNode.Remove();
            }

            doc.Save(filename);
            MessageBox.Show("Successfully saved puzzle.");
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
                if (ts.Seconds < 10)
                {
                    g_currentTime = ts.Minutes + ":0" + ts.Seconds;
                    if (ts.Hours > 0)
                        g_currentTime = ts.Hours + ":" + ts.Minutes + ":0" + ts.Seconds;
                }
                else if (ts.Hours > 0)
                {
                    g_currentTime = ts.Hours + ":" + ts.Minutes + ":" + ts.Seconds;
                }
                else
                {
                    g_currentTime = ts.Minutes + ":" + ts.Seconds;
                }
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
                        g_selectedCell.FontSize = 16;
                        if (!g_selectedCell.Text.Contains(((Button)sender).Content.ToString()))
                        {
                            g_selectedCell.Text += ((Button)sender).Content.ToString();//Previously Selected Cell is given the number of the button
                        }
                    }
                    else
                    {
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
        private void Cell_Selected(object sender, RoutedEventArgs e)
        {
            if (g_selectedCell != null)//sets previously focused cell to default colour
            {
                for (int i = 0; i < g_Cells.Count; i++)
                {
                    if (g_selectedCell == g_Cells[i] && (i == 3 || i == 4 || i == 5 || i == 12 || i == 13 || i == 14
                        || i == 21 || i == 22 || i == 23 || i == 27 || i == 28 || i == 29
                        || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38
                        || i == 42 || i == 43 || i == 44 || i == 45 || i == 46 || i == 47
                        || i == 51 || i == 52 || i == 53 || i == 57 || i == 58 || i == 59
                        || i == 66 || i == 67 || i == 68 || i == 75 || i == 76 || i == 77))
                    {
                        if (nightmode_chkbx.IsChecked == false)
                        {
                            g_selectedCell.Background = altCellColour;
                        }
                        else
                        {
                            g_selectedCell.Background = altDarkCellColour;
                        }
                    }
                    else if (g_selectedCell == g_Cells[i])
                    {
                        if (nightmode_chkbx.IsChecked == false)
                        {
                            g_selectedCell.Background = cellColour;
                        }
                        else
                        {
                            g_selectedCell.Background = darkColour;
                        }
                    }
                }
            }
            
            g_selectedCell = (TextBox)sender;
            if (nightmode_chkbx.IsChecked == false)//sets the current focused cell to a more prominent colour
            {
                ((TextBox)sender).Background = focusCell;
            }
            else
            {
                ((TextBox)sender).Background = darkFocusCell;
            }
            g_cellContents = g_selectedCell.Text;
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

        private void ToggleDrawing_Checked(object sender, RoutedEventArgs e)
        {
            ToggleDrawing.Content = "Drawing is ON";
            ColourCanvas.Visibility = Visibility.Visible;
            DrawSelection_grid.Visibility = Visibility.Visible;
        }
        private void ToggleDrawing_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleDrawing.Content = "Drawing is OFF";
            ColourCanvas.Visibility = Visibility.Hidden;
            DrawSelection_grid.Visibility = Visibility.Hidden;
            
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

        private void Helper_btn_Click(object sender, RoutedEventArgs e)
        {

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
        
        /// <summary>
        /// Beefy event handler to handle the text inputs into a cell, preventing more than one number being in a cell at once if notes are turned off.
        /// And allowing more than one number in the cell if it is turned on.
        /// Also prevents anything but numbers 1-9 being entered into a cell.
        /// This handler method definitely needs many improvements!!!!!!!
        /// SUPER MESSY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (g_selectedCell != null && g_toggleRecursion)
            {
                g_toggleRecursion = false;
                g_selectedCell.Text = g_selectedCell.Text.Trim(' '); 
                if (g_selectedCell.Text.Contains('n')) { g_selectedCell.Text = g_selectedCell.Text.Remove(g_selectedCell.Text.IndexOf('n'), 1); }
                else if (g_selectedCell.Text.Contains('-')) { g_selectedCell.Text = g_selectedCell.Text.Remove(g_selectedCell.Text.IndexOf('-'), 1); }
                else if (g_selectedCell.Text.Contains('+')) { g_selectedCell.Text = g_selectedCell.Text.Remove(g_selectedCell.Text.IndexOf('+'), 1); }

                if (g_selectedCell.Text != g_cellContents)
                {
                    
                    if (g_selectedCell.Text != "" && !int.TryParse(g_selectedCell.Text, out int result) || g_selectedCell.Text.Contains('0') || g_selectedCell.Text.Length > 9)
                    {
                        g_selectedCell.Text = g_cellContents;
                        g_toggleRecursion = true;
                        return;
                    }
                    else if (g_selectedCell.Text.Length > 1)
                    {
                        if (!g_pencilMarker)
                        {
                            for (int i = 0; i < g_cellContents.Length; i++)
                            {
                                if (g_selectedCell.Text.Contains(g_cellContents[i]))
                                {
                                    g_selectedCell.Text = g_selectedCell.Text.Remove(g_selectedCell.Text.IndexOf(g_cellContents[i]), 1);
                                }
                            }
                        }
                        else
                        {
                            string numbers = "";
                            for (int i = 0; i < g_selectedCell.Text.Length; i++)
                            {
                                if (!numbers.Contains(g_selectedCell.Text[i]))
                                {
                                    numbers += (g_selectedCell.Text[i]);
                                }
                            }
                            g_selectedCell.Text = numbers;
                        }
                    }
                    g_cellContents = g_selectedCell.Text;
                    if (!g_pencilMarker)
                    {
                        g_selectedCell.FontSize = 36;

                    }
                    else
                    {
                        g_selectedCell.FontSize = 16;
                    }
                }
                if (g_selectedCell.Text.Length < 2 && !g_pencilMarker)
                {
                    //Check if solved
                    char[][] basicGrid = new char[9][];
                    for (int i = 0; i < basicGrid.Length; i++)
                    {
                        basicGrid[i] = new char[9];
                    }
                    int c = 0;
                    int r = 0;
                    for (int i = 0; i < g_Cells.Count; i++, c++)//populate basic grid so it can be checked.
                    {
                        if (c == 9)
                        {
                            c = 0;
                            r++;
                        }
                        if (g_Cells[i].Text == "" || g_Cells[i].Text.Length > 1 || g_Cells[i].FontSize == 16)
                        {
                            basicGrid[r][c] = '0';
                        }
                        else
                        {
                            basicGrid[r][c] = g_Cells[i].Text[0];
                        }
                    }
                    if (g_Gen.CheckIfSolved_array(basicGrid))
                    {
                        g_solved = true;
                        Back_btn.Content = "Quit";
                        PauseTimer(3);
                    }
                }
                g_toggleRecursion = true;
            }
            
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
                    if (g_Cells.IndexOf(g_selectedCell) - 1 >= 0)
                    {
                        g_Cells[g_Cells.IndexOf(g_selectedCell) - 1].Focus();
                    }
                    break;
                case Key.Up:
                    if (g_Cells.IndexOf(g_selectedCell) - 9 >= 0)
                    {
                        g_Cells[g_Cells.IndexOf(g_selectedCell) - 9].Focus();
                    }
                    break;
                case Key.Right:
                    if (g_Cells.IndexOf(g_selectedCell) + 1 <= 80)
                    {
                        g_Cells[g_Cells.IndexOf(g_selectedCell) + 1].Focus();
                    }
                    break;
                case Key.Down:
                    if (g_Cells.IndexOf(g_selectedCell) + 9 <= 80)
                    {
                        g_Cells[g_Cells.IndexOf(g_selectedCell) + 9].Focus();
                    }
                    break;
                case Key.Delete:
                    if (!g_selectedCell.IsReadOnly && g_selectedCell.Text.Length > 0)
                    {
                        g_selectedCell.Text = "";
                    }
                    break;
                case Key.N:
                    PencilMarkONOFF('k');
                    break;
                case Key.P:
                    if (!g_solved)
                        PauseTimer(1);
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

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            PencilMarkONOFF('b');
        }
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (g_toggleRecursion)
            {
                g_toggleRecursion = false;
                PencilMarkONOFF('b');
                g_toggleRecursion = true;
            }
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
                darkColour = new SolidColorBrush(Color.FromArgb(255, 45, 45, 45));
                altDarkCellColour = new SolidColorBrush(Color.FromArgb(255, 66, 66, 66));
                darkButtonColour = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
                darkTextColour = new SolidColorBrush(Color.FromArgb(150, 240, 240, 240));
                darkFocusCell = new SolidColorBrush(Color.FromArgb(255, 127, 127, 127));
                darkHoverCell = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
            }
                
            this.Background = darkColour;
            cnvs.Background = darkerColour;
            Sudoku_Title.Foreground = darkTextColour;
            time_lbl.Foreground = darkTextColour;
            clue_lbl.Foreground = darkTextColour;
            Rating_lbl.Foreground = darkTextColour;
            selectPen_lbl.Foreground = darkTextColour;
            timer_txtbx.Foreground = darkTextColour;
            clue_txtbx.Foreground = darkTextColour;
            TogglePencil.Foreground = darkTextColour; TogglePencil.Background = darkButtonColour;
            Back_btn.Foreground = darkTextColour; Back_btn.Background = darkButtonColour;
            del_btn.Foreground = darkTextColour; del_btn.Background = darkButtonColour;
            newPuzzle_btn.Foreground = darkTextColour; newPuzzle_btn.Background = darkButtonColour;
            Help_btn.Foreground = darkTextColour; Help_btn.Background = darkButtonColour;
            Pause_btn.Foreground = darkTextColour; Pause_btn.Background = darkButtonColour;
            Helper_btn.Foreground = darkTextColour; Helper_btn.Background = darkButtonColour;
            nightmode_chkbx.Foreground = darkTextColour;
            PauseBlock.Background = darkerColour; PauseBlock.Foreground = darkTextColour;
            DrawSelection_grid.Background = darkerColour;
            changeDA_Button.Background = darkButtonColour; changeDA_Button.Foreground = darkTextColour;
            clearCanvas_Button.Background = darkButtonColour; clearCanvas_Button.Foreground = darkTextColour;
            ToggleDrawing.Background = darkButtonColour; ToggleDrawing.Foreground = darkTextColour;
            btn1.Foreground = darkTextColour; btn2.Foreground = darkTextColour; btn3.Foreground = darkTextColour; btn4.Foreground = darkTextColour;
            btn5.Foreground = darkTextColour; btn6.Foreground = darkTextColour; btn7.Foreground = darkTextColour; btn8.Foreground = darkTextColour; btn9.Foreground = darkTextColour;
            btn1.Background = darkButtonColour; btn2.Background = darkButtonColour;btn3.Background = darkButtonColour; btn4.Background = darkButtonColour;
            btn5.Background = darkButtonColour; btn6.Background = darkButtonColour; btn7.Background = darkButtonColour; btn8.Background = darkButtonColour; btn9.Background = darkButtonColour;
            for (int i = 0; i < g_Cells.Count; i++)
            {
                g_Cells[i].Foreground = darkTextColour;
                g_Cells[i].BorderBrush = darkTextColour;
                g_Cells[i].SelectionBrush = darkTextColour;
                if (i == 3 || i == 4 || i == 5 || i == 12 || i == 13 || i == 14
                    || i == 21 || i == 22 || i == 23 || i == 27 || i == 28 || i == 29
                    || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38
                    || i == 42 || i == 43 || i == 44 || i == 45 || i == 46 || i == 47
                    || i == 51 || i == 52 || i == 53 || i == 57 || i == 58 || i == 59
                    || i == 66 || i == 67 || i == 68 || i == 75 || i == 76 || i == 77)
                {
                    g_Cells[i].Background = altDarkCellColour;
                }
                else
                {
                    g_Cells[i].Background = darkColour;
                }
            }
            if (g_selectedCell != null)
            {
                g_selectedCell.Focus();
            }
        }
        /// <summary>
        /// Dark mode deactivated, changes all Controls back into light version
        /// Needs improving by implementing a function to reduce repeated code.
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
            Sudoku_Title.Foreground = Brushes.Black;
            time_lbl.Foreground = Brushes.Black;
            clue_lbl.Foreground = Brushes.Black;
            Rating_lbl.Foreground = Brushes.Black;
            selectPen_lbl.Foreground = Brushes.Black;
            timer_txtbx.Foreground = Brushes.Black;
            clue_txtbx.Foreground = Brushes.Black;
            TogglePencil.Foreground = Brushes.Black; TogglePencil.Background = buttonColour;
            Back_btn.Foreground = Brushes.Black; Back_btn.Background = buttonColour;
            del_btn.Foreground = Brushes.Black; del_btn.Background = buttonColour;
            newPuzzle_btn.Foreground = Brushes.Black; newPuzzle_btn.Background = buttonColour;
            Help_btn.Foreground = Brushes.Black; Help_btn.Background = buttonColour;
            Pause_btn.Foreground = Brushes.Black; Pause_btn.Background = buttonColour;
            Helper_btn.Foreground = Brushes.Black; Helper_btn.Background = buttonColour;
            nightmode_chkbx.Foreground = Brushes.Black;
            PauseBlock.Background = backgroundCol;
            PauseBlock.Foreground = Brushes.Black;
            DrawSelection_grid.Background = backgroundCol;
            changeDA_Button.Background = buttonColour; changeDA_Button.Foreground = Brushes.Black;
            clearCanvas_Button.Background = buttonColour; clearCanvas_Button.Foreground = Brushes.Black;
            ToggleDrawing.Background = buttonColour; ToggleDrawing.Foreground = Brushes.Black;
            btn1.Foreground = Brushes.Black; btn2.Foreground = Brushes.Black; btn3.Foreground = Brushes.Black; btn4.Foreground = Brushes.Black;
            btn5.Foreground = Brushes.Black; btn6.Foreground = Brushes.Black; btn7.Foreground = Brushes.Black; btn8.Foreground = Brushes.Black; btn9.Foreground = Brushes.Black;
            btn1.Background = buttonColour; btn2.Background = buttonColour; btn3.Background = buttonColour; btn4.Background = buttonColour;
            btn5.Background = buttonColour; btn6.Background = buttonColour; btn7.Background = buttonColour; btn8.Background = buttonColour; btn9.Background = buttonColour;
            for (int i = 0; i < g_Cells.Count; i++)
            {                
                g_Cells[i].Foreground = Brushes.Black;
                g_Cells[i].BorderBrush = Brushes.Black;
                g_Cells[i].SelectionBrush = Brushes.Black;
                if (i == 3 || i == 4 || i == 5 || i == 12 || i == 13 || i == 14 
                    || i == 21 || i == 22 || i == 23 || i == 27 || i == 28 || i == 29 
                    || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38 
                    || i == 42 || i == 43 || i == 44 || i == 45 || i == 46 || i == 47 
                    || i == 51 || i == 52 || i == 53 || i == 57 || i == 58 || i == 59 
                    || i == 66 || i == 67 || i == 68 || i == 75 || i == 76 || i == 77)
                {
                    g_Cells[i].Background = altCellColour;
                }
                else
                {
                    g_Cells[i].Background = cellColour;
                }
            }
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
            if (g_selectedCell != (TextBox)sender)
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
            if (g_selectedCell != (TextBox)sender)
            {
                for (int i = 0; i < g_Cells.Count; i++)
                {
                    if ((TextBox)sender == g_Cells[i] && (i == 3 || i == 4 || i == 5 || i == 12 || i == 13 || i == 14
                        || i == 21 || i == 22 || i == 23 || i == 27 || i == 28 || i == 29
                        || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38
                        || i == 42 || i == 43 || i == 44 || i == 45 || i == 46 || i == 47
                        || i == 51 || i == 52 || i == 53 || i == 57 || i == 58 || i == 59
                        || i == 66 || i == 67 || i == 68 || i == 75 || i == 76 || i == 77))
                    {
                        if (nightmode_chkbx.IsChecked == false)
                        {
                            ((TextBox)sender).Background = altCellColour;
                        }
                        else
                        {
                            ((TextBox)sender).Background = altDarkCellColour;
                        }
                    }
                    else if ((TextBox)sender == g_Cells[i])
                    {
                        if (nightmode_chkbx.IsChecked == false)
                        {
                            ((TextBox)sender).Background = cellColour;
                        }
                        else
                        {
                            ((TextBox)sender).Background = darkColour;
                        }
                    }
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
