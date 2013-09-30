using Cab9.Controller.Common;
using Cab9.Model;
using Cab9.Model.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using Cab9.Stats;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using Cab9.Common;

namespace Cab9.Controller
{
    public class ImageController : Cab9ApiController
    {
        private static Font MarkerFont = new Font("Montserrat", 44f, FontStyle.Regular);
        private static Font RegistrationFont = new Font("Montserrat", 16f, FontStyle.Regular);
        private static Font ProfileFont = new Font("Montserrat", 84f, FontStyle.Regular);

        private static Color DefaultColor1 = Color.FromArgb(38, 174, 144);
        private static Color DefaultColor2 = Color.FromArgb(210, 29, 29);
        private static Color DefaultColor3 = Color.FromArgb(61, 61, 61);

        [HttpGet]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Get(string imageType, string ownerType, int ownerId)
        {
            Image result = null;
            Color? background = null;
            String text = null;
            String imageUrl = null;

            switch (ownerType.ToUpper())
            {
                case "DRIVER":
                    var driver = Driver.SelectByID(ownerId);
                    if (driver == null)
                        throw new ArgumentException("ownerId not a valid driver", "ownerId");
                    text = driver.Forename + " " + driver.Surname;
                    imageUrl = driver.ImageUrl;
                    break;
                case "CLIENT":
                    var client = Client.SelectByID(ownerId);
                    if (client == null)
                        throw new ArgumentException("ownerId not a valid company", "ownerId");
                    text = client.Name;
                    imageUrl = client.LogoURL;
                    break;

                case "VEHICLE":
                    var vehicle = Vehicle.SelectByID(ownerId);
                    if (vehicle == null)
                        throw new ArgumentException("ownerId not a valid company", "ownerId");
                    text = vehicle.Registration;
                    break;

                default:
                    throw new ArgumentException("OwnerType not recognised", "ownerType");
            }

            switch (imageType)
            {
                case "Raw":
                    if (imageUrl != null && File.Exists(HttpRuntime.AppDomainAppPath + imageUrl))
                    {
                        result = Image.FromFile(HttpRuntime.AppDomainAppPath + imageUrl);
                    }
                    break;
                case "Profile":
                    background = GetColor(text);
                    text = text.GetInitials().ToUpper();
                    result = GenerateProfileImage(imageUrl, text, background);
                    break;
                case "Google":
                    background = GetColor(text);
                    text = text.GetInitials().ToUpper();
                    result = GenerateGoogleMarker(imageUrl, text, background);
                    break;
                case "VehicleReg":
                    background = GetColor(text);
                    text = text.ToUpper();
                    result = GenerateVehicleRegImage(text, background);
                    break;
                default:
                    break;
            }

            MemoryStream ms = new MemoryStream();
            result.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(ms.ToArray());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            return response;
        }

        private Color GetColor(string text)
        {
            double sum = 1 + text.Sum(x => x);
            var col = (sum % 33);
            return new HSLColor(col * 7.5, 240 * 0.55, 240 * 0.55);
        }

        private Image GenerateProfileImage(string imageURL, string text, Color? backColor)
        {
            Bitmap img = new Bitmap(250, 250);
            Graphics drawing = Graphics.FromImage(img);
            drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Color InsideColor = (backColor.HasValue) ? backColor.Value : DefaultColor1;
            Color TextColor = Color.White;

            drawing.Clear(InsideColor);

            if (imageURL != null && File.Exists(HttpRuntime.AppDomainAppPath + imageURL))
            {
                Image picture = Image.FromFile(HttpRuntime.AppDomainAppPath + imageURL);
                drawing.DrawImage(picture, 0, 0, 250, 250);
            }
            else if (text != null)
            {
                SizeF textSize = drawing.MeasureString(text, ProfileFont);

                Brush TextBrush = new SolidBrush(TextColor);
                drawing.DrawString(text, ProfileFont, TextBrush, 125 - (textSize.Width / 2), 125 - (textSize.Height / 2));
            }

            drawing.Save();

            return img;
        }

        private Image GenerateVehicleRegImage(string text, Color? backColor)
        {
            Bitmap img = new Bitmap(150, 40);
            Graphics drawing = Graphics.FromImage(img);
            drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Color InsideColor = Color.FromArgb(234, 216, 10);
            Color TextColor = Color.FromArgb(61, 61, 61);

            drawing.Clear(InsideColor);

            if (text != null)
            {
                SizeF textSize = drawing.MeasureString(text, RegistrationFont);

                Brush TextBrush = new SolidBrush(TextColor);
                drawing.DrawString(text, RegistrationFont, TextBrush, 75 - (textSize.Width / 2), 20 - (textSize.Height / 2));
            }

            drawing.Save();

            return img;
        }

        private Image GenerateGoogleMarker(string imageURL, string text, Color? backColor)
        {
            Bitmap img = new Bitmap(150, 150);
            Graphics drawing = Graphics.FromImage(img);
            drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            drawing.Clear(Color.FromArgb(61, 61, 61));

            Color OutsideColor = Color.FromArgb(0, 255, 0); //Green Screen (so not to clash with image colours)
            Color InsideColor = (backColor.HasValue) ? backColor.Value : DefaultColor1;
            Color BorderColor = DefaultColor3;
            Color TextColor = Color.White;

            List<Point> OutsidePoints = new List<Point>();
            OutsidePoints.Add(new Point(75, 150));
            OutsidePoints.Add(new Point(75, 145));
            OutsidePoints.Add(new Point(25, 100));
            OutsidePoints.Add(new Point(25, 20));
            OutsidePoints.Add(new Point(125, 20));
            OutsidePoints.Add(new Point(125, 100));
            OutsidePoints.Add(new Point(75, 145));
            OutsidePoints.Add(new Point(75, 150));
            OutsidePoints.Add(new Point(150, 150));
            OutsidePoints.Add(new Point(150, -1));
            OutsidePoints.Add(new Point(-1, -1));
            OutsidePoints.Add(new Point(-1, 150));

            List<Point> InsidePoints = new List<Point>();
            InsidePoints.Add(new Point(75, 145));
            InsidePoints.Add(new Point(25, 100));
            InsidePoints.Add(new Point(25, 20));
            InsidePoints.Add(new Point(125, 20));
            InsidePoints.Add(new Point(125, 100));
            InsidePoints.Add(new Point(75, 145));

            if (imageURL != null && File.Exists(HttpRuntime.AppDomainAppPath + imageURL))
            {
                drawing.DrawImage(Image.FromFile(HttpRuntime.AppDomainAppPath + imageURL), 12f, 20, 126, 126);
            }
            else if (text != null)
            {
                Brush InsideBrush = new SolidBrush(InsideColor);
                drawing.FillPolygon(InsideBrush, InsidePoints.ToArray());

                SizeF textSize = drawing.MeasureString(text, MarkerFont);

                Brush TextBrush = new SolidBrush(TextColor);
                drawing.DrawString(text, MarkerFont, TextBrush, 75 - (textSize.Width / 2), 60 - (textSize.Height / 2));
            }

            Brush OutsideBrush = new SolidBrush(OutsideColor);
            drawing.FillPolygon(OutsideBrush, OutsidePoints.ToArray());

            drawing.Save();
            img.MakeTransparent(Color.FromArgb(0, 255, 0));

            drawing = Graphics.FromImage(img);
            drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Pen BorderPen = new Pen(BorderColor, 2f);
            drawing.DrawPolygon(BorderPen, InsidePoints.ToArray());
            drawing.Save();

            return img;
        }
    }
}