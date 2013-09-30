using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace Cab9.App_Start
{
    internal static class MediaFormattersConfig
    {
        internal static void Register(MediaTypeFormatterCollection formatters)
        {
            formatters.Clear();
            formatters.Add(new JsonMediaTypeFormatter());
        }
    }
}