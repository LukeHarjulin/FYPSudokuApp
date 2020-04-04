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
            PuzzleGenerator generator = new PuzzleGenerator();
            PuzzleSolver solver = new PuzzleSolver();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            long averageTime = 0;
            int numPuzzles = int.Parse(Number_List_combo.SelectedItem.ToString());

            for (int i = 0; i < numPuzzles; i++)
            {
                sudokuPuzzles.Add(generator.Setter());
                char[][] puzzle = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
                for (int x = 0; x < 10; x++)
                {
                    puzzle = SudokuGridToArray(sudokuPuzzles[i], puzzle);
                    watch = Stopwatch.StartNew();
                    solver.BruteForceSolve_array(puzzle, 0, 0, 0);
                    watch.Stop();
                    averageTime += watch.ElapsedMilliseconds;
                }

            }
            
            
            using (XmlWriter writer = XmlWriter.Create("puzzles.xml"))
            {
                
                writer.WriteStartDocument();
                writer.WriteStartElement("\r\nSudokuPuzzles");
                writer.WriteStartElement("\r\nUnsolvedPuzzles");

                if (Difficulty_ComboBox.SelectedIndex == 0)
                {
                    writer.WriteStartElement("\r\nDifficulty_1");
                    foreach (SudokuGrid puzzle in sudokuPuzzles)
                    {
                        writer.WriteStartElement("\r\nPuzzle");
                        writer.WriteElementString("\r\nID", puzzle.PuzzleID.ToString());
                        writer.WriteElementString("\r\nPuzzleString", generator.SudokuToString(puzzle));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                else if (Difficulty_ComboBox.SelectedIndex == 1)
                {
                    writer.WriteStartElement("\r\nDifficulty_2");
                    foreach (SudokuGrid puzzle in sudokuPuzzles)
                    {
                        writer.WriteStartElement("\r\nPuzzle");
                        writer.WriteElementString("\r\nID", puzzle.PuzzleID.ToString());
                        writer.WriteElementString("\r\nPuzzleString", generator.SudokuToString(puzzle));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement("\r\nDifficulty_3");
                    foreach (SudokuGrid puzzle in sudokuPuzzles)
                    {
                        writer.WriteStartElement("\r\nPuzzle");
                        writer.WriteElementString("\r\nID", puzzle.PuzzleID.ToString());
                        writer.WriteElementString("\r\nPuzzleString", generator.SudokuToString(puzzle));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }
}
