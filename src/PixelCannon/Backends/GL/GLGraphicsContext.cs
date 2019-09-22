using System;
using System.Numerics;
using System.Runtime.InteropServices;
using static GLDotNet.GL;
using GLDotNet.Utilities;
using System.Collections.Generic;

namespace PixelCannon.Backends.GL
{
    public sealed class GLGraphicsContext : GraphicsContext
    {
        private struct FrameBufferData
        {
            public uint FrameBufferHandle { get; set; }

            public uint TextureHandle { get; set; }
        }

        private uint _vertexBuffer;
        private uint _indexBuffer;
        private uint _vertexArray;

        private uint _backbufferProgram;
        private int _backbufferVertTransformLocation;
        private int _backbufferFragSamplerLocation;

        private uint _framebufferProgram;
        private int _framebufferVertTransformLocation;
        private int _framebufferFragSamplerLocation;

        private Color _clearColor = Color.Black;

        private Dictionary<int, FrameBufferData> _frameBuffers = new Dictionary<int, FrameBufferData>();

        Matrix4x4 _transform = new Matrix4x4()
        {
            M33 = 1.0f,
            M44 = 1.0f,
            M41 = -1.0f,
            M42 = 1.0f
        };

        internal GLGraphicsContext(Func<string, IntPtr> getProcAddress, int maxVertices)
            : base(maxVertices)
        {
            glInit(getProcAddress, 4, 3);

            unsafe
            {
                glDebugMessageCallback(OnDebugMessage, null);
            }

            glClearColor(_clearColor.R, _clearColor.G, _clearColor.B, _clearColor.A);

            _vertexBuffer = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, _vertexBuffer);
            glBufferData(GL_ARRAY_BUFFER, Vertex.SizeInBytes * maxVertices, IntPtr.Zero, GL_DYNAMIC_DRAW);
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
            uint backbufferFragmentShader = GLUtility.CreateAndCompileShader(GL_FRAGMENT_SHADER, BackbufferFragmentShaderCode);
            uint framebufferFragmentShader = GLUtility.CreateAndCompileShader(GL_FRAGMENT_SHADER, FramebufferFragmentShaderCode);

            _backbufferProgram = GLUtility.CreateAndLinkProgram(vertexShader, backbufferFragmentShader);
            _backbufferVertTransformLocation = glGetUniformLocation(_backbufferProgram, "vertTransform");
            _backbufferFragSamplerLocation = glGetUniformLocation(_backbufferProgram, "fragSampler");

            _framebufferProgram = GLUtility.CreateAndLinkProgram(vertexShader, backbufferFragmentShader);
            _framebufferVertTransformLocation = glGetUniformLocation(_framebufferProgram, "vertTransform");
            _framebufferFragSamplerLocation = glGetUniformLocation(_framebufferProgram, "fragSampler");

            glDisable(GL_CULL_FACE);
            glCullFace(GL_BACK);
            glFrontFace(GL_CW);

            glEnable(GL_BLEND);
            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

            glEnable(GL_DEPTH_TEST);
            glDepthFunc(GL_LEQUAL);

            glActiveTexture(GL_TEXTURE0);

            Initialize();
        }

        private void OnDebugMessage(uint source, uint type, uint id, uint severity, int length, string message, IntPtr userParam)
        {
        }

        protected override void Dispose(bool disposing)
        {
            glDeleteBuffer(_vertexBuffer);
            glDeleteBuffer(_indexBuffer);
            glDeleteVertexArray(_vertexArray);
        }

