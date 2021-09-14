using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WPF_Malovani.Models
{
    public class Coordinates: ISaveable
    {        
        public int X { get; }       

       
        public int Y { get; }

        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public XElement Save()
        {
            return new XElement("cooridnates", new XElement("x", X), new XElement("y", Y));
        }

        public XElement Save(string name)
        {
            return new XElement(name, new XElement("x", X), new XElement("y", Y));
        }
    }
}
