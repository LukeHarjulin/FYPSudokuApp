using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private bool ShutdownApp()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit?", "Quit Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                return true;//Closes the application properly if the red 'X' is clicked
            }
            else
            {
                return false;
            }
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
                Hide();
                developerWindow.ShowDialog();
                Show();
            }
        }
        /// <summary>
        /// Exits application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Quit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (ShutdownApp())
            {
                Application.Current.Shutdown(0);//Closes the application properly if the red 'X' is clicked
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
            Hide();
            if (Difficulty_ComboBox.SelectedIndex == 0)//Beginner
            {
                playSudoku = new PlaySudoku("Beginner", "")
                {
                    Owner = this
                };
                playSudoku.ShowDialog();
            }
            else if (Difficulty_ComboBox.SelectedIndex == 1)//Moderate
            {
                playSudoku = new PlaySudoku("Moderate", "")
                {
                    Owner = this
                };
                playSudoku.ShowDialog();
            }
            else if (Difficulty_ComboBox.SelectedIndex == 2)//Advanced
            {
                playSudoku = new PlaySudoku("Advanced", "")
                {
                    Owner = this
                };
                playSudoku.ShowDialog();
            }
            else                                           //Extreme
            {
                playSudoku = new PlaySudoku("Extreme", "")
                {
                    Owner = this
                };
                playSudoku.ShowDialog();
            }
            Show();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Level_Selector_Click(object sender, RoutedEventArgs e)
        {
            PuzzleSelector selector = new PuzzleSelector()
            {
                Owner = this
            };
            Hide();
            selector.ShowDialog();
            Show();
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
            CreatePuzzles createPuzzles = new CreatePuzzles(5)//user is only allowed to create a max of 5 puzzles to prevent extensive wait times and potential crashes
            {
                Owner = this
            };
            createPuzzles.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
