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
        public List<MyPoint> GetPoints() => points;


        public Figure() => points = new List<MyPoint>();
        public Figure(List<MyPoint> points) => this.points = points;


        public void AddPoint(Graphics graphics, MouseEventArgs e, Pen drawPen)
        {
            points.Add(new MyPoint {X = e.X, Y = e.Y});
            if (points.Count > 1)
                graphics.DrawLine(drawPen, points[points.Count - 2].ToPoint(), points[points.Count - 1].ToPoint());
        }

        // Клонирование фигуры
        public List<MyPoint> Cloning() => points.ToList();

        // Алгоритм закрашивания внутри многоугольника
        public void FillIn(Graphics graphics, Pen drawPen, Pen drawPenBorder, int pictureBoxHeight, bool haveBorder)
        {
            PaintingLineInFigure(graphics, haveBorder, drawPenBorder);
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
                    graphics.DrawLine(drawPen, new Point((int) xs[i], y), new Point((int) xs[i + 1], y));

                xs.Clear();
            }
        }

        // Алгоритм для закрашивания многоугольника против часовой стрелки
        public void FillOut(Graphics graphics, bool haveBorder, Pen drawPen, Pen drawPenBorder, PictureBox pictureBox)
        {
            if (points.Count == 0) return;
            PaintingLineInFigure(graphics, haveBorder, drawPenBorder);
            var arr = SearchYMinAndMax(pictureBox.Height);
            var minY = (int) arr[0];
            var maxY = (int) arr[1];
            var cw = CalculationSquare((int) arr[2]);
            PaintAreasOutsideShape(graphics, drawPen, cw, 0, minY, pictureBox.Width);
            SecondCycleAlgorithm(graphics, drawPen, minY, maxY, cw, SearchXMinAndMax(), pictureBox.Width);
            PaintAreasOutsideShape(graphics, drawPen, cw, maxY, pictureBox.Height, pictureBox.Width);
        }

        // вычисление площади треугольника
        private bool CalculationSquare(int j)
        {
            if (j == 0)
                return Square(points.Count - 1, j, j + 1);
            return j == points.Count - 1 ? Square(j - 1, j, 0) : Square(j - 1, j, j + 1);
        }

        private bool Square(int prev, int now, int next) =>
            0.5 * ((points[prev].X * points[now].Y)
                   + (points[prev].Y * points[next].X)
                   + (points[now].X * points[next].Y)
                   - (points[now].Y * points[next].X)
                   - (points[prev].Y * points[now].X)
                   - (points[prev].X * points[next].Y)) < 0;

        public Tuple2<List<float>, List<float>> CalculationListXrAndXl(int y)
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
        private bool Check(int i, int k, int y) =>
            (points[i].Y < y && points[k].Y >= y) || (points[i].Y >= y && points[k].Y < y);

        /// <summary>
        ///  Проверка пересичения прямой Y c отрезком
        /// </summary>
        private List<float> CheckIntersection(List<float> xs, int i, int k, int y)
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
        private static void PaintAreasOutsideShape(Graphics graphics, Pen drawPen, bool cw, int y, int end, int width)
        {
            if (cw)
                for (var i = y; i < end; i++)
                    graphics.DrawLine(drawPen, new Point(0, i), new Point(width, i));
        }

        // Основная часть алгоритма закрашивания вне многоугольника
        private void SecondCycleAlgorithm(Graphics graphics, Pen drawPen, int minY, int maxY, bool cw, IReadOnlyList<float> arrForX,
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
                graphics.DrawLine(drawPen, new Point(0, y), new Point((int) xL[0], y));
                for (var i = 0; i < xL.Count && i < xR.Count; i++)
                    if (xL[i] <= xR[i])
                        graphics.DrawLine(drawPen, new Point((int) xL[i], y), new Point((int) xR[i], y));

                graphics.DrawLine(drawPen, new Point((int) xR[xR.Count - 1], y), new Point(width, y));
                xL.Clear();
                xR.Clear();
            }
        }

        /// <summary>
        ///  Рисование сторон
        /// </summary>
        public void PaintingLineInFigure(Graphics graphics, bool haveBorder, Pen drawPenBorder)
        {
            if (haveBorder && points.Count > 1)
            {
                for (var i = 0; i < points.Count - 1; i++)
                    graphics.DrawLine(drawPenBorder, points[i].ToPoint(), points[i + 1].ToPoint());

                graphics.DrawLine(drawPenBorder, points[0].ToPoint(), points[points.Count - 1].ToPoint());
            }
        }

        // Поиск мин/макс X
        private float[] SearchXMinAndMax()
        {
            var min = points[0].X;
            var max = 0.0f;
            for (var i = 0; i < points.Count; i++)
            {
                min = points[i].X < min ? points[i].X : min;
                max = points[i].X > max ? points[i].X : max;
            }

            return new[] {min, max};
        }

        // Поиск мин/макс Y
        public float[] SearchYMinAndMax(int height)
        {
            if (points.Count == 0)
                return new float[] {0, 0, 0};

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
            return new[] {min, max, j};
        }

        public bool ThisFigure(int mx, int my)
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

        public void Move(int dx, int dy)
        {
            var buffer = new MyPoint();
            for (var i = 0; i <= points.Count - 1; i++)
            {
                buffer.X = points[i].X + dx;
                buffer.Y = points[i].Y + dy;
                points[i] = buffer;
            }
        }

        public void Rotation(int rad, int height)
        {
            var alpha = rad / 57.3;
            var arrayY = SearchYMinAndMax(height);
            var arrayX = SearchXMinAndMax();
            var e = new MyPoint
            {
                X = (arrayX[0] + arrayX[1]) / 2,
                Y = (arrayY[0] + arrayY[1]) / 2
            };

            float[,] toCenter =
            {
                {1, 0, 0},
                {0, 1, 0},
                {-e.X, -e.Y, 1}
            };
            for (var i = 0; i <= points.Count - 1; i++)
                points[i] = Calculation.Matrix_1x3_3x3(points[i], toCenter);

            float[,] matrixRotation =
            {
                {(float) Math.Cos(alpha), (float) Math.Sin(alpha), 0.0f},
                {-(float) Math.Sin(alpha), (float) Math.Cos(alpha), 0.0f},
                {0.0f, 0.0f, 1.0f}
            };
            for (var i = 0; i <= points.Count - 1; i++)
                points[i] = Calculation.Matrix_1x3_3x3(points[i], matrixRotation);

            float[,] fromCenter =
            {
                {1, 0, 0},
                {0, 1, 0},
                {e.X, e.Y, 1}
            };
            for (var i = 0; i <= points.Count - 1; i++)
                points[i] = Calculation.Matrix_1x3_3x3(points[i], fromCenter);
        }


        #region Читерский вывод

        // Вывод многоугольнкиа с помощью FillPolygon
        private void PaintingFirstToAlgorithm(Graphics graphics, Pen drawPen)
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
        private void PaintingSecondToAlgorithm(Graphics graphics, Pen drawPen)
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