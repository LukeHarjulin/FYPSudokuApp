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

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for PlaySudoku.xaml
    /// </summary>
    public partial class PlaySudoku : Window
    {
        private MainWindow homePage = new MainWindow();
        private List<TextBox> txtBxList = new List<TextBox>();
        private PuzzleGenerator gen = new PuzzleGenerator();
        private SudokuGrid grid = new SudokuGrid();
        private TextBox selectedCell;
        private string cellContents = "", currentTime = "";
        private bool pencilMarker = false, allowTextChanged = true, puzzledSolved = false;
        private Stopwatch timer;
        private DispatcherTimer dT;
        public PlaySudoku() => InitializeComponent();

        public PlaySudoku(int difficulty)
        {
            InitializeComponent();
            timer_txtbx.Text = "0:00";
            txtBxList = new List<TextBox> {
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

            if (difficulty == 1)
            {
                Sudoku_Title.Content = "Beginner-Level Sudoku Puzzle";
                grid = gen.Setter();//Temp
                //pull random easy puzzle from xml file/database
            }
            else if (difficulty == 2)
            {
                Sudoku_Title.Content = "Intermediate Sudoku Puzzle";
                grid = gen.Setter();//Temp
                //pull random medium puzzle from xml file/database
            }
            else if (difficulty == 3)
            {
                Sudoku_Title.Content = "Advanced Sudoku Puzzle";
                grid = gen.Setter();//Temp
                //pull random hard puzzle from xml file/database
            }
            
            DeveloperWindow devWin = new DeveloperWindow();
            PopulateGrid(grid, txtBxList);
            Clipboard.SetText(gen.SudokuToString(grid));

            StartTimer();
            
        }

        private void StartTimer()
        {
            timer = new Stopwatch();
            dT = new DispatcherTimer();
            dT.Tick += new EventHandler(DT_Tick);
            dT.Interval = new TimeSpan(0,0,1);
            timer.Start();
            dT.Start();
        }

        private void DT_Tick(object sender, EventArgs e)
        {
            if (timer.IsRunning)
            {
                if (puzzledSolved)
                {
                    timer.Stop();
                    dT.Stop();
                    MessageBox.Show("Congratulations!\n\rSudoku Complete!");
                }
                TimeSpan ts = timer.Elapsed;
                if (ts.Seconds < 10)
                {
                    currentTime = ts.Minutes + ":0" + ts.Seconds.ToString();
                }
                else
                {
                    currentTime = ts.Minutes + ":" + ts.Seconds.ToString();
                }
                timer_txtbx.Text = currentTime;
            }
        }
        private void Pause_btn_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsRunning)
            {
                timer.Stop();
                dT.Stop();
            }
            else
            {
                timer.Start();
                dT.Start();
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
                if (grid.Rows[x][y].Num != 0) //0's are placeholders for when there is no value, so any 0's are turned into textboxes containing the candidate values.
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

        private void Num_Button_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCell != null)
            {
                if (!selectedCell.IsReadOnly)
                {
                    if (pencilMarker)
                    {
                        selectedCell.FontSize = 16;
                        if (!selectedCell.Text.Contains(((Button)sender).Content.ToString()))
                        {
                            selectedCell.Text += ((Button)sender).Content.ToString();//Previously Selected Cell is given the number of the button
                        }
                    }
                    else
                    {
                        selectedCell.Text = ((Button)sender).Content.ToString();//Previously Selected Cell is given the number of the button

                    }


                    selectedCell.Focus();
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
            if (selectedCell != null)
            {
                if (nightmode_chkbx.IsChecked == false)
                {
                    selectedCell.Background = null;
                }
                else
                {
                    selectedCell.Background = new SolidColorBrush(Color.FromArgb(100, 45, 45, 45));
                }
            }
            
            selectedCell = ((TextBox)sender);
            if (nightmode_chkbx.IsChecked == false)
            {
                ((TextBox)sender).Background = new SolidColorBrush(Color.FromArgb(255, 138, 255, 208));
            }
            else
            {
                ((TextBox)sender).Background = new SolidColorBrush(Color.FromArgb(100, 75, 75, 75));
            }
            cellContents = selectedCell.Text;
        }

        private void Cell_LostFocus(object sender, RoutedEventArgs e)
        {

        }
        

        private void Cell_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedCell != null && allowTextChanged)
            {
                allowTextChanged = false;
                selectedCell.Text = selectedCell.Text.Trim(' '); 
                if (selectedCell.Text.Contains('n')) { selectedCell.Text = selectedCell.Text.Remove(selectedCell.Text.IndexOf('n'), 1); }
                else if (selectedCell.Text.Contains('-')) { selectedCell.Text = selectedCell.Text.Remove(selectedCell.Text.IndexOf('-'), 1); }
                else if (selectedCell.Text.Contains('+')) { selectedCell.Text = selectedCell.Text.Remove(selectedCell.Text.IndexOf('+'), 1); }

                if (selectedCell.Text != cellContents)
                {
                    
                    if (selectedCell.Text != "" && !int.TryParse(selectedCell.Text, out int result) || selectedCell.Text.Contains('0') || selectedCell.Text.Length > 9)
                    {
                        selectedCell.Text = cellContents;
                        allowTextChanged = true;
                        return;
                    }
                    else if (selectedCell.Text.Length > 1)
                    {
                        if (!pencilMarker)
                        {
                            for (int i = 0; i < cellContents.Length; i++)
                            {
                                if (selectedCell.Text.Contains(cellContents[i]))
                                {
                                    selectedCell.Text = selectedCell.Text.Remove(selectedCell.Text.IndexOf(cellContents[i]), 1);
                                }
                            }
                        }
                        else
                        {
                            string numbers = "";
                            for (int i = 0; i < selectedCell.Text.Length; i++)
                            {
                                if (!numbers.Contains(selectedCell.Text[i]))
                                {
                                    numbers += (selectedCell.Text[i]);
                                }
                            }
                            selectedCell.Text = numbers;
                        }
                    }
                    if (cellContents != selectedCell.Text && selectedCell.Text.Length < 2 && !pencilMarker)
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
                        for (int i = 0; i < txtBxList.Count; i++, c++)//populate basic grid so it can be checked.
                        {
                            if (c == 9)
                            {
                                c = 0;
                                r++;
                            }
                            if (txtBxList[i].Text == "" || txtBxList[i].Text.Length > 1)
                            {
                                basicGrid[r][c] = '0';
                            }
                            else
                            {
                                basicGrid[r][c] = txtBxList[i].Text[0];
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
                        if (solved) { puzzledSolved = true; }
                    }
                    cellContents = selectedCell.Text;
                    if (!pencilMarker)
                    {
                        selectedCell.FontSize = 36;

                    }
                    else
                    {
                        selectedCell.FontSize = 16;
                    }
                }
                allowTextChanged = true;
            }
            
        }

        private void Cell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (txtBxList.IndexOf(selectedCell) - 1 >= 0)
                    {
                        txtBxList[txtBxList.IndexOf(selectedCell) - 1].Focus();
                    }
                    break;
                case Key.Up:
                    if (txtBxList.IndexOf(selectedCell) - 9 >= 0)
                    {
                        txtBxList[txtBxList.IndexOf(selectedCell) - 9].Focus();
                    }
                    break;
                case Key.Right:
                    if (txtBxList.IndexOf(selectedCell) + 1 <= 80)
                    {
                        txtBxList[txtBxList.IndexOf(selectedCell) + 1].Focus();
                    }
                    break;
                case Key.Down:
                    if (txtBxList.IndexOf(selectedCell) + 9 <= 80)
                    {
                        txtBxList[txtBxList.IndexOf(selectedCell) + 9].Focus();
                    }
                    break;
                case Key.Delete:
                    if (!selectedCell.IsReadOnly && selectedCell.Text.Length > 0)
                    {
                        selectedCell.Text = "";
                    }
                    break;
                case Key.N:
                    if (TogglePencil.IsChecked == false)
                    {
                        TogglePencil.IsChecked = true;
                        TogglePencil.Content = "Notes ON (N)";
                        pencilMarker = true;
                        selectedCell.Focus();
                    }
                    else if (TogglePencil.IsChecked == true)
                    {
                        TogglePencil.IsChecked = false;
                        TogglePencil.Content = "Notes OFF (N)";
                        pencilMarker = false;
                        selectedCell.Focus();
                    }

                    break;
                default:
                    break;
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            homePage = new MainWindow();
            homePage.Show();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ((ToggleButton)sender).Content = "Notes ON (N)";
            pencilMarker = true;
            if (selectedCell != null) selectedCell.Focus();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ((ToggleButton)sender).Content = "Notes OFF (N)";
            pencilMarker = false;
            if (selectedCell != null) selectedCell.Focus();
        }

        private void New_Btn_Click(object sender, RoutedEventArgs e)
        {
            PlaySudoku playSudoku = new PlaySudoku(1);
            this.Hide();
            playSudoku.Show();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //DarkMode
            SolidColorBrush darkerColour = new SolidColorBrush(Color.FromArgb(100, 30, 30, 30));
            SolidColorBrush darkColour = new SolidColorBrush(Color.FromArgb(100, 45, 45, 45));
            SolidColorBrush buttonColour = new SolidColorBrush(Color.FromArgb(100, 60, 60, 60));
            SolidColorBrush textColour = new SolidColorBrush(Color.FromArgb(100, 240, 240, 240));
            
            this.Background = darkColour;
            cnvs.Background = darkerColour;
            Sudoku_Title.Foreground = textColour;
            time_lbl.Foreground = textColour;
            timer_txtbx.Foreground = textColour;
            TogglePencil.Foreground = textColour; TogglePencil.Background = buttonColour;
            Back_btn.Foreground = textColour; Back_btn.Background = buttonColour;
            del_btn.Foreground = textColour; del_btn.Background = buttonColour;
            newPuzzle_btn.Foreground = textColour; newPuzzle_btn.Background = buttonColour;
            Help_btn.Foreground = textColour; Help_btn.Background = buttonColour;
            Pause_btn.Foreground = textColour; Pause_btn.Background = buttonColour;
            nightmode_chkbx.Foreground = textColour;

            btn1.Foreground = textColour; btn2.Foreground = textColour; btn3.Foreground = textColour; btn4.Foreground = textColour;
            btn5.Foreground = textColour; btn6.Foreground = textColour; btn7.Foreground = textColour; btn8.Foreground = textColour; btn9.Foreground = textColour;
            btn1.Background = darkColour; btn2.Background = darkColour;btn3.Background = darkColour; btn4.Background = darkColour;
            btn5.Background = darkColour; btn6.Background = darkColour; btn7.Background = darkColour; btn8.Background = darkColour; btn9.Background = darkColour;

            for (int i = 0; i < txtBxList.Count; i++)
            {
                txtBxList[i].Background = darkColour;
                txtBxList[i].Foreground = textColour;
                txtBxList[i].BorderBrush = textColour;
                txtBxList[i].SelectionBrush = textColour;
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //LightMode
            SolidColorBrush buttonColour = new SolidColorBrush(Color.FromArgb(100, 221, 221, 221));
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
            btn1.Background = Brushes.White; btn2.Background = Brushes.White; btn3.Background = Brushes.White; btn4.Background = Brushes.White;
            btn5.Background = Brushes.White; btn6.Background = Brushes.White; btn7.Background = Brushes.White; btn8.Background = Brushes.White; btn9.Background = Brushes.White;

            for (int i = 0; i < txtBxList.Count; i++)
            {
                txtBxList[i].Background = Brushes.White;
                txtBxList[i].Foreground = Brushes.Black;
                txtBxList[i].BorderBrush = Brushes.Black;
                txtBxList[i].SelectionBrush = Brushes.Black;
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
            if (selectedCell != null)
            {
                if (!selectedCell.IsReadOnly)
                {
                    if (selectedCell.Text.Length > 1)
                    {
                        selectedCell.Text = selectedCell.Text.Remove(selectedCell.Text.Length - 1);
                    }
                    else
                    {
                        selectedCell.Text = "";
                    }
                    selectedCell.Focus();
                }
                
            }
            
        }
    }
}
