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
            PuzzleSolverCharVer solver = new PuzzleSolverCharVer();
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
                                new XElement("Intermediate"),
                                new XElement("Advanced")
                                ),
                            new XElement("Started",
                                new XElement("Beginner"),
                                new XElement("Intermediate"),
                                new XElement("Advanced")
                                ),
                            new XElement("Complete",
                                new XElement("Beginner"),
                                new XElement("Intermediate"),
                                new XElement("Advanced")
                                )
                            )
                        );
                }
                
                for (int i = 0; i < numPuzzles; i++)
                {
                    sudokuPuzzles.Add(gen.Setter());
                    char[][] puzzle = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
                    
                    long averageTime = 0;
                    for (int x = 0; x < 10; x++)
                    {
                        puzzle = SudokuGridToArray(sudokuPuzzles[i], puzzle);
                        watch = Stopwatch.StartNew();
                        solver.BruteForceSolve(puzzle, 0, 0, 0);
                        watch.Stop();
                        averageTime += watch.ElapsedMilliseconds;
                    }
                    if(averageTime < 1000)
                    {
                        sudokuPuzzles[i].Difficulty = "Beginner";
                    }
                    else if (averageTime >= 1000 && averageTime < 2500)
                    {
                        sudokuPuzzles[i].Difficulty = "Intermediate";
                    }
                    else
                    {
                        sudokuPuzzles[i].Difficulty = "Advanced";
                    }
                    doc.Element("SudokuPuzzles").Element("NotStarted").Element(sudokuPuzzles[i].Difficulty).Add(
                        new XElement("puzzle",
                            new XElement("ID", sudokuPuzzles[i].PuzzleID),
                            new XElement("SudokuString", gen.SudokuToString(sudokuPuzzles[i]))
                            )
                        );
                }
                doc.Save(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error with writing: " + ex);
                throw;
            }
        }
    }
}
