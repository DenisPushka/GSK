namespace _3_Laba_GSK.access
{
    internal static class Calculation
    {
        public static PointF Matrix_1x3_3x3(PointF pointF, float[,] matrix3X3) => new PointF
        {
            X = pointF.X * matrix3X3[0, 0] + pointF.Y * matrix3X3[1, 0] + pointF.Constanta * matrix3X3[2, 0],
            Y = pointF.X * matrix3X3[0, 1] + pointF.Y * matrix3X3[1, 1] + pointF.Constanta * matrix3X3[2, 1],
            Constanta = pointF.X * matrix3X3[0, 2] + pointF.Y * matrix3X3[1, 2] + pointF.Constanta * matrix3X3[2, 2]
        };
    }
}
