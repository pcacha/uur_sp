using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WPF_Malovani.Models
{
    public class MyRectangle: IPaintable, ISaveable
    {
        public Color Color { get; }
        public Coordinates Start { get; private set; }
        public int Width { get; }
        public int Height { get; }
        public int Thickness { get; }
        public bool Filled { get; }

        public MyRectangle(int x1, int y1, int x2, int y2, int thickness, Color color, bool filled)
        {
            Color = color;
            Thickness = thickness;
            Filled = filled;

            Width = Math.Abs(x1 - x2);
            Height = Math.Abs(y1 - y2);

            int helpX = (x1 < x2) ? x1 : x2;
            int helpY = (y1 < y2) ? y1 : y2;
            Start = new Coordinates(helpX, helpY);
        }

        public MyRectangle(int x, int y, int widht, int height, int thickness, Color color, bool filled, bool determined)
        {
            Color = color;
            Thickness = thickness;
            Filled = filled;
            Width = widht;
            Height = height;            
            Start = new Coordinates(x, y);
        }

        private int Margin()
        {
            return (Thickness / 2) + 5;
        }

        public bool CompareTo(FrameworkElement element)
        {
            if (!(element is Rectangle))
            {
                return false;
            }

            Rectangle r = element as Rectangle;
            int x = (int)Canvas.GetLeft(r);
            int y = (int)Canvas.GetTop(r);
            if (!Filled)
            {
                return x == Start.X && y == Start.Y && r.Width == Width && r.Height == Height && r.StrokeThickness == Thickness;
            }
            else
            {
                return x == Start.X && y == Start.Y && r.Width == Width && r.Height == Height && r.StrokeThickness == Thickness && (r.Fill as SolidColorBrush).Color == Color;
            }
        }

        public bool Hit(int x, int y)
        {
            // souřadnice protilehlého vrcholu
            int k = Start.X + Width;
            int l = Start.Y + Height;
            if(!Filled)
            {
                if (x > Start.X - Margin() && x < k + Margin() && y > Start.Y - Margin() && y < l + Margin())
                {
                    if (x > Start.X + Margin() && x < k - Margin() && y > Start.Y + Margin() && y < l - Margin())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {

                if (x > Start.X - Margin() && x < k + Margin() && y > Start.Y - Margin() && y < l + Margin())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
        }

        public void Move(MyVector v)
        {
            Start = new Coordinates(Start.X + v.X, Start.Y + v.Y);
        }

        public XElement Save()
        {           
            XElement rectangele = new XElement("rectangle");
            rectangele.Add(new XElement("color", new XElement("r", Color.R), new XElement("g", Color.G), new XElement("b", Color.B), new XElement("a", Color.A)));
            rectangele.Add(Start.Save("start"));
            rectangele.Add(new XElement("width", Width));
            rectangele.Add(new XElement("height", Height));
            rectangele.Add(new XElement("thickness", Thickness));
            rectangele.Add(new XElement("filled", Filled));
            return rectangele;
        }
    }
}
