using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Cab9.Geography
{
    public class Polyline
    {
        public List<Point> Points { get; set; }
        public List<Line> Lines 
        {
            get
            {
                return GetLines();
            }
        }
        public string EncodedPolyline
        {
            get
            {
                return EncodePoints(Points);
            }
        }
        public Point Start
        {
            get
            {
                return Points.First();
            }
        }
        public Point End
        {
            get
            {
                return Points.Last();
            }
        }

        #region Constructors

        public Polyline(List<Point> points)
        {
            Points = points;
        }

        public Polyline(string encodedPoints)
        {
            Points = DecodePoints(encodedPoints);
        }

        public Polyline(Point start, Point end)
        {
            Points = new List<Point>
            {
                start,
                end
            };
        }

        public Polyline(decimal startLat, decimal startLong, decimal endLat, decimal endLong) : this(new Point(startLat, startLong), new Point(endLat, endLong))
        {
        }

        #endregion

        private List<Point> DecodePoints(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "") return new List<Point>();
            List<Point> poly = new List<Point>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            try
            {
                while (index < polylinechars.Length)
                {
                    // calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    Point p = new Point(Convert.ToDecimal(currentLat) / 100000.0M, Convert.ToDecimal(currentLng) / 100000.0M);
                    poly.Add(p);
                }
            }
            catch (Exception ex)
            {
            }
            return poly;
        }

        private string EncodePoints(List<Point> points)
        {
            var str = new StringBuilder();
            var encodeDiff = (Action<int>)(diff =>
            {
                int shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;
                int rem = shifted;
                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));
                    rem >>= 5;
                }
                str.Append((char)(rem + 63));
            });

            int lastLat = 0;
            int lastLng = 0;
            foreach (var point in points)
            {
                int lat = (int)Math.Round(point.latitude * 100000);
                int lng = (int)Math.Round(point.longitude * 100000);
                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);
                lastLat = lat;
                lastLng = lng;
            }
            return str.ToString();
        }

        private List<Line> GetLines()
        {
            var result = new List<Line>();
            Point prevPoint = null;
            foreach(Point point in Points)
            {
                if (!ReferenceEquals(prevPoint, null))
                {
                    result.Add(new Line(prevPoint, point));
                }
                prevPoint = point;
            }
            return result;
        }

        public bool EntersPolygon(Polygon toTest)
        {
            if (toTest.ContainsPoint(Start) || toTest.ContainsPoint(End)) return true;
            List<Line> lines = toTest.GetLines();
            foreach (Line aLine in lines)
            {
                Point intersect;
                if (Lines.Any(x => IntersectionExists(x, aLine, out intersect) > 0)) return true;
            }
            return false;
        }


        #region Refactor

        public Vector GetOverallVector()
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

        public static bool PointInPolyline(Point point, Polyline line)
        {
            return false;
        }

        #endregion
    }
}
