using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WPF_Malovani.Models
{
    public interface ISaveable
    {
        XElement Save();
    }
}
