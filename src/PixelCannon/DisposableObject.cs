using System;

namespace PixelCannon
{
    public class DisposableObject : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        ~DisposableObject()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            this.IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
