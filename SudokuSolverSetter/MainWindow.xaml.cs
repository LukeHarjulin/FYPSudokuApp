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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void DeveloperMode_Button_Click(object sender, RoutedEventArgs e)
        {

            PasswordBox passBox = new PasswordBox();
            if (passBox.ShowDialog() == true)
            {
                DeveloperWindow developerWindow = new DeveloperWindow();
                developerWindow.Show();
                this.Hide();
            }
        }

        private void Quit_btn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Play_Sudoku_Click(object sender, RoutedEventArgs e)
        {
            if (Difficulty_ComboBox.SelectedIndex == 0)
            {
                PlaySudoku playSudoku = new PlaySudoku(1);
                playSudoku.Show();
            }
            else if (Difficulty_ComboBox.SelectedIndex == 1)
            {
                PlaySudoku playSudoku = new PlaySudoku(2);
                playSudoku.Show();
            }
            else
            {
                PlaySudoku playSudoku = new PlaySudoku(3);
                playSudoku.Show();
            }

            this.Hide();
        }

        private void Window_Close(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Level_Selector_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
