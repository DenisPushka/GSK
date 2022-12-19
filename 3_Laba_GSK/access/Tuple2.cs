using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Laba_GSK
{
    public class Tuple2<T, TK>
    {
        public T First { get; }
        public TK Second { get; }

        public Tuple2(T first, TK second)
        {
            First = first;
            Second = second;
        }
    }
}
