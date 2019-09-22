using System;
using System.IO;
using ImageDotNet;

namespace PixelCannon
{
    public sealed class Texture : Sampler
    {
        private readonly GraphicsContext _graphics;

        public override int Width { get; }

        public override int Height { get; }

        public Texture(GraphicsContext graphics, int width, int height, Pixel[] data = null)
            : base()
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

            using (var file = File.OpenRead(fileName))
                return LoadFromStream(graphics, file, fileName);
        }

        public static Texture LoadFromStream(GraphicsContext graphics, Stream stream, string fileName = null)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var image = ImageUtil.LoadImage(stream, fileName);

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

            EnsurePixelData(data);

            using (var dataPtr = DataPointer.Create(data))
            {
                _graphics.BackendSetTextureData(Handle, Width, Height, dataPtr.Pointer);
            }
        }
    }
}
