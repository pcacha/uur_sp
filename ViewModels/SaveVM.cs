using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using WPF_Malovani.Models;
using WPF_Malovani.Utility;

namespace WPF_Malovani.ViewModels
{
    public class SaveVM
    {
        private int pixelsX;
        private int pixelsY;
        private List<IPaintable> shapes;
        private string picturePath;
        private string selectedDirectory;
        private RenderTargetBitmap bitmap;
        private bool pictureInserted;

        public string FileName { get; set; }
        public bool? SavePictureChecked { get; set; }
        public bool? SaveProjectChecked { get; set; }  

        public RelayCommand<IClosable> Save { get; private set; }
        public RelayCommand<IClosable> Cancel { get; private set; }
        public Action<string> DirectoryUpdate { get; private set; }
        public RelayCommand<object> SetDefaultDir { get; private set; }

        public SaveVM(int pixelsX, int pixelsY, List<IPaintable> shapes, string picturePath, RenderTargetBitmap bitmap)
        {
            this.pixelsX = pixelsX;
            this.pixelsY = pixelsY;
            this.shapes = shapes;
            this.picturePath = picturePath;
            this.bitmap = bitmap;
            pictureInserted = (picturePath == null) ? false : true;

            Save = new RelayCommand<IClosable>(SaveExecute);
            Cancel = new RelayCommand<IClosable>(CancelExecute);
            DirectoryUpdate = DirectoryUpdateHandler;
            SetDefaultDir = new RelayCommand<object>(SetDefaultDirExecute);
        }

        private void SaveExecute(IClosable window)
        {
            #region input verification
            if (string.IsNullOrEmpty(FileName))
            {
                MessageBox.Show("Jméno souboru musí být vyplněné!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SavePictureChecked != true && SaveProjectChecked != true)
            {
                MessageBox.Show("Zaškrtni formát k uložení!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(selectedDirectory))
            {
                MessageBox.Show("Cílová složka musí být zvolena", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(!VerifyFileName())
            {
                MessageBox.Show("Jméno souboru není validní!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            #endregion

            #region duplicated files verification
            bool saveCanceled = false;
            bool savePicture = false;
            if(SavePictureChecked == true)
            {
                savePicture = true;
                if (VerifyFileDuplicate(".png"))
                {
                    if(MessageBox.Show("Soubor .png s tímto názvem existuje. Přeješ si ho přepsat?", "Kolize", MessageBoxButton.YesNo,  MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        savePicture = false;
                        saveCanceled = true;
                    }
                }
            }
            bool saveProject = false;
            if (SaveProjectChecked == true)
            {
                saveProject = true;
                if (VerifyFileDuplicate(".xml"))
                {
                    if (MessageBox.Show("Soubor .xml s tímto názvem existuje. Přeješ si ho přepsat?", "Kolize", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        saveProject = false;
                        saveCanceled = true;
                    }
                }
            }
            #endregion

            #region saving
            if (savePicture)
            {
                try
                {
                    using (FileStream fs = new FileStream(Path.Combine(selectedDirectory, (FileName + ".png")), FileMode.Create))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        encoder.Save(fs);
                    }                    
                }
                catch(IOException)
                {
                    MessageBox.Show("Obrázek se nepodařilo uložit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    saveCanceled = true;
                }
            }
            if(saveProject)
            {
                try
                {                    
                    XDocument project = new XDocument(new XDeclaration("1.0", "UTF-8", null),
                        new XElement("project",
                        new XElement("pixelsX", pixelsX), new XElement("pixelsY", pixelsY), new XElement("picture", new XAttribute("inserted", pictureInserted), picturePath), new XElement("shapes", 
                        shapes.Select(s => (s as ISaveable).Save()))));                   

                    project.Save(Path.Combine(selectedDirectory, (FileName + ".xml")));
                }
                catch (IOException)
                {
                    MessageBox.Show("Projekt se nepodařilo uložit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    saveCanceled = true;
                }
            }
            #endregion

            if (!saveCanceled)
            {
                window.Close();
            }
        }

        private void CancelExecute(IClosable window)
        {
            window.Close();
        }

        private void DirectoryUpdateHandler(string path)
        {
            selectedDirectory = path;
        }

        #region verification
        private bool VerifyFileName()
        {
            char[] banned = new char[] { '\\', '/', '*', ':', '?', '\"', '<', '>', '|'};
            foreach (char c in banned)
            {
                if(FileName.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        private bool VerifyFileDuplicate(string extension)
        {
            DirectoryInfo di = new DirectoryInfo(selectedDirectory);
            foreach(FileInfo fi in di.GetFiles())
            {
                if (fi.Name == (FileName + extension))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        private void SetDefaultDirExecute(object obj)
        {
            Properties.Settings.Default.DefaultDirPath = selectedDirectory;
            Properties.Settings.Default.Save();
        }
    }
}
