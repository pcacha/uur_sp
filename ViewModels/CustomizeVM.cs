using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPF_Malovani.Models;
using WPF_Malovani.Utility;

namespace WPF_Malovani.ViewModels
{
    public class CustomizeVM
    {
        public Color SelectedColor { get; set; }
        public FontFamily SelectedFontFamily { get; set; }        
        public int SelectedFontSize { get; private set; }
        public string SelectedFontSizeString { get; set; }

        public bool ClosedProperly { get; private set; }

        public RelayCommand<IClosable> Apply { get; private set; }

        public CustomizeVM(Color color, FontFamily fontFamily, int fontSize)
        {
            SelectedColor = color;
            SelectedFontFamily = fontFamily;
            SelectedFontSize = fontSize;
            SelectedFontSizeString = fontSize.ToString();

            ClosedProperly = false;

            Apply = new RelayCommand<IClosable>(ApplyExecute);
        }

        private void ApplyExecute(IClosable window)
        {
            int helper;
            if(int.TryParse(SelectedFontSizeString, out helper))
            {
                if (helper > 3 && helper < 301)
                {
                    SelectedFontSize = helper;
                    ClosedProperly = true;
                    window.Close();
                }
            }            
            else
            {
                MessageBox.Show("Velikost písma musí být v rozmezí od 4 do 300!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
