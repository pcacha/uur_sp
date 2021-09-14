using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPF_Malovani.Models
{
    public interface IPaintable
    {
        void Move(MyVector v);
        bool CompareTo(FrameworkElement element);
        bool Hit(int x, int y);        
        Color Color { get; }   
        Coordinates Start { get; }
    }
}
