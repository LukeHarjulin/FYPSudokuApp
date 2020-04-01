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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<SudokuGrid> sudokuPuzzles = new List<SudokuGrid>();
            PuzzleGenerator generator = new PuzzleGenerator();
            try
            {
                for (int i = 0; i < Number_List_combo.Items.Count; i++)
                {
                    sudokuPuzzles.Add(generator.Setter());
                }
            }
            catch (Exception exc)
            {
                throw;
            }
            
            using (XmlWriter writer = XmlWriter.Create("puzzles.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("SudokuPuzzles");
                writer.WriteStartElement("UnsolvedPuzzles");

                if (Difficulty_ComboBox.SelectedIndex == 0)
                {
                    writer.WriteStartElement("Difficulty_1");
                    foreach (SudokuGrid puzzle in sudokuPuzzles)
                    {
                        writer.WriteStartElement("Puzzle");
                        writer.WriteElementString("ID", "1");
                        writer.WriteElementString("PuzzleString", generator.SudokuToString(puzzle));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                else if (Difficulty_ComboBox.SelectedIndex == 1)
                {
                    writer.WriteStartElement("Difficulty_2");
                    foreach (SudokuGrid puzzle in sudokuPuzzles)
                    {
                        writer.WriteStartElement("Puzzle");
                        writer.WriteElementString("ID", "1");
                        writer.WriteElementString("PuzzleString", generator.SudokuToString(puzzle));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement("Difficulty_3");
                    foreach (SudokuGrid puzzle in sudokuPuzzles)
                    {
                        writer.WriteStartElement("Puzzle");
                        writer.WriteElementString("ID", "1");
                        writer.WriteElementString("PuzzleString", generator.SudokuToString(puzzle));
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
