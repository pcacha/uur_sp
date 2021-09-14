using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Malovani.Models
{
    public class OpenProjectParameter
    {
        public IClosable Window { get; }
        public string FileName { get; }

        public OpenProjectParameter(IClosable window, string fileName)
        {
            Window = window;
            FileName = fileName;
        }
    }
}
