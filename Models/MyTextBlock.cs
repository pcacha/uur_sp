using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace WPF_Malovani.Models
{
    public class MyTextBlock : IPaintable, ISaveable
    {
        public string Text { get; }
        public Color Color { get; }
        public Coordinates Start { get; private set; }
        public int Width { get; }
        public int Height { get; }   
        public FontFamily FontFamily { get; }
        public int FontSize { get; }

        public MyTextBlock(int x, int y, string text, int width, int height, Color color, FontFamily fontFamily, int fontSize)
        {
            Text = text;
            Color = color;
            Width = width;
            Height = height;
            Start = new Coordinates(x, y);
            FontFamily = fontFamily;
            FontSize = fontSize;
        }

        private int Margin()
        {
            return 5;
        }

        public bool CompareTo(FrameworkElement element)
        {
            if (!(element is TextBlock))
            {
                return false;
            }

            TextBlock tb = element as TextBlock;
            int x = (int)Canvas.GetLeft(tb);
            int y = (int)Canvas.GetTop(tb);
            return x == Start.X && y == Start.Y && tb.Width == Width && tb.Height == Height;
        }

        public bool Hit(int x, int y)
        {
            // souřadnice protilehlého vrcholu
            int k = Start.X + Width;
            int l = Start.Y + Height;
            if (x > Start.X - Margin() && x < k + Margin() && y > Start.Y - Margin() && y < l + Margin())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Move(MyVector v)
        {
            Start = new Coordinates(Start.X + v.X, Start.Y + v.Y);
        }

        public XElement Save()
        {
            XElement textBlock = new XElement("textBlock");
            textBlock.Add(new XElement("text", Text));
            textBlock.Add(new XElement("color", new XElement("r", Color.R), new XElement("g", Color.G), new XElement("b", Color.B), new XElement("a", Color.A)));
            textBlock.Add(Start.Save("start"));
            textBlock.Add(new XElement("width", Width));
            textBlock.Add(new XElement("height", Height));
            textBlock.Add(new XElement("fontFamily", FontFamily.Source));
            textBlock.Add(new XElement("fontSize", FontSize));
            return textBlock;
        }
    }
}
