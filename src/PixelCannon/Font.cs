using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static PixelCannon.FreeType;

namespace PixelCannon
{
    public class Font
    {
        public class Character
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public int OffsetX { get; set; }

            public int OffsetY { get; set; }

            public int AdvanceX { get; set; }

            public int AdvanceY { get; set; }

            public Rectangle Rectangle => new Rectangle(X, Y, Width, Height);
        }

        public Texture Texture { get; }

        public IReadOnlyDictionary<char, Character> Characters { get; }

        public int LineHeight { get; }

        public Font(Texture texture, IReadOnlyDictionary<char, Character> characters, int lineHeight)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Characters = characters ?? throw new ArgumentNullException(nameof(characters));
            LineHeight = lineHeight;
        }

        public static Font Render(GraphicsContext graphics, string fontFile, uint fontSize, IEnumerable<char> characters, byte alpha = 0)
        {
            FT_Init_FreeType(out IntPtr library);

            if (FT_New_Face(library, fontFile, 0, out var face) != 0)
                throw new PixelCannonException("Could not open font.");

            FT_Set_Pixel_Sizes(face, 0, fontSize);

            if (FT_Load_Char(face, 'M', FT_LOAD_RENDER) != 0)
                throw new PixelCannonException($"Could not load character 'M'.");

            var glyph = face.Glyph();
            var metrics = glyph.Metrics();
            var bitmap = glyph.Bitmap();
            int fontWidth = (int)bitmap.width;
            int fontHeight = (int)bitmap.rows;
            int fontLineHeight = (int)(metrics.height / 64);

            var characterSurfaces = new Dictionary<char, Surface>();
            var characterData = new Dictionary<char, Font.Character>();

            foreach (var ch in characters)
            {
                if (FT_Load_Char(face, ch, FT_LOAD_RENDER) != 0)
                    throw new PixelCannonException($"Could not load character '{ch}'.");

                glyph = face.Glyph();
                metrics = glyph.Metrics();
                bitmap = glyph.Bitmap();
                var width = (int)(metrics.width / 64);
                var height = (int)(metrics.height / 64);
                var offsetX = (int)(metrics.horiBearingX / 64);
                var offsetY = fontLineHeight - (int)(metrics.horiBearingY / 64);
                var advance = glyph.Advance();

                characterSurfaces[ch] = RenderGlyph(bitmap, alpha);

                var data = new Character()
                {
                    Width = width,
                    Height = height,
                    OffsetX = offsetX,
                    OffsetY = offsetY,
                    AdvanceX = (int)(advance.x / 64),
                    AdvanceY = (int)(advance.y / 64),
                };

                characterData[ch] = data;
            }
            
            FT_Done_FreeType(library);

            var surface = RenderSheet(characterSurfaces, characterData, fontWidth, fontHeight, alpha);

            return new Font(surface.ToTexture(graphics), characterData, fontLineHeight);
        }

        private static Surface RenderGlyph(FT_Bitmap bitmap, byte alpha)
        {
            if (bitmap.rows == 0 || bitmap.pitch == 0)
                return new Surface(0, 0);

            var bytes = new byte[bitmap.rows * bitmap.pitch];
            Marshal.Copy(bitmap.buffer, bytes, 0, bytes.Length);

            return new Surface((int)bitmap.width, (int)bitmap.rows, TransformGlyphBytes(bytes, alpha));

            Pixel[] TransformGlyphBytes(byte[] input, byte a)
            {
                var output = new Pixel[input.Length];

                for (int i = 0; i < input.Length; i++)
                {
                    output[i].R = input[i];
                    output[i].G = input[i];
                    output[i].B = input[i];
                    output[i].A = input[i] != 0 ? (byte)255 : alpha;
                }

                return output;
            }
        }

        private static Surface RenderSheet(
            Dictionary<char, Surface> characterSurfaces,
            Dictionary<char, Character> characterData,
            int fontWidth,
            int fontHeight,
            byte alpha)
        {
            var halfCharacterCount = characterData.Values.Count / 4;
            var surfaceWidth = (int)MathHelper.RoundClosestPowerOf2((uint)(halfCharacterCount * fontWidth));

            var charactersPerLine = surfaceWidth / fontWidth;
            var neededLines = (int)Math.Ceiling(characterData.Values.Count / (float)charactersPerLine);
            var surfaceHeight = (int)MathHelper.RoundNextPowerOf2((uint)(fontHeight * neededLines));

            var surface = new Surface(surfaceWidth, surfaceHeight);
            surface.Clear(new Pixel() { R = 0, G = 0, B = 0, A = alpha });

            var x = 0;
            var y = 0;
            foreach (var character in characterSurfaces.Keys)
            {
                var characterSurface = characterSurfaces[character];
                var data = characterData[character];

                if (x + characterSurface.Width > surface.Width)
                {
                    y += fontHeight;
                    x = 0;
                }

                surface.Blit(characterSurface, new Point(x, y));
                data.X = x;
                data.Y = y;

                x += characterSurface.Width;
            }

            return surface;
        }

        public Size MeasureString(string s, int lineSpacing = 0)
        {
            return MeasureString(s, 0, s.Length, lineSpacing);
        }

        /// <summary>
        /// Measures the size of the string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="start">The index of the string at which to start measuring.</param>
        /// <param name="length">How many characters to measure from the start.</param>
        /// <returns></returns>
        public Size MeasureString(string s, int start, int length, int lineSpacing = 0)
        {
            if (start < 0 || start > s.Length)
                throw new ArgumentOutOfRangeException("start", "Start is not an index within the string.");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "Length must me >= 0.");

            if (start + length > s.Length)
                throw new ArgumentOutOfRangeException("length", "Start + length is greater than the string's length.");

            Size size = Size.Zero;

            size.Height = LineHeight;

            int lineWidth = 0;
            for (int i = start; i < length; i++)
            {
                if (s[i] == '\n')
                {
                    if (lineWidth > size.Width)
                        size.Width = lineWidth;

                    lineWidth = 0;

                    size.Height += LineHeight + lineSpacing;
                }
                else if (s[i] != '\r')
                {
                    lineWidth += Characters[s[i]].AdvanceX;
                }
            }

            if (lineWidth > size.Width)
                size.Width = lineWidth;

            return size;
        }
    }
}
