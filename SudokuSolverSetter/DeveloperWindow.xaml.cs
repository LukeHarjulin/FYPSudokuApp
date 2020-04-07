using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for DeveloperWindow.xaml
    /// </summary>
    public partial class DeveloperWindow : Window
    {
        private List<TextBox> txtBxList = new List<TextBox>();
        private MainWindow homePage = new MainWindow();
        private PuzzleGenerator gen = new PuzzleGenerator();
        private PuzzleSolver solve = new PuzzleSolver();
        private string currentTime = "";

        public DeveloperWindow()
        {
            InitializeComponent();
            Number_List_combo.Items.Add(1);
            Number_List_combo.Items.Add(10);
            Number_List_combo.Items.Add(25);
            Number_List_combo.Items.Add(50);
            Number_List_combo.Items.Add(100);
            Number_List_combo.Items.Add(500);
            Number_List_combo.Items.Add(1000);
            //Create list of all the cells so that they can be transformed
            txtBxList = new List<TextBox> {  x1y1g1, x1y2g1, x1y3g1, x1y4g2, x1y5g2, x1y6g2, x1y7g3, x1y8g3, x1y9g3,
                                             x2y1g1, x2y2g1, x2y3g1, x2y4g2, x2y5g2, x2y6g2, x2y7g3, x2y8g3, x2y9g3,
                                             x3y1g1, x3y2g1, x3y3g1, x3y4g2, x3y5g2, x3y6g2, x3y7g3, x3y8g3, x3y9g3,
                                             x4y1g4, x4y2g4, x4y3g4, x4y4g5, x4y5g5, x4y6g5, x4y7g6, x4y8g6, x4y9g6,
                                             x5y1g4, x5y2g4, x5y3g4, x5y4g5, x5y5g5, x5y6g5, x5y7g6, x5y8g6, x5y9g6,
                                             x6y1g4, x6y2g4, x6y3g4, x6y4g5, x6y5g5, x6y6g5, x6y7g6, x6y8g6, x6y9g6,
                                             x7y1g7, x7y2g7, x7y3g7, x7y4g8, x7y5g8, x7y6g8, x7y7g9, x7y8g9, x7y9g9,
                                             x8y1g7, x8y2g7, x8y3g7, x8y4g8, x8y5g8, x8y6g8, x8y7g9, x8y8g9, x8y9g9,
                                             x9y1g7, x9y2g7, x9y3g7, x9y4g8, x9y5g8, x9y6g8, x9y7g9, x9y8g9, x9y9g9
            };
            //SudokuGrid grid = gen.Setter();//Calling the automated puzzle generator method to create a puzzle - incomplete

            //Currently using pre-made sudoku puzzles found on sudokuwiki.org to develop my puzzle solver
            //Need to develop a method that takes a csv file/string of numbers and displays it as a puzzle
            #region Manual Puzzle Generation
            /*
            Cell[][] cells = new Cell[9][];
            for (int t = 0; t < cells.Length; t++)
            { cells[t] = new Cell[9]; }
            List<int> candiFiller = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            #region Test Puzzle One
            ///*cells[0][0] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][2] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][5] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[0][7] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[0][8] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][0] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][1] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][2] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][3] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][6] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][0] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][1] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][2] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][4] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][7] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][8] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[3][0] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][2] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[3][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[3][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][3] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][4] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][6] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][8] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][1] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][4] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][5] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][6] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][7] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[6][0] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][1] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][2] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][4] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][5] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[6][7] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[6][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][1] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][2] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][4] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][5] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][6] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            #endregion Test Puzzle One
            #region Test Puzzle Two
            cells[0][0] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            cells[0][1] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            cells[0][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            cells[0][3] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            cells[0][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            cells[0][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            cells[0][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            cells[0][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            cells[0][8] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            cells[1][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            cells[1][1] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            cells[1][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            cells[1][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            cells[1][4] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            cells[1][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            cells[1][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            cells[1][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            cells[1][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            cells[2][0] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            cells[2][1] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            cells[2][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            cells[2][3] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            cells[2][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            cells[2][5] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            cells[2][6] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            cells[2][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            cells[2][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            cells[3][0] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            cells[3][1] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            cells[3][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            cells[3][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            cells[3][4] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            cells[3][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            cells[3][6] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            cells[3][7] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            cells[3][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            cells[4][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            cells[4][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            cells[4][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            cells[4][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            cells[4][4] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            cells[4][5] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            cells[4][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            cells[4][7] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            cells[4][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            cells[5][0] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            cells[5][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            cells[5][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            cells[5][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            cells[5][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            cells[5][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            cells[5][6] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            cells[5][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            cells[5][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            cells[6][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            cells[6][1] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            cells[6][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            cells[6][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            cells[6][4] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            cells[6][5] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            cells[6][6] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            cells[6][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            cells[6][8] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            cells[7][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            cells[7][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            cells[7][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            cells[7][3] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            cells[7][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            cells[7][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            cells[7][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            cells[7][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            cells[7][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            cells[8][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            cells[8][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            cells[8][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            cells[8][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            cells[8][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            cells[8][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            cells[8][6] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            cells[8][7] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            cells[8][8] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            #endregion Test Puzzle Two
            #region Test Puzzle Three
            //cells[0][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][3] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][5] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[0][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[0][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][0] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][1] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][6] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][7] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][1] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][5] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][6] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[3][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][1] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][2] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][4] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[3][7] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[3][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][0] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][8] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][1] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][4] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][6] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][7] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[6][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][2] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][3] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[6][7] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[6][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][1] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][2] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][7] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][8] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][3] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][5] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            #endregion
            #region Test Puzzle Four
            ///*cells[0][0] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][2] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][6] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[0][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[0][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][0] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][3] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][5] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][1] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][2] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[3][0] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][1] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][4] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][6] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[3][7] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[3][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][0] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][3] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][5] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][8] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][1] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][2] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][4] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][7] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][8] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[6][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][6] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[6][7] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[6][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][3] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][5] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][8] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][2] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][6] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][8] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            #endregion
            #region Test Puzzle Five
            //cells[0][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[0][3] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][5] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[0][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[0][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[0][8] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][1] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[1][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][4] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[1][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][7] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[1][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 1, XLocation = 0, YLocation = 0 };
            //cells[2][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][4] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 2, XLocation = 0, YLocation = 0 };
            //cells[2][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[2][8] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 3, XLocation = 0, YLocation = 0 };
            //cells[3][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][1] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[3][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][5] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[3][6] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[3][7] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[3][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][0] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[4][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][4] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[4][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[4][8] = new Cell() { Num = 8, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][1] = new Cell() { Num = 5, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][2] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 4, XLocation = 0, YLocation = 0 };
            //cells[5][3] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 5, XLocation = 0, YLocation = 0 };
            //cells[5][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][7] = new Cell() { Num = 1, Candidates = { }, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[5][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 6, XLocation = 0, YLocation = 0 };
            //cells[6][0] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[6][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][4] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[6][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[6][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[6][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][0] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][1] = new Cell() { Num = 3, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[7][3] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][4] = new Cell() { Num = 6, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][5] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[7][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][7] = new Cell() { Num = 9, Candidates = { }, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[7][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][0] = new Cell() { Num = 2, Candidates = { }, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][1] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][2] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 7, XLocation = 0, YLocation = 0 };
            //cells[8][3] = new Cell() { Num = 4, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][4] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][5] = new Cell() { Num = 7, Candidates = { }, BlockLoc = 8, XLocation = 0, YLocation = 0 };
            //cells[8][6] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][7] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            //cells[8][8] = new Cell() { Num = 0, Candidates = candiFiller, BlockLoc = 9, XLocation = 0, YLocation = 0 };
            #endregion
            for (int x = 0; x < cells.Length; x++)
            {
                for (int y = 0; y < cells[x].Length; y++)
                {
                    cells[x][y].XLocation = x;
                    cells[x][y].YLocation = y;
                    if (cells[x][y].Num != 0)
                    {
                        cells[x][y].ReadOnly = true;
                    }

                }
            }

            SudokuGrid grid = new SudokuGrid()
            {
                PuzzleID = 0,
                Rows = cells
            };
            */
            #endregion

            //PopulateGrid(grid, txtBxList);
            //Clipboard.SetText(gen.SudokuToString(grid));
            //solve.Solver(grid);
            /*Tip for removing numbers.
            - Remove numbers till only one solution is possible. Then to make the puzzle more difficult, remove extra numbers and see if it is still solvable.
             */
        }
        public List<TextBox> PopulateGrid(SudokuGrid grid, List<TextBox> m_txtBxList)
        {
            /*This method populates the Uniform grid and its textboxes with all the given values from 'grid'.
            */
            int x = 0;//row number
            int y = 0;//column number
            for (int i = 0; i < m_txtBxList.Count; i++)
            {
                if (grid.Rows[x][y].Num == '0') //0's are placeholders for when there is no value, so any 0's are turned into textboxes containing the candidate values.
                {
                    m_txtBxList[i].FontSize = 12;
                    m_txtBxList[i].Text = "";
                    for (int c = 0; c < grid.Rows[x][y].Candidates.Count; c++)
                    {
                        m_txtBxList[i].Text += grid.Rows[x][y].Candidates[c].ToString() + " ";
                    }
                    

                }
                else
                {
                    m_txtBxList[i].FontSize = 36;
                    m_txtBxList[i].Text = grid.Rows[x][y].Num.ToString();
                    if (grid.Rows[x][y].ReadOnly == true)//The readonly property ensures that the default given values of the sudoku puzzle remain readonly.
                    {
                        m_txtBxList[i].FontWeight = FontWeights.SemiBold;
                        m_txtBxList[i].IsReadOnly = true;
                    }
                }
                y++;
                if (y == 9)//row needs to increment and column needs to reset to 0 once it reaches the end of the row
                {
                    y = 0;
                    x++;
                }
            }
            return m_txtBxList;
        }
        private void B_Solve_Click(object sender, RoutedEventArgs e)//This button on the interface is used to solve the grid that it is presented
        {
            //Initialising objects
            PuzzleSolver solve = new PuzzleSolver();
            PuzzleGenerator gen = new PuzzleGenerator();
            SudokuGrid gridSolve = new SudokuGrid();
            gridSolve.Rows = new Cell[9][];
            int cellNum = 0;

            //This transforms the text in the boxes to a useable grid object. Resource heavy - alternative method may be developed in improvements
            for (int r = 0; r < gridSolve.Rows.Length; r++)
            {
                gridSolve.Rows[r] = new Cell[9];
                for (int c = 0; c < gridSolve.Rows[r].Length; c++)
                {
                    string blockLoc = txtBxList[cellNum].Name[5].ToString();
                    if (txtBxList[cellNum].Text.Length > 1)
                    {
                        txtBxList[cellNum].Text = "0";
                        gridSolve.Rows[r][c] = new Cell()
                        {
                            Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' },
                            Num = '0',
                            BlockLoc = Convert.ToInt32(blockLoc),
                            XLocation = r,
                            YLocation = c,
                            ReadOnly = false
                        };
                    }
                    else
                    {
                        gridSolve.Rows[r][c] = new Cell()
                        {
                            Candidates = new List<char> { },
                            Num = txtBxList[cellNum].Text[0],
                            BlockLoc = Convert.ToInt32(blockLoc),
                            XLocation = r,
                            YLocation = c,
                            ReadOnly = true
                        };
                    }

                    cellNum++;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int nbCounter = 0;//nbCounter is neighbourcounter
                    gridSolve.Rows[i][j].NeighbourCells = new List<Cell[]>
                    {
                        new Cell[8],
                        new Cell[8],
                        new Cell[8]
                    };
                    for (int k = 0; k < 9; k++)
                    {
                        if (j != k)
                        {
                            gridSolve.Rows[i][j].NeighbourCells[0][nbCounter] = gridSolve.Rows[i][k];//add neighbour in i
                            nbCounter++;
                        }
                    }
                    nbCounter = 0;
                    for (int l = 0; l < 9; l++)
                    {
                        if (l != i)
                        {
                            gridSolve.Rows[i][j].NeighbourCells[1][nbCounter] = gridSolve.Rows[l][j];//add neighbour in column
                            nbCounter++;
                        }
                    }
                    nbCounter = 0;
                    int[] blockIndexes = gen.BlockIndexGetter(gridSolve.Rows[i][j].BlockLoc);

                    for (int x = blockIndexes[0]; x < blockIndexes[0] + 3; x++)
                    {
                        for (int y = blockIndexes[1]; y < blockIndexes[1] + 3; y++)
                        {
                            if (gridSolve.Rows[x][y] != gridSolve.Rows[i][j])
                            {
                                gridSolve.Rows[i][j].NeighbourCells[2][nbCounter] = gridSolve.Rows[x][y];//add neighbour in block
                                nbCounter++;
                            }
                        }
                    }
                }
            }
            char method = '1';
            if (((Button)sender) == Brute_Solve_Obj)
            {
                method = '2';
            }
            int iterations = int.Parse(Number_List_combo.SelectedItem.ToString());
            string puzzleString = gen.SudokuToString(gridSolve);
            double averageTime = 0;
            bool solved = false;
            int difficulty = 0;
            for (int i = 0; i < iterations; i++)
            {
                if (i != 0)
                {
                    int counter = 0;
                    for (int x = 0; x < 9; x++)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            if (puzzleString[counter] == '0')
                            {
                                gridSolve.Rows[x][y].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                            }
                            gridSolve.Rows[x][y].Num = puzzleString[counter];
                            counter++;
                        }
                    }
                }                
                var watch = System.Diagnostics.Stopwatch.StartNew();
                solved = solve.Solver(gridSolve, method);
                if (i==0)
                    difficulty = solve.difficulty;
                watch.Stop();
                averageTime += watch.ElapsedMilliseconds;
            }
            averageTime = averageTime / iterations;
            currentTime = iterations > 1 ? "Average time taken to solve: " + averageTime / 1000 : "Time taken to solve: " + averageTime / 1000;
            
            PopulateGrid(gridSolve, txtBxList);
            if (solved)
            {
                MessageBox.Show("SOLVED\r\n" + currentTime+"\r\nMeasured Difficulty(WIP):"+difficulty);
            }
            else
            {
                MessageBox.Show("FAILED\r\n" + currentTime);
            }

        }
        private void B_Solve1by1_Click(object sender, RoutedEventArgs e)//This button on the interface is used to solve in increments (e.g. once a value is placed into a cell, the solver stops)
        {
            //Initialising objects
            PuzzleSolver solve = new PuzzleSolver();
            List<TextBox> txtBxList = new List<TextBox>
            { x1y1g1, x1y2g1, x1y3g1, x1y4g2, x1y5g2, x1y6g2, x1y7g3, x1y8g3, x1y9g3,
              x2y1g1, x2y2g1, x2y3g1, x2y4g2, x2y5g2, x2y6g2, x2y7g3, x2y8g3, x2y9g3,
              x3y1g1, x3y2g1, x3y3g1, x3y4g2, x3y5g2, x3y6g2, x3y7g3, x3y8g3, x3y9g3,
              x4y1g4, x4y2g4, x4y3g4, x4y4g5, x4y5g5, x4y6g5, x4y7g6, x4y8g6, x4y9g6,
              x5y1g4, x5y2g4, x5y3g4, x5y4g5, x5y5g5, x5y6g5, x5y7g6, x5y8g6, x5y9g6,
              x6y1g4, x6y2g4, x6y3g4, x6y4g5, x6y5g5, x6y6g5, x6y7g6, x6y8g6, x6y9g6,
              x7y1g7, x7y2g7, x7y3g7, x7y4g8, x7y5g8, x7y6g8, x7y7g9, x7y8g9, x7y9g9,
              x8y1g7, x8y2g7, x8y3g7, x8y4g8, x8y5g8, x8y6g8, x8y7g9, x8y8g9, x8y9g9,
              x9y1g7, x9y2g7, x9y3g7, x9y4g8, x9y5g8, x9y6g8, x9y7g9, x9y8g9, x9y9g9
            };
            SudokuGrid gridSolve = new SudokuGrid() { PuzzleID = 0 };//Passes by reference, changed to deep copy
            gridSolve.Rows = new Cell[9][];
            int cellNum = 0;

            //This transforms the text in the boxes to a useable grid object. Resource heavy - alternative method may be developed in improvements
            for (int r = 0; r < gridSolve.Rows.Length; r++)
            {
                gridSolve.Rows[r] = new Cell[9];
                for (int c = 0; c < gridSolve.Rows[r].Length; c++)
                {
                    string blockLoc = txtBxList[cellNum].Name[5].ToString();
                    if (txtBxList[cellNum].Text.Length > 1)
                    {
                        List<char> candiList = new List<char>();
                        candiList.AddRange(txtBxList[cellNum].Text.ToCharArray());
                        candiList.RemoveAll(item => item == ' ');
                        gridSolve.Rows[r][c] = new Cell()
                        {

                            Candidates = candiList,
                            Num = '0',
                            BlockLoc = Convert.ToInt32(blockLoc),
                            XLocation = r,
                            YLocation = c
                        };
                    }
                    else
                    {
                        gridSolve.Rows[r][c] = new Cell()
                        {
                            Candidates = new List<char> { },
                            Num = txtBxList[cellNum].Text[0],
                            BlockLoc = Convert.ToInt32(blockLoc),
                            XLocation = r,
                            YLocation = c
                        };
                    }

                    cellNum++;
                }
            }

            gridSolve = solve.SolveCellByCell(gridSolve);
            PopulateGrid(gridSolve, txtBxList);
        }

        private void BruteForceChckBx_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            homePage = new MainWindow();
            homePage.Show();
        }

        private void Window_Close(object sender, EventArgs e)
        {
            homePage = new MainWindow();
            homePage.Show();
        }

        private void Create_Store_Puzzles_btn_Click(object sender, RoutedEventArgs e)
        {
            CreatePuzzles createPuzzles = new CreatePuzzles();
            createPuzzles.Show();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            ImportPuzzle importPuzzle = new ImportPuzzle();
            
            if (importPuzzle.ShowDialog() == true)
            {
                string importStr = importPuzzle.puzzleStr;
                Regex rgx = new Regex(@"[1-9]");
                for (int i = 0; i < 81; i++)
                {
                    if (!rgx.IsMatch(importStr[i].ToString()))
                    {
                        txtBxList[i].FontSize = 12;
                        txtBxList[i].Text = "1 2 3 4 5 6 7 8 9";
                        txtBxList[i].FontWeight = FontWeights.Normal;
                    }
                    else
                    {
                        txtBxList[i].FontSize = 36;
                        txtBxList[i].Text = importStr[i].ToString();
                        txtBxList[i].FontWeight = FontWeights.SemiBold;
                    }
                }
            }
            
        }

        private void GeneratePuzzle_Click(object sender, RoutedEventArgs e)
        {
            SudokuGrid grid = gen.Setter();//Calling the automated puzzle generator method to create a puzzle
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid.Rows[i][j].ReadOnly == true)
                    {
                        grid.Rows[i][j].Candidates = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                    }
                    else
                    {
                        grid.Rows[i][j].Candidates.Clear();
                    }
                }
            }
            txtBxList = PopulateGrid(grid, txtBxList);
            Clipboard.SetText(gen.SudokuToString(grid));
        }

        private void BruteSolve_char_Click(object sender, RoutedEventArgs e)
        {
            PuzzleSolverCharVer solver = new PuzzleSolverCharVer();
            char[][] puzzle = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
            int counter = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (txtBxList[counter].Text.Length > 1 || txtBxList[counter].Text == "0" || txtBxList[counter].Text.Length == 0)
                    {
                        puzzle[i][j] = '0';
                    }
                    else
                    {
                        puzzle[i][j] = txtBxList[counter].Text[0];
                    }
                    counter++;
                }
            }
            char method = '1';
            if (((Button)sender) == Brute_Solve_char)
            {
                method = '2';
            }
            int iterations = int.Parse(Number_List_combo.SelectedItem.ToString());
            char[][] puzzleTemp = new char[9][];
            double averageTime = 0;
            bool solved = false;
            for (int i = 0; i < iterations; i++)
            {
                puzzleTemp = new char[9][] { new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9], new char[9] };
                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        puzzleTemp[x][y] = puzzle[x][y];
                    }
                }
                var watch = System.Diagnostics.Stopwatch.StartNew();
                solved = solver.Solvers(puzzleTemp, '2');
                watch.Stop();
                averageTime += watch.ElapsedMilliseconds;
            }
            averageTime = averageTime / iterations;
            currentTime = iterations > 1 ? "Average time taken to solve: " + averageTime / 1000 : "Time taken to solve: " + averageTime / 1000;
            counter = 0;
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (puzzleTemp[x][y] != '0')
                    {
                        txtBxList[counter].FontSize = 36;
                        txtBxList[counter].Text = puzzleTemp[x][y].ToString();
                    }
                    else//should never really happpen
                    {
                        txtBxList[counter].Text = "0";
                    }
                    counter++;
                }
            }
            if (solved)
            {
                MessageBox.Show("SOLVED\r\n" + currentTime);
            }
            else
            {
                MessageBox.Show("FAILED\r\n" + currentTime);
            }
        }
    }
}
