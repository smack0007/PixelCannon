using System;
using System.Numerics;

namespace PixelCannon
{
    public abstract partial class GraphicsContext : DisposableObject
    {
        public const int DefaultMaxVertices = 8192;

        private readonly Vertex[] _vertices;
        private int _vertexCount;

        private FrameBuffer _frameBuffer;
        private Sampler _sampler;

        private bool _drawInProgress;

        public FrameBuffer BackBuffer { get; private set; }

        internal GraphicsContext(int maxVertices)
        {
            if (maxVertices <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxVertices), $"{nameof(maxVertices)} must be >= 1.");

            _vertices = new Vertex[maxVertices];
        }

        protected void Initialize()
        {
            BackBuffer = FrameBuffer.CreateBackBuffer();
        }

        protected internal abstract int BackendCreateFrameBuffer(bool isBackBuffer, int width, int height);

        protected internal abstract void BackendFreeFrameBuffer(int frameBuffer);

        protected internal abstract void BackendSetFrameBufferData(int frameBuffer, int width, int height, IntPtr data);

        protected internal abstract int BackendCreateTexture(int width, int height);

        protected internal abstract void BackendFreeTexture(int texture);

        protected internal abstract void BackendSetTextureData(int texture, int width, int height, IntPtr data);

        public void Clear(FrameBuffer frameBuffer, Color color)
        {
            BackendClear(frameBuffer, color);
        }

        protected abstract void BackendClear(FrameBuffer frameBuffer, Color color);

        private void EnsureDrawInProgress()
        {
            if (!_drawInProgress)
                throw new InvalidOperationException("Draw not currently in progress.");
        }

        public void Begin()
        {
            if (_drawInProgress)
                throw new InvalidOperationException("Draw already in progress.");

            _frameBuffer = BackBuffer;

            _drawInProgress = true;
        }

        public void End()
        {
            EnsureDrawInProgress();

            Flush();

            _frameBuffer = null;
            _sampler = null;

            _drawInProgress = false;
        }

        private void CalculateUV(float x, float y, ref Vector2 uv)
        {
            if (_sampler.Width != 1 || _sampler.Height != 1)
            {
                uv.X = x / _sampler.Width;
                uv.Y = y / _sampler.Height;
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
            FrameBuffer frameBuffer,
            Sampler sampler,
            Vector2 destination,
            Rectangle? source = null,
            Color? tint = null,
            Vector2? origin = null,
            Vector2? scale = null,
            float rotation = 0.0f,
            float layerDepth = 0.0f)
        {
            DrawSpriteInternal(
                frameBuffer,
                sampler,
                ref destination,
                source != null ? source.Value.Width : sampler.Width,
                source != null ? source.Value.Height : sampler.Height,
                source,
                tint,
                origin,
                scale,
                rotation,
                layerDepth);
        }

        public void DrawSprite(
            FrameBuffer frameBuffer,
            Sampler sampler,
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
                frameBuffer,
                sampler,
                ref vecDestination,
                destination.Width,
                destination.Height,
                source,
                tint,
                origin,
                scale,
                rotation,
                layerDepth);
            ;
        }

        private void DrawSpriteInternal(
            FrameBuffer frameBuffer,
            Sampler sampler,
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

            if (frameBuffer != _frameBuffer || sampler != _sampler)
                Flush();

            _frameBuffer = frameBuffer;
            _sampler = sampler;

            if (source == null)
                source = new Rectangle(0, 0, sampler.Width, sampler.Height);

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
            FrameBuffer frameBuffer,
            Font font,
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
                throw new ArgumentNullException(nameof(font));

            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (text.Length == 0)
                return position;

            if (textSize == null)
                textSize = font.MeasureString(text);

            return DrawString(
                frameBuffer,
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
            FrameBuffer frameBuffer,
            Font font,
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
                throw new ArgumentNullException(nameof(font));

            if (text == null)
                throw new ArgumentNullException(nameof(text));

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

                var characterOffset = new Vector2(character.OffsetX * scale.Value.X, character.OffsetY * scale.Value.Y);

                var characterOrigin = origin.Value;
                characterOrigin.X -= cursor.X - destination.X + characterOffset.X;
                characterOrigin.Y -= cursor.Y - destination.Y + characterOffset.Y;

                var letterSource = character.Rectangle;

                var letterDestination = new Rectangle(
                    (int)(cursor.X + characterOrigin.X + characterOffset.X),
                    (int)(cursor.Y + characterOrigin.Y + characterOffset.Y),
                    (int)widthOfCharacter,
                    (int)heightOfCharacter);

                DrawSprite(
                    frameBuffer,
                    font.Texture,
                    letterDestination,
                    letterSource,
                    tint,
                    characterOrigin,
                    scale,
                    rotation,
                    layerDepth);

                cursor.X += character.AdvanceX;
            }

            return cursor;
        }

        private void Flush()
        {
            if (_vertexCount > 0)
            {
                BackendDraw(_frameBuffer, _vertices.AsSpan(0, _vertexCount), _sampler);
                _vertexCount = 0;
            }
        }

        protected abstract void BackendDraw(
            FrameBuffer frameBuffer,
            ReadOnlySpan<Vertex> vertices,
            Sampler sampler);
    }
}
