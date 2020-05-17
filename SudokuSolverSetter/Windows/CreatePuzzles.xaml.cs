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
        #region Initialisation
        private bool g_Symmetry = false;
        public CreatePuzzles() => InitializeComponent(); 
        public CreatePuzzles(int addLimit, bool symmetry)
        {
            InitializeComponent();
            g_Symmetry = symmetry;
            Number_List_combo.Items.Add(1);
            if (addLimit > 10)
            {
                Number_List_combo.Items.Add(10);
                Number_List_combo.Items.Add(25);
                Number_List_combo.Items.Add(50);
                Number_List_combo.Items.Add(100);
                Number_List_combo.Items.Add(250);
                Number_List_combo.Items.Add(500);
                Number_List_combo.Items.Add(750);
                Number_List_combo.Items.Add(1000);
                Number_List_combo.Items.Add(2000);
                Number_List_combo.Items.Add(2500);
                Number_List_combo.Items.Add(5000);
                Number_List_combo.Items.Add(10000);
                Number_List_combo.Items.Add(20000);
            }
            else
            {
                for (int i = 2; i <= addLimit; i++)
                {
                    Number_List_combo.Items.Add(i);
                }
            }
        }
        #endregion
        #region Functions/Methods
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
        /// Function that reacts to the button click and generates the given number of puzzles
        /// </summary>
        /// <param name="numPuzzles"></param>
        public void GeneratePuzzles(int numPuzzles, bool symmetry)
        {
            List<SudokuGrid> sudokuPuzzles = new List<SudokuGrid>();
            PuzzleGenerator gen = new PuzzleGenerator();
            try
            {
                XDocument doc;
                string symmetric = "";
                if (symmetry)
                {
                    symmetric = @"Symmetric";
                }
                else
                {
                    symmetric = @"NonSymmetric";
                }
                string filename = symmetric+"/SudokuPuzzles.xml";
                if (File.Exists(filename))
                {
                    doc = XDocument.Load(filename);
                }
                else
                {
                    doc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XComment("Sudoku Puzzle Storage For SudokuSolverSetter App"),
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
                            new XElement("Completed",
                                new XElement("Beginner"),
                                new XElement("Moderate"),
                                new XElement("Advanced"),
                                new XElement("Extreme")
                                )
                            )
                        );
                }
                StreamWriter ratingWrite = new StreamWriter(symmetric+"/ratings.txt",true);
                StreamWriter difficWrite = new StreamWriter(symmetric+"/difficulties.txt", true); 
                StreamWriter givensWrite = new StreamWriter(symmetric+"/givens.txt", true);
                StreamWriter stratWrite = new StreamWriter(symmetric + "/strat.txt", true);
                Stopwatch Timer = new Stopwatch();
                Timer.Start();
                for (int i = 0; i < numPuzzles; i++)
                {
                    sudokuPuzzles.Add(gen.Setter(symmetry));
                    string puzzleString = gen.SudokuToString(sudokuPuzzles[i]);
                    PuzzleSolverObjDS solver = new PuzzleSolverObjDS();
                    long rating = GetDifficulty(sudokuPuzzles[i], puzzleString, solver);
                    doc.Element("SudokuPuzzles").Element("NotStarted").Element(sudokuPuzzles[i].Difficulty).Add(
                        new XElement("puzzle",
                            new XElement("DifficultyRating", rating),
                            new XElement("SudokuString", puzzleString)
                            )
                        );
                    
                    int givens = 0;
                    for (int x = 0; x < puzzleString.Length; x++)
                    {
                        if (puzzleString[x] != '0')
                        {
                            givens++;
                        }
                    }
                    ratingWrite.WriteLine(rating);
                    switch (sudokuPuzzles[i].Difficulty)
                    {
                        case "Beginner":
                            difficWrite.WriteLine("1");
                            break;
                        case "Moderate":
                            difficWrite.WriteLine("2");
                            break;
                        case "Advanced":
                            difficWrite.WriteLine("3");
                            break;
                        default:
                            difficWrite.WriteLine("4");
                            break;
                    }
                    givensWrite.WriteLine(givens);
                    for (int n = 1; n < solver.g_StrategyCount.Length; n++)
                    {
                        if (solver.g_StrategyCount[n] < 10)
                            stratWrite.Write(0);
                        stratWrite.Write(solver.g_StrategyCount[n] + ",");
                    }
                    stratWrite.Write(solver.g_StrategyCount[0]);
                    stratWrite.WriteLine();
                }
                Timer.Stop();
                ratingWrite.Close();
                difficWrite.Close();
                givensWrite.Close();
                stratWrite.Close();
                doc.Save(filename);
                MessageBox.Show("Successfully added " + numPuzzles + " puzzles.\r\nThe generator took " + Timer.Elapsed + " to generate and store all of those puzzles.", "Success!");

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
        public long GetDifficulty(SudokuGrid puzzleGrid, string puzzleString, PuzzleSolverObjDS solver)
        {
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
            rating = solver.g_Rating;
            puzzleGrid.Difficulty = solver.g_Difficulty;
            return rating;
        }
        #endregion
        #region Event Handlers
        /// <summary>
        /// Event handler that loads/creates an XML file for the puzzles, adding the number of puzzles specified to the XML file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GeneratePuzzles(int.Parse(Number_List_combo.SelectedItem.ToString()), g_Symmetry);
        }
        #endregion
    }
}
