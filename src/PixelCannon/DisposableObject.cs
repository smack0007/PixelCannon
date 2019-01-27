using System;

namespace PixelCannon
{
    public class DisposableObject : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        ~DisposableObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
