using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;

namespace Cab9.Common
{
    public static class ImageExtensions
    {
        public static Image Scale(this Image img, int maxWidth, int maxHeight)
        {
            double scale = 1;

            if (img.Width > maxWidth || img.Height > maxHeight)
            {
                double scaleW, scaleH;

                scaleW = maxWidth / (double)img.Width;
                scaleH = maxHeight / (double)img.Height;

                scale = scaleW < scaleH ? scaleW : scaleH;
            }

            return img.Resize((int)(img.Width * scale), (int)(img.Height * scale));
        }

        public static Image Resize(this Image img, int width, int height)
        {
            Bitmap newImage = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(img, new Rectangle(0, 0, width, height));
                gr.Save();
            }
            return newImage;
        }

        public static Image TrimCenter(this Image img, int width, int height)
        {
            int left, top;
            left = -((img.Width - width) / 2);
            top = -((img.Height - height) / 2);

            Bitmap newImage = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(img, new Rectangle(left, top, img.Width, img.Height));
                gr.Save();
            }
            return newImage;
        }

        public static Image TakeSquare(this Image img, int width, int height)
        {
            Image result;
            if (img.Width >= img.Height)
            {
                result = img.TrimCenter(img.Height, img.Height);
            }
            else
            {
                result = img.TrimCenter(img.Width, img.Width);
            }

            return result.Resize(width, height);
        }
    }
}
