﻿using System;
using System.Runtime.InteropServices;

namespace PixelCannon
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Pixel : IEquatable<Pixel>
    {
        #region Static colors

        public static readonly Pixel AliceBlue = new Pixel(0xF0, 0xF8, 0xFF, 0xFF);
        public static readonly Pixel AntiqueWhite = new Pixel(0xFA, 0xEB, 0xD7, 0xFF);
        public static readonly Pixel Aqua = new Pixel(0x00, 0xFF, 0xFF, 0xFF);
        public static readonly Pixel Aquamarine = new Pixel(0x7F, 0xFF, 0xD4, 0xFF);
        public static readonly Pixel Azure = new Pixel(0xF0, 0xFF, 0xFF, 0xFF);
        public static readonly Pixel Beige = new Pixel(0xF5, 0xF5, 0xDC, 0xFF);
        public static readonly Pixel Bisque = new Pixel(0xFF, 0xE4, 0xC4, 0xFF);
        public static readonly Pixel Black = new Pixel(0x00, 0x00, 0x00, 0xFF);
        public static readonly Pixel BlanchedAlmond = new Pixel(0xFF, 0xEB, 0xCD, 0xFF);
        public static readonly Pixel Blue = new Pixel(0x00, 0x00, 0xFF, 0xFF);
        public static readonly Pixel BlueViolet = new Pixel(0x8A, 0x2B, 0xE2, 0xFF);
        public static readonly Pixel Brown = new Pixel(0xA5, 0x2A, 0x2A, 0xFF);
        public static readonly Pixel BurlyWood = new Pixel(0xDE, 0xB8, 0x87, 0xFF);
        public static readonly Pixel CadetBlue = new Pixel(0x5F, 0x9E, 0xA0, 0xFF);
        public static readonly Pixel Chartreuse = new Pixel(0x7F, 0xFF, 0x00, 0xFF);
        public static readonly Pixel Chocolate = new Pixel(0xD2, 0x69, 0x1E, 0xFF);
        public static readonly Pixel Coral = new Pixel(0xFF, 0x7F, 0x50, 0xFF);
        public static readonly Pixel CornflowerBlue = new Pixel(0x64, 0x95, 0xED, 0xFF);
        public static readonly Pixel Cornsilk = new Pixel(0xFF, 0xF8, 0xDC, 0xFF);
        public static readonly Pixel Crimson = new Pixel(0xDC, 0x14, 0x3C, 0xFF);
        public static readonly Pixel Cyan = new Pixel(0x00, 0xFF, 0xFF, 0xFF);
        public static readonly Pixel DarkBlue = new Pixel(0x00, 0x00, 0x8B, 0xFF);
        public static readonly Pixel DarkCyan = new Pixel(0x00, 0x8B, 0x8B, 0xFF);
        public static readonly Pixel DarkGoldenRod = new Pixel(0xB8, 0x86, 0x0B, 0xFF);
        public static readonly Pixel DarkGray = new Pixel(0xA9, 0xA9, 0xA9, 0xFF);
        public static readonly Pixel DarkGrey = new Pixel(0xA9, 0xA9, 0xA9, 0xFF);
        public static readonly Pixel DarkGreen = new Pixel(0x00, 0x64, 0x00, 0xFF);
        public static readonly Pixel DarkKhaki = new Pixel(0xBD, 0xB7, 0x6B, 0xFF);
        public static readonly Pixel DarkMagenta = new Pixel(0x8B, 0x00, 0x8B, 0xFF);
        public static readonly Pixel DarkOliveGreen = new Pixel(0x55, 0x6B, 0x2F, 0xFF);
        public static readonly Pixel Darkorange = new Pixel(0xFF, 0x8C, 0x00, 0xFF);
        public static readonly Pixel DarkOrchid = new Pixel(0x99, 0x32, 0xCC, 0xFF);
        public static readonly Pixel DarkRed = new Pixel(0x8B, 0x00, 0x00, 0xFF);
        public static readonly Pixel DarkSalmon = new Pixel(0xE9, 0x96, 0x7A, 0xFF);
        public static readonly Pixel DarkSeaGreen = new Pixel(0x8F, 0xBC, 0x8F, 0xFF);
        public static readonly Pixel DarkSlateBlue = new Pixel(0x48, 0x3D, 0x8B, 0xFF);
        public static readonly Pixel DarkSlateGray = new Pixel(0x2F, 0x4F, 0x4F, 0xFF);
        public static readonly Pixel DarkSlateGrey = new Pixel(0x2F, 0x4F, 0x4F, 0xFF);
        public static readonly Pixel DarkTurquoise = new Pixel(0x00, 0xCE, 0xD1, 0xFF);
        public static readonly Pixel DarkViolet = new Pixel(0x94, 0x00, 0xD3, 0xFF);
        public static readonly Pixel DeepPink = new Pixel(0xFF, 0x14, 0x93, 0xFF);
        public static readonly Pixel DeepSkyBlue = new Pixel(0x00, 0xBF, 0xFF, 0xFF);
        public static readonly Pixel DimGray = new Pixel(0x69, 0x69, 0x69, 0xFF);
        public static readonly Pixel DimGrey = new Pixel(0x69, 0x69, 0x69, 0xFF);
        public static readonly Pixel DodgerBlue = new Pixel(0x1E, 0x90, 0xFF, 0xFF);
        public static readonly Pixel FireBrick = new Pixel(0xB2, 0x22, 0x22, 0xFF);
        public static readonly Pixel FloralWhite = new Pixel(0xFF, 0xFA, 0xF0, 0xFF);
        public static readonly Pixel ForestGreen = new Pixel(0x22, 0x8B, 0x22, 0xFF);
        public static readonly Pixel Fuchsia = new Pixel(0xFF, 0x00, 0xFF, 0xFF);
        public static readonly Pixel Gainsboro = new Pixel(0xDC, 0xDC, 0xDC, 0xFF);
        public static readonly Pixel GhostWhite = new Pixel(0xF8, 0xF8, 0xFF, 0xFF);
        public static readonly Pixel Gold = new Pixel(0xFF, 0xD7, 0x00, 0xFF);
        public static readonly Pixel GoldenRod = new Pixel(0xDA, 0xA5, 0x20, 0xFF);
        public static readonly Pixel Gray = new Pixel(0x80, 0x80, 0x80, 0xFF);
        public static readonly Pixel Grey = new Pixel(0x80, 0x80, 0x80, 0xFF);
        public static readonly Pixel Green = new Pixel(0x00, 0x80, 0x00, 0xFF);
        public static readonly Pixel GreenYellow = new Pixel(0xAD, 0xFF, 0x2F, 0xFF);
        public static readonly Pixel HoneyDew = new Pixel(0xF0, 0xFF, 0xF0, 0xFF);
        public static readonly Pixel HotPink = new Pixel(0xFF, 0x69, 0xB4, 0xFF);
        public static readonly Pixel IndianRed = new Pixel(0xCD, 0x5C, 0x5C, 0xFF);
        public static readonly Pixel Indigo = new Pixel(0x4B, 0x00, 0x82, 0xFF);
        public static readonly Pixel Ivory = new Pixel(0xFF, 0xFF, 0xF0, 0xFF);
        public static readonly Pixel Khaki = new Pixel(0xF0, 0xE6, 0x8C, 0xFF);
        public static readonly Pixel Lavender = new Pixel(0xE6, 0xE6, 0xFA, 0xFF);
        public static readonly Pixel LavenderBlush = new Pixel(0xFF, 0xF0, 0xF5, 0xFF);
        public static readonly Pixel LawnGreen = new Pixel(0x7C, 0xFC, 0x00, 0xFF);
        public static readonly Pixel LemonChiffon = new Pixel(0xFF, 0xFA, 0xCD, 0xFF);
        public static readonly Pixel LightBlue = new Pixel(0xAD, 0xD8, 0xE6, 0xFF);
        public static readonly Pixel LightCoral = new Pixel(0xF0, 0x80, 0x80, 0xFF);
        public static readonly Pixel LightCyan = new Pixel(0xE0, 0xFF, 0xFF, 0xFF);
        public static readonly Pixel LightGoldenRodYellow = new Pixel(0xFA, 0xFA, 0xD2, 0xFF);
        public static readonly Pixel LightGray = new Pixel(0xD3, 0xD3, 0xD3, 0xFF);
        public static readonly Pixel LightGrey = new Pixel(0xD3, 0xD3, 0xD3, 0xFF);
        public static readonly Pixel LightGreen = new Pixel(0x90, 0xEE, 0x90, 0xFF);
        public static readonly Pixel LightPink = new Pixel(0xFF, 0xB6, 0xC1, 0xFF);
        public static readonly Pixel LightSalmon = new Pixel(0xFF, 0xA0, 0x7A, 0xFF);
        public static readonly Pixel LightSeaGreen = new Pixel(0x20, 0xB2, 0xAA, 0xFF);
        public static readonly Pixel LightSkyBlue = new Pixel(0x87, 0xCE, 0xFA, 0xFF);
        public static readonly Pixel LightSlateGray = new Pixel(0x77, 0x88, 0x99, 0xFF);
        public static readonly Pixel LightSlateGrey = new Pixel(0x77, 0x88, 0x99, 0xFF);
        public static readonly Pixel LightSteelBlue = new Pixel(0xB0, 0xC4, 0xDE, 0xFF);
        public static readonly Pixel LightYellow = new Pixel(0xFF, 0xFF, 0xE0, 0xFF);
        public static readonly Pixel Lime = new Pixel(0x00, 0xFF, 0x00, 0xFF);
        public static readonly Pixel LimeGreen = new Pixel(0x32, 0xCD, 0x32, 0xFF);
        public static readonly Pixel Linen = new Pixel(0xFA, 0xF0, 0xE6, 0xFF);
        public static readonly Pixel Magenta = new Pixel(0xFF, 0x00, 0xFF, 0xFF);
        public static readonly Pixel Maroon = new Pixel(0x80, 0x00, 0x00, 0xFF);
        public static readonly Pixel MediumAquaMarine = new Pixel(0x66, 0xCD, 0xAA, 0xFF);
        public static readonly Pixel MediumBlue = new Pixel(0x00, 0x00, 0xCD, 0xFF);
        public static readonly Pixel MediumOrchid = new Pixel(0xBA, 0x55, 0xD3, 0xFF);
        public static readonly Pixel MediumPurple = new Pixel(0x93, 0x70, 0xD8, 0xFF);
        public static readonly Pixel MediumSeaGreen = new Pixel(0x3C, 0xB3, 0x71, 0xFF);
        public static readonly Pixel MediumSlateBlue = new Pixel(0x7B, 0x68, 0xEE, 0xFF);
        public static readonly Pixel MediumSpringGreen = new Pixel(0x00, 0xFA, 0x9A, 0xFF);
        public static readonly Pixel MediumTurquoise = new Pixel(0x48, 0xD1, 0xCC, 0xFF);
        public static readonly Pixel MediumVioletRed = new Pixel(0xC7, 0x15, 0x85, 0xFF);
        public static readonly Pixel MidnightBlue = new Pixel(0x19, 0x19, 0x70, 0xFF);
        public static readonly Pixel MintCream = new Pixel(0xF5, 0xFF, 0xFA, 0xFF);
        public static readonly Pixel MistyRose = new Pixel(0xFF, 0xE4, 0xE1, 0xFF);
        public static readonly Pixel Moccasin = new Pixel(0xFF, 0xE4, 0xB5, 0xFF);
        public static readonly Pixel NavajoWhite = new Pixel(0xFF, 0xDE, 0xAD, 0xFF);
        public static readonly Pixel Navy = new Pixel(0x00, 0x00, 0x80, 0xFF);
        public static readonly Pixel OldLace = new Pixel(0xFD, 0xF5, 0xE6, 0xFF);
        public static readonly Pixel Olive = new Pixel(0x80, 0x80, 0x00, 0xFF);
        public static readonly Pixel OliveDrab = new Pixel(0x6B, 0x8E, 0x23, 0xFF);
        public static readonly Pixel Orange = new Pixel(0xFF, 0xA5, 0x00, 0xFF);
        public static readonly Pixel OrangeRed = new Pixel(0xFF, 0x45, 0x00, 0xFF);
        public static readonly Pixel Orchid = new Pixel(0xDA, 0x70, 0xD6, 0xFF);
        public static readonly Pixel PaleGoldenRod = new Pixel(0xEE, 0xE8, 0xAA, 0xFF);
        public static readonly Pixel PaleGreen = new Pixel(0x98, 0xFB, 0x98, 0xFF);
        public static readonly Pixel PaleTurquoise = new Pixel(0xAF, 0xEE, 0xEE, 0xFF);
        public static readonly Pixel PaleVioletRed = new Pixel(0xD8, 0x70, 0x93, 0xFF);
        public static readonly Pixel PapayaWhip = new Pixel(0xFF, 0xEF, 0xD5, 0xFF);
        public static readonly Pixel PeachPuff = new Pixel(0xFF, 0xDA, 0xB9, 0xFF);
        public static readonly Pixel Peru = new Pixel(0xCD, 0x85, 0x3F, 0xFF);
        public static readonly Pixel Pink = new Pixel(0xFF, 0xC0, 0xCB, 0xFF);
        public static readonly Pixel Plum = new Pixel(0xDD, 0xA0, 0xDD, 0xFF);
        public static readonly Pixel PowderBlue = new Pixel(0xB0, 0xE0, 0xE6, 0xFF);
        public static readonly Pixel Purple = new Pixel(0x80, 0x00, 0x80, 0xFF);
        public static readonly Pixel Red = new Pixel(0xFF, 0x00, 0x00, 0xFF);
        public static readonly Pixel RosyBrown = new Pixel(0xBC, 0x8F, 0x8F, 0xFF);
        public static readonly Pixel RoyalBlue = new Pixel(0x41, 0x69, 0xE1, 0xFF);
        public static readonly Pixel SaddleBrown = new Pixel(0x8B, 0x45, 0x13, 0xFF);
        public static readonly Pixel Salmon = new Pixel(0xFA, 0x80, 0x72, 0xFF);
        public static readonly Pixel SandyBrown = new Pixel(0xF4, 0xA4, 0x60, 0xFF);
        public static readonly Pixel SeaGreen = new Pixel(0x2E, 0x8B, 0x57, 0xFF);
        public static readonly Pixel SeaShell = new Pixel(0xFF, 0xF5, 0xEE, 0xFF);
        public static readonly Pixel Sienna = new Pixel(0xA0, 0x52, 0x2D, 0xFF);
        public static readonly Pixel Silver = new Pixel(0xC0, 0xC0, 0xC0, 0xFF);
        public static readonly Pixel SkyBlue = new Pixel(0x87, 0xCE, 0xEB, 0xFF);
        public static readonly Pixel SlateBlue = new Pixel(0x6A, 0x5A, 0xCD, 0xFF);
        public static readonly Pixel SlateGray = new Pixel(0x70, 0x80, 0x90, 0xFF);
        public static readonly Pixel SlateGrey = new Pixel(0x70, 0x80, 0x90, 0xFF);
        public static readonly Pixel Snow = new Pixel(0xFF, 0xFA, 0xFA, 0xFF);
        public static readonly Pixel SpringGreen = new Pixel(0x00, 0xFF, 0x7F, 0xFF);
        public static readonly Pixel SteelBlue = new Pixel(0x46, 0x82, 0xB4, 0xFF);
        public static readonly Pixel Tan = new Pixel(0xD2, 0xB4, 0x8C, 0xFF);
        public static readonly Pixel Teal = new Pixel(0x00, 0x80, 0x80, 0xFF);
        public static readonly Pixel Thistle = new Pixel(0xD8, 0xBF, 0xD8, 0xFF);
        public static readonly Pixel Tomato = new Pixel(0xFF, 0x63, 0x47, 0xFF);
        public static readonly Pixel Transparent = new Pixel(0x00, 0x00, 0x00, 0x00);
        public static readonly Pixel Turquoise = new Pixel(0x40, 0xE0, 0xD0, 0xFF);
        public static readonly Pixel Violet = new Pixel(0xEE, 0x82, 0xEE, 0xFF);
        public static readonly Pixel Wheat = new Pixel(0xF5, 0xDE, 0xB3, 0xFF);
        public static readonly Pixel White = new Pixel(0xFF, 0xFF, 0xFF, 0xFF);
        public static readonly Pixel WhiteSmoke = new Pixel(0xF5, 0xF5, 0xF5, 0xFF);
        public static readonly Pixel Yellow = new Pixel(0xFF, 0xFF, 0x00, 0xFF);
        public static readonly Pixel YellowGreen = new Pixel(0x9A, 0xCD, 0x32, 0xFF);

        #endregion

        public byte R;

        public byte G;

        public byte B;

        public byte A;

        public Pixel(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public override string ToString() => $"{{{R}, {G}, {B}, {A}}}";

        public override bool Equals(object obj)
        {
            if (obj is Pixel p)
                return Equals(p);

            return false;
        }

        public bool Equals(Pixel other)
        {
            return other.R == R &&
                   other.G == G &&
                   other.B == B &&
                   other.A == A;
        }

        public override int GetHashCode()
        {
            return R.GetHashCode() ^
                   G.GetHashCode() ^
                   B.GetHashCode() ^
                   A.GetHashCode();
        }

        public static bool operator ==(Pixel p1, Pixel p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Pixel p1, Pixel p2)
        {
            return !p1.Equals(p2);
        }
    }
}
