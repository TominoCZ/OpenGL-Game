using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using OpenTK;
using OpenTK.Input;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    class GameMain
    {
        [STAThread]
        static void Main()
        {
            var window = new MainWindow();
            window.Run(60);
        }
    }

    public sealed class MainWindow : GameWindow
    {
        private static Vector3 hue;

        private Loader loader;
        private Model model;
        private Renderer renderer;

        private StaticShader shader;

        public MainWindow()
        {
            Title = "OpenGL Game";

            MakeCurrent();

            shader = new StaticShader("texture");

            loader = new Loader();
            renderer = new Renderer();

            var texture = new ModelTexture(loader.loadTexture("image"));
            var model = new Model(texture);

            model.addVertices(
                -0.5f, 0.5f, 0,
                -0.5f, -0.5f, 0,
                0.5f, -0.5f, 0,
                0.5f, 0.5f, 0);

            model.addIndices(
                0, 1, 3,
                3, 1, 2);

            model.addUV(
               0, 0,
               0, 1,
               1, 1,
               1, 0);

            model.bake(loader);

            this.model = model;

            new Thread(() =>
                  {
                      double a = 0;

                      while (true)
                      {
                          hue = Hue(a);

                          if (a >= 360)
                              a = 0;

                          a++;

                          Thread.Sleep(16);
                      }
                  })
            { IsBackground = true }.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            renderer.prepare();

            shader.start();
            renderer.render(model);
            shader.stop();

            /*
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(hue.X, hue.Y, hue.Z);

            GL.Enable(EnableCap.Lighting);
            GL.LightModel(LightModelParameter.LightModelAmbient, 1);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);

            GL.Vertex3(-1, -1, -1);
            GL.Vertex3(-1, 1, -1);
            GL.Vertex3(1, 1, -1);
            GL.Vertex3(1, -1, -1);

            GL.End();*/

            SwapBuffers();
            ProcessEvents(true);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Exit();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            shader.DetachShader();
            loader.cleanUp();
        }

        static Vector3 Hue(double angle)
        {
            double rad = Math.PI / 180 * angle;
            double third = Math.PI / 3;

            float x = (float)(Math.Sin(rad) * 0.5 + 0.5);
            float y = (float)(Math.Sin(rad + 2 * third) * 0.5 + 0.5);
            float z = (float)(Math.Sin(rad + 4 * third) * 0.5 + 0.5);

            return new Vector3 { X = x, Y = y, Z = z };
        }
    }
}
