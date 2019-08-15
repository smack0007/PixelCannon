using System;
using System.Collections.Generic;
using System.Text;

namespace PixelCannon
{
    public class Surface
    {
        private readonly Pixel[] _pixels;

        public int Length => _pixels.Length;

        public int Width { get; }

        public int Height { get; }

        public ref Pixel this[int i] => ref _pixels[i];

        public ref Pixel this[int x, int y] => ref _pixels[y * Width + x];

        public Surface(int width, int height)
        {
            Width = width;
            Height = height;
            _pixels = new Pixel[width * height];
        }

        public Surface(int width, int height, Pixel[] pixels)
        {
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));

            var pixelsLength = width * height;
            if (pixels.Length != pixelsLength)
                throw new ArgumentException($"Expected length of pixels array to be {pixelsLength} but was {pixels.Length}.", nameof(pixels));

            Width = width;
            Height = height;
            _pixels = pixels;
        }

        //public static Surface FromFile(string fileName)
        //{
        //    using (var file = File.OpenRead(fileName))
        //        return FromStream(file);
        //}

        //public static Surface FromStream(Stream stream)
        //{
        //    return new Surface(width, height, pixels);
        //}

        

        public Pixel[] AsArray() => _pixels;

        public void Clear(Pixel pixel)
        {
            for (int i = 0; i < _pixels.Length; i++)
            {
                _pixels[i] = pixel;
            }
        }

        public void Blit(Surface surface, Point destination, Rectangle? source = null)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            if (source == null)
                source = new Rectangle(0, 0, surface.Width, surface.Height);

            for (int y = 0; y < source.Value.Height; y++)
            {
                for (int x = 0; x < source.Value.Width; x++)
                {
                    this[destination.X + x, destination.Y + y] = surface[source.Value.X + x, source.Value.Y + y];
                }
            }
        }
    }
}
