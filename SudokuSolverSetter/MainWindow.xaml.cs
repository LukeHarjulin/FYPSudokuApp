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
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            PasswordBox passBox = new PasswordBox
            {
                Owner = this
            };
            if (passBox.ShowDialog() == true)
            {
                DeveloperWindow developerWindow = new DeveloperWindow
                {
                    Owner = this
                };
                developerWindow.Show();
                Hide();
            }
        }
        /// <summary>
        /// Exits application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Quit_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit?", "Quit Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Environment.Exit(0);//Closes the application properly if the red 'X' is clicked
            }
        }
        /// <summary>
        /// Calls the PlaySudoku class, sending the difficulty setting selected so that a puzzle with that difficulty can be selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play_Sudoku_Click(object sender, RoutedEventArgs e)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            PlaySudoku playSudoku;
            if (Difficulty_ComboBox.SelectedIndex == 0)//Beginner
            {
                playSudoku = new PlaySudoku("Beginner", "")
                {
                    Owner = this
                };
                playSudoku.Show();
            }
            else if (Difficulty_ComboBox.SelectedIndex == 1)//Moderate
            {
                playSudoku = new PlaySudoku("Moderate", "")
                {
                    Owner = this
                };
                playSudoku.Show();
            }
            else if (Difficulty_ComboBox.SelectedIndex == 2)//Advanced
            {
                playSudoku = new PlaySudoku("Advanced", "")
                {
                    Owner = this
                };
                playSudoku.Show();
            }
            else                                           //Extreme
            {
                playSudoku = new PlaySudoku("Extreme", "")
                {
                    Owner = this
                };
                playSudoku.Show();
            }
            
            Hide();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit?", "Quit Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Environment.Exit(0);//Closes the application properly if the red 'X' is clicked
            }
            else if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Level_Selector_Click(object sender, RoutedEventArgs e)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            PuzzleSelector selector = new PuzzleSelector
            {
                Owner = this
            };
            selector.Show();
            Hide();
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
            CreatePuzzles createPuzzles = new CreatePuzzles(10, true)//user is only allowed to create a max of 10 puzzles, all of which must be symmetrical
            {
                Owner = this
            };
            createPuzzles.ShowDialog();
        }
    }
}
