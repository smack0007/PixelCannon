using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static PixelCannon.FreeType;

namespace PixelCannon
{
    public class Font
    {
        public static Texture Render(GraphicsContext graphics, string fontFile)
        {
            FT_Init_FreeType(out IntPtr library);


            if (FT_New_Face(library, fontFile, 0, out var face) != 0)
                throw new PixelCannonException("Could not open font.");

            FT_Set_Pixel_Sizes(face, 0, 48);

            if (FT_Load_Char(face, 'T', FT_LOAD_RENDER) != 0)
                throw new PixelCannonException("Could not load character 'X'.");

            var bitmap = face.Glyph().Bitmap();

            var bytes = new byte[bitmap.rows * bitmap.pitch];
            Marshal.Copy(bitmap.buffer, bytes, 0, bytes.Length);

            bytes = TransformBytes(bytes);
            var texture = new Texture(graphics, (int)bitmap.width, (int)bitmap.rows, bytes);
            
            FT_Done_FreeType(library);

            return texture;
        }

        private static byte[] TransformBytes(byte[] input)
        {
            var output = new byte[input.Length * 4];

            for (int i = 0; i < input.Length; i++)
            {
                output[i * 4] = input[i];
                output[i * 4 + 1] = input[i];
                output[i * 4 + 2] = input[i];
                output[i * 4 + 3] = 255;
            }

            return output;
        }
    }
}
