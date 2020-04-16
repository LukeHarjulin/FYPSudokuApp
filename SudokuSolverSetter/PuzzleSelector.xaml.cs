using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for PuzzleSelector.xaml
    /// </summary>
    public partial class PuzzleSelector : Window
    {
        #region Initialisation
        private MainWindow homePage = new MainWindow();
        private List<TextBox> g_txtBxList = new List<TextBox>();
        private List<string> g_puzzles = new List<string>();
        private TextBox g_selectedTxBx = new TextBox { Text = "." };
        private Brush selectColour = new SolidColorBrush(Color.FromArgb(178, 219, 255, 192));
        public PuzzleSelector()
        {
            InitializeComponent();
            g_txtBxList = new List<TextBox> {
                bx1,bx2,bx3,bx4,bx5,bx6,bx7,bx8,bx9,
                bx10,bx11,bx12,bx13,bx14,bx15,bx16,bx17,bx18,bx19,
                bx20,bx21,bx22,bx23,bx24,bx25,bx26,bx27,bx28,bx29,
                bx30,bx31,bx32,bx33,bx34,bx35,bx36,bx37,bx38,bx39,
                bx40,bx41,bx42,bx43,bx44,bx45,bx46,bx47,bx48,bx49,
                bx50,bx51,bx52,bx53,bx54,bx55,bx56,bx57,bx58,bx59,
                bx60,bx61,bx62,bx63,bx64,bx65,bx66,bx67,bx68,bx69,
                bx70,bx71,bx72,bx73,bx74,bx75,bx76,bx77,bx78,bx79,
                bx80,bx81
            };
            AddPuzzlesExpanders();
        }
        #endregion
        #region Functions/Methods
        /// <summary>
        /// Updates the combo box with all the puzzles from the XML file, displayed and ordered by their rating
        /// </summary>
        public void AddPuzzlesExpanders()
        {
            string fileName = @"Symmetric/SudokuPuzzles.xml";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(fileName);
                XmlNode sudokuPuzzles = doc.DocumentElement.SelectSingleNode("/SudokuPuzzles");
                XmlNodeList puzzleLabels = sudokuPuzzles.ChildNodes;
                string sudokuString = "";
                foreach (XmlNode label in puzzleLabels)
                {
                    XmlNodeList difficulties = label.ChildNodes;
                    foreach (XmlNode difficulty in difficulties)
                    {
                        foreach (XmlNode puzzle in difficulty)
                        {
                            int difficulty_Num = 0;
                            Border border = new Border
                            {
                                BorderThickness = new Thickness(2),
                                BorderBrush = Brushes.DarkGray
                            };
                            
                            TextBox textBlock = new TextBox { Background = Brushes.Transparent, FontSize = 16, TextWrapping = TextWrapping.Wrap, Padding = new Thickness(5, 5, 5, 1), IsReadOnly = true, Cursor = Cursors.Hand, FontFamily = new FontFamily("Verdana")};
                            if (label.Name == "Started" || label.Name == "Completed")
                            {
                                sudokuString = puzzle["SudokuString"].InnerText;

                                textBlock.Text = "Rating: " + puzzle["DifficultyRating"].InnerText + "\r\nDifficulty: " + difficulty.Name + "\r\nElapsed Time: " + puzzle["TimeTaken"].InnerText + "\r\nLast Played: " + puzzle["Date"].InnerText;
                                border.Child = textBlock;
                                if (label.Name == "Started")
                                    Started_STKPNL.Children.Add(border);
                                else
                                    Completed_STKPNL.Children.Add(border);
                                
                            }
                            else
                            {
                                sudokuString = puzzle["SudokuString"].InnerText;
                                textBlock.Text = "Rating: " + puzzle["DifficultyRating"].InnerText;
                                border.Child = textBlock;
                                switch (difficulty.Name)
                                {
                                    case "Beginner":
                                        Beginner_STKPNL.Children.Add(border);
                                        difficulty_Num = 0;
                                        break;
                                    case "Moderate":
                                        Moderate_STKPNL.Children.Add(border);
                                        difficulty_Num = 1;
                                        break;
                                    case "Advanced":
                                        Advanced_STKPNL.Children.Add(border);
                                        difficulty_Num = 2;
                                        break;
                                    case "Extreme":
                                        Extreme_STKPNL.Children.Add(border);
                                        difficulty_Num = 3;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            int givens = 0;
                            textBlock.Name = "n"+difficulty_Num+"_" + g_puzzles.Count.ToString();
                            g_puzzles.Add(sudokuString);
                            for (int i = 0; i < sudokuString.Length; i++)
                            {
                                if (sudokuString[i] != '0')
                                {
                                    if (label.Name != "Started")
                                    {
                                        givens++;
                                    }
                                }
                            }
                            if (label.Name != "Started")
                            {
                                textBlock.Text += "\r\n# of Starting Numbers: " + givens;
                            }
                            textBlock.GotFocus += new RoutedEventHandler(Selected_Puzzle);
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Warning! No existing puzzles found in folder.");
            }
        }
        public void ReactToSelectedPuzzle(TextBox sender)
        {
            //Previous selected puzzle set to normal
            if (g_selectedTxBx.Text != ".")
            {
                g_selectedTxBx.Background = Brushes.Transparent;
            }

            g_selectedTxBx = sender;
            g_selectedTxBx.Background = selectColour;

            string index = g_selectedTxBx.Name[3].ToString();
            if (g_selectedTxBx.Name.Length > 4)
            {
                for (int k = 4; k < g_selectedTxBx.Name.Length; k++)
                {
                    index += g_selectedTxBx.Name[k].ToString();
                }
            }

            string sudokuString = g_puzzles[int.Parse(index)];
            g_txtBxList[0].Text = "";
            for (int i = 0, counter = 0; counter < sudokuString.Length; counter++)
            {
                if (sudokuString.Contains('.'))
                {
                    counter++;
                }
                if (sudokuString.Contains('_'))
                {
                    if (sudokuString[counter] == '|')
                    {
                        g_txtBxList[i].FontWeight = FontWeights.SemiBold;
                        counter++;
                    }
                    else if (sudokuString[counter] == '-')
                    {
                        g_txtBxList[i].FontSize = 16;
                        counter++;
                    }
                    if (counter == sudokuString.Length)
                    {
                        break;
                    }
                    if (sudokuString[counter] == '_')
                    {
                        i++;
                        g_txtBxList[i].Text = "";
                        counter++;
                    }
                    if (sudokuString[counter] != '0')
                    {
                        g_txtBxList[i].Text += sudokuString[counter].ToString();
                        if (g_txtBxList[i].Text.Length > 1)
                        {
                            g_txtBxList[i].FontSize = 16;
                        }
                        else
                        {
                            g_txtBxList[i].FontSize = 36;
                        }
                    }
                    else
                    {
                        g_txtBxList[i].Text = "";
                    }
                    
                }
                else
                {
                    if (sudokuString[counter] != '0')
                    {
                        g_txtBxList[counter].Text = sudokuString[counter].ToString();
                        g_txtBxList[counter].FontSize = 36;
                        g_txtBxList[counter].FontWeight = FontWeights.SemiBold;
                    }
                    else
                    {
                        g_txtBxList[counter].Text = "";
                    }
                }
                
            }
        }
        public void OpenSelectedPuzzle()
        {
            if (g_selectedTxBx.Text != ".")
            {
                string difficulty = "";
                switch (g_selectedTxBx.Name[1])
                {
                    case '0':
                        difficulty = "Beginner";
                        break;
                    case '1':
                        difficulty = "Moderate";
                        break;
                    case '2':
                        difficulty = "Advanced";
                        break;
                    case '3':
                        difficulty = "Extreme";
                        break;
                    default:
                        break;
                }
                string index = g_selectedTxBx.Name[3].ToString();
                if (g_selectedTxBx.Name.Length > 4)
                {
                    for (int k = 4; k < g_selectedTxBx.Name.Length; k++)
                    {
                        index += g_selectedTxBx.Name[k].ToString();
                    }
                }
                PlaySudoku play = new PlaySudoku(difficulty, g_puzzles[int.Parse(index)]);
                Hide();
                play.Owner = this;
                play.Show();
            }
        }
        #endregion
        #region Event Handlers
        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            homePage.Owner = this;
            homePage.Show();
        }
        private void Window_Closing(object sender, EventArgs e)
        {
            homePage.Show();
        }

        private void SelectPuzzle_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedPuzzle();
        }
        private void Selected_Puzzle(object sender, RoutedEventArgs e)
        {
            ReactToSelectedPuzzle((TextBox)sender);
        }
        #endregion
    }
}
