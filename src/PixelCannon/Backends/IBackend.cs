using System;

namespace PixelCannon
{
    public interface IBackend : IDisposable
    {
        void Initialize(int maxVertices);

        Texture CreateTexture(int width, int height, IntPtr? data);

        void FreeTexture(Texture texture);

        void SetTextureData(Texture texture, IntPtr data);

        void Clear(Color color);

        void Draw(Vertex[] vertices, int vertexCount, int texture);
    }
}
