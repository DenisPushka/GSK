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
        public bool DoTmo { get; set; } = false;
        private Pen DrawPenBorder { get; } = new Pen(Color.Orange, 5);
        public bool IsFunction { get; set; }

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
        private static double Factorial (int n)
        {
            double x = 1;
            for (var i = 1; i <= n; i++)
                x *= i;
            return x;
        }

        private static double Polinom (int i, int n, float t) => Factorial(n) / (Factorial(i) * Factorial(n - i))
                                                                * (float) Math.Pow(t, i) *
                                                                (float) Math.Pow(1 - t, n - i);

        // Кривая Безье
        public Figure DrawBezier (Pen drPen)
        {
            const double dt = 0.01;
            var t = dt;
            double xPred = points[0].X;
            double yPred = points[0].Y;
            var fig = new List<MyPoint>();
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

                fig.Add(new MyPoint((float) x, (float) y));
                Graphics.DrawLine(drPen, new Point((int) xPred, (int) yPred), new Point((int) x, (int) y));
                t += dt;
                xPred = x;
                yPred = y;
            }

            points.Clear();
            return new Figure(fig, Graphics);
        }

        // Клонирование точек фигуры
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
                    k = i < points.Count ? i + 1 : 0;
                    xs = CheckIntersection(xs, i, k, y);
                }

                xs = CheckIntersection(xs, k, 0, y);
                xs.Sort();

                for (var i = 0; i + 1 < xs.Count; i += 2)
                    Graphics.DrawLine(drawPen, new Point((int) xs[i], y), new Point((int) xs[i + 1], y));

                xs.Clear();
            }
        }

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
                    var x = -(y * (points[i].X - points[k].X)
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
                var x = -(y * (points[i].X - points[k].X) - points[i].X * points[k].Y + points[k].X * points[i].Y)
                        / (points[k].Y - points[i].Y);
                xs.Add(x);
            }

            return xs;
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

        public void PaintingLineInFigure (Pen drawPen)
        {
            for (var i = 0; i < points.Count - 1; i++)
                Graphics.DrawLine(drawPen, points[i].ToPoint(), points[i + 1].ToPoint());
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
            for (var i = 1; i < points.Count; i++)
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

        // TODO разобраться!!!!
        public void Move (int dx, int dy)
        {
            for (var i = 0; i < points.Count; i++)
            {
                var buffer = new MyPoint()
                {
                    X = points[i].X + dx,
                    Y = points[i].Y + dy
                };
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
            var e = new MyPoint();
            var arrayY = SearchYMinAndMax(height);
            var arrayX = SearchXMinAndMax();
            e.X = (arrayX[0] + arrayX[1]) / 2;
            e.Y = (arrayY[0] + arrayY[1]) / 2;
            return e;
        }

        /// <summary>
        ///  Отражение
        /// </summary>
        public void Mirror (char ch, int height, int operation, MouseEventArgs eventMouse)
        {
            var matrix = new float[,]
                    {
                        {-1, 0, 0},
                        {0, -1, 0},
                        {0, 0, 1}
                    };

            var e = CenterFigure(height);
            ToAndFromCenter(true, e);

            for (var i = 0; i < points.Count; i++)
                points[i] = Calculation.Matrix_1x3_3x3(points[i], matrix);

            ToAndFromCenter(false, e);
        }

        public void Zoom (int height, float[] zoom, MouseEventArgs eventMouse)
        {
            if (zoom[0] <= 0) zoom[0] = -0.1f;
            if (zoom[1] <= 0) zoom[1] = -0.1f;
            if (zoom[0] >= 0) zoom[0] = 0.1f;
            if (zoom[1] >= 0) zoom[1] = 0.1f;

            var sx = 1;
            var sy = 1 + zoom[1];
            float[,] matrix =
            {
                {sx, 0, 0},
                {0, sy, 0},
                {0, 0, 1}
            };

            var e = new MyPoint(eventMouse.X, eventMouse.Y);

            ToAndFromCenter(true, e);

            for (var i = 0; i < points.Count; i++)
                points[i] = Calculation.Matrix_1x3_3x3(points[i], matrix);

            ToAndFromCenter(false, e);
        }

        private int updateAlpha;

        public void Rotation (int mouse, int height, TextBox textBox2, MouseEventArgs eventMouse)
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

            var e = new MyPoint(eventMouse.X, eventMouse.Y);
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