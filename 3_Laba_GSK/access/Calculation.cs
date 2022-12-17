namespace _3_Laba_GSK.access
{
    internal static class Calculation
    {
        public static MyPoint Matrix_1x3_3x3(MyPoint point, float[,] matrix3X3) =>
            new MyPoint
            {
                X = point.X * matrix3X3[0, 0] + point.Y * matrix3X3[1, 0] + point.Constanta * matrix3X3[2, 0],
                Y = point.X * matrix3X3[0, 1] + point.Y * matrix3X3[1, 1] + point.Constanta * matrix3X3[2, 1],
                Constanta = point.X * matrix3X3[0, 2] + point.Y * matrix3X3[1, 2] + point.Constanta * matrix3X3[2, 2]
            };
    }
}
