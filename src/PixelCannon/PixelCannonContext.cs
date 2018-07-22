using System;
using static GLDotNet.GL;

namespace PixelCannon
{
    public class PixelCannonContext
    {
        private Color clearColor = Color.Black;

        public PixelCannonContext(Func<string, IntPtr> getProcAddress)
        {
            glInit(getProcAddress, 4, 0);

            glClearColor(this.clearColor.R, this.clearColor.G, this.clearColor.B, this.clearColor.A);
        }

        public void Clear(Color color)
        {
            if (color != this.clearColor)
            {
                this.clearColor = color;
                glClearColor(this.clearColor.R, this.clearColor.G, this.clearColor.B, this.clearColor.A);
            }

            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        }
    }
}
