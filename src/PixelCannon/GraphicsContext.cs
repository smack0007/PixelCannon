using System;
using System.Numerics;
using System.Runtime.InteropServices;
using ImageDotNet;
using static GLDotNet.GL;

namespace PixelCannon
{
    public class GraphicsContext
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Vertex
        {
            public static int SizeInBytes => Marshal.SizeOf<Vertex>();
            
            public Vector3 Position;
            public Color Color;
            public Vector2 UV;
        }

        private Color _clearColor = Color.Black;

        private Vertex[] _vertices;
        private int _vertexCount;

        private Texture _texture;

        private uint _vertexBuffer;
        private uint _indexBuffer;
        private uint _vertexArray;

        private uint _program;
        private int _vertTranformLocation;
        private int _fragSamplerLocation;

        private bool _drawInProgress;

        Matrix4x4 _transform = new Matrix4x4()
        {
            M33 = 1.0f,
            M44 = 1.0f,
            M41 = -1.0f,
            M42 = 1.0f
        };

        public GraphicsContext(Func<string, IntPtr> getProcAddress, int maxSprites = 1024)
        {
            if (getProcAddress == null)
                throw new ArgumentNullException(nameof(getProcAddress));

            if (maxSprites <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxSprites), $"{nameof(maxSprites)} must be >= 1.");

            glInit(getProcAddress, 4, 0);

            glClearColor(_clearColor.R, _clearColor.G, _clearColor.B, _clearColor.A);

            _vertices = new Vertex[maxSprites * 4];

