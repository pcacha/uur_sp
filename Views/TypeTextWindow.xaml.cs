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
using WPF_Malovani.Models;

namespace WPF_Malovani.Views
{
    /// <summary>
    /// Interaction logic for TypeTextWindow.xaml
    /// </summary>
    public partial class TypeTextWindow : Window, IClosable
    {
        public TypeTextWindow()
        {
            InitializeComponent();
        }
    }
}
