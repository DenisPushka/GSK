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
        private readonly List<Figure> listFigure = new List<Figure>();

        /// <summary>
        ///  Цвет рисовки сторон
        /// </summary>
        private readonly Pen drawPenBorder = new Pen(Color.Orange, 5);

        /// <summary>
        ///  Массив для совместной обработки исходных границ сегмента
        /// </summary>
        private M[] arrayM;

        /// <summary>
        ///  Спец фигура формируется
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
        private readonly int[] setQ = new int[2];

        /// <summary>
        ///  Выбор цвета закрашивания фигуры
        /// </summary>
        private readonly Pen drawPen = new Pen(Color.Black, 1);

        /// <summary>
        ///  Проверка на рисование сторон
        /// </summary>
        private bool haveBorder;

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

        private int alpha;

        public Form1()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);
            figure = new Figure();
            StartPosition = FormStartPosition.CenterScreen;
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
                        return;
                    }

                    if (e.Button == MouseButtons.Right)
                        IncludeFill();
                    else
                        figure.AddPoint(graphics, e, drawPen);
                }
                    break;
                // Перемещение
                case 1:
                    ThisFugure(e);
                    break;
                // Вращение
                case 2:
                    ThisFugure(e);
                    break;
            }

            pictureBox1.Image = bitmap;
        }

        // Обработчик события
        private void PictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && operation == 1 & checkFigure)
                MoveFigure(e);
            else if (e.Button == MouseButtons.Left && operation == 2 & checkFigure)
            {
                var buff = alpha;
                alpha = (int) Math.Sqrt(Math.Pow(e.X - pictureBoxMousePosition.X, 2) +
                                        Math.Pow(e.Y - pictureBoxMousePosition.Y, 2));
                textBox2.Text = alpha.ToString();
                if (alpha != buff)
                    Rotetion();
            }
        }

        private void Rotetion()
        {
            if (operation == 2)
            {
                listFigure[listFigure.Count - 1].Rotation(alpha, pictureBox1.Height);
                graphics.Clear(pictureBox1.BackColor);
                listFigure[listFigure.Count - 1]
                    .FillIn(graphics, drawPen, drawPenBorder, pictureBox1.Height, haveBorder);
                pictureBox1.Image = bitmap;
            }
        }

        private void ThisFugure(MouseEventArgs e)
        {
            if (listFigure[listFigure.Count - 1].ThisFigure(e.X, e.Y))
            {
                graphics.DrawEllipse(new Pen(Color.Blue), e.X - 2, e.Y - 2, 10, 10);
                checkFigure = true;
            }
            else
                checkFigure = false;
        }

        private void MoveFigure(MouseEventArgs e)
        {
            listFigure[listFigure.Count - 1].Move(e.X - pictureBoxMousePosition.X, e.Y - pictureBoxMousePosition.Y);
            graphics.Clear(pictureBox1.BackColor);
            listFigure[listFigure.Count - 1].FillIn(graphics, drawPen, drawPenBorder, pictureBox1.Height, haveBorder);
            pictureBox1.Image = bitmap;
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
                figure.FillIn(graphics, drawPen, drawPenBorder, pictureBox1.Height, haveBorder);
            else
                figure.FillOut(graphics, haveBorder, drawPen, drawPenBorder, pictureBox1);
            listFigure.Add(CloningFigure());
            figure.GetPoints().Clear();
            pictureBox1.Image = bitmap;
        }

        private Figure CloningFigure() => new Figure(figure.Cloning());

        private void CheckWorkTmo()
        {
            if (listFigure.Count > 1)
            {
                graphics.Clear(Color.White);
                listFigure[0].PaintingLineInFigure(graphics, haveBorder, drawPenBorder); // Рисуем стороны первой фигуры
                listFigure[1].PaintingLineInFigure(graphics, haveBorder, drawPenBorder); // Рисуем стороны второй фигуры
                Tmo();
            }

            figure.GetPoints().Clear();
        }

        // Алгоритм теоретико-множественных операций
        private void Tmo()
        {
            var arr = listFigure[0].SearchYMinAndMax(pictureBox1.Height);
            var arr2 = listFigure[1].SearchYMinAndMax(pictureBox1.Height);
            var minY = arr[0] < arr2[0] ? arr[0] : arr2[0];
            var maxY = arr[1] > arr2[1] ? arr[1] : arr2[1];
            for (var Y = (int) minY; Y < maxY; Y++)
            {
                var A = listFigure[0].CalculationListXrAndXl(Y);
                List<float> xAl = A.First;
                List<float> xAr = A.Second;
                var B = listFigure[1].CalculationListXrAndXl(Y);
                List<float> xBl = B.First;
                List<float> xBr = B.Second;
                if (xAl.Count == 0 && xBl.Count == 0)
                    continue;

                #region Заполнение массива arrayM

                arrayM = new M[xAl.Count * 2 + xBl.Count * 2];
                for (int i = 0; i < xAl.Count; i++)
                    arrayM[i] = new M(xAl[i], 2);

                var nM = xAl.Count;
                for (int i = 0; i < xAr.Count; i++)
                    arrayM[nM + i] = new M(xAr[i], -2);

                nM += xAr.Count;
                for (int i = 0; i < xBl.Count; i++)
                    arrayM[nM + i] = new M(xBl[i], 1);

                nM += xBl.Count;
                for (int i = 0; i < xBr.Count; i++)
                    arrayM[nM + i] = new M(xBr[i], -1);
                nM += xBr.Count;

                #endregion

                // Сортировка
                SortArrayM();

                var Q = 0;
                List<int> xrl = new List<int>();
                List<int> xrr = new List<int>();
                // Особый случай для правой границы сегмента
                if (arrayM[0].X >= 0 && arrayM[0].Dq < 0)
                {
                    xrl.Add(0);
                    Q = -arrayM[1].Dq;
                }

                for (var i = 0; i < nM; i++)
                {
                    var x = arrayM[i].X;
                    var Qnew = Q + arrayM[i].Dq;
                    if (!IncludeQInSetQ(Q) && IncludeQInSetQ(Qnew))
                        xrl.Add((int) x);
                    else if (IncludeQInSetQ(Q) && !IncludeQInSetQ(Qnew))
                        xrr.Add((int) x);

                    Q = Qnew;
                }

                // Если не найдена правая граница последнего сегмента
                if (IncludeQInSetQ(Q))
                    xrr.Add(pictureBox1.Height);

                for (var i = 0; i < xrr.Count; i++)
                    graphics.DrawLine(drawPen, new Point(xrr[i], Y), new Point(xrl[i], Y));
            }
        }

        // Проверка вхождения Q в множество setQ
        private bool IncludeQInSetQ(int Q) => setQ[0] <= Q && Q <= setQ[1];

        /// <summary>
        ///  Сортировка по Х
        /// </summary>
        private void SortArrayM()
        {
            _ = new M(0, 0);
            for (var write = 0; write < arrayM.Length; write++)
            {
                for (var sort = 0; sort < arrayM.Length - 1; sort++)
                {
                    if (arrayM[sort].X > arrayM[sort + 1].X)
                    {
                        var buuf = new M(arrayM[sort + 1].X, arrayM[sort + 1].Dq);
                        arrayM[sort + 1] = arrayM[sort];
                        arrayM[sort] = buuf;
                    }
                }
            }
        }

        #region Создание фигур

        private void CreateTriangle(MouseEventArgs e)
        {
            var triangle = new List<MyPoint>()
            {
                new MyPoint(e.X, e.Y - 200),
                new MyPoint(e.X + 200, e.Y + 100),
                new MyPoint(e.X - 200, e.Y + 100)
            };
            figure = new Figure(triangle);
        }

        private void CreateFlag(MouseEventArgs e)
        {
            var flag = new List<MyPoint>()
            {
                new MyPoint(e.X - 250, e.Y - 150),
                new MyPoint(e.X + 250, e.Y - 150),
                new MyPoint(e.X, e.Y),
                new MyPoint(e.X + 250, e.Y + 150),
                new MyPoint(e.X - 250, e.Y + 150)
            };
            figure = new Figure(flag);
        }

        private void CreateUgl1(MouseEventArgs e)
        {
            var ugl1 = new List<MyPoint>()
            {
                new MyPoint(e.X - 150, e.Y - 150),
                new MyPoint(e.X + 150, e.Y - 150),
                new MyPoint(e.X + 50, e.Y - 50),
                new MyPoint(e.X - 50, e.Y - 50),
                new MyPoint(e.X - 50, e.Y + 50),
                new MyPoint(e.X - 150, e.Y + 150)
            };
            figure = new Figure(ugl1);
        }

        private void CreateUgl2(MouseEventArgs e)
        {
            var ugl2 = new List<MyPoint>()
            {
                new MyPoint(e.X - 150, e.Y - 150),
                new MyPoint(e.X, e.Y - 150),
                new MyPoint(e.X, e.Y),
                new MyPoint(e.X + 150, e.Y),
                new MyPoint(e.X + 150, e.Y + 150),
                new MyPoint(e.X - 150, e.Y + 150),
            };
            figure = new Figure(ugl2);
        }

        private void CreateBox(MouseEventArgs e)
        {
            var box = new List<MyPoint>()
            {
                new MyPoint(e.X - 150, e.Y - 150),
                new MyPoint(e.X + 150, e.Y - 150),
                new MyPoint(e.X + 150, e.Y + 150),
                new MyPoint(e.X - 150, e.Y + 150),
            };
            figure = new Figure(box);
        }

        private void CreateStar(MouseEventArgs e)
        {
            const double R = 25; // радиусы
            const double r = 50; // радиусы
            const double d = 0; // поворот
            double a = d, da = Math.PI / angleCount, l;
            var star = new List<MyPoint>();
            for (var k = 0; k < 2 * angleCount + 1; k++)
            {
                l = k % 2 == 0 ? r : R;
                star.Add(new MyPoint((int) (e.X + l * Math.Cos(a)), (int) (e.Y + l * Math.Sin(a))));
                a += da;
            }

            figure = new Figure(star);
        }

        #endregion

        #region Выбор пользователя

        // Выбираем фигуру
        private void ComboBox_SelectFigure(object sender, EventArgs e)
        {
            switch (comboBox3.SelectedIndex)
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
            switch (comboBox2.SelectedIndex)
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
            switch (comboBox1.SelectedIndex)
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
            switch (comboBox4.SelectedIndex)
            {
                case 0:
                    setQ[0] = 1;
                    setQ[1] = 3; // объединение
                    break;
                case 1:
                    setQ[0] = 3;
                    setQ[1] = 3;
                    ; // пересечение
                    break;
                case 2:
                    setQ[0] = 1;
                    setQ[1] = 2; // Сим. разность
                    break;
                case 3:
                    setQ[0] = 2;
                    setQ[1] = 2; // Разность А/В
                    break;
                case 4:
                    setQ[0] = 1;
                    setQ[1] = 1; // Разность В/А
                    break;
            }
        }

        // Ввод кол-ва углов
        private void ComboBox_AngleCount(object sender, EventArgs e) => angleCount = comboBox5.SelectedIndex + 5;

        private void ComboBox_SelectOperation(object sender, EventArgs e) => operation = comboBox6.SelectedIndex;

        // Кнопка очистки формы
        private void Button_Clear(object sender, EventArgs e)
        {
            figure.GetPoints().Clear();
            pictureBox1.Image = bitmap;
            graphics.Clear(Color.White);
            listFigure.Clear();
            operation = 1;
        }

        private void Button_RunTMO(object sender, EventArgs e)
        {
            CheckWorkTmo();
            pictureBox1.Image = bitmap;
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
    }
}