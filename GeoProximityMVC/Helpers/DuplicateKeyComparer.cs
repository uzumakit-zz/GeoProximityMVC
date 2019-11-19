using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoProximityMVC.Helpers
{
    public class DuplicateKeyComparer<T> : IComparer<T> where T : IComparable
    {
        public int Compare(T x, T y)
        {
            var result = x.CompareTo(y);
            if (result == 0)
                return 1;
            else
                return result;         
        }
    }
}