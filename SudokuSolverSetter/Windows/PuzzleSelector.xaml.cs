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
            if (Started_STKPNL.Children.Count == 0)
            {
                Beginner_Expander.IsExpanded = true;
            }
            else
            {
                Started_Expander.IsExpanded = true;
            }
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
                        int counter = 0;
                        List<List<string>> allPuzzles = new List<List<string>>();
                        foreach (XmlNode puzzle in difficulty)
                        {
                            List<string> pzle = new List<string>() { puzzle["SudokuString"].InnerText, puzzle["DifficultyRating"].InnerText };
                            if (label.Name == "Started" || label.Name == "Completed")
                            {
                                pzle.Add(puzzle["TimeTaken"].InnerText);
                                pzle.Add(puzzle["Date"].InnerText);
                            }
                            ///Sort puzzles by rating!
                            int rating = int.Parse(pzle[1]), i = 0;
                            bool added = false;
                            if (allPuzzles.Count > 0)
                            {
                                for (i = 0; i < allPuzzles.Count; i++)
                                {
                                    if (rating < int.Parse(allPuzzles[i][1]))
                                    {
                                        allPuzzles.Insert(i, pzle);
                                        added = true;
                                        break;
                                    }
                                }
                                if (!added)
                                    allPuzzles.Add(pzle);
                            }
                            else
                                allPuzzles.Add(pzle);
                            if (++counter == 500)
                                break;
                        }
                        for (int n = 0; n < allPuzzles.Count; n++)
                        {
                            int difficulty_Num = 0;
                            Border border = new Border
                            {
                                BorderThickness = new Thickness(2),
                                BorderBrush = Brushes.DarkGray
                            };
                            sudokuString = allPuzzles[n][0];
                            TextBox textBox = new TextBox { Background = Brushes.Transparent, FontSize = 16, TextWrapping = TextWrapping.Wrap, Padding = new Thickness(5, 5, 5, 1), IsReadOnly = true, Cursor = Cursors.Hand, FontFamily = new FontFamily("Verdana") };
                            if (label.Name == "Started" || label.Name == "Completed")
                            {
                                textBox.Text = "Rating: " + allPuzzles[n][1] + "\r\nDifficulty: " + difficulty.Name + "\r\nElapsed Time: " + allPuzzles[n][2] + "\r\nLast Played: " + allPuzzles[n][3];
                                border.Child = textBox;
                                if (label.Name == "Started")
                                    Started_STKPNL.Children.Add(border);
                                else
                                    Completed_STKPNL.Children.Add(border);
                                switch (difficulty.Name)
                                {
                                    case "Beginner":
                                        difficulty_Num = 0;
                                        break;
                                    case "Moderate":
                                        difficulty_Num = 1;
                                        break;
                                    case "Advanced":
                                        difficulty_Num = 2;
                                        break;
                                    case "Extreme":
                                        difficulty_Num = 3;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                textBox.Text = "Rating: " + allPuzzles[n][1];
                                border.Child = textBox;
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
                            textBox.Name = "n" + difficulty_Num + "_" + g_puzzles.Count.ToString();
                            g_puzzles.Add(sudokuString);
                            if (label.Name != "Started" && label.Name != "Completed")
                            {
                                for (int i = 0; i < sudokuString.Length; i++)
                                {
                                    if (sudokuString[i] != '0')
                                    {
                                        givens++;
                                    }
                                }
                                textBox.Text += "\r\n# of Starting Numbers: " + givens;
                            }
                            textBox.GotFocus += new RoutedEventHandler(Selected_Puzzle);
                            textBox.MouseEnter += new MouseEventHandler(MouseEnter_Textbox);
                            textBox.MouseLeave += new MouseEventHandler(MouseLeave_Textbox);
                        }
                    }
                }
                if (Started_STKPNL.Children.Count > 0)
                {
                    g_selectedTxBx = (TextBox)((Border)Started_STKPNL.Children[0]).Child;
                    ReactToSelectedPuzzle(g_selectedTxBx);
                }
                else if (Beginner_STKPNL.Children.Count > 0)
                {
                    g_selectedTxBx = (TextBox)((Border)Beginner_STKPNL.Children[0]).Child;
                    ReactToSelectedPuzzle(g_selectedTxBx);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Warning! No existing puzzles found in folder.");
            }
        }

        
        /// <summary>
        /// Highlight and displays selected puzzle
        /// </summary>
        /// <param name="sender"></param>
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
                if (sudokuString[counter] == '.')
                {
                    counter++;
                }
                if (sudokuString.Contains('_'))
                {
                    if (sudokuString[counter] == '|')
                    {
                        g_txtBxList[i].FontWeight = FontWeights.Bold;
                        counter++;
                    }
                    else if (sudokuString[counter] == '-')
                    {
                        g_txtBxList[i].FontSize = 16;
                        g_txtBxList[i].FontWeight = FontWeights.Normal;
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
                        g_txtBxList[i].Text = sudokuString[counter].ToString();
                        g_txtBxList[i].FontSize = 36;
                        g_txtBxList[i].FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        g_txtBxList[i].Text = "";
                    }
                    i++;
                }
                
            }
        }
        /// <summary>
        /// Redirects user to play sudoku window with selected puzzle
        /// </summary>
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
                PlaySudoku play = new PlaySudoku(difficulty, g_puzzles[int.Parse(index)])
                {
                    Owner = Owner
                };
                Hide();
                play.ShowDialog();
                Close();
            }
        }
        #endregion
        #region Event Handlers
        private void MouseLeave_Textbox(object sender, MouseEventArgs e)
        {
            if ((TextBox)sender != g_selectedTxBx)
            {
                ((TextBox)sender).Background = Brushes.Transparent;
            }
        }
        private void MouseEnter_Textbox(object sender, MouseEventArgs e)
        {
            if ((TextBox)sender != g_selectedTxBx)
            {
                ((TextBox)sender).Background = new SolidColorBrush(Color.FromArgb(200, 255, 224, 223));
            }
            
        }
        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void SelectPuzzle_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedPuzzle();
        }
        private void Selected_Puzzle(object sender, RoutedEventArgs e)
        {
            ReactToSelectedPuzzle((TextBox)sender);
        }
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (((StackPanel)((ScrollViewer)((Expander)sender).Content).Content).Children.Count > 0)
            {
                if (g_selectedTxBx.Text != ".")
                    g_selectedTxBx.Background = Brushes.Transparent;
                g_selectedTxBx = (TextBox)((Border)((StackPanel)((ScrollViewer)((Expander)sender).Content).Content).Children[0]).Child;
                ReactToSelectedPuzzle(g_selectedTxBx);
            }
            if ((Expander)sender != Started_Expander)
            {
                Started_Expander.IsExpanded = false;
            }
            if ((Expander)sender != Beginner_Expander)
            {
                Beginner_Expander.IsExpanded = false;
            }
            if ((Expander)sender != Moderate_Expander)
            {
                Moderate_Expander.IsExpanded = false;
            }
            if ((Expander)sender != Advanced_Expander)
            {
                Advanced_Expander.IsExpanded = false;
            }
            if ((Expander)sender != Extreme_Expander)
            {
                Extreme_Expander.IsExpanded = false;
            }
            if ((Expander)sender != Completed_Expander)
            {
                Completed_Expander.IsExpanded = false;
            }
        }
        #endregion
    }
}
