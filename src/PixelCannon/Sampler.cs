using System;

namespace PixelCannon
{
    public abstract class Sampler : DisposableObject
    {
        protected internal int Handle { get; protected set; }

        public abstract int Width { get; }

        public abstract int Height { get; }

        internal Sampler()
        {
        }

        protected void EnsurePixelData(Pixel[] data)
        {
            var length = Width * Height;
            if (data.Length != length)
                throw new ArgumentException(paramName: "data", message: $"Expected pixel array of length {length} but got {data.Length}.");
        }
    }
}
