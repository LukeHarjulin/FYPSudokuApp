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
        public PuzzleSelector()
        {
            InitializeComponent();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow homePage = new MainWindow();
            this.Hide();
            homePage = new MainWindow();
            homePage.Show();
        }
    }
}
