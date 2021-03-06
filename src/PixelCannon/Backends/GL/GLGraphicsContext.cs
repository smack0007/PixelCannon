﻿using System;
using System.Numerics;
using System.Runtime.InteropServices;
using static GLDotNet.GL;
using GLDotNet.Utilities;

namespace PixelCannon.Backends.GL
{
    public sealed class GLGraphicsContext : GraphicsContext
    {
        private uint _vertexBuffer;
        private uint _indexBuffer;
        private uint _vertexArray;

        private uint _program;
        private int _vertTranformLocation;
        private int _fragSamplerLocation;

        private Color _clearColor = Color.Black;

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
            glInit(getProcAddress, 4, 0);

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

            glActiveTexture(GL_TEXTURE0);
        }

        protected override void Dispose(bool disposing)
        {
            glDeleteBuffer(_vertexBuffer);
            glDeleteBuffer(_indexBuffer);
            glDeleteVertexArray(_vertexArray);
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

        protected override void BackendClear(Color color)
        {
            if (color != _clearColor)
            {
                _clearColor = color;
                glClearColor(_clearColor.R, _clearColor.G, _clearColor.B, _clearColor.A);
            }

            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        }

        protected override void BackendDraw(ReadOnlySpan<Vertex> vertices, Sampler sampler)
        {
            Rectangle viewport = new Rectangle();
            glGetIntegerv(GL_VIEWPORT, ref viewport.X);
            _transform.M11 = 2f / viewport.Width;
            _transform.M22 = -2f / viewport.Height;

            glBindBuffer(GL_ARRAY_BUFFER, _vertexBuffer);
            glBufferData(GL_ARRAY_BUFFER, vertices, GL_DYNAMIC_DRAW);
            GLUtility.CheckErrors(nameof(glBufferData));

            glBindVertexArray(_vertexArray);

            glUseProgram(_program);

            glUniformMatrix4fv(_vertTranformLocation, 1, false, ref _transform.M11);
            GLUtility.CheckErrors(nameof(glUniformMatrix4fv));

            glActiveTexture(GL_TEXTURE0);

            if (sampler is Texture texture)
            {
                glBindTexture(GL_TEXTURE_2D, (uint)texture.Handle);
            }

            glUniform1i(_fragSamplerLocation, 0);
            GLUtility.CheckErrors(nameof(glUniform1ui));

            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, _indexBuffer);
            glDrawElements(GL_TRIANGLES, (vertices.Length / 4) * 6, GL_UNSIGNED_SHORT, IntPtr.Zero);
            GLUtility.CheckErrors(nameof(glDrawElements));
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