            _vertexBuffer = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, _vertexBuffer);
            glBufferData(GL_ARRAY_BUFFER, Vertex.SizeInBytes * _vertices.Length, IntPtr.Zero, GL_DYNAMIC_DRAW);
            GLUtility.CheckErrors(nameof(glBufferData));

            _vertexArray = glGenVertexArray();
            glBindVertexArray(_vertexArray);
            glVertexAttribPointer(0, 3, GL_FLOAT, false, Vertex.SizeInBytes, IntPtr.Zero);
            GLUtility.CheckErrors(nameof(glVertexAttribPointer));
            glVertexAttribPointer(1, 4, GL_FLOAT, false, Vertex.SizeInBytes, (IntPtr)Marshal.SizeOf<Vector3>());
            GLUtility.CheckErrors(nameof(glVertexAttribPointer));
            glVertexAttribPointer(2, 2, GL_FLOAT, false, Vertex.SizeInBytes, (IntPtr)(Marshal.SizeOf<Vector3>() + Marshal.SizeOf<Vector4>()));
            GLUtility.CheckErrors(nameof(glVertexAttribPointer));
            glEnableVertexAttribArray(0);
            glEnableVertexAttribArray(1);
            glEnableVertexAttribArray(2);

            ushort[] indices = new ushort[1024 * 6];
            for (ushort i = 0, vertex = 0; i < indices.Length; i += 6, vertex += 4)
            {
                indices[i] = vertex;
                indices[i + 1] = (ushort)(vertex + 1);
                indices[i + 2] = (ushort)(vertex + 3);
                indices[i + 3] = (ushort)(vertex + 1);
                indices[i + 4] = (ushort)(vertex + 2);
                indices[i + 5] = (ushort)(vertex + 3);
            }

            _indexBuffer = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, _indexBuffer);
            glBufferData(GL_ARRAY_BUFFER, sizeof(ushort) * indices.Length, indices, GL_STATIC_DRAW);
            GLUtility.CheckErrors(nameof(glBufferData));

            uint vertexShader = GLUtility.CreateAndCompileShader(GL_VERTEX_SHADER, VertexShaderCode);
            uint fragmentShader = GLUtility.CreateAndCompileShader(GL_FRAGMENT_SHADER, FragmentShaderCode);
            _program = GLUtility.CreateAndLinkProgram(vertexShader, fragmentShader);

            _vertTranformLocation = glGetUniformLocation(_program, "vertTransform");
            _fragSamplerLocation = glGetUniformLocation(_program, "fragSampler");

            glDisable(GL_CULL_FACE);
            glCullFace(GL_BACK);
            glFrontFace(GL_CW);

            glEnable(GL_BLEND);
            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

            glEnable(GL_DEPTH_TEST);
            glDepthFunc(GL_LEQUAL);
        }

        ~GraphicsContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            glDeleteBuffer(_vertexBuffer);
            glDeleteBuffer(_indexBuffer);
            glDeleteVertexArray(_vertexArray);
        }

        public Texture LoadTexture(string fileName)
        {
            uint texture = glGenTexture();
            glActiveTexture(GL_TEXTURE0);
            glBindTexture(GL_TEXTURE_2D, texture);

            IImage image = null;

            if (fileName.EndsWith(".tga"))
            {
                image = Image.LoadTga(fileName);
            }
            else if (fileName.EndsWith(".png"))
            {
                image = Image.LoadPng(fileName);
            }
            
            if (image == null)
                throw new PixelCannonException("Unsupported image type.");

            bool updateAlpha = image.BytesPerPixel != 4;

            image = image.To<Rgba32>();

            if (updateAlpha)
            {
                for (int i = 0; i < image.Length; i++)
                    ((Image<Rgba32>)image)[i].A = 255;
            }

            using (var data = image.GetDataPointer())
            {
                glTexImage2D(GL_TEXTURE_2D, 0, (int)GL_RGBA, image.Width, image.Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, data.Pointer);
            }

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_LINEAR);

            return new Texture(texture, image.Width, image.Height);
        }

        public void FreeTexture(Texture texture)
        {
            glDeleteTexture(texture.Handle);
        }

        public void Clear(Color color)
        {
            if (color != _clearColor)
            {
                _clearColor = color;
                glClearColor(_clearColor.R, _clearColor.G, _clearColor.B, _clearColor.A);
            }

            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        }

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

        private void Flush()
        {
            if (_vertexCount > 0)
            {
                Rectangle viewport = new Rectangle();
                glGetIntegerv(GL_VIEWPORT, ref viewport.X);
                _transform.M11 = 2f / viewport.Width;
                _transform.M22 = -2f / viewport.Height;

                glBindBuffer(GL_ARRAY_BUFFER, _vertexBuffer);
                glBufferData(GL_ARRAY_BUFFER, Vertex.SizeInBytes * _vertexCount, _vertices, GL_DYNAMIC_DRAW);
                GLUtility.CheckErrors(nameof(glBufferData));

                glBindVertexArray(_vertexArray);

                glUseProgram(_program);

                glUniformMatrix4fv(_vertTranformLocation, 1, false, ref _transform.M11);
                GLUtility.CheckErrors(nameof(glUniformMatrix4fv));

                glActiveTexture(GL_TEXTURE0);
                glBindTexture(GL_TEXTURE_2D, _texture.Handle);
                glUniform1i(_fragSamplerLocation, 0);
                GLUtility.CheckErrors(nameof(glUniform1ui));

                glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, _indexBuffer);
                glDrawElements(GL_TRIANGLES, (_vertexCount / 4) * 6, GL_UNSIGNED_SHORT, IntPtr.Zero);
                GLUtility.CheckErrors(nameof(glDrawElements));

                _vertexCount = 0;
            }
        }

        private const string VertexShaderCode =
@"#version 400

uniform mat4 vertTransform; 

layout(location = 0) in vec3 vertPosition; 
layout(location = 1) in vec4 vertColor; 
layout(location = 2) in vec2 vertUV; 

smooth out vec4 fragColor;
smooth out vec2 fragUV;

void main() 
{ 
    gl_Position = vertTransform * vec4(vertPosition, 1.0);
    fragColor = vertColor;
    fragUV = vertUV; 
}";

        private const string FragmentShaderCode =
@"#version 400

uniform sampler2D fragSampler;

smooth in vec4 fragColor;
smooth in vec2 fragUV; 

out vec4 outColor; 

void main() 
{ 
	outColor = texture(fragSampler, vec2(fragUV.x, fragUV.y)) * fragColor;
}";
    }
}
