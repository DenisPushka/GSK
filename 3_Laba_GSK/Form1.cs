using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using _3_Laba_GSK.access;

namespace _3_Laba_GSK
{
    public partial class Form1 : Form
    {
        private enum Figures
        {
            Cross,
            Star
        }

        private readonly Graphics graphics;
        private Figure figure;

        /// <summary>
        ///  Список фигур
        /// </summary>
        private readonly List<Figure> figures = new List<Figure>();

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
        /// Выбор фигуры для рисования
        /// </summary>
        private Figures figureEnum;
        /// <summary>
        ///  Множество для ТМО
        /// </summary>
        private readonly int[] setQ = new int[2];
        /// <summary>
        ///  Выбор цвета закрашивания фигуры
        /// </summary>
        private readonly Pen drawPen = new Pen(Color.Black, 1);
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

        private bool checkFigure;
        private Point pictureBoxMousePosition;

        public Form1()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);
            figure = new Figure(graphics);
            StartPosition = FormStartPosition.CenterScreen;
            MouseWheel += GeometricTransformation;
            MouseWheel += DoMirror;
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
                        Create(e);
                        FillIncl();
                        isSpecialFigureBeingFormed = false;
                    }
                    else if (operation == 4 && figures.Count != 0)
                        ThisFigure(e);
                    else if (e.Button == MouseButtons.Right && haveBezies)
                    {
                        figure = figure.DrawBezier(drawPen);
                        figures.Add(CloningFigure());
                        figures[figures.Count - 1].IsFunction = true;
                        figure.GetPoints().Clear();
                    }
                    else if (e.Button == MouseButtons.Right)
                        FillIncl();
                    else
                        figure.AddPoint(e, drawPen);
                }
                    break;
                // Перемещение
                default:
                    ThisFigure(e);
                    break;
            }

            pictureBox1.Image = bitmap;
        }

        // Обработчик события
        private void PictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && operation == 4 & checkFigure)
                MoveFigure(e);
        }

        private void GeometricTransformation(object sender, MouseEventArgs e)
        {
            var figureBuff = figures[figures.Count - 1];
            if (figureBuff.HaveTmo)
            {
                TG(figureBuff, e);
                TG(figures[figures.Count - 2], e);
                graphics.Clear(Color.White);
                Tmo();
                pictureBox1.Image = bitmap;
            }
            else
                TG(figureBuff, e);
        }

        private void TG(Figure figureBuff, MouseEventArgs e)
        {
            if (operation == 1)
                figureBuff.Rotation(e.Delta, textBox2, e);
            else if (operation == 2)
                figureBuff.Zoom(pictureBox1.Height, new float[] {e.Delta, e.Delta});

            graphics.Clear(pictureBox1.BackColor);
            if (figureBuff.IsFunction)
                figureBuff.PaintingLineInFigure(drawPen);
            else
                figureBuff.FillIn(drawPen, pictureBox1.Height);
            pictureBox1.Image = bitmap;
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
        
        private void MoveFigure (MouseEventArgs e)
        {
            figures[figures.Count - 1].Move(e.X - pictureBoxMousePosition.X, e.Y - pictureBoxMousePosition.Y);
            graphics.Clear(pictureBox1.BackColor);
            figures[figures.Count - 1].FillIn(drawPen, pictureBox1.Height);
            pictureBox1.Image = bitmap;
            pictureBoxMousePosition = e.Location;
        }

        private void Create(MouseEventArgs e)
        {
            switch (figureEnum)
            {
                case Figures.Cross:
                    CreateCross(e);
                    break;
                case Figures.Star:
                    CreateStar(e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FillIncl()
        {
            figure.FillIn(drawPen, pictureBox1.Height);
            figures.Add(CloningFigure());
            figure.GetPoints().Clear();
            pictureBox1.Image = bitmap;
        }

        private Figure CloningFigure() => new Figure(figure.Cloning(), graphics);

        private void CheckWorkTmo()
        {
            if (figures.Count > 1)
            {
                graphics.Clear(Color.White);
                Tmo();
            }

            figure.GetPoints().Clear();
        }

        // Алгоритм теоретико-множественных операций
        private void Tmo()
        {
            var figure1 = figures[figures.Count - 2];
            var figure2 = figures[figures.Count - 1];
            var arr = figure1.SearchYMinAndMax(pictureBox1.Height);
            var arr2 = figure2.SearchYMinAndMax(pictureBox1.Height);
            figure1.HaveTmo = true;
            figure2.HaveTmo = true;
            var minY = arr[0] < arr2[0] ? arr[0] : arr2[0];
            var maxY = arr[1] > arr2[1] ? arr[1] : arr2[1];
            for (var Y = (int) minY; Y < maxY; Y++)
            {
                var A = figure1.CalculationListXrAndXl(Y);
                List<float> xAl = A[0];
                List<float> xAr = A[1];
                var B = figure2.CalculationListXrAndXl(Y);
                List<float> xBl = B[0];
                List<float> xBr = B[1];
                if (xAl.Count == 0 && xBl.Count == 0)
                    continue;

                #region Заполнение массива arrayM

                arrayM = new M[xAl.Count * 2 + xBl.Count * 2];
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
                SortM();

                var Q = 0;
                var xrl = new List<int>();
                var xrr = new List<int>();
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
        private bool IncludeQInSetQ(int q) => setQ[0] <= q && q <= setQ[1];

        /// <summary>
        ///  Сортировка по Х
        /// </summary>
        private void SortM()
        {
            for (var write = 0; write < arrayM.Length; write++)
            for (var sort = 0; sort < arrayM.Length - 1; sort++)
                if (arrayM[sort].X > arrayM[sort + 1].X)
                {
                    var buf = new M(arrayM[sort + 1].X, arrayM[sort + 1].Dq);
                    arrayM[sort + 1] = arrayM[sort];
                    arrayM[sort] = buf;
                }
        }

        private void CreateCross(MouseEventArgs e)
        {
            var cross = new List<PointF>
            {
                new PointF(e.X - 150, e.Y - 50),
                new PointF(e.X - 50, e.Y - 50),
                new PointF(e.X - 50, e.Y - 150),
                new PointF(e.X + 50, e.Y - 150),
                new PointF(e.X + 50, e.Y - 50),
                new PointF(e.X + 150, e.Y - 50),
                new PointF(e.X + 150, e.Y + 50),
                new PointF(e.X + 50, e.Y + 50),
                new PointF(e.X + 50, e.Y + 150),
                new PointF(e.X - 50, e.Y + 150),
                new PointF(e.X - 50, e.Y + 50),
                new PointF(e.X - 150, e.Y + 50),
            };
            figure = new Figure(cross, graphics);
        }

        private void CreateStar(MouseEventArgs e)
        {
            const double R = 25;
            const double r = 50;
            const double d = 0;
            double a = d, da = Math.PI / angleCount;
            var star = new List<PointF>();
            for (var k = 0; k < 2 * angleCount + 1; k++)
            {
                var l = k % 2 == 0 ? r : R;
                star.Add(new PointF((int) (e.X + l * Math.Cos(a)), (int) (e.Y + l * Math.Sin(a))));
                a += da;
            }

            figure = new Figure(star, graphics);
        }

        #region Выбор пользователя

        // Выбираем фигуру
        private void ComboBox_SelectFigure(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == 0)
                figureEnum = Figures.Cross;
            else if (comboBox3.SelectedIndex == 1) 
                figureEnum = Figures.Star;

            isSpecialFigureBeingFormed = true;
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
                    setQ[0] = 2;
                    setQ[1] = 2; // Разность А/В
                    break;
                case 2:
                    setQ[0] = 1;
                    setQ[1] = 1; // Разность В/А
                    break;
            }
        }

        // Ввод кол-ва вершин звезды
        private void ComboBox_AngleCount(object sender, EventArgs e) => angleCount = comboBox5.SelectedIndex + 5;

        private void ComboBox_SelectOperation(object sender, EventArgs e) => operation = comboBox6.SelectedIndex;

        // Кнопка очистки формы
        private void Button_Clear(object sender, EventArgs e)
        {
            figure.GetPoints().Clear();
            graphics.Clear(Color.White);
            figures.Clear();
            pictureBox1.Image = bitmap;
            operation = 0;
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

        /// <summary>
        /// Отражение
        /// </summary>
        private void DoMirror(object sender, MouseEventArgs e)
        {
            if (figures.Count == 0) return;

            // Отражение
            if (operation == 3)
                figures[figures.Count - 1].Mirror(e);

            pictureBox1.Image = bitmap;
        }

        private void CheckBoxBeziers(object sender, EventArgs e) => haveBezies = checkBox2.Checked;
    }
}