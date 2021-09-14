using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Malovani.Models
{
    public class MyVector
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public MyVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public MyVector(int x1, int y1, int x2, int y2)
        {
            X = x2 - x1;
            Y = y2 - y1;
        }
    }
}
