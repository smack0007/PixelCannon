using System;
using ImageDotNet;

namespace PixelCannon
{
    public sealed class Texture : DisposableObject
    {
        private readonly IBackend _backend;

        internal int Handle { get; }

        public int Width { get; }

        public int Height { get; }

        internal Texture(IBackend backend, int handle, int width, int height)
        {
            _backend = backend;
            Handle = handle;
            Width = width;
            Height = height;
        }

        protected override void Dispose(bool disposing)
        {
            _backend.FreeTexture(this);
        }

        public static Texture LoadFromFile(GraphicsContext context, string fileName)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            IImage image = null;

            if (fileName.EndsWith(".tga"))
            {
                image = Image.LoadTga(fileName);
            }
            else if (fileName.EndsWith(".png"))
            {
                image = Image.LoadPng(fileName);
            }

            if (image == null)
                throw new PixelCannonException("Unsupported image type.");

            bool updateAlpha = image.BytesPerPixel != 4;

            image = image.To<Rgba32>();

            if (updateAlpha)
            {
                for (int i = 0; i < image.Length; i++)
                    ((Image<Rgba32>)image)[i].A = 255;
            }

            using (var data = image.GetDataPointer())
            {
                return context.Backend.CreateTexture(image.Width, image.Height, data.Pointer);
            }
        }
    }
}
