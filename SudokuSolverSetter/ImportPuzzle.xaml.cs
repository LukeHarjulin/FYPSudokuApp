using System.Windows;

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
