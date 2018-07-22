using System;
using System.Numerics;
using PixelCannon;
using static GLFWDotNet.GLFW;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
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

            var context = new PixelCannonContext(glfwGetProcAddress);

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

            glfwTerminate();
        }

        private static Texture texture;

        private static void Init(PixelCannonContext context)
        {
            texture = context.LoadTexture("Box.tga");
        }

        private static void Draw(PixelCannonContext context)
        {
            context.Clear(Color.Red);
            context.Begin();
            context.Draw(texture, Vector2.Zero);
            context.End();
        }
    }
}
