using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Cab9.Common
{
    public static class StringExtensions
    {
        public static string GetInitials(this string input, int number = 2)
        {
            var array = input.Split(' ');

            if (array.Length == 0)
                return "";

            if (array.Length == 1)
                return array.First()[0].ToString();

            if (number == 2)
                return array.First()[0].ToString() + array.Last()[0].ToString();
            else
                return new String(array.Select(x => x[0]).ToArray());
        }

        public static decimal MatchPercentage(this string input, string other)
        {
            var inputPairs = input.ToUpper().StringPairs();
            var otherPairs = other.ToUpper().StringPairs();
            int count = 0, total = inputPairs.Count() + otherPairs.Count();

            for (var o = 0; o < inputPairs.Count; o++)
            {
                for (var i = 0; i < otherPairs.Count; i++)
                {
                    if (inputPairs[o] == otherPairs[i])
                    {
                        count++;
                        otherPairs.RemoveAt(i);
                        break;
                    }
                }
            }
            return (2.0M * count) / total;
        }

        public static List<string> StringPairs(this string input)
        {
            List<string> result = new List<string>();
            var array = input.ToCharArray();

            for (var o = 0; o < (array.Length - 1); o++)
            {
                result.Add(array[o].ToString() + array[o + 1].ToString());
            }
            return result;
        }

        public static bool IsShortPostcode(this string input)
        {
            Regex test = new Regex("[A-Z][A-Z][0-9][0-9]?");
            if (test.Match(input.ToUpper()).Success) return true;
            else return false;
        }
    }
}