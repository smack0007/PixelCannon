using System;
using System.Runtime.InteropServices;
using static PixelCannon.FreeType;

namespace PixelCannon
{
    public class Font
    {
        public static Texture Render(GraphicsContext graphics, string fontFile, uint fontHeight)
        {
            FT_Init_FreeType(out IntPtr library);

            if (FT_New_Face(library, fontFile, 0, out var face) != 0)
                throw new PixelCannonException("Could not open font.");

            FT_Set_Pixel_Sizes(face, 0, fontHeight);

            if (FT_Load_Char(face, 'T', FT_LOAD_RENDER) != 0)
                throw new PixelCannonException("Could not load character 'X'.");

            var bitmap = face.Glyph().Bitmap();

            var bytes = new byte[bitmap.rows * bitmap.pitch];
            Marshal.Copy(bitmap.buffer, bytes, 0, bytes.Length);

            var pixels = TransformGlyphBytes(bytes);
            var texture = new Texture(graphics, (int)bitmap.width, (int)bitmap.rows, pixels);
            
            FT_Done_FreeType(library);

            return texture;
        }

        private static Pixel[] TransformGlyphBytes(byte[] input)
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
