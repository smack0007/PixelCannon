namespace PixelCannon
{
    public abstract class Sampler : DisposableObject
    {
        public int Width { get; protected set; }

        public int Height { get; protected set; }

        internal Sampler()
        {
        }
    }
}
