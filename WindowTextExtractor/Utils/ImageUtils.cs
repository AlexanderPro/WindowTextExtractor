using System.Drawing;
using System.Drawing.Drawing2D;

namespace WindowTextExtractor.Utils
{
    static class ImageUtils
    {
        public static Bitmap Reduce(Bitmap source, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(source, new Rectangle(0, 0, width, height), new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }
            return bitmap;
        }
    }
}
