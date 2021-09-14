using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class MyEllipse : IPaintable, ISaveable
    {
        public Color Color { get; }
        public Coordinates Start { get; private set; }
        public int Width { get; }
        public int Height { get; }
        public int Thickness { get; }
        public bool Filled { get; }

        public MyEllipse(int x1, int y1, int x2, int y2, int thickness, Color color, bool filled)
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

        public MyEllipse(int x, int y, int width, int height, int thickness, Color color, bool filled, bool determined)
        {
            Color = color;
            Thickness = thickness;
            Filled = filled;
            Width = width;
            Height = height;
            Start = new Coordinates(x, y);
        }

        private int Margin()
        {
            return (Thickness / 2) + 5;
        }

        public bool CompareTo(FrameworkElement element)
        {
            if (!(element is Ellipse))
            {
                return false;
            }

            Ellipse e = element as Ellipse;
            int x = (int)Canvas.GetLeft(e);
            int y = (int)Canvas.GetTop(e);
            if(!Filled)
            {
                return x == Start.X && y == Start.Y && e.Width == Width && e.Height == Height && e.StrokeThickness == Thickness;
            }
            else
            {                
               return x == Start.X && y == Start.Y && e.Width == Width && e.Height == Height && e.StrokeThickness == Thickness && (e.Fill as SolidColorBrush).Color == Color;
            }
        }

        public bool Hit(int x, int y)
        {
            // focal points: F[f1, f2]   G[g1, g2], excentricity e, center C[c1, c2], reference distance of a point which certainly belongs to the ellipse: referenceDistance
            int f1, f2, g1, g2, e, c1, c2, referenceDistance, firstHitPointDistance, secondHitPointDistance, hitPointDistance;
            c1 = Start.X + (Width / 2);
            c2 = Start.Y + (Height / 2);

            //for a horizontal ellipse
            if (Width > Height)
            {
                e = (int)Math.Sqrt((Width / 2) * (Width / 2) - (Height / 2) * (Height / 2));
                f2 = c2;
                g2 = c2;
                f1 = c1 + e;
                g1 = c1 - e;
                referenceDistance = Width;

                //counting of distance of ellipse point from focal points: ...
                //hitting a focal point or the center
                if ((x == f1 && y == f2) || (x == g1 && y == g2) || (x == c1 && y == c2))
                {
                    hitPointDistance = 2 * e;
                }
                //hitting the line connection focal points
                else if (y == f2)
                {
                    hitPointDistance = Math.Abs(x - f1) + Math.Abs(x - g1);
                }
                //hitting x-coordinate of one of the focal points
                else if (x == f1)
                {
                    firstHitPointDistance = Math.Abs(y - f2);
                    secondHitPointDistance = (int)Math.Sqrt(Math.Abs(x - g1) * Math.Abs(x - g1) + Math.Abs(y - g2) * Math.Abs(y - g2));
                    hitPointDistance = firstHitPointDistance + secondHitPointDistance;
                }
                else if (x == g1)
                {
                    firstHitPointDistance = (int)Math.Sqrt(Math.Abs(x - f1) * Math.Abs(x - f1) + Math.Abs(y - f2) * Math.Abs(y - f2));
                    secondHitPointDistance = Math.Abs(y - g2);
                    hitPointDistance = firstHitPointDistance + secondHitPointDistance;
                }
                else
                {
                    firstHitPointDistance = (int)Math.Sqrt(Math.Abs(x - f1) * Math.Abs(x - f1) + Math.Abs(y - f2) * Math.Abs(y - f2));
                    secondHitPointDistance = (int)Math.Sqrt(Math.Abs(x - g1) * Math.Abs(x - g1) + Math.Abs(y - g2) * Math.Abs(y - g2));
                    hitPointDistance = firstHitPointDistance + secondHitPointDistance;
                }

                //comparting distances and managing a filled ellipse
                int difference =  (hitPointDistance - referenceDistance) / 2;
                if ((Math.Abs(difference) < Margin() && !Filled) || (difference < Margin() && Filled))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //for a circle
            else if (Height == Width)
            {
                int radius = Width / 2;
                double hitPointFromCenter, distance;

                if (c1 == x && c2 == y)
                {
                    if (radius < 6)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (c1 == x)
                {
                    hitPointFromCenter = Math.Abs(c2 - y);
                }
                else if (c2 == y)
                {
                    hitPointFromCenter = Math.Abs(c1 - x);
                }
                else
                {
                    hitPointFromCenter = Math.Sqrt(Math.Abs(c1 - x) * Math.Abs(c1 - x) + Math.Abs(c2 - y) * Math.Abs(c2 - y));
                }

                //comparting distances and managing a filled circle
                distance = hitPointFromCenter - radius;                
                if ((Math.Abs(distance) < Margin() && !Filled) || (distance < Margin() && Filled))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //for a vertical ellipse
            else
            {
                e = (int)Math.Sqrt((Height / 2) * (Height / 2) - (Width / 2) * (Width / 2));
                f1 = c1;
                g1 = c1;
                f2 = c2 + e;
                g2 = c2 - e;
                referenceDistance = Height;

                //counting of distance of ellipse point from focal points: ...
                //hitting a focal point or the center
                if ((x == f1 && y == f2) || (x == g1 && y == g2) || (x == c1 && y == c2))
                {
                    hitPointDistance = 2 * e;
                }
                //hitting the line connection focal points
                else if (x == f1)
                {
                    hitPointDistance = Math.Abs(y - f2) + Math.Abs(y - g2);
                }
                //hitting y-coordinate of one of the focal points
                else if (y == f2)
                {
                    firstHitPointDistance = Math.Abs(x - f1);
                    secondHitPointDistance = (int)Math.Sqrt(Math.Abs(x - g1) * Math.Abs(x - g1) + Math.Abs(y - g2) * Math.Abs(y - g2));
                    hitPointDistance = firstHitPointDistance + secondHitPointDistance;
                }
                else if (y == g2)
                {
                    firstHitPointDistance = (int)Math.Sqrt(Math.Abs(x - f1) * Math.Abs(x - f1) + Math.Abs(y - f2) * Math.Abs(y - f2));
                    secondHitPointDistance = Math.Abs(x - g1);
                    hitPointDistance = firstHitPointDistance + secondHitPointDistance;
                }
                else
                {
                    firstHitPointDistance = (int)Math.Sqrt(Math.Abs(x - f1) * Math.Abs(x - f1) + Math.Abs(y - f2) * Math.Abs(y - f2));
                    secondHitPointDistance = (int)Math.Sqrt(Math.Abs(x - g1) * Math.Abs(x - g1) + Math.Abs(y - g2) * Math.Abs(y - g2));
                    hitPointDistance = firstHitPointDistance + secondHitPointDistance;
                }

                //comparting distances and managing a filled ellipse
                int difference = (hitPointDistance - referenceDistance) / 2;
                if ((Math.Abs(difference) < Margin() && !Filled) || (difference < Margin() && Filled))
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
            XElement ellipse = new XElement("ellipse");
            ellipse.Add(new XElement("color", new XElement("r", Color.R), new XElement("g", Color.G), new XElement("b", Color.B), new XElement("a", Color.A)));
            ellipse.Add(Start.Save("start"));
            ellipse.Add(new XElement("width", Width));
            ellipse.Add(new XElement("height", Height));
            ellipse.Add(new XElement("thickness", Thickness));
            ellipse.Add(new XElement("filled", Filled));
            return ellipse;
        }
    }
}
