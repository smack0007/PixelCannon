using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace PixelCannon
{
    public sealed class TextureFont : DisposableObject
    {
        public class Character
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public int XOffset { get; set; }

            public int YOffset { get; set; }

            public int XAdvance { get; set; }

            public int Page { get; set; }

            public int Channel { get; set; }

            public Rectangle Rectangle => new Rectangle(this.X, this.Y, this.Width, this.Height);
        }

        public Texture Texture { get; private set; }

        private Dictionary<char, Character> characters;

        public IReadOnlyDictionary<char, Character> Characters => this.characters;

        public int LineHeight { get; private set; }

        public TextureFont()
        {
        }

        public static TextureFont LoadFromStream(GraphicsContext graphics, Stream stream, Func<string, Texture> loadTexture)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (loadTexture == null)
                throw new ArgumentNullException(nameof(loadTexture));

            var doc = XDocument.Load(stream);

            return ParseXml(doc, loadTexture);
        }

        private static TextureFont ParseXml(XDocument doc, Func<string, Texture> loadTexture)
        {
            var font = new TextureFont();

            foreach (var element in doc.Root.Elements())
            {
                switch (element.Name.LocalName.ToLower())
                {
                    case "info":
                        break;

                    case "common":
                        font.LineHeight = element.AttributeValueAsInt32("lineHeight");
                        break;

                    case "pages":
                        if (element.Elements().Count() != 1)
                            throw new InvalidOperationException("Currently only fonts containing a single page are supported.");

                        var textureFileName = element.Element("page").Attribute("file").Value;
                        font.Texture = loadTexture(textureFileName);

                        break;

                    case "chars":
                        font.characters = new Dictionary<char, Character>(element.AttributeValueAsInt32("count", defaultValue: 95));

                        foreach (var charElement in element.Elements())
                            ParseXmlChar(font, charElement);

                        break;
                }
            }

            return font;
        }

        private static void ParseXmlChar(TextureFont font, XElement charElement)
        {
            int id = charElement.AttributeValueAsInt32("id");

            font.characters[(char)id] = new Character()
            {
                X = charElement.AttributeValueAsInt32("x"),
                Y = charElement.AttributeValueAsInt32("y"),
                Width = charElement.AttributeValueAsInt32("width"),
                Height = charElement.AttributeValueAsInt32("height"),
                XOffset = charElement.AttributeValueAsInt32("xoffset"),
                YOffset = charElement.AttributeValueAsInt32("yoffset"),
                XAdvance = charElement.AttributeValueAsInt32("xadvance"),
                Page = charElement.AttributeValueAsInt32("page"),
                Channel = charElement.AttributeValueAsInt32("chnl"),
            };
        }

        public Size MeasureString(string s, int lineSpacing = 0)
        {
            return MeasureString(s, 0, s.Length);
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

            size.Height = this.LineHeight;

            int lineWidth = 0;
            for (int i = start; i < length; i++)
            {
                if (s[i] == '\n')
                {
                    if (lineWidth > size.Width)
                        size.Width = lineWidth;

                    lineWidth = 0;

                    size.Height += this.LineHeight + lineSpacing;
                }
                else if (s[i] != '\r')
                {
                    lineWidth += this.Characters[s[i]].XAdvance;
                }
            }

            if (lineWidth > size.Width)
                size.Width = lineWidth;

            return size;
        }
    }
}
