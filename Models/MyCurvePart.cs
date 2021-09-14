using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WPF_Malovani.Models
{
    public class MyCurvePart: ISaveable
    {        
        private MyCurve parent;

        public Coordinates Start { get; private set; }
        public Coordinates End { get; private set; }        

        public MyCurvePart(int x1, int y1, int x2, int y2, MyCurve parent)
        {            
            Start = new Coordinates(x1, y1);
            End = new Coordinates(x2, y2);
            this.parent = parent;
        }

        private int Margin()
        {
            return (parent.Thickness / 2) + 5;
        }

        public bool CompareTo(FrameworkElement element)
        {
            if (!(element is Line))
            {
                return false;
            }

            Line l = element as Line;
            return l.X1 == Start.X && l.Y1 == Start.Y && l.X2 == End.X && l.Y2 == End.Y && l.StrokeThickness == parent.Thickness;
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
            XElement curvePart = new XElement("curvePart");
            curvePart.Add(Start.Save("start"));
            curvePart.Add(End.Save("end"));
            return curvePart;
        }
    }
}
