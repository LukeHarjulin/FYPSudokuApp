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
    /// Interaction logic for ImportPuzzle.xaml
    /// </summary>
    public partial class ImportPuzzle : Window
    {
        public string puzzleStr = "";
        public ImportPuzzle()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (import_txtbx.Text.Length < 81)
            {
                MessageBox.Show("Insufficient number of characters");
            }
            else if (import_txtbx.Text.Length > 81)
            {
                MessageBox.Show("Too many characters");
            }
            else
            {
                puzzleStr = import_txtbx.Text;
                this.DialogResult = true;
            }
        }
    }
}
