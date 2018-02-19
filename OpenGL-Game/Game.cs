using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using OpenGL_Game.world;
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

        private Stopwatch timer = Stopwatch.StartNew();
        private Point mouseLast;

        public MouseOverObject mouseOverObject = new MouseOverObject();

        private GameRenderer _gameRenderer;

        public EntityPlayerSP player;
        public World world;

        public GuiScreen guiScreen { get; private set; }

        public static Game INSTANCE { get; private set; }

        public Game()
        {
            INSTANCE = this;

            Title = "OpenGL Game";
            CursorVisible = false;
            VSync = VSyncMode.Off;
            MakeCurrent();

            Console.WriteLine("DEBUG: stitching textures");
            TextureManager.stitchTextures();

            init();
        }

        private void init()
        {
            Console.WriteLine("DEBUG: loading models");

            var shader = new BlockShader("block");
            var shader_unlit = new BlockShaderUnlit("block_unlit");

            var stoneModel = new BlockModel(EnumBlock.STONE, shader);
            var grassModel = new BlockModel(EnumBlock.GRASS, shader);
            var dirtModel = new BlockModel(EnumBlock.DIRT, shader);
            var bedrockModel = new BlockModel(EnumBlock.BEDROCK, shader);
            var rareModel = new BlockModel(EnumBlock.RARE, shader_unlit);

            ModelManager.registerBlockModel(stoneModel);
            ModelManager.registerBlockModel(grassModel);
            ModelManager.registerBlockModel(dirtModel);
            ModelManager.registerBlockModel(bedrockModel);
            ModelManager.registerBlockModel(rareModel);

            var loadedWorld = WorldLoader.loadWorld();

            if (loadedWorld == null)
            {
                Console.WriteLine("DEBUG: generating world");
                world = WorldGenerator.generate(0);
                player = new EntityPlayerSP(Vector3.UnitY * (world.getHeightAtPos(0, 0) + 1));
            }
            else
            {
                world = loadedWorld;
            }

            player.setEquippedItem(new ItemBlock(EnumBlock.STONE));

            _gameRenderer = new GameRenderer(player.camera);

            world.generateChunkModels();
            world.addEntity(player);

            startGameUpdateThreads();
        }

        private void startGameUpdateThreads()
        {
            new Thread(() =>
                {
                    while (true)
                    {
                        if (Visible)
                        {
                            timer.Stop();
                            GameLoop();
                            timer.Restart();
                        }

                        Thread.Sleep(50);
                    }
                })
            { IsBackground = true }.Start();

            new Thread(() =>
                {
                    bool wasSpaceDown = false;

                    while (true)
                    {
                        if (Visible)
                        {
                            var state = OpenTK.Input.Mouse.GetState();
                            var stateScreen = OpenTK.Input.Mouse.GetCursorState();

                            var point = new Point(state.X, state.Y);

                            if (guiScreen == null)
                            {
                                var clientPoint = PointToClient(new Point(stateScreen.X, stateScreen.Y));

                                bool outside = clientPoint.X < 0 || clientPoint.Y < 0 ||
                                               clientPoint.X > ClientRectangle.Width ||
                                               clientPoint.Y > ClientRectangle.Height;

                                var b = Focused && !outside;

                                CursorVisible = !b;

                                if (b)
                                {
                                    var delta = new Point(mouseLast.X - point.X, mouseLast.Y - point.Y);

                                    player.camera.yaw -= delta.X / 1000f;
                                    player.camera.pitch -= delta.Y / 1000f;

                                    var keyboardState = Keyboard.GetState();

                                    if (keyboardState.IsKeyDown(Key.Space) && !wasSpaceDown && player.onGround)
                                    {
                                        wasSpaceDown = true;
                                        player.motion.Y = 0.475F;
                                    }
                                    else if ((!keyboardState.IsKeyDown(Key.Space) || player.onGround) && wasSpaceDown)
                                        wasSpaceDown = false;

                                    getMouseOverObject();
                                }
                            }

                            mouseLast = point;
                        }

                        Thread.Sleep(16);
                    }
                })
            { IsBackground = true }.Start();
        }

        private void GameLoop()
        {
            world.updateEntities();
        }

        public void closeGuiScreen()
        {
            guiScreen?.onClose();
            guiScreen = null;

            CursorVisible = false;
        }

        public void openGuiScreen(GuiScreen guiScreen)
        {
            this.guiScreen = guiScreen;

            if (guiScreen is GuiIngameMenu)
            {
                var middle = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
                middle = PointToScreen(middle);

                OpenTK.Input.Mouse.SetPosition(middle.X, middle.Y);
            }
        }

        private void getMouseOverObject()
        {
            int radius = 5;

            List<MouseOverObject> moos = new List<MouseOverObject>();

            for (int z = -radius; z <= radius; z++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        var vec = new Vector3(x, y, z) + Vector3.One * 0.5f + player.camera.pos;
                        float f = (vec - player.camera.pos).LengthFast;

                        if (f <= radius)
                        {
                            var pos = new BlockPos(vec);
                            var block = world.getBlock(pos);

                            if (block != EnumBlock.AIR)
                            {
                                var model = ModelManager.getModelForBlock(block);
                                var bb = model.boundingBox.offset(pos.vector);

                                var hitSomething = RayHelper.rayIntersectsBB(player.camera.pos,
                                    player.camera.getLookVec(), bb, out var hitPos, out var normal);

                                if (hitSomething)
                                {
                                    var sideHit = EnumFacing.UP;

                                    if (normal.X < 0)
                                        sideHit = EnumFacing.WEST;
                                    else if (normal.X > 0)
                                        sideHit = EnumFacing.EAST;
                                    if (normal.Y < 0)
                                        sideHit = EnumFacing.DOWN;
                                    else if (normal.Y > 0)
                                        sideHit = EnumFacing.UP;
                                    if (normal.Z < 0)
                                        sideHit = EnumFacing.NORTH;
                                    else if (normal.Z > 0)
                                        sideHit = EnumFacing.SOUTH;

                                    moos.Add(new MouseOverObject
                                    {
                                        hit = block,
                                        hitVec = hitPos,
                                        blockPos = new BlockPos(hitPos - normal * 0.5f),
                                        normal = normal,
                                        sideHit = sideHit
                                    });
                                }
                            }
                        }
                    }
                }
            }

            float dist = float.MaxValue;

            MouseOverObject closest = null;

            for (var index = 0; index < moos.Count; index++)
            {
                var moo = moos[index];
                var l = Math.Abs((player.camera.pos - (moo.hitVec - moo.normal * 0.5f)).Length);

                if (l < dist && (EnumBlock)moo.hit != EnumBlock.AIR)
                {
                    dist = l;

                    closest = moo;
                }
            }

            if (closest != null)
            {
                mouseOverObject.hitVec = closest.hitVec;
                mouseOverObject.blockPos = closest.blockPos;
                mouseOverObject.normal = closest.normal;
                mouseOverObject.sideHit = closest.sideHit;
            }

            mouseOverObject.hit = closest?.hit;
        }

        public float getRenderPartialTicks()
        {
            return timer.ElapsedMilliseconds / 50f;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            float partialTicks = getRenderPartialTicks();

            if (!Focused && guiScreen == null)
                openGuiScreen(new GuiScreen());

            _gameRenderer.render(partialTicks);

            SwapBuffers();
            ProcessEvents(false);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (Keyboard.GetState().IsKeyDown(Key.Escape))
            {
                if (guiScreen != null)
                    closeGuiScreen();
                else
                    openGuiScreen(new GuiIngameMenu());
            }

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
                if (guiScreen == null)
                {
                    if (mouseOverObject.hit is EnumBlock)
                    {
                        var pos = mouseOverObject.blockPos;

                        //pickBlock
                        if (e.Button == MouseButton.Middle)
                        {
                            var clickedBlock = world.getBlock(pos);

                            if (clickedBlock != EnumBlock.AIR)
                            {
                                player.setEquippedItem(new ItemBlock(clickedBlock));
                            }
                        }

                        //place
                        if (player.getEquippedItem() is ItemBlock itemBlock && e.Button == MouseButton.Right)
                        {
                            pos = pos.offset(mouseOverObject.sideHit);

                            var blockAtPos = world.getBlock(pos);

                            var heldBlock = itemBlock.getBlock();
                            var blockBB = ModelManager.getModelForBlock(heldBlock).boundingBox.offset(pos.vector);

                            if (blockAtPos == EnumBlock.AIR && world.getIntersectingEntitiesBBs(blockBB).Count == 0)
                            {
                                var posUnder = pos.offset(EnumFacing.DOWN);
                                var blockUnder = world.getBlock(posUnder);

                                if (blockUnder == EnumBlock.GRASS)
                                    world.setBlock(EnumBlock.DIRT, posUnder, false);

                                world.setBlock(heldBlock, pos, true);
                            }
                        }

                        //break;
                        if (e.Button == MouseButton.Left)
                            world.setBlock(EnumBlock.AIR, pos, true);
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, ClientRectangle.Width, ClientRectangle.Height, 0, _gameRenderer.NEAR_PLANE, _gameRenderer.FAR_PLANE);
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            ProcessEvents(false);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ModelManager.cleanUp();
            TextureManager.cleanUp();

            GraphicsManager.cleanUp();

            WorldLoader.saveWorld(world);
        }
    }
}
