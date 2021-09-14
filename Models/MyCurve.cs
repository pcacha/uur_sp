using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WPF_Malovani.Models
{
    public class MyCurve : IPaintable, ISaveable
    {
        private List<MyCurvePart> parts;

        public Color Color { get; }
        public int Thickness { get; }   
        public Coordinates Start { get; }

        public int Count
        {
            get
            {
                return parts.Count;
            }                 
        }

        public MyCurve(Color color, int thickness)
        {
            Color = color;
            Thickness = thickness;
            Start = new Coordinates(0, 0);

            parts = new List<MyCurvePart>();
        }

        public bool CompareTo(FrameworkElement element)
        {
            foreach (MyCurvePart mcp in parts)
            {
                if (mcp.CompareTo(element))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Hit(int x, int y)
        {
            foreach(MyCurvePart mcp in parts)
            {
                if(mcp.Hit(x,y))
                {
                    return true;
                }
            }
            return false;
        }

        public void Move(MyVector v)
        {
            foreach (MyCurvePart mcp in parts)
            {
                mcp.Move(v);
            }
        }

        public void Add(MyCurvePart part)
        {
            parts.Add(part);
        }

        public MyCurvePart GetPart(int index)
        {
            return parts[index];
        }

        public XElement Save()
        {
            XElement curve = new XElement("curve");
            curve.Add(new XElement("color", new XElement("r", Color.R), new XElement("g", Color.G), new XElement("b", Color.B), new XElement("a", Color.A)));
            curve.Add(new XElement("thickness", Thickness));
            XElement partsXml = new XElement("parts");
            curve.Add(partsXml);
            parts.ForEach(p => partsXml.Add(p.Save()));
            return curve;
        }
    }
}
