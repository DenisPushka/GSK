using _3_Laba_GSK.access;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace _3_Laba_GSK
{
    internal class Figure
    {
        private readonly List<MyPoint> points;
        public List<MyPoint> GetPoints () => points;

        private Graphics Graphics { get; }

        private Pen DrawPenBorder { get; } = new Pen(Color.Orange, 5);
        public Figure (Graphics graphics)
        {
            Graphics = graphics;
            points = new List<MyPoint>();
        }

        public Figure (List<MyPoint> points, Graphics graphics)
        {
            Graphics = graphics;
            this.points = points;
        }

        public void AddPoint (MouseEventArgs e, Pen drawPen)
        {
            points.Add(new MyPoint { X = e.X, Y = e.Y });
            if (points.Count > 1)
                Graphics.DrawLine(drawPen, points[points.Count - 2].ToPoint(), points[points.Count - 1].ToPoint());
        }

        // Факториал
        double Factorial (int n)
        {
            double x = 1;
            for (var i = 1; i <= n; i++)
                x *= i;
            return x;
        }

        private double Polinom (int i, int n, float t) => Factorial(n) / (Factorial(i) * Factorial(n - i))
            * (float) Math.Pow(t, i) * (float) Math.Pow(1 - t, n - i);

        // Кривая Безье
        public void DrawBezie (Pen DrPen)
        {
            var dt = 0.01;
            var t = dt;
            double xPred = points[0].X;
            double yPred = points[0].Y;
            while (t < 1)
            {
                double x = 0;
                double y = 0;
                
                for (var i = 0; i < points.Count; i++)
                {
                    var b = Polinom(i, points.Count - 1, (float) t);
                    x += points[i].X * b;
                    y += points[i].Y * b;
                }

                Graphics.DrawLine(DrPen, new Point((int) xPred, (int) yPred), new Point((int) x, (int) y));
                t += dt;
                xPred = x;
                yPred = y;
            }
            points.Clear();
        }

        // Клонирование фигуры
        public List<MyPoint> Cloning () => points.ToList();

        // Алгоритм закрашивания внутри многоугольника
        public void FillIn (Pen drawPen, int pictureBoxHeight, bool haveBorder)
        {
            PaintingLineInFigure(haveBorder);
            var arr = SearchYMinAndMax(pictureBoxHeight);
            var min = arr[0];
            var max = arr[1];
            var xs = new List<float>();

            for (var y = (int) min; y < max; y++)
            {
                var k = 0;
                for (var i = 0; i < points.Count - 1; i++)
                {
                    k = i < points.Count ? i + 1 : 1;
                    xs = CheckIntersection(xs, i, k, y);
                }

                xs = CheckIntersection(xs, k, 0, y);
                xs.Sort();

                for (var i = 0; i + 1 < xs.Count; i += 2)
                    Graphics.DrawLine(drawPen, new Point((int) xs[i], y), new Point((int) xs[i + 1], y));

                xs.Clear();
            }
        }

        // Алгоритм для закрашивания многоугольника против часовой стрелки
        public void FillOut (bool haveBorder, Pen drawPen, PictureBox pictureBox)
        {
            if (points.Count == 0) return;
            PaintingLineInFigure(haveBorder);
            var arr = SearchYMinAndMax(pictureBox.Height);
            var minY = (int) arr[0];
            var maxY = (int) arr[1];
            var cw = CalculationSquare((int) arr[2]);
            PaintAreasOutsideShape(drawPen, cw, 0, minY, pictureBox.Width);
            SecondCycleAlgorithm(drawPen, minY, maxY, cw, SearchXMinAndMax(), pictureBox.Width);
            PaintAreasOutsideShape(drawPen, cw, maxY, pictureBox.Height, pictureBox.Width);
        }

        // вычисление площади треугольника
        private bool CalculationSquare (int j)
        {
            if (j == 0)
                return Square(points.Count - 1, j, j + 1);
            return j == points.Count - 1 ? Square(j - 1, j, 0) : Square(j - 1, j, j + 1);
        }

        private bool Square (int prev, int now, int next) =>
            0.5 * ((points[prev].X * points[now].Y)
                   + (points[prev].Y * points[next].X)
                   + (points[now].X * points[next].Y)
                   - (points[now].Y * points[next].X)
                   - (points[prev].Y * points[now].X)
                   - (points[prev].X * points[next].Y)) < 0;

        public Tuple2<List<float>, List<float>> CalculationListXrAndXl (int y)
        {
            var k = 0;
            var xR = new List<float>();
            var xL = new List<float>();
            for (var i = 0; i < points.Count - 1; i++)
            {
                k = i < points.Count ? i + 1 : 1;
                if (Check(i, k, y))
                {
                    var x = -((y * (points[i].X - points[k].X))
                                - points[i].X * points[k].Y + points[k].X * points[i].Y)
                            / (points[k].Y - points[i].Y);
                    if (points[k].Y - points[i].Y > 0)
                        xR.Add(x);
                    else
                        xL.Add(x);
                }
            }

            if (Check(k, 0, y))
            {
                var x = -((y * (points[k].X - points[0].X))
                            - points[k].X * points[0].Y + points[0].X * points[k].Y)
                        / (points[0].Y - points[k].Y);
                if (points[0].Y - points[k].Y > 0)
                    xR.Add(x);
                else
                    xL.Add(x);
            }

            return new Tuple2<List<float>, List<float>>(xL, xR);
        }

        /// <summary>
        ///  Условие пересечения
        /// </summary>
        private bool Check (int i, int k, int y) =>
            (points[i].Y < y && points[k].Y >= y) || (points[i].Y >= y && points[k].Y < y);

        /// <summary>
        ///  Проверка пересичения прямой Y c отрезком
        /// </summary>
        private List<float> CheckIntersection (List<float> xs, int i, int k, int y)
        {
            if (Check(i, k, y))
            {
                var x = -((y * (points[i].X - points[k].X)) - points[i].X * points[k].Y + points[k].X * points[i].Y)
                        / (points[k].Y - points[i].Y);
                xs.Add(x);
            }

            return xs;
        }

        // Вывод области вне фигуры
        private void PaintAreasOutsideShape (Pen drawPen, bool cw, int y, int end, int width)
        {
            if (cw)
                for (var i = y; i < end; i++)
                    Graphics.DrawLine(drawPen, new Point(0, i), new Point(width, i));
        }

        // Основная часть алгоритма закрашивания вне многоугольника
        private void SecondCycleAlgorithm (Pen drawPen, int minY, int maxY, bool cw, IReadOnlyList<float> arrForX,
            int width)
        {
            for (var y = minY; y < maxY; y++)
            {
                var arr = CalculationListXrAndXl(y);
                var xR = arr.Second;
                var xL = arr.First;

                if (cw)
                {
                    xL.Add(arrForX[0]);
                    xR.Add(arrForX[1]);
                }

                xL.Sort();
                xR.Sort();

                Graphics.DrawLine(drawPen, new Point(0, y), new Point((int) xL[0], y));
                for (var i = 0; i < xL.Count && i < xR.Count; i++)
                    if (xL[i] <= xR[i])
                        Graphics.DrawLine(drawPen, new Point((int) xL[i], y), new Point((int) xR[i], y));

                Graphics.DrawLine(drawPen, new Point((int) xR[xR.Count - 1], y), new Point(width, y));
                xL.Clear();
                xR.Clear();
            }
        }

        /// <summary>
        ///  Рисование сторон
        /// </summary>
        public void PaintingLineInFigure (bool haveBorder)
        {
            if (haveBorder && points.Count > 1)
            {
                for (var i = 0; i < points.Count - 1; i++)
                    Graphics.DrawLine(DrawPenBorder, points[i].ToPoint(), points[i + 1].ToPoint());

                Graphics.DrawLine(DrawPenBorder, points[0].ToPoint(), points[points.Count - 1].ToPoint());
            }
        }

        // Поиск мин/макс X
        private float[] SearchXMinAndMax ()
        {
            var min = points[0].X;
            var max = 0.0f;
            for (var i = 0; i < points.Count; i++)
            {
                min = points[i].X < min ? points[i].X : min;
                max = points[i].X > max ? points[i].X : max;
            }

            return new[] { min, max };
        }

        // Поиск мин/макс Y
        public float[] SearchYMinAndMax (int height)
        {
            if (points.Count == 0)
                return new float[] { 0, 0, 0 };

            var min = points[0].Y;
            var max = points[0].Y;
            var j = 0;
            for (var i = 0; i < points.Count; i++)
            {
                var item = points[i];
                min = points[i].Y < min ? points[i].Y : min;

                if (item.Y > max)
                {
                    max = item.Y;
                    j = i;
                }
            }

            min = min < 0 ? 0 : min;
            max = max > height ? height : max;
            return new[] { min, max, j };
        }

        public bool ThisFigure (int mx, int my)
        {
            var m = 0;
            for (var i = 0; i <= points.Count - 1; i++)
            {
                var k = i < points.Count - 1 ? i + 1 : 0;
                var pi = points[i];
                var pk = points[k];
                if ((pi.Y < my) & (pk.Y >= my) | (pi.Y >= my) & (pk.Y < my)
                    && (my - pi.Y) * (pk.X - pi.X) / (pk.Y - pi.Y) + pi.X < mx)
                    m++;
            }

            return m % 2 == 1;
        }

        public void Move (int dx, int dy)
        {
            var buffer = new MyPoint();
            for (var i = 0; i <= points.Count - 1; i++)
            {
                buffer.X = points[i].X + dx;
                buffer.Y = points[i].Y + dy;
                points[i] = buffer;
            }
        }

        private void ToAndFromCenter (bool start, MyPoint e)
        {
            if (start)
            {
                float[,] toCenter =
                {
                    {1, 0, 0},
                    {0, 1, 0},
                    {-e.X, -e.Y, 1}
                };
                for (var i = 0; i < points.Count; i++)
                    points[i] = Calculation.Matrix_1x3_3x3(points[i], toCenter);
            }
            else
            {
                float[,] fromCenter =
                {
                    {1, 0, 0},
                    {0, 1, 0},
                    {e.X, e.Y, 1}
                };
                for (var i = 0; i < points.Count; i++)
                    points[i] = Calculation.Matrix_1x3_3x3(points[i], fromCenter);
            }
        }

        private MyPoint CenterFigure (int height)
        {
            float[] arrayY, arrayX;
            MyPoint e = new MyPoint();
            arrayY = SearchYMinAndMax(height);
            arrayX = SearchXMinAndMax();
            e.X = (arrayX[0] + arrayX[1]) / 2;
            e.Y = (arrayY[0] + arrayY[1]) / 2;
            return e;
        }

        /// <summary>
        ///  Отражение
        /// </summary>
        public void Mirror (char ch, int height)
        {
            float[,] matrix = new float[3, 3];
            switch (ch)
            {
                case 'x':
                    {
                        matrix = new float[3, 3]{
                            { 1, 0, 0},
                            { 0,-1, 0},
                            { 0, 0, 1}
                        };
                    }
                    break;
                case 'y':
                    {
                        matrix = new float[3, 3]{
                            {-1, 0, 0},
                            { 0, 1, 0},
                            { 0, 0, 1}
                        };
                    }
                    break;
                case 'o':
                    {
                        matrix = new float[3, 3]{
                            {-1, 0, 0},
                            { 0,-1, 0},
                            { 0, 0, 1}
                        };
                    }
                    break;
                default:
                    break;
            }

            var e = CenterFigure(height);
            ToAndFromCenter(true, e);

            for (int i = 0; i < points.Count; i++)
                points[i] = Calculation.Matrix_1x3_3x3(points[i], matrix);

            ToAndFromCenter(false, e);
        }

        public void Zoom (int height, float[] zoom)
        {
            if (zoom[0] <= 0) zoom[0] = -0.1f;
            if (zoom[1] <= 0) zoom[1] = -0.1f;
            if (zoom[0] >= 0) zoom[0] = 0.1f;
            if (zoom[1] >= 0) zoom[1] = 0.1f;

            var sx = 1 + zoom[0]; // для расстяжения по OY комментим + zoom[0];
            var sy = 1 + zoom[1];
            float[,] matrix = {
                {sx,  0, 0 },
                { 0, sy, 0 },
                { 0,  0, 1 }
            };

            var e = CenterFigure(height);
            ToAndFromCenter(true, e);

            for (int i = 0; i < points.Count; i++)
                points[i] = Calculation.Matrix_1x3_3x3(points[i], matrix);

            ToAndFromCenter(false, e);
        }

        private int updateAlpha = 0;
        public void Rotation (int mouse, int height, TextBox textBox2)
        {
            float alpha = 0;
            if (mouse > 0)
            {
                alpha += 0.0175f;
                updateAlpha++;
            }
            else
            {
                alpha -= 0.0175f;
                updateAlpha--;
            }

            textBox2.Text = updateAlpha.ToString();
            var e = CenterFigure(height);
            ToAndFromCenter(true, e);

            float[,] matrixRotation =
            {
                {(float) Math.Cos(alpha), (float) Math.Sin(alpha), 0.0f},
                {-(float) Math.Sin(alpha), (float) Math.Cos(alpha), 0.0f},
                {0.0f, 0.0f, 1.0f}
            };
            for (var i = 0; i < points.Count; i++)
                points[i] = Calculation.Matrix_1x3_3x3(points[i], matrixRotation);

            ToAndFromCenter(false, e);

        }
        #region Читерский вывод

        // Вывод многоугольнкиа с помощью FillPolygon
        private void PaintingFirstToAlgorithm (Graphics graphics, Pen drawPen)
        {
            var pgVertex = new Point[points.Count];
            for (var i = 0; i < points.Count; i++)
            {
                pgVertex[i].X = (int) points[i].X;
                pgVertex[i].Y = (int) points[i].Y;
            }

            graphics.FillPolygon(new SolidBrush(drawPen.Color), pgVertex);
        }

        // Вывод многоугольнкиа вне с помощью FillPolygon
        private void PaintingSecondToAlgorithm (Graphics graphics, Pen drawPen)
        {
            graphics.Clear(drawPen.Color);
            var pgVertex = new Point[points.Count];
            for (var i = 0; i < points.Count; i++)
            {
                pgVertex[i].X = (int) points[i].X;
                pgVertex[i].Y = (int) points[i].Y;
            }

            graphics.FillPolygon(new SolidBrush(new Pen(Color.White).Color), pgVertex);
        }

        #endregion
    }
}