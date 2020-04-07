using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for PlaySudoku.xaml
    /// </summary>
    public partial class PlaySudoku : Window
    {
        #region Global variables
        private MainWindow homePage = new MainWindow();
        private List<TextBox> g_txtBxList = new List<TextBox>();
        private TextBox g_selectedCell;
        private SudokuGrid g_grid = new SudokuGrid();
        private string g_cellContents = "", g_currentTime = "", g_rating, g_difficulty, g_originalPuzzleString;
        private bool g_pencilMarker = false, g_toggleRecursion = true, g_puzzledSolved = false;

        private SolidColorBrush focusCell = new SolidColorBrush(Color.FromArgb(255, 176, 231, 233));
        private SolidColorBrush cellColour = new SolidColorBrush(Color.FromArgb(255, 255, 221, 192));
        private SolidColorBrush hoverCell = new SolidColorBrush(Color.FromArgb(255, 255, 241, 230));
        private SolidColorBrush backgroundCol, darkFocusCell, darkHoverCell, darkerColour, darkColour, darkButtonColour, textColour, buttonColour;

        public DispatcherTimer DT { get; set; }
        public Stopwatch Timer { get; set; }
        #endregion
        public PlaySudoku() => InitializeComponent();
        public PlaySudoku(string difficulty, string puzzleString)//Initialize window
        {
            InitializeComponent();
            
            PuzzleGenerator gen = new PuzzleGenerator();
            timer_txtbx.Text = "0:00";
            g_txtBxList = new List<TextBox> {
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
            string fileName = @"SudokuPuzzles.xml";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(fileName);
                XmlNode sudokuPuzzles = doc.DocumentElement.SelectSingleNode("/SudokuPuzzles");
                if (puzzleString == "")//pulling a non-started puzzle
                {
                    XmlNode notStartedPuzzles = sudokuPuzzles.FirstChild;
                    XmlNodeList puzzleDifficulties = notStartedPuzzles.ChildNodes;

                    if (difficulty == "1")
                    {
                        Sudoku_Title.Content = "Beginner Sudoku Puzzle";
                        XmlNodeList beginnerPuzzles = puzzleDifficulties[0].ChildNodes;
                        Random rnd = new Random();
                        XmlNode puzzle = beginnerPuzzles[rnd.Next(0, beginnerPuzzles.Count)];
                        puzzleString = puzzle.SelectSingleNode("/SudokuString").InnerText;
                        g_rating = puzzle.SelectSingleNode("/DifficultyRating").InnerText;
                        //pull random easy puzzle from xml file/database
                    }
                    else if (difficulty == "2")
                    {
                        Sudoku_Title.Content = "Moderate Sudoku Puzzle";
                        XmlNodeList moderatePuzzles = puzzleDifficulties[0].ChildNodes;
                        Random rnd = new Random();
                        XmlNode puzzle = moderatePuzzles[rnd.Next(0, moderatePuzzles.Count)];
                        puzzleString = puzzle.SelectSingleNode("/SudokuString").InnerText;
                        g_rating = puzzle.SelectSingleNode("/DifficultyRating").InnerText;
                        //pull random medium puzzle from xml file/database
                    }
                    else if (difficulty == "3")
                    {
                        Sudoku_Title.Content = "Advanced Sudoku Puzzle";
                        XmlNodeList advancedPuzzles = puzzleDifficulties[0].ChildNodes;
                        Random rnd = new Random();
                        XmlNode puzzle = advancedPuzzles[rnd.Next(0, advancedPuzzles.Count)];
                        puzzleString = puzzle.SelectSingleNode("/SudokuString").InnerText;
                        g_rating = puzzle.SelectSingleNode("/DifficultyRating").InnerText;
                        //pull random hard puzzle from xml file/database
                    }
                    else if (difficulty == "4")
                    {
                        Sudoku_Title.Content = "Extreme Sudoku Puzzle";
                        XmlNodeList extremePuzzles = puzzleDifficulties[0].ChildNodes;
                        Random rnd = new Random();
                        XmlNode puzzle = extremePuzzles[rnd.Next(0, extremePuzzles.Count)];
                        puzzleString = puzzle.SelectSingleNode("/SudokuString").InnerText;
                        g_rating = puzzle.SelectSingleNode("/DifficultyRating").InnerText;
                        //pull random very hard puzzle from xml file/database
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No puzzles exist... Generating Random Puzzle for you...");
                g_grid = gen.Setter();
                PopulateGrid(g_grid, g_txtBxList);
                g_originalPuzzleString = gen.SudokuToString(g_grid);
                //Clipboard.SetText(gen.SudokuToString(grid));
            }
            ///Populate grid with puzzle from xml doc
            for (int i = 0; i < 81; i++)
            {
                if (puzzleString[i] != '0')
                    g_txtBxList[i].Text = puzzleString[i].ToString();
            }
            StartTimer();
            g_originalPuzzleString = puzzleString;
            g_difficulty = difficulty;//crash if puzzle generation has to occur
        }
        #region Functions
        private void StartTimer()
        {
            Timer = new Stopwatch();
            DT = new DispatcherTimer();
            DT.Tick += new EventHandler(DT_Tick);
            DT.Interval = new TimeSpan(0,0,1);
            Timer.Start();
            DT.Start();
        }
        private void PauseTimer()
        {
            if (Timer.IsRunning)
            {
                Timer.Stop();
                DT.Stop();
            }
            else
            {
                Timer.Start();
                DT.Start();
            }
        }
        private void PencilMarkONOFF(char source)//source is either button click or 'n' key down
        {
            if (source == 'b')
            {
                if (TogglePencil.IsChecked == true)
                {
                    TogglePencil.Content = "Notes ON (N)";
                    g_pencilMarker = true;
                    if (g_selectedCell != null)
                    {
                        g_selectedCell.Focus();
                    }
                }
                else if (TogglePencil.IsChecked == false)
                {
                    TogglePencil.Content = "Notes OFF (N)";
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
                    TogglePencil.Content = "Notes ON (N)";
                    g_pencilMarker = true;
                    if (g_selectedCell != null)
                    {
                        g_selectedCell.Focus();
                    }
                }
                else if (TogglePencil.IsChecked == true)
                {
                    TogglePencil.IsChecked = false;
                    TogglePencil.Content = "Notes OFF (N)";
                    g_pencilMarker = false;
                    if (g_selectedCell != null)
                    {
                        g_selectedCell.Focus();
                    }
                }
            }
            
            
        }

        public void PopulateGrid(SudokuGrid grid, List<TextBox> m_txtBxList)
        {
            /*This method populates the Uniform grid and its textboxes with all the given values from 'grid'.
            */
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
                        m_txtBxList[i].FontWeight = FontWeights.SemiBold;
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
        private void SavePuzzle(string puzzle, bool completed)
        {
            ///Save Puzzle to Started/Completed
            XDocument doc;
            string filename = @"SudokuPuzzles.xml", sudokuInString = "", candidatesInString = "";

            doc = File.Exists(filename)
                ? XDocument.Load(filename)
                : new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("This is a new comment"),
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
                        new XElement("Complete",
                            new XElement("Beginner"),
                            new XElement("Moderate"),
                            new XElement("Advanced"),
                            new XElement("Extreme")
                            )
                        )
                    );
            if (!completed)
            {
                for (int i = 0; i < 81; i++)
                {
                    if (g_txtBxList[i].Text.Length > 1)
                    {
                        candidatesInString += g_txtBxList[i].Text;
                    }
                    else if (g_txtBxList[i].Text == "")
                    {
                        sudokuInString += "0";
                        candidatesInString += "0";
                    }
                    else
                    {
                        sudokuInString += g_txtBxList[i].Text;
                        candidatesInString += g_txtBxList[i].Text;
                    }
                    if (i != 80)
                    {
                        candidatesInString += ",";
                    }
                }
                doc.Element("SudokuPuzzles").Element("Started").Element("Beginner"/*grab difficulty*/).Add(
                           new XElement("puzzle",
                               new XElement("DifficultyRating", 100/*grab rating*/),
                               new XElement("SudokuString", sudokuInString),
                               new XElement("Time taken", g_currentTime)
                               )
                           );
            }
            else
            {
                for (int i = 0; i < 81; i++)
                {
                    sudokuInString += g_txtBxList[i].Text;
                }
                doc.Element("SudokuPuzzles").Element("Completed").Element(g_difficulty).Add(
                           new XElement("puzzle",
                               new XElement("DifficultyRating", g_rating/*grab rating*/),
                               new XElement("SudokuString", sudokuInString),
                               new XElement("Time taken", g_currentTime)
                               )
                           );
            }

            doc.Save(filename);
            MessageBox.Show("Successfully saved puzzle.");
        }
        #endregion
        #region All Event Actions
        private void DT_Tick(object sender, EventArgs e)
        {
            if (Timer.IsRunning)
            {
                if (g_puzzledSolved)
                {
                    Timer.Stop();
                    DT.Stop();
                    MessageBox.Show("Congratulations!\n\rSudoku Complete!");
                    string puzzle = "";
                    for (int i = 0; i < 81; i++)
                    {
                        g_txtBxList[i].IsReadOnly = true;
                        puzzle += g_txtBxList[i];
                    }
                    SavePuzzle(puzzle, true);
                }
                TimeSpan ts = Timer.Elapsed;
                if (ts.Seconds < 10)
                {
                    g_currentTime = ts.Minutes + ":0" + ts.Seconds.ToString();
                }
                else
                {
                    g_currentTime = ts.Minutes + ":" + ts.Seconds.ToString();
                }
                timer_txtbx.Text = g_currentTime;
            }
        }
        private void Pause_btn_Click(object sender, RoutedEventArgs e)
        {
            PauseTimer();
        }
                
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

        private void Window_Close(object sender, EventArgs e)
        {
            homePage = new MainWindow();
            homePage.Show();
        }
        private void Cell_Selected(object sender, RoutedEventArgs e)
        {
            if (g_selectedCell != null)//sets previously focused cell to default colour
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
            
            g_selectedCell = ((TextBox)sender);
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
                    if (g_cellContents != g_selectedCell.Text && g_selectedCell.Text.Length < 2 && !g_pencilMarker)
                    {
                        //Check if solved
                        bool solved = true;
                        char[][] basicGrid = new char[9][];
                        for (int i = 0; i < basicGrid.Length; i++)
                        {
                            basicGrid[i] = new char[9];
                        }
                        int c = 0;
                        int r = 0;
                        for (int i = 0; i < g_txtBxList.Count; i++, c++)//populate basic grid so it can be checked.
                        {
                            if (c == 9)
                            {
                                c = 0;
                                r++;
                            }
                            if (g_txtBxList[i].Text == "" || g_txtBxList[i].Text.Length > 1)
                            {
                                basicGrid[r][c] = '0';
                            }
                            else
                            {
                                basicGrid[r][c] = g_txtBxList[i].Text[0];
                            }
                        }

                        List<char> numberList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                        for (int row = 0; row < 9; row++)
                        {
                            for (int col = 0; col < 9; col++)
                            {
                                if (basicGrid[row][col] == 0)
                                {
                                    solved = false;
                                }
                                else if (numberList.Contains(basicGrid[row][col]))
                                {
                                    numberList.Remove(basicGrid[row][col]);
                                }
                            }
                            if (numberList.Count > 0)
                            {
                                solved = false;
                            }
                            else
                            {
                                numberList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                            }
                        }
                        if (solved) { g_puzzledSolved = true; }
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
                g_toggleRecursion = true;
            }
            
        }

        private void Cell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (g_txtBxList.IndexOf(g_selectedCell) - 1 >= 0)
                    {
                        g_txtBxList[g_txtBxList.IndexOf(g_selectedCell) - 1].Focus();
                    }
                    break;
                case Key.Up:
                    if (g_txtBxList.IndexOf(g_selectedCell) - 9 >= 0)
                    {
                        g_txtBxList[g_txtBxList.IndexOf(g_selectedCell) - 9].Focus();
                    }
                    break;
                case Key.Right:
                    if (g_txtBxList.IndexOf(g_selectedCell) + 1 <= 80)
                    {
                        g_txtBxList[g_txtBxList.IndexOf(g_selectedCell) + 1].Focus();
                    }
                    break;
                case Key.Down:
                    if (g_txtBxList.IndexOf(g_selectedCell) + 9 <= 80)
                    {
                        g_txtBxList[g_txtBxList.IndexOf(g_selectedCell) + 9].Focus();
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
                    PauseTimer();
                    break;
                default:
                    break;
            }
        }

        private void SaveQuit_Button_Click(object sender, RoutedEventArgs e)
        {
            ///Save puzzle
            
            this.Hide();
            homePage.Show();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            PencilMarkONOFF('b');
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            
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

        private void New_Btn_Click(object sender, RoutedEventArgs e)
        {
            PlaySudoku playSudoku = new PlaySudoku(g_difficulty, "");
            this.Hide();
            playSudoku.Show();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //DarkMode
            if (darkerColour == null)//reduces memory clutter by only ever assigning these colours once. Also, the objects are only created if the user activates darkmode.
            {
                darkerColour = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
                darkColour = new SolidColorBrush(Color.FromArgb(255, 45, 45, 45));
                darkButtonColour = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
                textColour = new SolidColorBrush(Color.FromArgb(150, 240, 240, 240));
                darkFocusCell = new SolidColorBrush(Color.FromArgb(255, 75, 75, 75));
                darkHoverCell = new SolidColorBrush(Color.FromArgb(255, 95, 95, 95));
            }
                
            this.Background = darkColour;
            cnvs.Background = darkerColour;
            Sudoku_Title.Foreground = textColour;
            time_lbl.Foreground = textColour;
            timer_txtbx.Foreground = textColour;
            TogglePencil.Foreground = textColour; TogglePencil.Background = darkButtonColour;
            Back_btn.Foreground = textColour; Back_btn.Background = darkButtonColour;
            del_btn.Foreground = textColour; del_btn.Background = darkButtonColour;
            newPuzzle_btn.Foreground = textColour; newPuzzle_btn.Background = darkButtonColour;
            Help_btn.Foreground = textColour; Help_btn.Background = darkButtonColour;
            Pause_btn.Foreground = textColour; Pause_btn.Background = darkButtonColour;
            nightmode_chkbx.Foreground = textColour;

            btn1.Foreground = textColour; btn2.Foreground = textColour; btn3.Foreground = textColour; btn4.Foreground = textColour;
            btn5.Foreground = textColour; btn6.Foreground = textColour; btn7.Foreground = textColour; btn8.Foreground = textColour; btn9.Foreground = textColour;
            btn1.Background = darkButtonColour; btn2.Background = darkButtonColour;btn3.Background = darkButtonColour; btn4.Background = darkButtonColour;
            btn5.Background = darkButtonColour; btn6.Background = darkButtonColour; btn7.Background = darkButtonColour; btn8.Background = darkButtonColour; btn9.Background = darkButtonColour;

            for (int i = 0; i < g_txtBxList.Count; i++)
            {
                g_txtBxList[i].Background = darkColour;
                g_txtBxList[i].Foreground = textColour;
                g_txtBxList[i].BorderBrush = textColour;
                g_txtBxList[i].SelectionBrush = textColour;
            }
            if (g_selectedCell != null)
            {
                g_selectedCell.Focus();
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //LightMode
                //reduces memory clutter by only ever assigning these colours once. Also, the object is only created if the user deactivates darkmode.
            if (buttonColour == null)
            {
                buttonColour = new SolidColorBrush(Color.FromArgb(255, 255, 247, 230));
                backgroundCol = new SolidColorBrush(Color.FromArgb(255, 238, 241, 255));
            }

            this.Background = Brushes.White;
            cnvs.Background = Brushes.White;
            Sudoku_Title.Foreground = Brushes.Black;
            time_lbl.Foreground = Brushes.Black;
            timer_txtbx.Foreground = Brushes.Black;
            TogglePencil.Foreground = Brushes.Black; TogglePencil.Background = buttonColour;
            Back_btn.Foreground = Brushes.Black; Back_btn.Background = buttonColour;
            del_btn.Foreground = Brushes.Black; del_btn.Background = buttonColour;
            newPuzzle_btn.Foreground = Brushes.Black; newPuzzle_btn.Background = buttonColour;
            Help_btn.Foreground = Brushes.Black; Help_btn.Background = buttonColour;
            Pause_btn.Foreground = Brushes.Black; Pause_btn.Background = buttonColour;
            nightmode_chkbx.Foreground = Brushes.Black;

            btn1.Foreground = Brushes.Black; btn2.Foreground = Brushes.Black; btn3.Foreground = Brushes.Black; btn4.Foreground = Brushes.Black;
            btn5.Foreground = Brushes.Black; btn6.Foreground = Brushes.Black; btn7.Foreground = Brushes.Black; btn8.Foreground = Brushes.Black; btn9.Foreground = Brushes.Black;
            btn1.Background = buttonColour; btn2.Background = buttonColour; btn3.Background = buttonColour; btn4.Background = buttonColour;
            btn5.Background = buttonColour; btn6.Background = buttonColour; btn7.Background = buttonColour; btn8.Background = buttonColour; btn9.Background = buttonColour;

            for (int i = 0; i < g_txtBxList.Count; i++)
            {
                g_txtBxList[i].Background = cellColour;
                g_txtBxList[i].Foreground = Brushes.Black;
                g_txtBxList[i].BorderBrush = Brushes.Black;
                g_txtBxList[i].SelectionBrush = Brushes.Black;
            }
            if (g_selectedCell != null)
            {
                g_selectedCell.Focus();
            }
        }

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

        private void Button_Mouse_Leave(object sender, MouseEventArgs e)
        {
            if (nightmode_chkbx.IsChecked == true)
            {
                if (sender.GetType().Name == "Button")
                {
                    ((Button)sender).Foreground = textColour;
                }
                else
                {
                    ((ToggleButton)sender).Foreground = textColour;
                }
            }
        }

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

        private void MouseLeave_Cell(object sender, MouseEventArgs e)
        {
            if (g_selectedCell != (TextBox)sender)
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

        private void Help_btn_Click(object sender, RoutedEventArgs e)
        {
            //Produce help window
        }

        private void Timer_txtbx_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

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
