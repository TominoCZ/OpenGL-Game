using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
            var window = new Game();
            window.Run(0);
        }
    }

    sealed class Game : GameWindow
    {
        private WindowState lastWindowState;

        private StaticShader shader;

        private Renderer renderer;

        public EntityPlayerSP player;

        public World world;

        private Point mouseLast;

        private Stopwatch sw;
        private int frames;

        public static Game INSTANCE { get; private set; }

        public Game()
        {
            INSTANCE = this;

            Title = "OpenGL Game";

            MakeCurrent();

            CursorVisible = false;
            VSync = VSyncMode.Off;

            TextureRegistry.stitchTextures();

            init();

            sw = new Stopwatch();

            new Thread(() =>
                {
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
                                var delta = new Point(mouseLast.X - point.X, mouseLast.Y - point.Y);

                                player.camera.yaw -= delta.X / 1000f;
                                player.camera.pitch -= delta.Y / 1000f;
                            }

                            mouseLast = point;
                        }

                        Thread.Sleep(16);
                    }
                })
            { IsBackground = true }.Start();

            new Thread(() =>
                {
                    long counter = 0;
                    while (true)
                    {
                        if (Visible)
                            GameLoop();

                        sw.Start();
                        Thread.Sleep(50);
                        sw.Reset();
                        counter++;

                        if (counter % 20 == 0)
                        {
                            Console.WriteLine(frames.ToString("N1"));
                            frames = 0;
                        }
                    }
                })
            { IsBackground = true }.Start();
        }

        public float getRenderPartialTicks()
        {
            var time = (float)sw.Elapsed.TotalMilliseconds / 50;
            return time == 0 ? 1 : time;
        }

        private void init()
        {
            player = new EntityPlayerSP(Vector3.UnitZ);
            shader = new StaticShader("block");

            renderer = new Renderer(this, shader, player.camera);

            world = WorldGenerator.generate(0);

            var stoneModel = new BlockModel(EnumBlock.STONE, shader);
            var grassModel = new BlockModel(EnumBlock.GRASS, shader);
            var dirtModel = new BlockModel(EnumBlock.DIRT, shader);
            var bedrockModel = new BlockModel(EnumBlock.BEDROCK, shader);
            var rareModel = new BlockModel(EnumBlock.RARE, shader);

            ModelRegistry.registerBlockModel(stoneModel);
            ModelRegistry.registerBlockModel(grassModel);
            ModelRegistry.registerBlockModel(dirtModel);
            ModelRegistry.registerBlockModel(bedrockModel);
            ModelRegistry.registerBlockModel(rareModel);

            var pos = new BlockPos(player.pos);

            player.pos = Vector3.UnitY * (world.getHeightAtPos(pos.x, pos.z) + 1);

            world.generateChunkModels();
            world.addEntity(player);
        }

        private void GameLoop()
        {
            world.updateEntities();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            renderer.render(getRenderPartialTicks());

            SwapBuffers();
            ProcessEvents(false);

            frames++;
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
                var pos = new BlockPos(player.camera.pos + player.camera.getLookVec() * 3.5f);

                if (e.Button == MouseButton.Right)
                {
                    if (world.getBlock(pos) == EnumBlock.AIR)
                        world.setBlock(EnumBlock.STONE, pos, true);
                }
                else if (e.Button == MouseButton.Left)
                {
                    world.setBlock(EnumBlock.AIR, pos, true);
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
