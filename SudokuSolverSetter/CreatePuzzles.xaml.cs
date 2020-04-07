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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<SudokuGrid> sudokuPuzzles = new List<SudokuGrid>();
            PuzzleGenerator gen = new PuzzleGenerator();
            PuzzleSolverCharVer solverChar = new PuzzleSolverCharVer();
            PuzzleSolver solver = new PuzzleSolver();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
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
                    char[][] puzzle = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
                    string puzzleString = gen.SudokuToString(sudokuPuzzles[i]);
                    long rating = 0;
                    int difficulty = solver.difficulty = 0;
                    for (int n = 0; n < 10; n++)
                    {
                        int counter = 0;
                        for (int x = 0; x < 9; x++)
                        {
                            for (int y = 0; y < 9; y++)
                            {
                                if (puzzleString[counter] == '0')
                                {
                                    sudokuPuzzles[i].Rows[x][y].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                                }
                                sudokuPuzzles[i].Rows[x][y].Num = puzzleString[counter];
                                counter++;
                            }
                        }
                        //puzzle = SudokuGridToArray(sudokuPuzzles[i], puzzle);
                        watch = Stopwatch.StartNew();
                        //solverChar.BruteForceSolve(puzzle, 0, 0, 0);
                        solver.Solver(sudokuPuzzles[i], '1');
                        watch.Stop();
                        if(n==0)
                        {
                            difficulty = solver.difficulty;
                        }
                        rating += watch.ElapsedMilliseconds;
                    }
                    rating += difficulty;
                    if(rating < 100)
                    {
                        sudokuPuzzles[i].Difficulty = "Beginner";
                    }
                    else if (rating >= 100 && rating < 200)
                    {
                        sudokuPuzzles[i].Difficulty = "Moderate";
                    }
                    else if (rating >= 200 && rating < 500)
                    {
                        sudokuPuzzles[i].Difficulty = "Advanced";
                    }
                    else
                    {
                        sudokuPuzzles[i].Difficulty = "Extreme";
                    }

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
    }
}
