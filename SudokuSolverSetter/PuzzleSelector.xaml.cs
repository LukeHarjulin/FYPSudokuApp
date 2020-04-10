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
    /// Interaction logic for PuzzleSelector.xaml
    /// </summary>
    public partial class PuzzleSelector : Window
    {
        private MainWindow homePage = new MainWindow();
        public PuzzleSelector()
        {
            InitializeComponent();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            homePage = new MainWindow();
            homePage.Show();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            homePage = new MainWindow();
            homePage.Show();
        }

        private void SelectPuzzle_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
