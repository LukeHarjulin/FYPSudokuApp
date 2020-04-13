using System;
using System.IO;
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
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for CreatePuzzles.xaml
    /// </summary>
    public partial class CreatePuzzles : Window
    {
        public CreatePuzzles()
        {
            InitializeComponent();
            Number_List_combo.Items.Add(1);
            for (int i = 2; i <= 50; i++)
            {
                Number_List_combo.Items.Add(i);
            }
            
        }
        /// <summary>
        /// Takes a SudokuGrid object and converts into a char[][] type 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="puzzle"></param>
        /// <returns></returns>
        private char[][] SudokuGridToArray(SudokuGrid grid, char[][] puzzle)
        {
            
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    puzzle[x][y] = grid.Rows[x][y].Num;
                }
            }
            return puzzle;
        }
        /// <summary>
        /// Event handler that loads/creates an XML file for the puzzles, adding the number of puzzles specified to the XML file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<SudokuGrid> sudokuPuzzles = new List<SudokuGrid>();
            PuzzleGenerator gen = new PuzzleGenerator();
            PuzzleSolverCharVer solverChar = new PuzzleSolverCharVer();
            PuzzleSolver solver = new PuzzleSolver();
            
            
            int numPuzzles = int.Parse(Number_List_combo.SelectedItem.ToString());

            try
            {
                XDocument doc;
                string filename = @"SudokuPuzzles.xml";
                if (File.Exists(filename))
                {                    
                    doc = XDocument.Load(filename);
                }
                else
                {
                    doc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XComment("This is a new comment"),
                        new XElement("SudokuPuzzles",
                            new XElement("NotStarted",
                                new XElement("Beginner"),
                                new XElement("Moderate"),
                                new XElement("Advanced"),
                                new XElement("Extreme")
                                ),
                            new XElement("Started",
                                new XElement("Beginner"),
                                new XElement("Moderate"),
                                new XElement("Advanced"),
                                new XElement("Extreme")
                                ),
                            new XElement("Complete",
                                new XElement("Beginner"),
                                new XElement("Moderate"),
                                new XElement("Advanced"),
                                new XElement("Extreme")
                                )
                            )
                        );
                }
                
                for (int i = 0; i < numPuzzles; i++)
                {
                    sudokuPuzzles.Add(gen.Setter());
                    string puzzleString = gen.SudokuToString(sudokuPuzzles[i]);
                    long rating = GetDifficulty(sudokuPuzzles[i], puzzleString);
                    doc.Element("SudokuPuzzles").Element("NotStarted").Element(sudokuPuzzles[i].Difficulty).Add(
                        new XElement("puzzle",
                            new XElement("DifficultyRating", rating),
                            new XElement("SudokuString", puzzleString)
                            )
                        );
                }
                doc.Save(filename);
                MessageBox.Show("Successfully added " + numPuzzles + " puzzles.");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error with writing: " + ex);
                throw;
            }
        }
        /// <summary>
        /// Function that recieves a puzzle and the string version of the puzzle and tests the difficulty of the puzzle by solving it using the Human-Strategy solver
        /// </summary>
        /// <param name="puzzleGrid"></param>
        /// <param name="puzzleString"></param>
        /// <returns>returns the difficulty rating of the puzzle</returns>
        public long GetDifficulty(SudokuGrid puzzleGrid, string puzzleString)
        {
            PuzzleSolver solver = new PuzzleSolver();
            long rating = 0;
            int counter = 0;
            
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (puzzleString[counter] == '0')
                    {
                        puzzleGrid.Rows[x][y].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                    }
                    else
                    {
                        puzzleGrid.Rows[x][y].Candidates = new List<char> { };
                    }
                    puzzleGrid.Rows[x][y].Num = puzzleString[counter];
                    counter++;
                }
            }
            solver.Solver(puzzleGrid, 1);
            rating = solver.g_Difficulty;
            if (rating < 800)
            {
                puzzleGrid.Difficulty = "Beginner";
            }
            else if (rating >= 800 && rating < 1300)
            {
                puzzleGrid.Difficulty = "Moderate";
            }
            else if (rating >= 1300 && rating < 2000)
            {
                puzzleGrid.Difficulty = "Advanced";
            }
            else
            {
                puzzleGrid.Difficulty = "Extreme";
            }
            return rating;
        }
    }
}
