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
        MainWindow homePage = new MainWindow();
        TextBox selectedCell;
        string cellContents = "";
        public PlaySudoku()
        {
            InitializeComponent();
        }
        public PlaySudoku(int difficulty)
        {
            InitializeComponent();
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
        }

        private void Num_Button_Click(object sender, RoutedEventArgs e)
        {
            selectedCell.Text = ((Button)sender).Content.ToString();//Previously Selected Cell is given the number of the button
        }

        private void Window_Closed(object sender, EventArgs e)
        {

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
                selectedCell.FontWeight = FontWeights.Normal;
            }
            
            selectedCell = ((TextBox)sender);
            ((TextBox)sender).Background = new SolidColorBrush(Color.FromArgb(255, 138, 255, 208));
            selectedCell.FontWeight = FontWeights.SemiBold;
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
                cellContents = selectedCell.Text;
            }
            
        }
    }
}
