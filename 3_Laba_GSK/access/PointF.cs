using System.Drawing;

namespace _3_Laba_GSK
{
    internal struct PointF
    {
        public float X;
        public float Y;
        public float Constanta;

        public PointF(float x = 0.0f, float y = 0.0f, float constanta = 1.0f) {
            X = x;
            Y = y;
            Constanta = constanta;
        }

        public Point ToPoint() => new Point((int)X, (int)Y);
    }
}
