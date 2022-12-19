using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using _3_Laba_GSK.access;

namespace _3_Laba_GSK
{
    public partial class Form1 : Form
    {
        private enum Fill
        {
            In,
            Out
        }

        private enum FigureEnum
        {
            Triangle,
            Flag,
            Box,
            Ugl1,
            Ugl2,
            Star
        }

        private readonly Graphics graphics;
        private Figure figure;

        /// <summary>
        ///  Список фигур
        /// </summary>
        private readonly List<Figure> figures = new List<Figure>();

        /// <summary>
        ///  Рисуется фигура, выбранная пользователем
        /// </summary>
        private bool isSpecialFigureBeingFormed;

        /// <summary>
        ///  Буффер
        /// </summary>
        private readonly Bitmap bitmap;

        #region To select a user

        /// <summary>
        ///  Выбор алгоритма закрашивания фигуры (заливка)
        /// </summary>
        private Fill fill;

        /// <summary>
        /// Выбор фигуры для рисования
        /// </summary>
        private FigureEnum figureEnum;

        /// <summary>
        ///  Множество для ТМО
        /// </summary>
        private int[] setQ = new int[2];

        /// <summary>
        ///  Выбор цвета закрашивания фигуры
        /// </summary>
        private readonly Pen drawPen = new Pen(Color.Black, 1);

        /// <summary>
        ///  Проверка на рисование сторон
        /// </summary>
        private bool haveBorder;

        /// <summary>
        ///  Проверка на кривой Безье
        /// </summary>
        private bool haveBezies;

        /// <summary>
        ///  Количество углов
        /// </summary>
        private int angleCount;

        /// <summary>
        ///  Выбор операции
        /// </summary>
        private int operation;

        #endregion

        #region For move figure

        private bool checkFigure;
        private Point pictureBoxMousePosition;

        #endregion

