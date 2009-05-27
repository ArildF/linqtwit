using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.Infrastructure
{
    public class Tuple
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 first, T2 second)
        {
            return new Tuple<T1, T2>(first, second);
        }
    }

    public struct Tuple<T1, T2>
    {
        public readonly T1 First;
        public readonly T2 Second;

        public Tuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

    }
}
