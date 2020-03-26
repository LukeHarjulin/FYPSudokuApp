using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for PlaySudoku.xaml
    /// </summary>
    public partial class PlaySudoku : Window
    {
        private MainWindow homePage = new MainWindow();
        private TextBox selectedCell;
        private string cellContents = "";
        private List<TextBox> txtBxList = new List<TextBox>();
        private PuzzleGenerator gen = new PuzzleGenerator();
        private SudokuGrid grid = new SudokuGrid();


        public PlaySudoku() => InitializeComponent();

        public PlaySudoku(int difficulty)
        {
            InitializeComponent();
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
                //pull random easy puzzle from xml file/database
            }
            else if (difficulty == 2)
            {
                Sudoku_Title.Content = "Intermediate Sudoku Puzzle";
                //pull random medium puzzle from xml file/database
            }
            else if (difficulty == 3)
            {
                Sudoku_Title.Content = "Advanced Sudoku Puzzle";
                //pull random hard puzzle from xml file/database
            }
            grid = gen.Setter();
            DeveloperWindow devWin = new DeveloperWindow();
            devWin.PopulateGrid(grid, txtBxList);
            Clipboard.SetText(gen.SudokuToString(grid));

        }

        private void Num_Button_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCell != null)
            {
                if (!selectedCell.IsReadOnly)
                {
                    selectedCell.Text = ((Button)sender).Content.ToString();//Previously Selected Cell is given the number of the button
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
                selectedCell.Background = Brushes.White;
            }
            
            selectedCell = ((TextBox)sender);
            ((TextBox)sender).Background = new SolidColorBrush(Color.FromArgb(255, 138, 255, 208));
            cellContents = selectedCell.Text;
        }

        private void Cell_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Cell_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedCell != null)
            {
                if (!int.TryParse(selectedCell.Text, out int result) || selectedCell.Text.Contains('0'))
                {
                    selectedCell.Text = cellContents;
                }
                else if (selectedCell.Text.Length > 1)
                {
                    selectedCell.Text = selectedCell.Text.Remove(selectedCell.Text.IndexOf(cellContents[0]));
                }
                if (cellContents != selectedCell.Text)
                {
                    //Check if solved
                    bool solved = false;
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
                        if (txtBxList[i].Text == "")
                        {
                            basicGrid[c][i] = '0';
                        }
                        else
                        {
                            basicGrid[r][c] = txtBxList[i].Text[0];
                        }
                    }

                    List<int> numberList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
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
                            numberList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        }
                    }
                    solved = true;
                    if (solved)
                    {//Fix
                        //Do something
                    }
                }
                cellContents = selectedCell.Text;
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
    }
}
