using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            var window = new Game();
            window.Run(0);
        }
    }

    sealed class Game : GameWindow
    {
        private WindowState lastWindowState;

        private Stopwatch frameTimer = Stopwatch.StartNew();
        private Stopwatch timer = Stopwatch.StartNew();
        private Point mouseLast;

        public MouseOverObject mouseOverObject = new MouseOverObject();

        public GameRenderer gameRenderer;

        public EntityPlayerSP player;
        public World world;

        private int FPS;
        private float mouseWheelLast;

        public GuiScreen guiScreen { get; private set; }
        public static Game INSTANCE { get; private set; }

        public static ThreadSafeList<ThreadLock> MAIN_THREAD_QUEUE = new ThreadSafeList<ThreadLock>();

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

            var shader = new BlockShader("block", PrimitiveType.Quads);
            //var shader_unlit = new BlockShaderUnlit("block_unlit", PrimitiveType.Quads);

            var stoneModel = new BlockModel(EnumBlock.STONE, shader, false);
            var grassModel = new BlockModel(EnumBlock.GRASS, shader, false);
            var dirtModel = new BlockModel(EnumBlock.DIRT, shader, false);
            var cobblestoneModel = new BlockModel(EnumBlock.COBBLESTONE, shader, false);
            var planksModel = new BlockModel(EnumBlock.PLANKS, shader, false);
            var craftingTableModel = new BlockModel(EnumBlock.CRAFTING_TABLE, shader, true);
            var furnaceModel = new BlockModel(EnumBlock.FURNACE, shader, true);
            var bedrockModel = new BlockModel(EnumBlock.BEDROCK, shader, false);
            var rareModel = new BlockModel(EnumBlock.RARE, shader, false);
            var glassModel = new BlockModel(EnumBlock.GLASS, shader, false);

            ModelManager.registerBlockModel(stoneModel);
            ModelManager.registerBlockModel(grassModel);
            ModelManager.registerBlockModel(dirtModel);
            ModelManager.registerBlockModel(cobblestoneModel);
            ModelManager.registerBlockModel(planksModel);
            ModelManager.registerBlockModel(craftingTableModel);
            ModelManager.registerBlockModel(furnaceModel);
            ModelManager.registerBlockModel(bedrockModel);
            ModelManager.registerBlockModel(rareModel);
            ModelManager.registerBlockModel(glassModel);

            gameRenderer = new GameRenderer(new Camera());

            SettingsManager.load();

            openGuiScreen(new GuiScreenMainMenu());
        }

        public void startGame()
        {
            var loadedWorld = WorldLoader.loadWorld();

            if (loadedWorld == null)
            {
                Console.WriteLine("DEBUG: generating world");

                var r = new Random();

                var playerPos = new BlockPos(-100 + (float)r.NextDouble() * 200, 0, -100 + (float)r.NextDouble() * 200);

                world = new World(0);
                world.generateChunk(playerPos, true);

                player = new EntityPlayerSP(new Vector3(playerPos.x, world.getHeightAtPos(playerPos.x, playerPos.z), playerPos.z));

                player.setItemInHotbar(0, new ItemBlock(EnumBlock.CRAFTING_TABLE));
                player.setItemInHotbar(1, new ItemBlock(EnumBlock.FURNACE));
                player.setItemInHotbar(2, new ItemBlock(EnumBlock.COBBLESTONE));
                player.setItemInHotbar(3, new ItemBlock(EnumBlock.PLANKS));
                player.setItemInHotbar(4, new ItemBlock(EnumBlock.GLASS));
            }
            else
            {
                world = loadedWorld;
            }

            resetMouse();

            var state = OpenTK.Input.Mouse.GetState();
            mouseLast = new Point(state.X, state.Y);

            gameRenderer.setCamera(player.camera);

            world.addEntity(player);

            runUpdateThreads();

            ShaderManager.updateProjectionMatrix();
        }

        private void runUpdateThreads()
        {
            new Thread(() =>
            {
                while (true)
                {
                    if (Visible)
                    {
                        checkForEmptyChunks();
                    }

                    Thread.Sleep(250);
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

                        var point = new Point(state.X, state.Y);

                        if (guiScreen == null)
                        {
                            if (!(CursorVisible = !Focused))
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

                                resetMouse();
                            }
                        }

                        mouseLast = point;
                    }

                    Thread.Sleep(2);
                }
            })
            { IsBackground = true }.Start();
        }

        private void resetMouse()
        {
            var middle = PointToScreen(new Point(ClientSize.Width / 2, ClientSize.Height / 2));
            OpenTK.Input.Mouse.SetPosition(middle.X, middle.Y);
        }

        private void GameLoop()
        {
            if (guiScreen == null && !Focused)
            {
                openGuiScreen(new GuiScreenIngameMenu());
            }

            var wheelValue = Mouse.WheelPrecise;

            if (player != null)
            {
                if (wheelValue < mouseWheelLast)
                    player.selectNextItem();
                else if (wheelValue > mouseWheelLast)
                    player.selectPreviousItem();
            }

            mouseWheelLast = wheelValue;

            world?.updateEntities();
        }

        private void checkForEmptyChunks()
        {
            if (world == null || player == null)
                return;

            var rDist = gameRenderer.worldRenderer.RenderDistance;

            for (int x = -rDist; x < rDist; x++)
            {
                for (int z = -rDist; z < rDist; z++)
                {
                    var pos = new BlockPos(x * 16 + player.pos.X, 0, z * 16 + player.pos.Z).ChunkPos() +
                              new BlockPos(8, 0, 8);

                    var dist = MathUtil.distance(pos.vector.Xz, player.camera.pos.Xz);

                    if (dist <= rDist * 16)
                    {
                        var chunk = world.getChunkFromPos(pos);

                        if (chunk == null)
                        {
                            new Thread(() => world.generateChunk(pos, true)).Start();

                            Console.WriteLine("generated a chunk!");
                        }
                        else if (!world.doesChunkHaveModel(pos))
                        {
                            world.updateModelForChunk(pos);
                        }
                    }
                }
            }
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

            if (guiScreen is GuiScreenIngameMenu)
            {
                var middle = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
                middle = PointToScreen(middle);

                OpenTK.Input.Mouse.SetPosition(middle.X, middle.Y);
            }
        }

        public float getRenderPartialTicks()
        {
            return (float)timer.Elapsed.TotalMilliseconds / 50f;
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
                var l = Math.Abs((player.camera.pos - (moo.blockPos.vector + Vector3.One * 0.5f)).Length);

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

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (timer.ElapsedMilliseconds >= 50)
            {
                GameLoop();
                timer.Restart();
            }

            float partialTicks = getRenderPartialTicks();

            gameRenderer.render(partialTicks);

            SwapBuffers();
            ProcessEvents(false);

            if (MAIN_THREAD_QUEUE.Count > 0)
            {
                for (int i = 0; i < MAIN_THREAD_QUEUE.Count; i++)
                {
                    var task = MAIN_THREAD_QUEUE[0];

                    task.ExecuteCode();

                    MAIN_THREAD_QUEUE.Remove(task);
                }
            }

            if (frameTimer.ElapsedMilliseconds >= 1000)
            {
                frameTimer.Restart();

                Console.WriteLine($"{FPS} FPS");

                FPS = 0;
            }

            FPS++;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (Keyboard.GetState().IsKeyDown(Key.Escape))
            {
                if (guiScreen is GuiScreenMainMenu)
                    return;

                if (guiScreen != null)
                    closeGuiScreen();
                else
                {
                    openGuiScreen(new GuiScreenIngameMenu());

                    WorldLoader.saveWorld(world);
                }
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
                                player.setItemInSelectedSlot(new ItemBlock(clickedBlock));
                            }
                        }

                        //place/interact
                        if (e.Button == MouseButton.Right)
                        {
                            var block = world.getBlock(pos);
                            var model = ModelManager.getModelForBlock(block);

                            if (model != null && model.canBeInteractedWith)
                            {
                                switch (block)
                                {
                                    case EnumBlock.FURNACE:
                                    case EnumBlock.CRAFTING_TABLE:
                                        openGuiScreen(new GuiScreenCrafting());
                                        break;
                                }
                            }
                            else if (player.getEquippedItem() is ItemBlock itemBlock)
                            {
                                pos = pos.offset(mouseOverObject.sideHit);

                                var blockAtPos = world.getBlock(pos);

                                var heldBlock = itemBlock.getBlock();
                                var blockBB = ModelManager.getModelForBlock(heldBlock).boundingBox.offset(pos.vector);

                                if (blockAtPos == EnumBlock.AIR && world.getIntersectingEntitiesBBs(blockBB).Count == 0)
                                {
                                    var posUnder = pos.offset(EnumFacing.DOWN);

                                    var blockUnder = world.getBlock(posUnder);
                                    var blockAbove = world.getBlock(pos.offset(EnumFacing.UP));

                                    if (blockUnder == EnumBlock.GRASS)
                                        world.setBlock(posUnder, EnumBlock.DIRT, false);
                                    if (blockAbove != EnumBlock.AIR && heldBlock == EnumBlock.GRASS)
                                        world.setBlock(pos, EnumBlock.DIRT, true);
                                    else
                                        world.setBlock(pos, heldBlock, true);
                                }
                            }
                        }

                        //break
                        if (e.Button == MouseButton.Left)
                            world.setBlock(pos, EnumBlock.AIR, true);
                    }
                }
                else
                {
                    var state = OpenTK.Input.Mouse.GetCursorState();
                    var point = PointToClient(new Point(state.X, state.Y));

                    guiScreen.onMouseClick(point.X, point.Y);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (ClientSize.Width < 640)
                ClientSize = new Size(640, ClientSize.Height);
            if (ClientSize.Height < 480)
                ClientSize = new Size(ClientSize.Width, 480);

            base.OnResize(e);

            GL.Viewport(ClientRectangle);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, ClientRectangle.Width, ClientRectangle.Height, 0, gameRenderer.NEAR_PLANE, gameRenderer.FAR_PLANE);

            ShaderManager.updateProjectionMatrix();
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            ProcessEvents(false);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ShaderManager.cleanUp();
            TextureManager.cleanUp();

            GraphicsManager.cleanUp();

            WorldLoader.saveWorld(world);
            SettingsManager.save();
        }
    }

    class SettingsManager
    {
        public static void load()
        {
            var dir = "SharpCraft_Data";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var file = dir + "/settings.txt";

            if (File.Exists(file))
            {
                var data = File.ReadLines(file);

                foreach (var line in data)
                {
                    var parsed = line.Trim().Replace(" ", "").ToLower();
                    var split = parsed.Split('=');

                    if (split.Length < 2)
                        continue;

                    if (parsed.Contains("renderdistance="))
                    {
                        int.TryParse(split[1], out var num);
                        Game.INSTANCE.gameRenderer.worldRenderer.RenderDistance = num;
                    }
                }
            }
        }

        public static void save()
        {
            var dir = "SharpCraft_Data";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var file = dir + "/settings.txt";

            StringBuilder sb = new StringBuilder();
            sb.Append($"renderDistance={Game.INSTANCE.gameRenderer.worldRenderer.RenderDistance}");

            File.WriteAllText(file, sb.ToString());
        }
    }
}
