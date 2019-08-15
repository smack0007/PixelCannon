using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static PixelCannon.FreeType;

namespace PixelCannon
{
    public class Font
    {
        public static Texture Render(GraphicsContext graphics, string fontFile, uint fontHeight, IEnumerable<char> characters)
        {
            FT_Init_FreeType(out IntPtr library);

            if (FT_New_Face(library, fontFile, 0, out var face) != 0)
                throw new PixelCannonException("Could not open font.");

            FT_Set_Pixel_Sizes(face, 0, fontHeight);

            if (FT_Load_Char(face, 'M', FT_LOAD_RENDER) != 0)
                throw new PixelCannonException($"Could not load character 'M'.");

            var metrics = face.Glyph().Metrics();

            var characterSurfaces = new Dictionary<char, Surface>();

            foreach (var ch in characters)
            {
                characterSurfaces[ch] = RenderGlyph(face, ch);
            }
            
            FT_Done_FreeType(library);

            return characterSurfaces.Values.Last().ToTexture(graphics);
        }

        private static Surface RenderGlyph(FT_Face face, char ch)
        {
            if (FT_Load_Char(face, ch, FT_LOAD_RENDER) != 0)
                throw new PixelCannonException($"Could not load character '{ch}'.");

            var bitmap = face.Glyph().Bitmap();

            if (bitmap.rows == 0 || bitmap.pitch == 0)
                return new Surface(0, 0);

            var bytes = new byte[bitmap.rows * bitmap.pitch];
            Marshal.Copy(bitmap.buffer, bytes, 0, bytes.Length);

            return new Surface((int)bitmap.width, (int)bitmap.rows, TransformGlyphBytes(bytes));

            Pixel[] TransformGlyphBytes(byte[] input)
            {
                var output = new Pixel[input.Length];

                for (int i = 0; i < input.Length; i++)
                {
                    output[i].R = input[i];
                    output[i].G = input[i];
                    output[i].B = input[i];
                    output[i].A = 255;
                }

                return output;
            }
        }
    }
}
