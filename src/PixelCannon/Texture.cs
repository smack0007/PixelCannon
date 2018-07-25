namespace PixelCannon
{
    public struct Texture
    {
        public uint Handle { get; }

        public int Width { get; }

        public int Height { get; }

        internal Texture(uint handle, int width, int height)
        {
            this.Handle = handle;
            this.Width = width;
            this.Height = height;
        }
    }
}
