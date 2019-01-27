using System;
using System.Numerics;

namespace PixelCannon
{
    public abstract partial class GraphicsContext : DisposableObject
    {
        public const int DefaultMaxVertices = 8192;

        private readonly Vertex[] _vertices;
        private int _vertexCount;

        private Texture _texture;

        private bool _drawInProgress;

        internal GraphicsContext(int maxVertices)
        {
            if (maxVertices <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxVertices), $"{nameof(maxVertices)} must be >= 1.");

            _vertices = new Vertex[maxVertices];
        }

        protected internal abstract Texture BackendCreateTexture(int width, int height, IntPtr? data);

        protected internal abstract void BackendFreeTexture(Texture texture);

        protected internal abstract void BackendSetTextureData(Texture texture, IntPtr data);

        private void EnsureDrawInProgress()
        {
            if (!_drawInProgress)
                throw new InvalidOperationException("Draw not currently in progress.");
        }

        public void Begin()
        {
            if (_drawInProgress)
                throw new InvalidOperationException("Draw already in progress.");

            _drawInProgress = true;
        }

        public void End()
        {
            EnsureDrawInProgress();

            Flush();

            _drawInProgress = false;
        }

        private void CalculateUV(float x, float y, ref Vector2 uv)
        {
            if (_texture.Width != 1 || _texture.Height != 1)
            {
                uv.X = x / _texture.Width;
                uv.Y = y / _texture.Height;
            }
            else
            {
                uv.X = 0;
                uv.Y = 0;
            }
        }

        private void AddQuad(
            ref Vector2 topLeft,
            ref Vector2 topRight,
            ref Vector2 bottomRight,
            ref Vector2 bottomLeft,
            Rectangle source,
            Color color,
            float layerDepth)
        {
            if (_vertexCount == _vertices.Length)
                Flush();

            _vertices[_vertexCount].Position = new Vector3(topLeft, layerDepth);
            CalculateUV(source.Left, source.Top, ref _vertices[_vertexCount].UV);
            _vertices[_vertexCount].Color = color;

            _vertices[_vertexCount + 1].Position = new Vector3(topRight, layerDepth);
            CalculateUV(source.Right, source.Top, ref _vertices[_vertexCount + 1].UV);
            _vertices[_vertexCount + 1].Color = color;

            _vertices[_vertexCount + 2].Position = new Vector3(bottomRight, layerDepth);
            CalculateUV(source.Right, source.Bottom, ref _vertices[_vertexCount + 2].UV);
            _vertices[_vertexCount + 2].Color = color;

            _vertices[_vertexCount + 3].Position = new Vector3(bottomLeft, layerDepth);
            CalculateUV(source.Left, source.Bottom, ref _vertices[_vertexCount + 3].UV);
            _vertices[_vertexCount + 3].Color = color;

            _vertexCount += 4;
        }

        public void DrawSprite(
            Texture texture,
            Vector2 destination,
            Rectangle? source = null,
            Color? tint = null,
            Vector2? origin = null,
            Vector2? scale = null,
            float rotation = 0.0f,
            float layerDepth = 0.0f)
        {
            DrawSpriteInternal(
                texture,
                ref destination,
                source != null ? source.Value.Width : texture.Width,
                source != null ? source.Value.Height : texture.Height,
                source,
                tint,
                origin,
                scale,
                rotation,
                layerDepth);
        }

        public void DrawSprite(
            Texture texture,
            Rectangle destination,
            Rectangle? source = null,
            Color? tint = null,
            Vector2? origin = null,
            Vector2? scale = null,
            float rotation = 0.0f,
            float layerDepth = 0.0f)
        {
            var vecDestination = new Vector2(destination.X, destination.Y);

            DrawSpriteInternal(
                texture,
                ref vecDestination,
                destination.Width,
                destination.Height,
                source,
                tint,
                origin,
                scale,
                rotation,
                layerDepth);
        }

        private void DrawSpriteInternal(
            Texture texture,
            ref Vector2 destination,
            int width,
            int height,
            Rectangle? source,
            Color? tint,
            Vector2? origin,
            Vector2? scale,
            float rotation,
            float layerDepth)
        {
            EnsureDrawInProgress();

            if (texture != _texture)
                Flush();

            _texture = texture;

            if (source == null)
                source = new Rectangle(0, 0, texture.Width, texture.Height);

            if (tint == null)
                tint = Color.White;

            if (origin == null)
                origin = Vector2.Zero;

            if (scale == null)
                scale = Vector2.One;

            Vector2 topLeft = new Vector2(-origin.Value.X, -origin.Value.Y);
            Vector2 topRight = new Vector2(width - origin.Value.X, -origin.Value.Y);
            Vector2 bottomRight = new Vector2(width - origin.Value.X, height - origin.Value.Y);
            Vector2 bottomLeft = new Vector2(-origin.Value.X, height - origin.Value.Y);

            Matrix4x4 rotationMatrix = Matrix4x4.CreateRotationZ(rotation);
            Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale.Value.X, scale.Value.Y, 1.0f);
            Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(destination.X, destination.Y, 0.0f);

