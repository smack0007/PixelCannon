using System;
using PixelCannon.Backends.GL;

namespace PixelCannon
{
    public abstract partial class GraphicsContext
    {
        public static GLGraphicsContext CreateGLContext(
            Func<string, IntPtr> getProcAddress,
            int maxVertices = DefaultMaxVertices)
        {
            return new GLGraphicsContext(getProcAddress, maxVertices);
        }
    }
}
