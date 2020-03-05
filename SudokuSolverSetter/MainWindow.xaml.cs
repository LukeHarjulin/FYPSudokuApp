﻿using System;
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

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void DeveloperMode_Button_Click(object sender, RoutedEventArgs e)
        {
            PasswordBox passBox = new PasswordBox();

            if (passBox.ShowDialog() == true)
            {
                DeveloperWindow developerWindow = new DeveloperWindow();
                developerWindow.Show();
                this.Hide();
            }
        }

        private void Quit_btn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
