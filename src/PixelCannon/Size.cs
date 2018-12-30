using System;
using System.Runtime.InteropServices;

namespace PixelCannon
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Size : IEquatable<Size>
    {
        public static int SizeInBytes => Marshal.SizeOf(typeof(Size));

        public static Size Zero => new Size();

        public int Width;

        public int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Size))
                return false;

            return Equals((Size)obj);
        }

        public bool Equals(Size other)
        {
            return Width == other.Width &&
                   Height == other.Height;
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        public static bool operator ==(Size s1, Size s2)
        {
            return s1.Equals(s2);
        }

        public static bool operator !=(Size s1, Size s2)
        {
            return !s1.Equals(s2);
        }
    }
}
