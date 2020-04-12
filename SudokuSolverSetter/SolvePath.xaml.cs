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
    /// Interaction logic for SolvePath.xaml
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
