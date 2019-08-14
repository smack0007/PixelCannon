/**
 * Just enough FreeType binding to be able to render a font.
 */

using System;
using System.Runtime.InteropServices;

namespace PixelCannon
{
    internal static class FreeType
    {
        private const string Library = "freetype";

        public const long FT_LOAD_RENDER = (1L << 2);

        [DllImport(Library)]
        public static extern int FT_Init_FreeType(out IntPtr alibrary);

        [DllImport(Library)]
        public static extern int FT_Done_FreeType(IntPtr library);

        [DllImport(Library, EntryPoint = "FT_New_Face")]
        private static extern int _FT_New_Face(IntPtr library, string filepathname, int face_index, out IntPtr aface);

        public static int FT_New_Face(IntPtr library, string filepathname, int face_index, out FT_Face aface)
        {
            IntPtr afacePointer;
            var result = _FT_New_Face(library, filepathname, face_index, out afacePointer);
            aface = new FT_Face(afacePointer);
            return result;
        }

        [DllImport(Library, EntryPoint = "FT_Set_Pixel_Sizes")]
        private static extern int _FT_Set_Pixel_Sizes(IntPtr face, uint pixel_width, uint pixel_height);

        public static int FT_Set_Pixel_Sizes(FT_Face face, uint pixel_width, uint pixel_height)
        {
            return _FT_Set_Pixel_Sizes(face._pointer, pixel_width, pixel_height);
        }

        [DllImport(Library, EntryPoint = "FT_Load_Char")]
        private static extern int _FT_Load_Char(IntPtr face, ulong char_code, ulong load_flags);

        public static int FT_Load_Char(FT_Face face, ulong char_code, ulong load_flags)
        {
            return _FT_Load_Char(face._pointer, char_code, load_flags);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FT_Generic
        {
            public IntPtr data;
            public IntPtr finalizier;
        }

        public class FT_Face
        {
            [StructLayout(LayoutKind.Sequential)]
            private struct RecWin32
            {
                public int num_faces;
                public int face_index;

                public int face_flags;
                public int style_flags;

                public int num_glyphs;

                public string family_name;
                public string style_name;

                public int num_fixed_sizes;
                public IntPtr available_sizes;

                public int num_charmaps;
                public IntPtr charmaps;

                public FT_Generic generic;

                public int bbox_xMin;
                public int bbox_yMin;
                public int bbox_xMax;
                public int bbox_yMax;

                public ushort units_per_EM;
                public short ascender;
                public short descender;
                public short height;

                public short max_advance_width;
                public short max_advance_height;

                public short underline_position;
                public short underline_thickness;

                public IntPtr glyph;
                public IntPtr size;
                public IntPtr charmap;
            }

            internal readonly IntPtr _pointer;

            internal FT_Face(IntPtr pointer)
            {
                _pointer = pointer;
            }

            public FT_Glyph Glyph()
            {
                var rec = Marshal.PtrToStructure<RecWin32>(_pointer);
                return new FT_Glyph(rec.glyph);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FT_MetricsWin32
        {
            public int width;
            public int height;
            public int horiBearingX;
            public int horiBearingY;
            public int horiAdvance;
            public int vertBearingX;
            public int vertBearingY;
            public int vertAdvance;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FT_Bitmap
        {
            public uint rows;
            public uint width;
            public int pitch;
            public IntPtr buffer;
            public ushort num_grays;
            public byte pixel_mode;
            public byte palette_mode;
            public IntPtr palette;
        }

        public class FT_Glyph
        {
            [StructLayout(LayoutKind.Sequential)]
            private struct RecWin32
            {
                public IntPtr library;
                public IntPtr face;
                public IntPtr next;
                public uint glyph_index;
                public FT_Generic generic;

                public FT_MetricsWin32 metrics;

                public int linearHoriAdvance;
                public int linearVertAdvance;
                public int advance_x;
                public int advance_y;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public byte[] format;

                public FT_Bitmap bitmap;                                
                public int bitmap_left;
                public int bitmap_top;

                //FT_Outline outline;

                //FT_UInt num_subglyphs;
                //FT_SubGlyph subglyphs;

                //void* control_data;
                //long control_len;

                //FT_Pos lsb_delta;
                //FT_Pos rsb_delta;

                //void* other;

                //FT_Slot_Internal internal;
            }

            internal readonly IntPtr _pointer;

            internal FT_Glyph(IntPtr pointer)
            {
                _pointer = pointer;
                //var format = BitConverter.GetBytes(((RecWin32)_struct).format).Select(x => (char)x).Reverse().ToArray();
            }

            public FT_Bitmap Bitmap()
            {
                var rec = Marshal.PtrToStructure<RecWin32>(_pointer);
                return rec.bitmap;
            }
        }
    }
}
