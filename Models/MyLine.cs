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
    public class MyLine : IPaintable, ISaveable
    {
        public Color Color { get; }
        public Coordinates Start { get; private set; }
        public Coordinates End { get; private set; }
        public int Thickness { get; }        

        public MyLine(int x1, int y1, int x2, int y2, int thickness, Color color)
        {
            Color = color;
            Start = new Coordinates(x1, y1);
            End = new Coordinates(x2, y2);
            Thickness = thickness;
        }
        private int Margin()
        {
            return (Thickness / 2) + 5;
        }

        public bool CompareTo(FrameworkElement element)
        {
            if(!(element is Line))
            {
                return false;
            }

            Line l = element as Line;
            return l.X1 == Start.X && l.Y1 == Start.Y && l.X2 == End.X && l.Y2 == End.Y && l.StrokeThickness == Thickness;
        }

        public bool Hit(int x, int y)
        {
            if (x < Math.Max(Start.X, End.X) + Margin() && x > Math.Min(Start.X, End.X) - Margin() && y < Math.Max(Start.Y, End.Y) + Margin() && y > Math.Min(Start.Y, End.Y) - Margin()
               && Math.Abs((Start.Y - End.Y) * x + (End.X - Start.X) * y + (End.Y - Start.Y) * Start.X + (Start.X - End.X) * Start.Y) / Math.Sqrt((Start.Y - End.Y) * (Start.Y - End.Y) + (End.X - Start.X) * (End.X - Start.X)) <= Margin())
            {
                return true;
            }

            return false;
        }

        public void Move(MyVector v)
        {
            Start = new Coordinates(Start.X + v.X, Start.Y + v.Y);
            End = new Coordinates(End.X + v.X, End.Y + v.Y);
        }

        public XElement Save()
        {
            XElement line = new XElement("line");
            line.Add(new XElement("color", new XElement("r", Color.R), new XElement("g", Color.G), new XElement("b", Color.B), new XElement("a", Color.A)));
            line.Add(Start.Save("start"));
            line.Add(End.Save("end"));
            line.Add(new XElement("thickness", Thickness));
            return line;
        }
    }
}
