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
        public CreatePuzzles() => InitializeComponent(); 
        public CreatePuzzles(int addLimit)
        {
            InitializeComponent();
            Number_List_combo.Items.Add(1);
            if (addLimit > 10)
            {
                Number_List_combo.Items.Add(5);
                Number_List_combo.Items.Add(10);
                Number_List_combo.Items.Add(25);
                Number_List_combo.Items.Add(50);
                Number_List_combo.Items.Add(100);
                Number_List_combo.Items.Add(250);
                Number_List_combo.Items.Add(500);
                Number_List_combo.Items.Add(750);
                Number_List_combo.Items.Add(1000);
                Number_List_combo.Items.Add(2000);
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
        public void GeneratePuzzles(int numPuzzles)
        {
            List<SudokuGrid> sudokuPuzzles = new List<SudokuGrid>();
            PuzzleGenerator gen = new PuzzleGenerator();
            try
            {
                XDocument doc;
                string filename = @"Puzzles/SudokuPuzzles.xml";
                int seed = 0;
                if (File.Exists(filename))
                {
                    doc = XDocument.Load(filename);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filename);
                    XmlNode types = xmlDoc.DocumentElement.SelectSingleNode("/SudokuPuzzles");
                    XmlNode notStarted = types.FirstChild;
                    foreach (XmlNode item in notStarted.ChildNodes)
                    {
                        seed += item.ChildNodes.Count;
                    }
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
                StreamWriter ratingWrite = new StreamWriter(@"Puzzles/ratings.txt",true);
                StreamWriter difficWrite = new StreamWriter(@"Puzzles/difficulties.txt", true); 
                StreamWriter givensWrite = new StreamWriter(@"Puzzles/givens.txt", true);
                StreamWriter seedsWrite = new StreamWriter(@"Puzzles/seeds.txt", true);
                #region Strategy Files
                StreamWriter NS = new StreamWriter(@"Puzzles/StratsCounts/nakedsingles.txt", true), HS = new StreamWriter(@"Puzzles/StratsCounts/hiddensingles.txt", true), NP = new StreamWriter(@"Puzzles/StratsCounts/nakedpair.txt", true),
                HP = new StreamWriter(@"Puzzles/StratsCounts/hiddenpair.txt", true), PP = new StreamWriter(@"Puzzles/StratsCounts/pointline.txt", true), BLR = new StreamWriter(@"Puzzles/StratsCounts/blocklinereduc.txt", true), NT = new StreamWriter(@"Puzzles/StratsCounts/nakedtriple.txt", true),
                HT = new StreamWriter(@"Puzzles/StratsCounts/hiddentriple.txt", true), XW = new StreamWriter(@"Puzzles/StratsCounts/xwing.txt", true), YW = new StreamWriter(@"Puzzles/StratsCounts/ywing.txt", true), XYZ = new StreamWriter(@"Puzzles/StratsCounts/xyzwing.txt", true),
                SC = new StreamWriter(@"Puzzles/StratsCounts/singlechains.txt", true), UR1 = new StreamWriter(@"Puzzles/StratsCounts/uniquerecttyp1.txt", true), BT = new StreamWriter(@"Puzzles/StratsCounts/backtrack.txt", true);
                #endregion
                Stopwatch Timer = new Stopwatch();
                Timer.Start();
                for (int i = 0; i < numPuzzles; i++)
                {
                    sudokuPuzzles.Add(gen.Setter());
                    string puzzleString = gen.GridToString(sudokuPuzzles[i]);
                    PuzzleSolverObjDS solver = new PuzzleSolverObjDS();
                    long rating = GetDifficulty(sudokuPuzzles[i], puzzleString, solver);
                    doc.Element("SudokuPuzzles").Element("NotStarted").Element(sudokuPuzzles[i].Difficulty).Add(
                        new XElement("puzzle",
                            new XElement("Seed", ++seed),
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
                    ratingWrite.WriteLine(rating);
                    givensWrite.WriteLine(givens);
                    seedsWrite.WriteLine(seed);
                    NS.WriteLine(solver.g_StrategyCount[1]);HS.WriteLine(solver.g_StrategyCount[2]);NP.WriteLine(solver.g_StrategyCount[3]);
                    HP.WriteLine(solver.g_StrategyCount[4]);PP.WriteLine(solver.g_StrategyCount[5]);BLR.WriteLine(solver.g_StrategyCount[6]);
                    NT.WriteLine(solver.g_StrategyCount[7]);HT.WriteLine(solver.g_StrategyCount[8]);XW.WriteLine(solver.g_StrategyCount[9]);
                    YW.WriteLine(solver.g_StrategyCount[10]);XYZ.WriteLine(solver.g_StrategyCount[11]);SC.WriteLine(solver.g_StrategyCount[12]);
                    UR1.WriteLine(solver.g_StrategyCount[13]);BT.WriteLine(solver.g_StrategyCount[0]);
                }
                Timer.Stop();
                ratingWrite.Close();
                seedsWrite.Close();
                difficWrite.Close();
                givensWrite.Close();
                NS.Close();HS.Close();NP.Close();HP.Close();PP.Close();BLR.Close();NT.Close();
                HT.Close();XW.Close();YW.Close();XYZ.Close();SC.Close();UR1.Close();BT.Close();
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
            GeneratePuzzles(int.Parse(Number_List_combo.SelectedItem.ToString()));
        }
        #endregion
    }
}
