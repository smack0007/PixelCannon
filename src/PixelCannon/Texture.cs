namespace PixelCannon
{
    public struct Texture
    {
        internal uint Handle { get; }

        internal int Width { get; }

        internal int Height { get; }

        internal Texture(uint handle, int width, int height)
        {
            this.Handle = handle;
            this.Width = width;
            this.Height = height;
        }
    }
}
