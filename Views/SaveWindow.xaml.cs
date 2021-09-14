using System;
using System.Collections.Generic;
using System.IO;
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
using WPF_Malovani.Models;
using WPF_Malovani.ViewModels;

namespace WPF_Malovani.Views
{
    /// <summary>
    /// Interaction logic for SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window, IClosable
    {        
        public SaveWindow()
        {
            InitializeComponent();            
        }

        private void folderTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(string drive in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = drive;
                item.Tag = drive;
                item.Items.Add(null);
                item.Expanded += Folder_Expanded;
                folderTreeView.Items.Add(item);
            }
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;

            if (item.Items.Count != 1 || item.Items[0] != null)
            {
                return;
            }

            item.Items.Clear();
            string itemPath = (string)item.Tag;

            try
            {                
                foreach (string folder in Directory.GetDirectories(itemPath))
                {
                    if(!new DirectoryInfo(folder).Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        TreeViewItem folderItem = new TreeViewItem();
                        folderItem.Header = new DirectoryInfo(folder).Name;
                        folderItem.Tag = folder;
                        folderItem.Items.Add(null);
                        folderItem.Expanded += Folder_Expanded;
                        item.Items.Add(folderItem);
                    }                 
                }
            }
            catch(IOException)
            {
                MessageBox.Show("Nepodařilo se načíst podsložky.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
