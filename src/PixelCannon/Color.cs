using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace PixelCannon
{
    /// <summary>
    /// Represents a RGBA color.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Color : IEquatable<Color>
    {
        #region Static colors

        public static readonly Color AliceBlue = FromBytes(0xF0, 0xF8, 0xFF, 0xFF);
        public static readonly Color AntiqueWhite = FromBytes(0xFA, 0xEB, 0xD7, 0xFF);
        public static readonly Color Aqua = FromBytes(0x00, 0xFF, 0xFF, 0xFF);
        public static readonly Color Aquamarine = FromBytes(0x7F, 0xFF, 0xD4, 0xFF);
        public static readonly Color Azure = FromBytes(0xF0, 0xFF, 0xFF, 0xFF);
        public static readonly Color Beige = FromBytes(0xF5, 0xF5, 0xDC, 0xFF);
        public static readonly Color Bisque = FromBytes(0xFF, 0xE4, 0xC4, 0xFF);
        public static readonly Color Black = FromBytes(0x00, 0x00, 0x00, 0xFF);
        public static readonly Color BlanchedAlmond = FromBytes(0xFF, 0xEB, 0xCD, 0xFF);
        public static readonly Color Blue = FromBytes(0x00, 0x00, 0xFF, 0xFF);
        public static readonly Color BlueViolet = FromBytes(0x8A, 0x2B, 0xE2, 0xFF);
        public static readonly Color Brown = FromBytes(0xA5, 0x2A, 0x2A, 0xFF);
        public static readonly Color BurlyWood = FromBytes(0xDE, 0xB8, 0x87, 0xFF);
        public static readonly Color CadetBlue = FromBytes(0x5F, 0x9E, 0xA0, 0xFF);
        public static readonly Color Chartreuse = FromBytes(0x7F, 0xFF, 0x00, 0xFF);
        public static readonly Color Chocolate = FromBytes(0xD2, 0x69, 0x1E, 0xFF);
        public static readonly Color Coral = FromBytes(0xFF, 0x7F, 0x50, 0xFF);
        public static readonly Color CornflowerBlue = FromBytes(0x64, 0x95, 0xED, 0xFF);
        public static readonly Color Cornsilk = FromBytes(0xFF, 0xF8, 0xDC, 0xFF);
        public static readonly Color Crimson = FromBytes(0xDC, 0x14, 0x3C, 0xFF);
        public static readonly Color Cyan = FromBytes(0x00, 0xFF, 0xFF, 0xFF);
        public static readonly Color DarkBlue = FromBytes(0x00, 0x00, 0x8B, 0xFF);
        public static readonly Color DarkCyan = FromBytes(0x00, 0x8B, 0x8B, 0xFF);
        public static readonly Color DarkGoldenRod = FromBytes(0xB8, 0x86, 0x0B, 0xFF);
        public static readonly Color DarkGray = FromBytes(0xA9, 0xA9, 0xA9, 0xFF);
        public static readonly Color DarkGrey = FromBytes(0xA9, 0xA9, 0xA9, 0xFF);
        public static readonly Color DarkGreen = FromBytes(0x00, 0x64, 0x00, 0xFF);
        public static readonly Color DarkKhaki = FromBytes(0xBD, 0xB7, 0x6B, 0xFF);
        public static readonly Color DarkMagenta = FromBytes(0x8B, 0x00, 0x8B, 0xFF);
        public static readonly Color DarkOliveGreen = FromBytes(0x55, 0x6B, 0x2F, 0xFF);
        public static readonly Color Darkorange = FromBytes(0xFF, 0x8C, 0x00, 0xFF);
        public static readonly Color DarkOrchid = FromBytes(0x99, 0x32, 0xCC, 0xFF);
        public static readonly Color DarkRed = FromBytes(0x8B, 0x00, 0x00, 0xFF);
        public static readonly Color DarkSalmon = FromBytes(0xE9, 0x96, 0x7A, 0xFF);
        public static readonly Color DarkSeaGreen = FromBytes(0x8F, 0xBC, 0x8F, 0xFF);
        public static readonly Color DarkSlateBlue = FromBytes(0x48, 0x3D, 0x8B, 0xFF);
        public static readonly Color DarkSlateGray = FromBytes(0x2F, 0x4F, 0x4F, 0xFF);
        public static readonly Color DarkSlateGrey = FromBytes(0x2F, 0x4F, 0x4F, 0xFF);
        public static readonly Color DarkTurquoise = FromBytes(0x00, 0xCE, 0xD1, 0xFF);
        public static readonly Color DarkViolet = FromBytes(0x94, 0x00, 0xD3, 0xFF);
        public static readonly Color DeepPink = FromBytes(0xFF, 0x14, 0x93, 0xFF);
        public static readonly Color DeepSkyBlue = FromBytes(0x00, 0xBF, 0xFF, 0xFF);
        public static readonly Color DimGray = FromBytes(0x69, 0x69, 0x69, 0xFF);
        public static readonly Color DimGrey = FromBytes(0x69, 0x69, 0x69, 0xFF);
        public static readonly Color DodgerBlue = FromBytes(0x1E, 0x90, 0xFF, 0xFF);
        public static readonly Color FireBrick = FromBytes(0xB2, 0x22, 0x22, 0xFF);
        public static readonly Color FloralWhite = FromBytes(0xFF, 0xFA, 0xF0, 0xFF);
        public static readonly Color ForestGreen = FromBytes(0x22, 0x8B, 0x22, 0xFF);
        public static readonly Color Fuchsia = FromBytes(0xFF, 0x00, 0xFF, 0xFF);
        public static readonly Color Gainsboro = FromBytes(0xDC, 0xDC, 0xDC, 0xFF);
        public static readonly Color GhostWhite = FromBytes(0xF8, 0xF8, 0xFF, 0xFF);
        public static readonly Color Gold = FromBytes(0xFF, 0xD7, 0x00, 0xFF);
        public static readonly Color GoldenRod = FromBytes(0xDA, 0xA5, 0x20, 0xFF);
        public static readonly Color Gray = FromBytes(0x80, 0x80, 0x80, 0xFF);
        public static readonly Color Grey = FromBytes(0x80, 0x80, 0x80, 0xFF);
        public static readonly Color Green = FromBytes(0x00, 0x80, 0x00, 0xFF);
        public static readonly Color GreenYellow = FromBytes(0xAD, 0xFF, 0x2F, 0xFF);
        public static readonly Color HoneyDew = FromBytes(0xF0, 0xFF, 0xF0, 0xFF);
        public static readonly Color HotPink = FromBytes(0xFF, 0x69, 0xB4, 0xFF);
        public static readonly Color IndianRed = FromBytes(0xCD, 0x5C, 0x5C, 0xFF);
        public static readonly Color Indigo = FromBytes(0x4B, 0x00, 0x82, 0xFF);
        public static readonly Color Ivory = FromBytes(0xFF, 0xFF, 0xF0, 0xFF);
        public static readonly Color Khaki = FromBytes(0xF0, 0xE6, 0x8C, 0xFF);
        public static readonly Color Lavender = FromBytes(0xE6, 0xE6, 0xFA, 0xFF);
        public static readonly Color LavenderBlush = FromBytes(0xFF, 0xF0, 0xF5, 0xFF);
        public static readonly Color LawnGreen = FromBytes(0x7C, 0xFC, 0x00, 0xFF);
        public static readonly Color LemonChiffon = FromBytes(0xFF, 0xFA, 0xCD, 0xFF);
        public static readonly Color LightBlue = FromBytes(0xAD, 0xD8, 0xE6, 0xFF);
        public static readonly Color LightCoral = FromBytes(0xF0, 0x80, 0x80, 0xFF);
        public static readonly Color LightCyan = FromBytes(0xE0, 0xFF, 0xFF, 0xFF);
        public static readonly Color LightGoldenRodYellow = FromBytes(0xFA, 0xFA, 0xD2, 0xFF);
        public static readonly Color LightGray = FromBytes(0xD3, 0xD3, 0xD3, 0xFF);
        public static readonly Color LightGrey = FromBytes(0xD3, 0xD3, 0xD3, 0xFF);
        public static readonly Color LightGreen = FromBytes(0x90, 0xEE, 0x90, 0xFF);
        public static readonly Color LightPink = FromBytes(0xFF, 0xB6, 0xC1, 0xFF);
        public static readonly Color LightSalmon = FromBytes(0xFF, 0xA0, 0x7A, 0xFF);
        public static readonly Color LightSeaGreen = FromBytes(0x20, 0xB2, 0xAA, 0xFF);
        public static readonly Color LightSkyBlue = FromBytes(0x87, 0xCE, 0xFA, 0xFF);
        public static readonly Color LightSlateGray = FromBytes(0x77, 0x88, 0x99, 0xFF);
        public static readonly Color LightSlateGrey = FromBytes(0x77, 0x88, 0x99, 0xFF);
        public static readonly Color LightSteelBlue = FromBytes(0xB0, 0xC4, 0xDE, 0xFF);
        public static readonly Color LightYellow = FromBytes(0xFF, 0xFF, 0xE0, 0xFF);
        public static readonly Color Lime = FromBytes(0x00, 0xFF, 0x00, 0xFF);
        public static readonly Color LimeGreen = FromBytes(0x32, 0xCD, 0x32, 0xFF);
        public static readonly Color Linen = FromBytes(0xFA, 0xF0, 0xE6, 0xFF);
        public static readonly Color Magenta = FromBytes(0xFF, 0x00, 0xFF, 0xFF);
        public static readonly Color Maroon = FromBytes(0x80, 0x00, 0x00, 0xFF);
        public static readonly Color MediumAquaMarine = FromBytes(0x66, 0xCD, 0xAA, 0xFF);
        public static readonly Color MediumBlue = FromBytes(0x00, 0x00, 0xCD, 0xFF);
        public static readonly Color MediumOrchid = FromBytes(0xBA, 0x55, 0xD3, 0xFF);
        public static readonly Color MediumPurple = FromBytes(0x93, 0x70, 0xD8, 0xFF);
        public static readonly Color MediumSeaGreen = FromBytes(0x3C, 0xB3, 0x71, 0xFF);
        public static readonly Color MediumSlateBlue = FromBytes(0x7B, 0x68, 0xEE, 0xFF);
        public static readonly Color MediumSpringGreen = FromBytes(0x00, 0xFA, 0x9A, 0xFF);
        public static readonly Color MediumTurquoise = FromBytes(0x48, 0xD1, 0xCC, 0xFF);
        public static readonly Color MediumVioletRed = FromBytes(0xC7, 0x15, 0x85, 0xFF);
        public static readonly Color MidnightBlue = FromBytes(0x19, 0x19, 0x70, 0xFF);
        public static readonly Color MintCream = FromBytes(0xF5, 0xFF, 0xFA, 0xFF);
        public static readonly Color MistyRose = FromBytes(0xFF, 0xE4, 0xE1, 0xFF);
        public static readonly Color Moccasin = FromBytes(0xFF, 0xE4, 0xB5, 0xFF);
        public static readonly Color NavajoWhite = FromBytes(0xFF, 0xDE, 0xAD, 0xFF);
        public static readonly Color Navy = FromBytes(0x00, 0x00, 0x80, 0xFF);
        public static readonly Color OldLace = FromBytes(0xFD, 0xF5, 0xE6, 0xFF);
        public static readonly Color Olive = FromBytes(0x80, 0x80, 0x00, 0xFF);
        public static readonly Color OliveDrab = FromBytes(0x6B, 0x8E, 0x23, 0xFF);
        public static readonly Color Orange = FromBytes(0xFF, 0xA5, 0x00, 0xFF);
        public static readonly Color OrangeRed = FromBytes(0xFF, 0x45, 0x00, 0xFF);
        public static readonly Color Orchid = FromBytes(0xDA, 0x70, 0xD6, 0xFF);
        public static readonly Color PaleGoldenRod = FromBytes(0xEE, 0xE8, 0xAA, 0xFF);
        public static readonly Color PaleGreen = FromBytes(0x98, 0xFB, 0x98, 0xFF);
        public static readonly Color PaleTurquoise = FromBytes(0xAF, 0xEE, 0xEE, 0xFF);
        public static readonly Color PaleVioletRed = FromBytes(0xD8, 0x70, 0x93, 0xFF);
        public static readonly Color PapayaWhip = FromBytes(0xFF, 0xEF, 0xD5, 0xFF);
        public static readonly Color PeachPuff = FromBytes(0xFF, 0xDA, 0xB9, 0xFF);
        public static readonly Color Peru = FromBytes(0xCD, 0x85, 0x3F, 0xFF);
        public static readonly Color Pink = FromBytes(0xFF, 0xC0, 0xCB, 0xFF);
        public static readonly Color Plum = FromBytes(0xDD, 0xA0, 0xDD, 0xFF);
        public static readonly Color PowderBlue = FromBytes(0xB0, 0xE0, 0xE6, 0xFF);
        public static readonly Color Purple = FromBytes(0x80, 0x00, 0x80, 0xFF);
        public static readonly Color Red = FromBytes(0xFF, 0x00, 0x00, 0xFF);
        public static readonly Color RosyBrown = FromBytes(0xBC, 0x8F, 0x8F, 0xFF);
        public static readonly Color RoyalBlue = FromBytes(0x41, 0x69, 0xE1, 0xFF);
        public static readonly Color SaddleBrown = FromBytes(0x8B, 0x45, 0x13, 0xFF);
        public static readonly Color Salmon = FromBytes(0xFA, 0x80, 0x72, 0xFF);
        public static readonly Color SandyBrown = FromBytes(0xF4, 0xA4, 0x60, 0xFF);
        public static readonly Color SeaGreen = FromBytes(0x2E, 0x8B, 0x57, 0xFF);
        public static readonly Color SeaShell = FromBytes(0xFF, 0xF5, 0xEE, 0xFF);
        public static readonly Color Sienna = FromBytes(0xA0, 0x52, 0x2D, 0xFF);
        public static readonly Color Silver = FromBytes(0xC0, 0xC0, 0xC0, 0xFF);
        public static readonly Color SkyBlue = FromBytes(0x87, 0xCE, 0xEB, 0xFF);
        public static readonly Color SlateBlue = FromBytes(0x6A, 0x5A, 0xCD, 0xFF);
        public static readonly Color SlateGray = FromBytes(0x70, 0x80, 0x90, 0xFF);
        public static readonly Color SlateGrey = FromBytes(0x70, 0x80, 0x90, 0xFF);
        public static readonly Color Snow = FromBytes(0xFF, 0xFA, 0xFA, 0xFF);
        public static readonly Color SpringGreen = FromBytes(0x00, 0xFF, 0x7F, 0xFF);
        public static readonly Color SteelBlue = FromBytes(0x46, 0x82, 0xB4, 0xFF);
        public static readonly Color Tan = FromBytes(0xD2, 0xB4, 0x8C, 0xFF);
        public static readonly Color Teal = FromBytes(0x00, 0x80, 0x80, 0xFF);
        public static readonly Color Thistle = FromBytes(0xD8, 0xBF, 0xD8, 0xFF);
        public static readonly Color Tomato = FromBytes(0xFF, 0x63, 0x47, 0xFF);
        public static readonly Color Transparent = FromBytes(0x00, 0x00, 0x00, 0x00);
        public static readonly Color Turquoise = FromBytes(0x40, 0xE0, 0xD0, 0xFF);
        public static readonly Color Violet = FromBytes(0xEE, 0x82, 0xEE, 0xFF);
        public static readonly Color Wheat = FromBytes(0xF5, 0xDE, 0xB3, 0xFF);
        public static readonly Color White = FromBytes(0xFF, 0xFF, 0xFF, 0xFF);
        public static readonly Color WhiteSmoke = FromBytes(0xF5, 0xF5, 0xF5, 0xFF);
        public static readonly Color Yellow = FromBytes(0xFF, 0xFF, 0x00, 0xFF);
        public static readonly Color YellowGreen = FromBytes(0x9A, 0xCD, 0x32, 0xFF);

        #endregion

        private float r;
        private float g;
        private float b;
        private float a;

        /// <summary>
        /// The red value.
        /// </summary>
        public float R
        {
            get { return this.r; }
            set { this.r = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }

        /// <summary>
        /// The green value.
        /// </summary>
        public float G
        {
            get { return this.g; }
            set { this.g = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }

        /// <summary>
        /// The blue value.
        /// </summary>
        public float B
        {
            get { return this.b; }
            set { this.b = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }

        /// <summary>
        /// The alpha value.
        /// </summary>
        public float A
        {
            get { return this.a; }
            set { this.a = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public Color(float r, float g, float b, float a = 1.0f)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static Color FromBytes(byte r, byte g, byte b, byte a = 255)
        {
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }

        /// <summary>
        /// Returns a new color with the same values as this one limited by the given color values.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public Color Limit(float r, float g, float b, float a)
        {
            return new Color(Math.Min(this.R, r),
                             Math.Min(this.G, g),
                             Math.Min(this.B, b),
                             Math.Min(this.A, a));
        }

        /// <summary>
        /// Returns a new color with the values limited by the given color's values.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public Color Limit(Color color)
        {
            return new Color(
                Math.Min(this.R, color.R),
                Math.Min(this.G, color.G),
                Math.Min(this.B, color.B),
                Math.Min(this.A, color.A));
        }

        public Vector4 ToVector4()
        {
            return new Vector4(this.R, this.G, this.B, this.A);
        }

        public override string ToString()
        {
            return "{ " + this.R + ", " + this.G + ", " + this.B + ", " + this.A + " }";
        }

        /// <summary>
        /// Converts the color to a hex string.
        /// </summary>
        /// <returns></returns>
        public string ToHexString()
        {
            char[] chars = new char[8];

            int bits = (int)(this.R * 255.0f);
            chars[0] = HexHelper.HexDigits[bits >> 4];
            chars[1] = HexHelper.HexDigits[bits & 0xF];

            bits = (int)(this.G * 255.0f);
            chars[2] = HexHelper.HexDigits[bits >> 4];
            chars[3] = HexHelper.HexDigits[bits & 0xF];

            bits = (int)(this.B * 255.0f);
            chars[4] = HexHelper.HexDigits[bits >> 4];
            chars[5] = HexHelper.HexDigits[bits & 0xF];

            bits = (int)(this.A * 255.0f);
            chars[6] = HexHelper.HexDigits[bits >> 4];
            chars[7] = HexHelper.HexDigits[bits & 0xF];

            return new string(chars);
        }

        /// <summary>
        /// Creates a color from a hex string.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color FromHexString(string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length != 8 || !HexHelper.IsValidHexString(hex))
                return Black;

            int r = (HexHelper.HexDigitToByte(hex[0]) << 4) + HexHelper.HexDigitToByte(hex[1]);
            int g = (HexHelper.HexDigitToByte(hex[2]) << 4) + HexHelper.HexDigitToByte(hex[3]);
            int b = (HexHelper.HexDigitToByte(hex[4]) << 4) + HexHelper.HexDigitToByte(hex[5]);
            int a = (HexHelper.HexDigitToByte(hex[6]) << 4) + HexHelper.HexDigitToByte(hex[7]);

            return FromBytes((byte)r, (byte)g, (byte)b, (byte)a);
        }

        public override bool Equals(object obj)
        {
            if (obj is Color c)
                return this.Equals(c);

            return false;
        }

        public bool Equals(Color other)
        {
            return other.r == this.r &&
                   other.g == this.g &&
                   other.b == this.b &&
                   other.a == this.a;
        }

        public override int GetHashCode()
        {
            return this.R.GetHashCode() ^
                   this.G.GetHashCode() ^
                   this.B.GetHashCode() ^
                   this.A.GetHashCode();
        }

        public static bool operator ==(Color c1, Color c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Color c1, Color c2)
        {
            return !c1.Equals(c2);
        }
    }
}