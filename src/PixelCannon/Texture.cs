namespace PixelCannon
{
    public sealed class Texture : DisposableObject
    {
        internal int Handle { get; }

        public int Width { get; }

        public int Height { get; }

        internal Texture(int handle, int width, int height)
        {
            Handle = handle;
            Width = width;
            Height = height;
        }
    }
}
