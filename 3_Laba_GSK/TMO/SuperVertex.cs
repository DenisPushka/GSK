namespace _3_Laba_GSK.TMO
{
    public struct SuperVertex
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Index1FigureA { get; set; }
        public int Index2FigureA { get; set; }
        public int Index1FigureB { get; set; }
        public int Index2FigureB { get; set; }

        /// <summary>
        /// Количество связей с вершиной
        /// </summary>
        public int NumberLink { get; set; }
    }
}