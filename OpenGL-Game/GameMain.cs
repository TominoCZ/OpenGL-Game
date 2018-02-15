using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
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
            window.Run(0);
        }
    }

    public sealed class MainWindow : GameWindow
    {
        private WindowState lastWindowState;

        private StaticShader shader;

        private Renderer renderer;
        private Camera camera;

        private World world;

        private Point mouseLast;

        private Stopwatch sw;
        private int frames;

        public MainWindow()
        {
            Title = "OpenGL Game";

            MakeCurrent();

            CursorVisible = false;
            VSync = VSyncMode.Off;

            TextureRegistry.stitchTextures();

            init();

            sw = new Stopwatch();
            sw.Start();

            new Thread(() =>
            {
                int i = 0;

                while (true)
                {
                    if (Visible)
                    {
                        var state = OpenTK.Input.Mouse.GetState();
                        var stateScreen = OpenTK.Input.Mouse.GetCursorState();

                        var point = new Point(state.X, state.Y);

                        var clientPoint = PointToClient(new Point(stateScreen.X, stateScreen.Y));

                        bool outside = clientPoint.X < 0 || clientPoint.Y < 0 ||
                                       clientPoint.X > ClientRectangle.Width || clientPoint.Y > ClientRectangle.Height;

                        var b = Focused && !outside;

                        CursorVisible = !b;

                        if (b)
                        {
                            // every 16ms
                            if (++i >= 2)
                            {
                                camera.move();

                                var pos = Camera.INSTANCE.pos + Camera.INSTANCE.getLookVec() * 3.5f;

                                pos.X = (float)Math.Floor(pos.X);
                                pos.Y = (float)Math.Floor(pos.Y);
                                pos.Z = (float)Math.Floor(pos.Z);

                                renderer.blockRenderer.PreviewModelPos = new BlockPos(pos);

                                i = 0;
                            }

                            var delta = new Point(mouseLast.X - point.X, mouseLast.Y - point.Y);

                            camera.yaw -= delta.X / 1000f;
                            camera.pitch -= delta.Y / 1000f;
                        }

                        mouseLast = point;
                    }

                    Thread.Sleep(8);
                }
            })
            { IsBackground = true }.Start();
        }

        void init()
        {
            shader = new StaticShader("block");
            camera = new Camera();
            renderer = new Renderer(this, shader, camera);

            var stoneModel = new BlockModel(EnumBlock.STONE, shader);
            var dirtModel = new BlockModel(EnumBlock.DIRT, shader);
            var bedrockModel = new BlockModel(EnumBlock.BEDROCK, shader);
            var rareModel = new BlockModel(EnumBlock.RARE, shader);

            ModelRegistry.registerBlockModel(stoneModel);
            ModelRegistry.registerBlockModel(dirtModel);
            ModelRegistry.registerBlockModel(bedrockModel);
            ModelRegistry.registerBlockModel(rareModel);

            var rand = new Random();

            for (int y = 0; y < 5; y++)
            {
                var layer = EnumBlock.STONE;

                if (y == 0)
                    layer = EnumBlock.BEDROCK;
                else if (y == 4)
                    layer = EnumBlock.DIRT;

                for (int x = 0; x < 32; x++)
                {
                    for (int z = 0; z < 32; z++)
                    {
                        renderer.blockRenderer.setBlock((rand.NextDouble() >= 0.95 && y != 0 && y != 4) ? EnumBlock.RARE : layer, new BlockPos(x, y - 8, z));
                    }
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            frames++;

            if (sw.ElapsedMilliseconds >= 1000)
            {
                Console.WriteLine(frames + " FPS");

                frames = 0;
                sw.Restart();
            }

            renderer.render();

            SwapBuffers();
            ProcessEvents(false);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (Keyboard.GetState().IsKeyDown(Key.LAlt | Key.F4))
                Exit();

            if (e.Key == Key.F11)
            {
                if (WindowState != WindowState.Fullscreen)
                {
                    lastWindowState = WindowState;
                    WindowState = WindowState.Fullscreen;
                }
                else
                    WindowState = lastWindowState;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.IsPressed)
            {
                var target = renderer.blockRenderer.PreviewModelPos;

                if (e.Button == MouseButton.Right)
                {
                    if (renderer.blockRenderer.getBlock(target) == EnumBlock.AIR)
                        renderer.blockRenderer.setBlock(EnumBlock.STONE, target);
                }
                else if (e.Button == MouseButton.Left)
                {
                    renderer.blockRenderer.setBlock(EnumBlock.AIR, target);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, ClientRectangle.Width, ClientRectangle.Height, 0, renderer.NEAR_PLANE, renderer.FAR_PLANE);
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            ProcessEvents(false);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            shader.DetachShader();
            Loader.cleanUp();
        }
    }
}
