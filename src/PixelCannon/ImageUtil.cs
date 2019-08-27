using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageDotNet;

namespace PixelCannon
{
    internal static class ImageUtil
    {
        public static Image<Rgba32> LoadImage(Stream stream, string fileName = null)
        {
            IImage image = null;

            if (fileName != null)
            {
                if (fileName.EndsWith(".tga"))
                {
                    image = Image.LoadTga(stream);
                }
                else if (fileName.EndsWith(".png"))
                {
                    image = Image.LoadPng(stream);
                }
            }
            else
            {
                image = Image.LoadPng(stream);
            }

            if (image == null)
                throw new PixelCannonException("Unsupported image type.");

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