        public Form1()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBoxMain.Width, pictureBoxMain.Height);
            graphics = Graphics.FromImage(bitmap);
            figure = new Figure(graphics, drawPen, pictureBoxMain.Height, pictureBoxMain.Width);
            StartPosition = FormStartPosition.CenterScreen;
            MouseWheel += GeometricTransformation;
        }

        // Обработчик события
        private void PictureMouseDown(object sender, MouseEventArgs e)
        {
            pictureBoxMousePosition = e.Location;
            switch (operation)
            {
                // Зарисовка
                case 0:
                {
                    if (isSpecialFigureBeingFormed)
                    {
                        CreateFigure(e);
                        IncludeFill();
                        isSpecialFigureBeingFormed = false;
                    }
                    else
                        switch (e.Button)
                        {
                            case MouseButtons.Right when haveBezies:
                                if (figure.GetPoints().Count < 2)
                                {
                                    figure.AddPoint(e.X, e.Y);
                                    return;
                                }

                                figure = DrawBezier();
                                figures.Add(figure.Cloning());
                                figures[figures.Count - 1].IsFunction = true;
                                figure.GetPoints().Clear();
                                break;
                            case MouseButtons.Right:
                                IncludeFill();
                                break;
                            case MouseButtons.Left:
                            case MouseButtons.None:
                            case MouseButtons.Middle:
                            case MouseButtons.XButton1:
                            case MouseButtons.XButton2:
                            default:
                                figure.AddPoint(e.X, e.Y);
                                break;
                        }
                }
                    break;
                default:
                    ThisFigure(e);
                    break;
            }

            pictureBoxMain.Image = bitmap;
        }

        // Обработчик события
        private void PictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && operation == 1 & checkFigure)
                MoveFigure(e);
        }

        private void GeometricTransformation(object sender, MouseEventArgs e)
        {
            var figureBuff = figures[figures.Count - 1];
            if (figureBuff.DoTmo)
            {
                OperationGeometric(figureBuff, e);
                OperationGeometric(figures[figures.Count - 2], e);
                graphics.Clear(Color.White);
                Tmo();
                pictureBoxMain.Image = bitmap;
            }
            else
                OperationGeometric(figureBuff, e);
        }

        private void OperationGeometric(Figure figureBuff, MouseEventArgs e)
        {
            switch (operation)
            {
                // Вращение
                case 2:
                case 3:
                    figureBuff.Rotation(e.Delta, pictureBoxMain.Height, textBox2, e, operation);
                    break;
                // Масштабирование
                case 4:
                case 5:
                case 6:
                    figureBuff.Zoom(pictureBoxMain.Height, new float[] {e.Delta, e.Delta}, operation, e);
                    break;
                // Отражение
                // ОХ
                case 7:
                    figures[figures.Count - 1].Mirror('x', pictureBoxMain.Height);
                    break;
                // OY
                case 8:
                    figures[figures.Count - 1].Mirror('y', pictureBoxMain.Height);
                    break;
                // относительно центра
                case 9:
                    figures[figures.Count - 1].Mirror('o', pictureBoxMain.Height);
                    break;
            }

            graphics.Clear(pictureBoxMain.BackColor);
            figureBuff.FillIn(drawPen, pictureBoxMain.Height, haveBorder);
            pictureBoxMain.Image = bitmap;
        }

        private void ThisFigure(MouseEventArgs e)
        {
            if (figures[figures.Count - 1].ThisFigure(e.X, e.Y))
            {
                graphics.DrawEllipse(new Pen(Color.Blue), e.X - 2, e.Y - 2, 10, 10);
                checkFigure = true;
            }
            else
                checkFigure = false;
        }

        private void MoveFigure(MouseEventArgs e)
        {
            figures[figures.Count - 1].Move(e.X - pictureBoxMousePosition.X, e.Y - pictureBoxMousePosition.Y);
            graphics.Clear(pictureBoxMain.BackColor);
            figures[figures.Count - 1].FillIn(drawPen, pictureBoxMain.Height, haveBorder);
            pictureBoxMain.Image = bitmap;
            pictureBoxMousePosition = e.Location;
        }

        private void CreateFigure(MouseEventArgs e)
        {
            switch (figureEnum)
            {
                case FigureEnum.Triangle:
                    CreateTriangle(e);
                    break;
                case FigureEnum.Flag:
                    CreateFlag(e);
                    break;
                case FigureEnum.Box:
                    CreateBox(e);
                    break;
                case FigureEnum.Ugl1:
                    CreateUgl1(e);
                    break;
                case FigureEnum.Ugl2:
                    CreateUgl2(e);
                    break;
                case FigureEnum.Star:
                    CreateStar(e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void IncludeFill()
        {
            if (fill == Fill.In)
                figure.FillIn(drawPen, pictureBoxMain.Height, haveBorder);
            else
                figure.FillOut(haveBorder, drawPen, pictureBoxMain);
            figures.Add(figure.Cloning());
            figure.GetPoints().Clear();
            pictureBoxMain.Image = bitmap;
        }

        #region TMO

        private void CheckWorkTmo()
        {
            if (figures.Count > 1)
            {
                graphics.Clear(Color.White);
                figures[0].PaintingLineInFigure(haveBorder); // Рисуем стороны первой фигуры
                figures[1].PaintingLineInFigure(haveBorder); // Рисуем стороны второй фигуры
                Tmo();
            }

            figure.GetPoints().Clear();
        }

        // Алгоритм теоретико-множественных операций
        private void Tmo()
        {
            var figure1 = figures[0];
            var figure2 = figures[1];
            var arr = figure1.SearchYMinAndMax(pictureBoxMain.Height);
            var arr2 = figure2.SearchYMinAndMax(pictureBoxMain.Height);
            figure1.DoTmo = true;
            figure2.DoTmo = true;
            var minY = arr[0] < arr2[0] ? arr[0] : arr2[0];
            var maxY = arr[1] > arr2[1] ? arr[1] : arr2[1];
            for (var y = (int) minY; y < maxY; y++)
            {
                var a = figure1.CalculationListXrAndXl(y);
                var xAl = a.First;
                var xAr = a.Second;
                var b = figure2.CalculationListXrAndXl(y);
                var xBl = b.First;
                var xBr = b.Second;
                if (xAl.Count == 0 && xBl.Count == 0)
                    continue;

                #region Заполнение массива arrayM

                var arrayM = new M[xAl.Count * 2 + xBl.Count * 2];
                for (var i = 0; i < xAl.Count; i++)
                    arrayM[i] = new M(xAl[i], 2);

                var nM = xAl.Count;
                for (var i = 0; i < xAr.Count; i++)
                    arrayM[nM + i] = new M(xAr[i], -2);

                nM += xAr.Count;
                for (var i = 0; i < xBl.Count; i++)
                    arrayM[nM + i] = new M(xBl[i], 1);

                nM += xBl.Count;
                for (var i = 0; i < xBr.Count; i++)
                    arrayM[nM + i] = new M(xBr[i], -1);
                nM += xBr.Count;

                #endregion

                // Сортировка
                SortArrayM(arrayM);

                var q = 0;
                var xrl = new List<int>();
                var xrr = new List<int>();
                // Особый случай для правой границы сегмента
                if (arrayM[0].X >= 0 && arrayM[0].Dq < 0)
                {
                    xrl.Add(0);
                    q = -arrayM[1].Dq;
                }

                for (var i = 0; i < nM; i++)
                {
                    var x = arrayM[i].X;
                    var qNew = q + arrayM[i].Dq;
                    if (!IncludeQInSetQ(q) && IncludeQInSetQ(qNew))
                        xrl.Add((int) x);
                    else if (IncludeQInSetQ(q) && !IncludeQInSetQ(qNew))
                        xrr.Add((int) x);

                    q = qNew;
                }

                // Если не найдена правая граница последнего сегмента
                if (IncludeQInSetQ(q))
                    xrr.Add(pictureBoxMain.Height);

                for (var i = 0; i < xrr.Count; i++)
                    graphics.DrawLine(drawPen, new Point(xrr[i], y), new Point(xrl[i], y));
            }
        }

        // Проверка вхождения Q в множество setQ
        private bool IncludeQInSetQ(int q) => setQ[0] <= q && q <= setQ[1];

        /// <summary>
        ///  Сортировка по Х
        /// </summary>
        private static void SortArrayM(IList<M> arrayM)
        {
            for (var write = 0; write < arrayM.Count; write++)
            for (var sort = 0; sort < arrayM.Count - 1; sort++)
                if (arrayM[sort].X > arrayM[sort + 1].X)
                {
                    var buff = new M(arrayM[sort + 1].X, arrayM[sort + 1].Dq);
                    arrayM[sort + 1] = arrayM[sort];
                    arrayM[sort] = buff;
                }
        }

        #endregion

        #region Создание фигур

        private void CreateTriangle(MouseEventArgs e)
        {
            var triangle = new List<MyPoint>
            {
                new MyPoint(e.X, e.Y - 200),
                new MyPoint(e.X + 200, e.Y + 100),
                new MyPoint(e.X - 200, e.Y + 100)
            };
            figure = new Figure(triangle, graphics, drawPen, pictureBoxMain.Height, pictureBoxMain.Width);
        }

        private void CreateFlag(MouseEventArgs e)
        {
            var flag = new List<MyPoint>
            {
                new MyPoint(e.X - 250, e.Y - 150),
                new MyPoint(e.X + 250, e.Y - 150),
                new MyPoint(e.X, e.Y),
                new MyPoint(e.X + 250, e.Y + 150),
                new MyPoint(e.X - 250, e.Y + 150)
            };
            figure = new Figure(flag, graphics, drawPen, pictureBoxMain.Height, pictureBoxMain.Width);
        }

        private void CreateUgl1(MouseEventArgs e)
        {
            var ugl1 = new List<MyPoint>
            {
                new MyPoint(e.X - 150, e.Y - 150),
                new MyPoint(e.X + 150, e.Y - 150),
                new MyPoint(e.X + 50, e.Y - 50),
                new MyPoint(e.X - 50, e.Y - 50),
                new MyPoint(e.X - 50, e.Y + 50),
                new MyPoint(e.X - 150, e.Y + 150)
            };
            figure = new Figure(ugl1, graphics, drawPen, pictureBoxMain.Height, pictureBoxMain.Width);
        }

        private void CreateUgl2(MouseEventArgs e)
        {
            var ugl2 = new List<MyPoint>
            {
                new MyPoint(e.X - 150, e.Y - 150),
                new MyPoint(e.X, e.Y - 150),
                new MyPoint(e.X, e.Y),
                new MyPoint(e.X + 150, e.Y),
                new MyPoint(e.X + 150, e.Y + 150),
                new MyPoint(e.X - 150, e.Y + 150),
            };
            figure = new Figure(ugl2, graphics, drawPen, pictureBoxMain.Height, pictureBoxMain.Width);
        }

        private void CreateBox(MouseEventArgs e)
        {
            var box = new List<MyPoint>
            {
                new MyPoint(e.X - 150, e.Y - 150),
                new MyPoint(e.X + 150, e.Y - 150),
                new MyPoint(e.X + 150, e.Y + 150),
                new MyPoint(e.X - 150, e.Y + 150),
            };
            figure = new Figure(box, graphics, drawPen, pictureBoxMain.Height, pictureBoxMain.Width);
        }

        private void CreateStar(MouseEventArgs e)
        {
            const double rR = 25;
            const double r = 50;
            const double d = 0;
            double a = d, da = Math.PI / angleCount, l;
            var star = new List<MyPoint>();
            for (var k = 0; k < 2 * angleCount + 1; k++)
            {
                l = k % 2 == 0 ? r : rR;
                star.Add(new MyPoint((int) (e.X + l * Math.Cos(a)), (int) (e.Y + l * Math.Sin(a))));
                a += da;
            }

            figure = new Figure(star, graphics, drawPen, pictureBoxMain.Height, pictureBoxMain.Width);
        }

        // Факториал
        private static double Factorial(int n)
        {
            double x = 1;
            for (var i = 1; i <= n; i++)
                x *= i;
            return x;
        }

        private static double Polinom(int i, int n, float t) => Factorial(n) / (Factorial(i) * Factorial(n - i))
                                                                * (float) Math.Pow(t, i) *
                                                                (float) Math.Pow(1 - t, n - i);

        // Кривая Безье
        private Figure DrawBezier()
        {
            const double dt = 0.01;
            var t = dt;
            var listVertexes = figure.GetPoints();
            double xPred = listVertexes[0].X;
            double yPred = listVertexes[0].Y;
            var newFigure = new List<MyPoint>();
            while (t < 1)
            {
                double x = 0;
                double y = 0;

                for (var i = 0; i < listVertexes.Count; i++)
                {
                    var b = Polinom(i, listVertexes.Count - 1, (float) t);
                    x += listVertexes[i].X * b;
                    y += listVertexes[i].Y * b;
                }

                newFigure.Add(new MyPoint((float) x, (float) y));
                graphics.DrawLine(drawPen, new Point((int) xPred, (int) yPred), new Point((int) x, (int) y));
                t += dt;
                xPred = x;
                yPred = y;
            }

            return new Figure(newFigure, graphics, drawPen, pictureBoxMain.Height, pictureBoxMain.Width);
        }

        #endregion

        #region Выбор пользователя

        // Выбираем фигуру

        private void ComboBox_SelectFigure(object sender, EventArgs e)
        {
            switch (comboBoxSelectFigure.SelectedIndex)
            {
                case 0:
                    figureEnum = FigureEnum.Triangle;
                    break;
                case 1:
                    figureEnum = FigureEnum.Flag;
                    break;
                case 2:
                    figureEnum = FigureEnum.Box;
                    break;
                case 3:
                    figureEnum = FigureEnum.Ugl1;
                    break;
                case 4:
                    figureEnum = FigureEnum.Ugl2;
                    break;
                case 5:
                    figureEnum = FigureEnum.Star;
                    break;
            }

            isSpecialFigureBeingFormed = true;
        }

        // Выбираем алгоритма закрашивания (внутри/снаружи)
        private void ComboBox_SelectPainting(object sender, EventArgs e)
        {
            switch (comboBoxFill.SelectedIndex)
            {
                case 0:
                    fill = Fill.In;
                    break;
                case 1:
                    fill = Fill.Out;
                    break;
            }
        }

        // Выбираем цвет закраски
        private void ComboBox_GetColor(object sender, EventArgs e)
        {
            switch (comboBoxColor.SelectedIndex)
            {
                case 0:
                    drawPen.Color = Color.Black;
                    break;
                case 1:
                    drawPen.Color = Color.Red;
                    break;
                case 2:
                    drawPen.Color = Color.Green;
                    break;
                case 3:
                    drawPen.Color = Color.Blue;
                    break;
            }
        }

        // Имеют ли фигуры границы
        private void ComboBox_HaveBorder(object sender, EventArgs e) => haveBorder = checkBox1.Checked;

        // Выбор ТМО
        private void ComboBox_SelectTMO(object sender, EventArgs e)
        {
            switch (comboBoxSelectTmo.SelectedIndex)
            {
                case 0:
                    setQ = new[] {1, 3}; // Объединение
                    break;
                case 1:
                    setQ = new[] {3, 3}; // Пересечение
                    break;
                case 2:
                    setQ = new[] {1, 2}; // Симметричная разность
                    break;
                case 3:
                    setQ = new[] {2, 2}; // Разность А/В
                    break;
                case 4:
                    setQ = new[] {1, 1}; // Разность В/А
                    break;
            }
        }

        // Ввод количества вершин
        private void ComboBox_VertexCount(object sender, EventArgs e) =>
            angleCount = comboBoxVertCount.SelectedIndex + 5;

        private void ComboBox_SelectOperation(object sender, EventArgs e) =>
            operation = comboBoxOperation.SelectedIndex;

        // Кнопка очистки формы
        private void Button_Clear(object sender, EventArgs e)
        {
            figure.GetPoints().Clear();
            pictureBoxMain.Image = bitmap;
            graphics.Clear(Color.White);
            figures.Clear();
            operation = 0;
        }

        private void Button_RunTMO(object sender, EventArgs e)
        {
            CheckWorkTmo();
            pictureBoxMain.Image = bitmap;
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void CheckBoxBeziers(object sender, EventArgs e) => haveBezies = checkBox2.Checked;
    }
}