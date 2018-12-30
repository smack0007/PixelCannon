using System;

namespace PixelCannon
{
    internal class PixelCannonException : Exception
    {
        public PixelCannonException()
        {
        }

        public PixelCannonException(string message)
            : base(message)
        {
        }

        public PixelCannonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
