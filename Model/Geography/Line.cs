using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.Geography
{
    public class Line
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public Line(decimal startLat, decimal startLong, decimal endLat, decimal endLong)
            : this(new Point(startLat, startLong), new Point(endLat, endLong))
        {
        }

        public bool EntersPolygon(Polygon toTest)
        {
            if (toTest.ContainsPoint(Start) || toTest.ContainsPoint(End)) return true;
            List<Line> lines = toTest.GetLines();
            Point intersect;
            return lines.Any(x => IntersectionExists(this, x, out intersect) > 0);
        }

        public Vector GetVector()
        {
            return new Vector(Start, End);
        }


        public static int IntersectionExists(Line line1, Line line2, out Point intersect)
        {
            decimal ua = (line2.End.longitude - line2.Start.longitude) * (line1.Start.latitude - line2.Start.latitude) - (line2.End.latitude - line2.Start.latitude) * (line1.Start.longitude - line2.Start.longitude);
            decimal ub = (line1.End.longitude - line1.Start.longitude) * (line1.Start.latitude - line2.Start.latitude) - (line1.End.latitude - line1.Start.latitude) * (line1.Start.longitude - line2.Start.longitude);
            decimal denominator = (line2.End.latitude - line2.Start.latitude) * (line1.End.longitude - line1.Start.longitude) - (line2.End.longitude - line2.Start.longitude) * (line1.End.latitude - line1.Start.latitude);
            intersect = null;

            if (Math.Abs(denominator) <= 0.00001M)
            {
                if (Math.Abs(ua) <= 0.00001M && Math.Abs(ub) <= 0.00001M)
                {
                    return 1;
                    //intersectionPoint = (line1.Start + line1.End) / 2;
                }
            }
            else
            {
                ua /= denominator;
                ub /= denominator;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                {
                    intersect = new Point(line1.Start.latitude + ua * (line1.End.latitude - line1.Start.latitude), line1.Start.longitude + ua * (line1.End.longitude - line1.Start.longitude));
                    return 2;
                }
            }
            return 0;
        }

        public static bool PointInPolyline(Point point, Line line)
        {
            return false;
        }

        //public static int InterceptsLine(Polyline line1, Polyline line2, out Point? intersectionBegin, out Point? intersectionEnd)
        //{
        //    Vector v1 = line1.GetVector();
        //    Vector v2 = line2.GetVector();
        //    Vector v3 = new Vector(line1.Start, line2.Start);

        //    decimal perp = Vector.Perp(v1, v2);

        //    intersectionBegin = null;
        //    intersectionEnd = null;

        //    //Test if parallel
        //    if (Math.Abs(perp) < 0.00001F)
        //    {
        //        //Check if not collinear i.e. overlap
        //        if (Vector.Perp(v1, v3) != 0 || Vector.Perp(v2, v3) != 0)
        //        {
        //            return 0;
        //        }
        //    }

        //    // they are collinear or degenerate, check if they are degenerate  points
        //    decimal dot1 = Vector.DotProduct(v1, v1);
        //    decimal dot2 = Vector.DotProduct(v2, v2);

        //    //Check if both lines are infact points
        //    if (dot1 == 0 && dot2 == 0)
        //    {
        //        //Check if points are different
        //        if (line1.Start != line2.Start)
        //        {
        //            return 0;
        //        }
        //        intersectionBegin = line1.Start;
        //        return 1;
        //    }

        //    //if line1 is a single point
        //    if (dot1 == 0)
        //    {
        //        if (PointInPolyline(line1.Start, line2))
        //        {
        //            intersectionBegin = line1.Start; //line1 point is in line2
        //            return 1;
        //        }
        //        return 0;
        //    }

        //    //if line2 is a single point
        //    if (dot2 == 0)
        //    {
        //        if (PointInPolyline(line2.Start, line1))
        //        {
        //            intersectionBegin = line2.Start; //line1 point is in line2
        //            return 1;
        //        }
        //        return 0;
        //    }

        //    //they are colinear segments for which we can now check for an overlap
        //    decimal t0, t1;
        //    Vector v4 = new Vector(line1.End, line2.Start);

        //    if (v2.XMagnitude != 0)
        //    {
        //        t0 = v3.XMagnitude / v2.XMagnitude;
        //        t1 = v4.XMagnitude / v2.XMagnitude;
        //    }
        //    else
        //    {
        //        t0 = v3.YMagnitude / v2.YMagnitude;
        //        t1 = v4.YMagnitude / v2.YMagnitude;
        //    }

        //    //t0 needs to be smallest
        //    if (t0 > t1)
        //    {
        //        t0 = t0 + t1;
        //        t1 = t0 - t1;
        //        t0 = t0 - t1;
        //    }

        //    if (t0 > 1 | t1 < 0)
        //    {
        //        return 0; // no overlap
        //    }

        //    //clip to between 0 and 1
        //    t0 = (t0 < 0) ? 0 : t0;
        //    t1 = (t1 > 1) ? 1 : t1;

        //    if (t0 == t1)
        //    {
        //        intersectionBegin = line2.Start + (t0 * v2);
        //        return 1;
        //    }

        //    intersectionBegin = line2.Start + (t0 * v2);
        //    intersectionEnd = line2.Start + (t1 * v2);
        //    return 2;
        //}


    }
}
