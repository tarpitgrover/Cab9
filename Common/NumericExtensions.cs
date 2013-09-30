using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.Common
{
    public static class NumericExtensions
    {
        public static decimal SafeDivision(this decimal Numerator, decimal Denominator)
        {
            return (Denominator == 0) ? 0 : Numerator / Denominator;
        }

        public static decimal SafeDivision(this int Numerator, int Denominator)
        {
            return (Denominator == 0) ? 0 : Numerator / Denominator;
        }

        public static decimal SafeDivision(this long Numerator, long Denominator)
        {
            return (Denominator == 0) ? 0 : Numerator / Denominator;
        }
    }

}