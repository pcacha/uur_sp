using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF_Malovani.Models;
using WPF_Malovani.Utility;

namespace WPF_Malovani.ViewModels
{
    public class StartupVM
    {
        public string PixelsX { get; set; }
        public string PixelsY { get; set; }        
        public List<string> LatestProjects { get; private set; }
        public string DefaultDirectory { get; private set; }

        public RelayCommand<IClosable> NewProject { get; private set; }
        public RelayCommand<IClosable> OpenProject { get; private set; }    
        public RelayCommand<OpenProjectParameter> OpenSelectedProject { get; private set; }

        public StartupVM()
        {
            DefaultDirectory = LoadDefaultDirectory();           

            PixelsX = "1280";
            PixelsY = "720";

            NewProject = new RelayCommand<IClosable>(NewProjectExecute);
            OpenProject = new RelayCommand<IClosable>(OpenProjectExecute);
            OpenSelectedProject = new RelayCommand<OpenProjectParameter>(OpenSelectedProjectExecute);
        }       

        private void NewProjectExecute(IClosable win)
        {
            if (!CheckPixels())
            {
                MessageBox.Show("Velikost plátna v pixelech není validní (min = 10, max = 1920)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PaintWindow pw = new PaintWindow();
            pw.DataContext = new PaintVM(int.Parse(PixelsX), int.Parse(PixelsY));
            pw.Show();
            win.Close();
        }

        private void OpenProjectExecute(IClosable win)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML soubor s projektem (*.xml)|*.xml";
            if (ofd.ShowDialog() == true)
            {                
                PaintWindow pw = new PaintWindow();
                pw.DataContext = new PaintVM(ofd.FileName);
                pw.Show();
                win.Close();
            }
        }        

        private bool CheckPixels()
        {
            int x = 0;
            int y = 0;

            if(!int.TryParse(PixelsX, out x) || !int.TryParse(PixelsY, out y))
            {
                return false;
            }

            if(x < 1921 && y < 1921 && x > 9 && y > 9)
            {
                return true;
            }

            return false;
        }

        private void OpenSelectedProjectExecute(OpenProjectParameter parameter)
        {
            PaintWindow pw = new PaintWindow();           
            pw.DataContext = new PaintVM(Path.Combine(DefaultDirectory, (parameter.FileName + ".xml")));
            pw.Show();
            parameter.Window.Close();
        }

        private string LoadDefaultDirectory()
        {
            string path = Properties.Settings.Default.DefaultDirPath;
            if(string.IsNullOrEmpty(path))
            {
                return null;
            }
            return path;
        }
    }
}