            Matrix4x4 transform = scaleMatrix * rotationMatrix * translationMatrix;

            topLeft = Vector2.Transform(topLeft, transform);
            topRight = Vector2.Transform(topRight, transform);
            bottomRight = Vector2.Transform(bottomRight, transform);
            bottomLeft = Vector2.Transform(bottomLeft, transform);

            AddQuad(
                ref topLeft,
                ref topRight,
                ref bottomRight,
                ref bottomLeft,
                source.Value,
                tint.Value,
                layerDepth);
        }

        public Vector2 DrawString(
            TextureFont font,
            string text,
            Vector2 position,
            int lineSpacing = 0,
            Size? textSize = null,
            Color? tint = null,
            Vector2? origin = null,
            Vector2? scale = null,
            float rotation = 0.0f,
            float layerDepth = 0.0f)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            if (text == null)
                throw new ArgumentNullException("text");

            if (text.Length == 0)
                return position;

            if (textSize == null)
                textSize = font.MeasureString(text);

            return DrawString(
                font,
                text,
                new Rectangle((int)position.X, (int)position.Y, textSize.Value.Width, textSize.Value.Height),
                lineSpacing,
                tint,
                origin,
                scale,
                rotation,
                layerDepth);
        }

        public Vector2 DrawString(
            TextureFont font,
            string text,
            Rectangle destination,
            int lineSpacing = 0,
            Color? tint = null,
            Vector2? origin = null,
            Vector2? scale = null,
            float rotation = 0.0f,
            float layerDepth = 0.0f)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            if (text == null)
                throw new ArgumentNullException("text");

            if (text.Length == 0)
                return new Vector2(destination.X, destination.Y);

            if (tint == null)
                tint = Color.White;

            if (origin == null)
                origin = Vector2.Zero;

            if (scale == null)
                scale = Vector2.One;

            float heightOfSingleLine = font.LineHeight * scale.Value.Y;

            if (heightOfSingleLine > destination.Height) // We can't draw anything
                return new Vector2(destination.X, destination.Y);

            Vector2 cursor = new Vector2(destination.X, destination.Y);

            for (int i = 0; i < text.Length; i++)
            {
                // Skip characters we can't render.
                if (text[i] == '\r')
                    continue;

                var character = font.Characters[text[i]];
                float widthOfCharacter = character.Width * scale.Value.X;
                float heightOfCharacter = character.Height * scale.Value.Y;

                if (text[i] == '\n' || cursor.X + widthOfCharacter > destination.Right)
                {
                    cursor.X = destination.X;
                    cursor.Y += heightOfSingleLine + lineSpacing;

                    // If the next line extends past the destination, quit.
                    if (cursor.Y + heightOfSingleLine > destination.Bottom)
                        return cursor;

                    // We can't render a new line.
                    if (text[i] == '\n')
                        continue;
                }

                Vector2 characterOrigin = origin.Value;
                characterOrigin.X -= cursor.X - destination.X + character.XOffset;
                characterOrigin.Y -= cursor.Y - destination.Y + character.YOffset;

                var letterSource = character.Rectangle;

                var letterDestination = new Rectangle(
                    (int)cursor.X + (int)characterOrigin.X + character.XOffset,
                    (int)cursor.Y + (int)characterOrigin.Y + character.YOffset,
                    (int)widthOfCharacter,
                    (int)heightOfCharacter);

                DrawSprite(
                    font.Texture,
                    letterDestination,
                    letterSource,
                    tint,
                    characterOrigin,
                    scale,
                    rotation,
                    layerDepth);

                cursor.X += character.XAdvance;
            }

            return cursor;
        }

        public void Clear(Color color)
        {
            BackendClear(color);
        }

        protected abstract void BackendClear(Color color);

        private void Flush()
        {
            if (_vertexCount > 0)
            {
                BackendDraw(_vertices, _vertexCount, _texture.Handle);
                _vertexCount = 0;
            }
        }

        protected abstract void BackendDraw(Vertex[] vertices, int vertexCount, int textueHandle);
    }
}
