using System.Windows;

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for SolvePath.xaml
    /// This whole window's purpose is to display the path in which the solver that utilises strategies, reaches the solution of the puzzle. 
    /// For each detail of the puzzle that is changed by the solver, the change is documented in a string list and it is all printed out at the end, in a clear format.
    /// 
    /// Main purpose is to check if implemented strategy works and to see how the solver approaches a puzzle.
    /// </summary>
    public partial class SolvePath : Window
    {
        public SolvePath()
        {
            InitializeComponent();

        }
        public void PopulateTextBlock(int difficultyRating, string timeString, PuzzleSolver puzzleSolver)
        {
            Show();
            AdditionalInfo.Text = "Difficulty Rating (WIP): " + difficultyRating + "\r\n" + timeString;
            string solvePath = "";
            for (int i = 0; i < puzzleSolver.g_SolvePath.Count; i++)
            {
                if (puzzleSolver.g_SolvePath[i][0] != '-')
                {
                    solvePath += "\r\n" + puzzleSolver.g_SolvePath[i] + "\r\n\r\n";
                }
                else
                {
                    solvePath += puzzleSolver.g_SolvePath[i] + "\r\n";
                }
            }
            solvePathBlock.Text = solvePath;
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