        protected internal override int BackendCreateFrameBuffer(bool isBackBuffer, int width, int height)
        {
            if (isBackBuffer)
            {
                return 0;
            }
            else
            {
                uint frameBufferHandle = glGenFramebuffer();

                uint textureHandle = glGenTexture();
                glBindTexture(GL_TEXTURE_2D, textureHandle);
                glTexImage2D(GL_TEXTURE_2D, 0, (int)GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, IntPtr.Zero);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, (int)GL_REPEAT);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, (int)GL_REPEAT);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_LINEAR);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_LINEAR);

                glBindFramebuffer(GL_FRAMEBUFFER, frameBufferHandle);
                glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, textureHandle, 0);
                GLUtility.CheckErrors(nameof(glFramebufferTexture2D));

                uint renderBufferHandle;
                unsafe { glGenRenderbuffers(1, &renderBufferHandle); }
                glBindRenderbuffer(GL_RENDERBUFFER, renderBufferHandle);
                glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT, width, height);

                glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, renderBufferHandle);
                GLUtility.CheckErrors(nameof(glFramebufferRenderbuffer));

                unsafe
                {
                    var drawBuffers = new uint[] { GL_COLOR_ATTACHMENT0 };
                    fixed (uint* drawBuffersPtr = drawBuffers)
                    {
                        glDrawBuffers(1, drawBuffersPtr);
                    }
                }

                if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
                {
                    throw new PixelCannonException("Failed while creating FrameBuffer.");
                }

                glBindFramebuffer(GL_FRAMEBUFFER, 0);
                glBindTexture(GL_TEXTURE_2D, 0);

                int handle = _frameBuffers.Count + 1;

                while (_frameBuffers.ContainsKey(handle))
                    handle++;

                _frameBuffers[handle] = new FrameBufferData()
                {
                    FrameBufferHandle = frameBufferHandle,
                    TextureHandle = textureHandle,
                };

                return handle;
            }
        }

        private FrameBufferData GetFrameBufferData(int frameBuffer)
        {
            if (!_frameBuffers.TryGetValue(frameBuffer, out var handle))
                throw new PixelCannonException("Unknown FrameBuffer.");

            return handle;
        }

        protected internal override void BackendFreeFrameBuffer(int frameBuffer)
        {
            var frameBufferData = GetFrameBufferData(frameBuffer);

            glDeleteTexture(frameBufferData.TextureHandle);
            glDeleteFramebuffer(frameBufferData.FrameBufferHandle);
        }

        protected internal override void BackendSetFrameBufferData(int frameBuffer, int width, int height, IntPtr data)
        {
            var frameBufferData = GetFrameBufferData(frameBuffer);

            glBindTexture(GL_TEXTURE_2D, frameBufferData.TextureHandle);
            glTexImage2D(GL_TEXTURE_2D, 0, (int)GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, data);
        }        

        protected internal override int BackendCreateTexture(int width, int height)
        {
            uint handle = glGenTexture();

            glBindTexture(GL_TEXTURE_2D, handle);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_NEAREST);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_NEAREST);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, (int)GL_REPEAT);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, (int)GL_REPEAT);
            glTexImage2D(GL_TEXTURE_2D, 0, (int)GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, IntPtr.Zero);

            return (int)handle;
        }

        protected internal override void BackendFreeTexture(int texture)
        {
            glDeleteTexture((uint)texture);
        }

        protected internal override void BackendSetTextureData(int texture, int width, int height, IntPtr data)
        {
            glBindTexture(GL_TEXTURE_2D, (uint)texture);
            glTexImage2D(GL_TEXTURE_2D, 0, (int)GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, data);
        }

        protected override void BackendClear(FrameBuffer frameBuffer, Color color)
        {
            if (frameBuffer.IsBackBuffer)
            {
                glBindFramebuffer(GL_FRAMEBUFFER, 0);
            }
            else
            {
                var frameBufferData = GetFrameBufferData(frameBuffer.Handle);
                glBindFramebuffer(GL_FRAMEBUFFER, frameBufferData.FrameBufferHandle);
            }

            if (color != _clearColor)
            {
                _clearColor = color;
                glClearColor(_clearColor.R, _clearColor.G, _clearColor.B, _clearColor.A);
            }

            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        }

        protected override void BackendDraw(
            FrameBuffer frameBuffer,
            ReadOnlySpan<Vertex> vertices,
            Sampler sampler)
        {
            glBindTexture(GL_TEXTURE_2D, 0);
            glBindFramebuffer(GL_FRAMEBUFFER, 0);

            if (frameBuffer.IsBackBuffer)
            {
                glBindFramebuffer(GL_FRAMEBUFFER, 0);
                GLUtility.CheckErrors(nameof(glBindFramebuffer));

                //Rectangle viewport = new Rectangle();
                //glGetIntegerv(GL_VIEWPORT, ref viewport.X);
                glViewport(0, 0, 1024, 768);
                _transform.M11 = 2f / 1024;
                _transform.M22 = -2f / 768;

                glEnable(GL_DEPTH_TEST);
            }
            else
            {
                var frameBufferData = GetFrameBufferData(frameBuffer.Handle);
                glBindFramebuffer(GL_FRAMEBUFFER, frameBufferData.FrameBufferHandle);
                GLUtility.CheckErrors(nameof(glBindFramebuffer));

                _transform.M11 = 2f / frameBuffer.Width;
                _transform.M11 = -2f / frameBuffer.Height;

                glViewport(0, 0, frameBuffer.Width, frameBuffer.Height);

                glDisable(GL_DEPTH_TEST);
            }

            glBindBuffer(GL_ARRAY_BUFFER, _vertexBuffer);
            glBufferData(GL_ARRAY_BUFFER, vertices, GL_DYNAMIC_DRAW);
            GLUtility.CheckErrors(nameof(glBufferData));

            glBindVertexArray(_vertexArray);

            int vertTransformLocation = 0;
            int fragSamplerLocation = 0;

            if (frameBuffer.IsBackBuffer)
            {
                glUseProgram(_backbufferProgram);
                vertTransformLocation = _backbufferVertTransformLocation;
                fragSamplerLocation = _backbufferFragSamplerLocation;
            }
            else
            {
                glUseProgram(_framebufferProgram);
                vertTransformLocation = _framebufferVertTransformLocation;
                fragSamplerLocation = _framebufferFragSamplerLocation;
            }

            glUniformMatrix4fv(vertTransformLocation, 1, false, ref _transform.M11);
            GLUtility.CheckErrors(nameof(glUniformMatrix4fv));

            if (sampler is Texture textureSampler)
            {
                glActiveTexture(GL_TEXTURE0);

                glBindTexture(GL_TEXTURE_2D, (uint)textureSampler.Handle);
                GLUtility.CheckErrors(nameof(glBindTexture));

                glUniform1i(fragSamplerLocation, 0);
                GLUtility.CheckErrors(nameof(glUniform1ui));
            }
            else if (sampler is FrameBuffer frameBufferSampler)
            {
                glActiveTexture(GL_TEXTURE0 + 1);

                var frameBufferData = GetFrameBufferData(frameBufferSampler.Handle);
                glBindTexture(GL_TEXTURE_2D, frameBufferData.TextureHandle);
                GLUtility.CheckErrors(nameof(glBindTexture));

                glUniform1i(fragSamplerLocation, 1);
                GLUtility.CheckErrors(nameof(glUniform1ui));
            }

            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, _indexBuffer);
            glDrawElements(GL_TRIANGLES, (vertices.Length / 4) * 6, GL_UNSIGNED_SHORT, IntPtr.Zero);
            GLUtility.CheckErrors(nameof(glDrawElements));

            glFlush();
            glFinish();
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

        private const string BackbufferFragmentShaderCode =
@"#version 400

uniform sampler2D fragSampler;

smooth in vec4 fragColor;
smooth in vec2 fragUV; 

out vec4 outColor; 

void main() 
{ 
	outColor = texture(fragSampler, vec2(fragUV.x, fragUV.y)) * fragColor;
}";

        private const string FramebufferFragmentShaderCode =
@"#version 400

uniform sampler2D fragSampler;

smooth in vec4 fragColor;
smooth in vec2 fragUV; 

layout(location = 0) out vec4 outColor; 

void main() 
{ 
	outColor = texture(fragSampler, vec2(fragUV.x, fragUV.y)) * fragColor;
}";
    }
}
