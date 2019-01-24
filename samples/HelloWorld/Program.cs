using System;
using System.IO;
using System.Reflection;
using PixelCannon;
using static GLFWDotNet.GLFW;

namespace HelloWorld
{
    class Program
    {
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

            var window = glfwCreateWindow(800, 600, "Hello Pixel Cannon", IntPtr.Zero, IntPtr.Zero);
            if (window == IntPtr.Zero)
            {
                Console.Error.WriteLine("Failed to create window!");
                return;
            }

            glfwMakeContextCurrent(window);

            var context = new GraphicsContext(new GLBackend(glfwGetProcAddress));

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

        private static Texture texture;

        private static void Init(GraphicsContext graphics)
        {
            texture = Texture.LoadFromFile(graphics, "Box.tga");
        }

        private static void Draw(GraphicsContext graphics)
        {
            graphics.Clear(Color.Black);

            graphics.Begin();
            graphics.DrawSprite(texture, new Rectangle(0, 0, texture.Width * 2, texture.Height * 2), tint: new Color(1, 1, 1, 0.2f));
            graphics.End();
        }

        private static void Shutdown(GraphicsContext graphics)
        {
            texture.Dispose();
        }
    }
}
