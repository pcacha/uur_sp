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
using WPF_Malovani.ViewModels;
using WPF_Malovani.Views;

namespace WPF_Malovani.Views
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window, IClosable
    {
        private bool lightMode;

        public StartupWindow()
        {
            InitializeComponent();
            DataContext = new StartupVM();
            lightMode = true;
        }

        private void mode_Click(object sender, RoutedEventArgs e)
        {
            if(lightMode)
            {
                Application.Current.Resources["Background"] = new SolidColorBrush(Colors.Gray);
                Application.Current.Resources["Foreground"] = new SolidColorBrush(Colors.LightGray);               
                Application.Current.Resources["FontColor"] = new SolidColorBrush(Colors.DarkBlue);
                Application.Current.Resources["IsMouseOverColor"] = new SolidColorBrush(Colors.Blue);
                Application.Current.Resources["ControlsPanel"] = new SolidColorBrush(Colors.WhiteSmoke);                
                Application.Current.Resources["ColorPickBackground"] = new SolidColorBrush(Colors.WhiteSmoke);
                Application.Current.Resources["FontPickBackground"] = new SolidColorBrush(Colors.LightGray);               
                Application.Current.Resources["TreeViewBackground"] = new SolidColorBrush(Colors.WhiteSmoke);
                Application.Current.Resources["BackgroundTB"] = new SolidColorBrush(Colors.White);
                Application.Current.Resources["SelectedForeground"] = new SolidColorBrush(Colors.DarkRed);

                lightMode = false;
                modeTB.Text = "Světlý mód";
            }            
            else
            {
                Application.Current.Resources["Background"] = new SolidColorBrush(Colors.SkyBlue);
                Application.Current.Resources["Foreground"] = new SolidColorBrush(Colors.White);               
                Application.Current.Resources["FontColor"] = new SolidColorBrush(Colors.RoyalBlue);
                Application.Current.Resources["IsMouseOverColor"] = new SolidColorBrush(Colors.DarkBlue);
                Application.Current.Resources["ControlsPanel"] = new SolidColorBrush(Colors.WhiteSmoke);
                Application.Current.Resources["LabelColor"] = new SolidColorBrush(Colors.Black);
                Application.Current.Resources["PixelsAmount"] = new SolidColorBrush(Colors.Black);
                Application.Current.Resources["ColorPickBackground"] = new SolidColorBrush(Colors.Wheat);
                Application.Current.Resources["FontPickBackground"] = new SolidColorBrush(Colors.White);              
                Application.Current.Resources["TreeViewBackground"] = new SolidColorBrush(Colors.DarkGray);
                Application.Current.Resources["BackgroundTB"] = new SolidColorBrush(Colors.LightGray);
                Application.Current.Resources["SelectedForeground"] = new SolidColorBrush(Colors.Red);

                lightMode = true;
                modeTB.Text = "Tmavý mód";
            }
        }
    }
}
