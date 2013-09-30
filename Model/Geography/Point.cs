using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.Geography
{
    public class Point
    {
        public decimal longitude;
        public decimal latitude;

        public Point(decimal latitude, decimal longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public override string ToString()
        {
            return latitude + ","+ longitude;
        }

        //public static bool operator ==(Point a, Point b)
        //{
        //    if (a == null && b == null) return true;
        //    if (a == null || b == null) return false;
        //    return (a.latitude == b.latitude && a.longitude == b.longitude);
        //}

        //public static bool operator !=(Point a, Point b)
        //{

        //    return !(a == b);
        //}

        public static Point operator +(Point a, Vector b)
        {
            return new Point(a.longitude + b.XMagnitude, a.latitude + b.YMagnitude);
        }
    }
}