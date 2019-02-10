using System;
using System.Runtime.InteropServices;

namespace PixelCannon
{
    public struct DataPointer : IDisposable
    {
        private GCHandle _handle;

        public IntPtr Pointer { get; }

        public int Length { get; }

        private DataPointer(GCHandle handle, int length)
        {
            _handle = handle;
            Pointer = _handle.AddrOfPinnedObject();
            Length = length;
        }

        public void Dispose()
        {
            _handle.Free();
        }

        public static DataPointer Create<T>(T[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            return new DataPointer(handle, data.Length);
        }
    }
}
