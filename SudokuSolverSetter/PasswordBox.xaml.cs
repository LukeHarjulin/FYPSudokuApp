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

namespace SudokuSolverSetter
{
    /// <summary>
    /// Interaction logic for PasswordBox.xaml
    /// </summary>
    public partial class PasswordBox : Window
    {
        public PasswordBox()
        {
            InitializeComponent();
        }

        public string PasswordText
        {
            get { return PassBx.Password; }
            set { PassBx.Password = value; }
        }
        private void Enter_btn_Click(object sender, RoutedEventArgs e)
        {
            if (PassBx.Password == "iamadeveloper" || PassBx.Password == "lazy")
            {
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
            
        }
    }
}
