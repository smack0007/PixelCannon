using System;
using ImageDotNet;

namespace PixelCannon
{
    public sealed class Texture : DisposableObject
    {
        private readonly GraphicsContext _graphics;

        internal int Handle { get; }

        public int Width { get; }

        public int Height { get; }

        public Texture(GraphicsContext graphics, int width, int height, Pixel[] data = null)
        {
            _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            Width = width;
            Height = height;

            Handle = _graphics.BackendCreateTexture(width, height);

            if (data != null)
                SetData(data);
        }

        private Texture(GraphicsContext graphics, int handle, int width, int height)
        {
            _graphics = graphics;
            Handle = handle;
            Width = width;
            Height = height;
        }

        protected override void Dispose(bool disposing)
        {
            _graphics.BackendFreeTexture(Handle);
        }

        public static Texture LoadFromFile(GraphicsContext graphics, string fileName)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));

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

            var texture = graphics.BackendCreateTexture(image.Width, image.Height);

            using (var data = image.GetDataPointer())
            {
                graphics.BackendSetTextureData(texture, image.Width, image.Height, data.Pointer);
            }

            return new Texture(graphics, texture, image.Width, image.Height);
        }

        public void SetData(Pixel[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var length = Width * Height;
            if (data.Length != length)
                throw new ArgumentException(paramName: "data", message: $"Expected pixel array of length {length} but got {data.Length}.");

            using (var dataPtr = DataPointer.Create(data))
            {
                _graphics.BackendSetTextureData(Handle, Width, Height, dataPtr.Pointer);
            }
        }
    }
}
