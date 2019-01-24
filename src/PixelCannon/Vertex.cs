using System.Numerics;
using System.Runtime.InteropServices;

namespace PixelCannon
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public static int SizeInBytes => Marshal.SizeOf<Vertex>();

        public Vector3 Position;
        public Color Color;
        public Vector2 UV;
    }
}
