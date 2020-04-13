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
        /// <summary>
        /// Exits application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Quit_btn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        /// <summary>
        /// Calls the PlaySudoku class, sending the difficulty setting selected so that a puzzle with that difficulty can be selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play_Sudoku_Click(object sender, RoutedEventArgs e)
        {
            if (Difficulty_ComboBox.SelectedIndex == 0)//Beginner
            {
                PlaySudoku playSudoku = new PlaySudoku("Beginner", "");
                playSudoku.Show();
            }
            else if (Difficulty_ComboBox.SelectedIndex == 1)//Moderate
            {
                PlaySudoku playSudoku = new PlaySudoku("Moderate", "");
                playSudoku.Show();
            }
            else if (Difficulty_ComboBox.SelectedIndex == 2)//Advanced
            {
                PlaySudoku playSudoku = new PlaySudoku("Advanced", "");
                playSudoku.Show();
            }
            else                                           //Extreme
            {
                PlaySudoku playSudoku = new PlaySudoku("Extreme", "");
                playSudoku.Show();
            }

            this.Hide();
        }
        private void Window_Close(object sender, EventArgs e)
        {
            Environment.Exit(0);//Closes the application properly if the red 'X' is clicked
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Level_Selector_Click(object sender, RoutedEventArgs e)
        {
            PuzzleSelector selector = new PuzzleSelector();
            selector.Show();
            this.Hide();
        }
        /// <summary>
        /// Produces a help menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Help_btn_Click(object sender, RoutedEventArgs e)
        {
            if (HelpBlock.IsVisible == false)
            {
                HelpBlock.Visibility = Visibility.Visible;
                Help_btn.Content = "CLOSE HELP";
            }
            else
            {
                HelpBlock.Visibility = Visibility.Hidden;
                Help_btn.Content = "OPEN HELP";
            }
        }
        private void Create_Store_Puzzles_btn_Click(object sender, RoutedEventArgs e)
        {
            CreatePuzzles createPuzzles = new CreatePuzzles();
            createPuzzles.ShowDialog();
        }
    }
}
