namespace PixelCannon
{
    public sealed class Texture : DisposableObject
    {
        public uint Handle { get; }

        public int Width { get; }

        public int Height { get; }

        internal Texture(uint handle, int width, int height)
        {
            Handle = handle;
            Width = width;
            Height = height;
        }
    }
}
