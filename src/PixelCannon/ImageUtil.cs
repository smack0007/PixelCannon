using System.IO;
using ImageDotNet;

namespace PixelCannon
{
    internal static class ImageUtil
    {
        public static Image<Rgba32> LoadImage(Stream stream, string fileName = null)
        {
            IImage image;

            if (fileName != null)
            {
                image = Image.LoadByFileExtension(fileName, stream);
            }
            else
            {
                image = Image.Load(stream);
            }

            bool updateAlpha = image.BytesPerPixel != 4;

            var rgba32Image = image.To<Rgba32>();

            if (updateAlpha)
            {
                for (int i = 0; i < image.Length; i++)
                    rgba32Image[i].A = 255;
            }

            return rgba32Image;
        }
    }
}
