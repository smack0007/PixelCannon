using System;

namespace PixelCannon
{
    public sealed class FrameBuffer : Sampler
    {
        private readonly GraphicsContext _graphics;

        private readonly int _width;
        private readonly int _height;

        public bool IsBackBuffer { get; }

        public override int Width => IsBackBuffer ? GetBackBufferWidth() : _width;

        public override int Height => IsBackBuffer ? GetBackBufferHeight() : _height;

        private FrameBuffer()
        {
            IsBackBuffer = true;
        }

        public FrameBuffer(GraphicsContext graphics, int width, int height)
            : base()
        {
            _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            _width = width;
            _height = height;

            Handle = _graphics.BackendCreateFrameBuffer(false, width, height);
        }

        internal static FrameBuffer CreateBackBuffer()
        {
            return new FrameBuffer();
        }

        private int GetBackBufferWidth()
        {
            return 1024;
        }

        private int GetBackBufferHeight()
        {
            return 768;
        }

        protected override void Dispose(bool disposing)
        {
            _graphics.BackendFreeFrameBuffer(Handle);
        }

        public void SetData(Pixel[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            EnsurePixelData(data);

            using (var dataPtr = DataPointer.Create(data))
            {
                _graphics.BackendSetFrameBufferData(Handle, Width, Height, dataPtr.Pointer);
            }
        }
    }
}
