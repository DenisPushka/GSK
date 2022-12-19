using System.Collections.Generic;

namespace _3_Laba_GSK.TMO
{
    public class Tmo
    {
        /// <summary>
        /// Пересечение
        /// </summary>
        public Figure TmoIntersection(Figure figureOne, Figure figureSecond)
        {
            var bufferResultFigure = new List<SuperVertex>();
            bufferResultFigure = AddVertexFromSecondFigure(figureOne, figureSecond, bufferResultFigure, 'a');
            bufferResultFigure = AddVertexFromSecondFigure(figureSecond, figureOne, bufferResultFigure, 'b');
            var listVertexes1 = figureOne.GetPoints();
            var listVertexes2 = figureSecond.GetPoints();

            #region Поиск вершин

            // Поиск пересечений двух фигур
            var k1 = 0;
            for (var i1 = 0; i1 < listVertexes1.Count - 1; i1++)
            {
                k1 = i1 + 1;
                var k2 = 0;
                for (var i2 = 0; i2 < listVertexes2.Count - 1; i2++)
                {
                    k2 = i2 + 1;
                    bufferResultFigure = CalculationVertex(listVertexes1, i1, k1, listVertexes2, i2, k2, bufferResultFigure);
                }

                CalculationVertex(listVertexes1, i1, k1, listVertexes2, k2, 0, bufferResultFigure);
            }

            // Поиск пересечения последнего отрезка первой фигуры со второй фигурой
            var k3 = 0;
            for (var i2 = 0; i2 < listVertexes2.Count - 1; i2++)
            {
                k3 = i2 + 1;
                bufferResultFigure = CalculationVertex(listVertexes1, k1, 0, listVertexes2, i2, k3, bufferResultFigure);
            }

            // Поиск пересечения последнего отрезка первой фигуры с последним отрезком второй фигуры
            bufferResultFigure = CalculationVertex(listVertexes1, k1, 0, listVertexes2, k3, 0, bufferResultFigure);

            #endregion

            if (bufferResultFigure.Count == 0) return new Figure();

            var arraySuperVertexes = new SuperVertex[bufferResultFigure.Count];   
            // Поиск соседних вершин для первой вершины буфферного списка
            
            
            return new Figure();
        }

        private List<SuperVertex> CalculationVertex(IReadOnlyList<MyPoint> listVertexes1, int i1, int k1,
            IReadOnlyList<MyPoint> listVertexes2, int i2, int k2, List<SuperVertex> resultFigure)
        {
            if (CheckAndAddVertex(listVertexes1[i1].X, listVertexes1[i1].Y,
                                listVertexes1[k1].X, listVertexes1[k1].Y,
                                listVertexes2[i2].X, listVertexes2[i2].Y,
                                listVertexes2[k2].X, listVertexes2[k2].Y))
                resultFigure.Add(new SuperVertex
                {
                    X = vertex[0], Y = vertex[1],
                    Index1FigureA = i1, Index2FigureA = k1,
                    Index1FigureB = i2, Index2FigureB = k2
                });
            return resultFigure;
        }

        /// <summary>
        /// Координаты вершины внутри фигуры
        /// </summary>
        private readonly float[] vertex = new float[2];

        private bool CheckAndAddVertex(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            float n;
            if (y2 - y1 != 0)
            {
                var q = (x2 - x1) / (y1 - y2);
                var sn = (x3 - x4) + (y3 - y4) * q;
                if (sn == 0)
                    return false;

                var fn = (x3 - x1) + (y3 - y1) * q; // b(x) + b(y)*q
                n = fn / sn;
            }
            else
            {
                if (y3 - y4 == 0)
                    return false;

                n = (y3 - y1) / (y3 - y4); // c(y)/b(y)
            }

            vertex[0] = x3 + (x4 - x3) * n; // x3 + (-b(x))*n
            vertex[1] = y3 + (y4 - y3) * n; // y3 +(-b(y))*n
            return true;
        }

        /// <summary>
        /// Добавление вершины, при нахождении вершины фигуры1 в фигуре2
        /// </summary>
        /// <param name="figureOne">Фигура 1</param>
        /// <param name="figureSecond">Фигура 2</param>
        /// <param name="resultFigure">Результирующая фигура</param>
        /// <param name="atr">Обозначение фигуры 1 и 2</param>
        /// <returns>resultFigure</returns>
        private static List<SuperVertex> AddVertexFromSecondFigure(Figure figureOne, Figure figureSecond,
            List<SuperVertex> resultFigure, char atr)
        {
            var list = figureOne.GetPoints();
            for (var index = 0; index < list.Count; index++)
            {
                var vertex = list[index];
                if (figureSecond.ThisFigure((int) vertex.X, (int) vertex.Y))
                {
                    switch (atr)
                    {
                        case 'a':
                            resultFigure.Add(new SuperVertex
                            {
                                X = vertex.X, Y = vertex.Y,
                                Index1FigureA = index, Index2FigureA = index,
                                Index1FigureB = -1, Index2FigureB = -1
                            });
                            break;
                        case 'b':
                            resultFigure.Add(new SuperVertex
                            {
                                X = vertex.X, Y = vertex.Y,
                                Index1FigureA = -1, Index2FigureA = -1,
                                Index1FigureB = index, Index2FigureB = index
                            });
                            break;
                    }
                }
            }

            return resultFigure;
        }
        
        
        
        
        
        /// <summary>
        /// Объединение
        /// </summary>
        public Figure TmoUnion()
        {
            return new Figure();
        }

        /// <summary>
        /// Симметричная разность
        /// </summary>
        public Figure TmoSymmetricDifference()
        {
            return new Figure();
        }

        /// <summary>
        /// Разность
        /// </summary>
        public Figure TmoDifference()
        {
            return new Figure();
        }
    }
}