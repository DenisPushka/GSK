using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace _3_Laba_GSK
{
    internal struct MyPoint
    {
        public float X;
        public float Y;
        public float Constanta;

        public MyPoint(float x = 0.0f, float y = 0.0f, float constanta = 1.0f) {
            X = x;
            Y = y;
            Constanta = constanta;
        }

        public Point ToPoint() => new Point((int)X, (int)Y);
    }
}
