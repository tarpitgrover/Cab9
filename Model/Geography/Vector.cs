using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.Geography
{
    public class Vector
    {
        public decimal XMagnitude { get; set; }
        public decimal YMagnitude { get; set; }

        public Vector(decimal xMagnitude, decimal yMagnitude)
        {
            XMagnitude = xMagnitude;
            YMagnitude = yMagnitude;
        }

        public Vector(Point start, Point end)
        {
            XMagnitude = end.longitude - start.longitude;
            YMagnitude = end.latitude - start.latitude;
        }

        public static decimal Perp(Vector v1, Vector v2)
        {
            return (v1.XMagnitude * v2.YMagnitude) - (v1.YMagnitude * v2.XMagnitude);
        }

        public static decimal DotProduct(Vector v1, Vector v2)
        {
            return (v1.XMagnitude * v2.XMagnitude) + (v1.YMagnitude * v2.YMagnitude);
        }

        public static Vector operator *(decimal a, Vector b)
        {
            return new Vector((b.XMagnitude * a), (b.YMagnitude * a));
        }
    }
}