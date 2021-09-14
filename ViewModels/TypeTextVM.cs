using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Malovani.Models;
using WPF_Malovani.Utility;

namespace WPF_Malovani.ViewModels
{
    public class TypeTextVM
    {
        public string Text { get; set; }
        public bool ClosedProperly { get; private set; }

        public RelayCommand<IClosable> Apply { get; private set; }

        public TypeTextVM()
        {
            ClosedProperly = false;
            Apply = new RelayCommand<IClosable>(ApplyExecute);
        }

        private void ApplyExecute(IClosable window)
        {
            ClosedProperly = true;
            window.Close();
        }
    }
}
