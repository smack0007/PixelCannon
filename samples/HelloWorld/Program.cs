using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using PixelCannon;
using static GLFWDotNet.GLFW;

namespace HelloWorld
{
    class Program
    {
        private static IntPtr window;

        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            if (glfwInit() == 0)
            {
                Console.Error.WriteLine($"{nameof(glfwInit)} failed!");
                return;
            }

            glfwWindowHint(GLFW_CLIENT_API, GLFW_OPENGL_API);
            glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
            glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
            glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 0);

            window = glfwCreateWindow(1024, 768, "Hello Pixel Cannon", IntPtr.Zero, IntPtr.Zero);
            if (window == IntPtr.Zero)
            {
                Console.Error.WriteLine("Failed to create window!");
                return;
            }

            glfwMakeContextCurrent(window);

            var context = GraphicsContext.CreateGLContext(glfwGetProcAddress);

            Init(context);

            bool done = false;
            while (!done)
            {
                if (glfwWindowShouldClose(window) != 0)
                {
                    done = true;
                    break;
                }

                glfwPollEvents();

                Draw(context);

                glfwSwapBuffers(window);
            }

            Shutdown(context);

            glfwTerminate();
        }

        private static FrameBuffer frameBuffer;
        private static Texture texture;
        private static Font font;

        private static void Init(GraphicsContext graphics)
        {
            frameBuffer = new FrameBuffer(graphics, 1024, 768);

            texture = Texture.LoadFromFile(graphics, "Box.png");

            //texture = new Texture(graphics, 2, 2);
            //texture.SetData(new byte[] {
            //    255, 255, 255, 255,
            //    0, 0, 0, 0,
            //    0, 0, 0, 0,
            //    255, 255, 255, 255,
            //});

            font = Font.RenderFromFile(graphics, "OpenSans-Regular.ttf", 48, Enumerable.Range(32, 126 - 32).Select(x => (char)x));
        }

        private static void Draw(GraphicsContext graphics)
        {
            graphics.Clear(graphics.BackBuffer, Color.Black);
            graphics.Clear(frameBuffer, Color.Magenta);

            graphics.Begin();
            graphics.DrawSprite(frameBuffer, texture, new Rectangle(0, 0, 1024, 768));
            graphics.DrawSprite(graphics.BackBuffer, frameBuffer, Vector2.Zero);
            graphics.DrawString(graphics.BackBuffer, font, "goodbye World!", Vector2.Zero);
            graphics.End();
        }

        private static void Shutdown(GraphicsContext graphics)
        {
            frameBuffer.Dispose();
            texture.Dispose();
        }
    }
}
