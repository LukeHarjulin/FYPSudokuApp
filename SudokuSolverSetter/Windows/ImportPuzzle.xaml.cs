using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        /// <summary>
        /// Checks if the imported puzzle string is valid for a sudoku puzzle.
        /// Checks:-
        /// -number of distinct numbers
        /// -number of given numbers
        /// -if two of the same numbers share a group
        /// </summary>
        /// <returns></returns>
        private bool ValidPuzzle()
        {
            char[][] grid = new char[9][];

            for (int r = 0, count = 0; r < 9; r++)
            {
                grid[r] = new char[9];
                for (int c = 0; c < 9; c++, count++)
                {
                    grid[r][c] = import_txtbx.Text[count];
                }
            }
            bool invalid = false;
            int blockNumber = 0;


            PuzzleGenerator gen = new PuzzleGenerator();
            Regex rgx = new Regex(@"[1-9]");
            int givenCounter = 0;
            List<char> numList = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (rgx.IsMatch(grid[r][c].ToString()))
                    {
                        givenCounter++;
                        numList.Remove(grid[r][c]);
                        int[] indexes = new int[2];
                        blockNumber = (r / 3) * 3 + (c / 3) + 1;
                        indexes = gen.BlockIndexGetter(blockNumber);
                        for (int i = 0; i < 9; i++)
                        {
                            if (grid[r][c] == grid[r][i] && i != c)
                            {
                                invalid = true;
                            }
                            if (grid[r][c] == grid[i][c] && i != r)
                            {
                                invalid = true;
                            }
                            if (grid[indexes[0]][indexes[1]] == grid[r][c] && indexes[0] != r && indexes[1] != c)
                            {
                                invalid = true;
                            }
                            indexes[1]++;
                            if (indexes[1] == 3 || indexes[1] == 6 || indexes[1] == 9)
                            {
                                indexes[0]++;
                                indexes[1] -= 3;
                            }
                        }
                    }
                }
            }
            if (numList.Count > 1 || invalid || givenCounter < 17)
            {
                string s = "";
                if (givenCounter < 17)
                {
                    s += "Puzzle invalid: insufficient amount of given numbers\r\n\r\n";
                }
                if (numList.Count > 1)
                {
                    s += "Puzzle invalid: insufficient amount of unique numbers\r\n\r\n";
                }
                if (invalid)
                {
                    s += "Puzzle invalid: at least two of the same numbers share a group";
                }
                MessageBox.Show(s);
                return false;
            }
            return true;
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
            else if (ValidPuzzle())
            {
                puzzleStr = import_txtbx.Text;
                this.DialogResult = true;
            }
        }
    }
}
