using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Malovani.Components
{
    public class MyListBox: ListBox
    {
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(MyListBox), new PropertyMetadata((myListBox, evenArgs) => Update(myListBox)));

        private static void Update(DependencyObject dependencyObject)
        {
            MyListBox myListBox = (dependencyObject as MyListBox);
            if (!string.IsNullOrEmpty(myListBox.Path))
            {
                try
                {
                    myListBox.LoadLatestProjects(myListBox.Path);
                }
                catch (IOException)
                {
                    MessageBox.Show("Nepodařilo se načíst nedávné projekty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    myListBox.ItemsSource = new List<string>();
                }
            }
            else
            {
                myListBox.ItemsSource = new List<string>();
            }
        }

        private void LoadLatestProjects(string defaultDirectory)
        {
            List<string> result = new List<string>();
            DirectoryInfo directory = new DirectoryInfo(defaultDirectory);
            FileInfo[] files = directory.GetFiles("*.xml");
            foreach (FileInfo file in files)
            {
                result.Add(file.Name.Remove(file.Name.Length - 4));
            }
            ItemsSource = result;          
        }

    }
}
