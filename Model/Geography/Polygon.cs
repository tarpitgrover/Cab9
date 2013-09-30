using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Cab9.Geography
{
    public class Polygon
    {
        public List<Point> Points;

        public Polygon()
        {
            Points = new List<Point>();
        }

        public Polygon(List<Point> points)
        {
            Points = points;
        }

        public Polygon(string encoded)
        {
            Points = DecodePoints(encoded);
        }

        public List<Line> GetLines()
        {
            var result = new List<Line>();
            for (int i = 0; i < Points.Count; i++)
            {
                if (i + 1 == Points.Count)
                {
                    // We've reached the end, go back to the start
                    result.Add(new Line(Points[i], Points[0]));
                }
                else
                {
                    result.Add(new Line(Points[i], Points[i + 1]));
                }
            }
            return result;
        }

        public void AddPoint(Point point)
        {
            Points.Add(point);
        }

        public void AddPoint(decimal latitude, decimal longitude)
        {
            AddPoint(new Point(latitude, longitude));
        }

        public decimal Area(bool kilometers = false)
        {
            var lines = GetLines();
            decimal result = 0;
            foreach (var line in lines)
            {
                if (line.End.longitude > line.Start.longitude)
                    result += ((line.Start.latitude + line.End.latitude) / 2) * (line.End.longitude - line.Start.longitude);
                if (line.End.longitude < line.Start.longitude)
                    result -= ((line.Start.latitude + line.End.latitude) / 2) * (line.End.longitude - line.Start.longitude);
            }

            if (kilometers)
                return result * 1.852M;
            else
                return result * 1.15M;
        }

        public bool ContainsPoint(Point point)
        {
            bool result = false;

            for (int i = 0, j = (Points.Count - 1); i < Points.Count; j = i++)
            {
                if (
                        (
                            (Points.ElementAt(i).latitude > point.latitude) != (Points.ElementAt(j).latitude > point.latitude)
                        ) 
                   && 
                        (
                            point.longitude < ( 
                                                Points.ElementAt(j).longitude - Points.ElementAt(i).longitude
                                              ) 
                            * (point.latitude - Points.ElementAt(i).latitude)
                            / (Points.ElementAt(j).latitude - Points.ElementAt(i).latitude)
                            + Points.ElementAt(i).longitude
                        )
                   )
                {
                    result = !result;
                }
            }

            return result;
        }

        public bool ContainsPoint(decimal latitude, decimal longitude)
        {
            return ContainsPoint(new Point(latitude, longitude));
        }

        private List<Point> DecodePoints(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "") return null;
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

        public string ToEncodedString()
        {
            return EncodePoints(Points);
        }
    }
}