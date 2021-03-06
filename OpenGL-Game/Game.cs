﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    internal class GameMain
    {
        [STAThread]
        private static void Main()
        {
            var window = new Game();
            window.Run(0);
        }
    }

    internal sealed class Game : GameWindow
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
            var shader_unlit = new BlockShaderUnlit("block_unlit", PrimitiveType.Quads);

            var stoneModel = new BlockModel(EnumBlock.STONE, shader, false);
            var grassModel = new BlockModel(EnumBlock.GRASS, shader, false);
            var dirtModel = new BlockModel(EnumBlock.DIRT, shader, false);
            var cobblestoneModel = new BlockModel(EnumBlock.COBBLESTONE, shader, false);
            var planksModel = new BlockModel(EnumBlock.PLANKS, shader, false);
            var craftingTableModel = new BlockModel(EnumBlock.CRAFTING_TABLE, shader, true);
            var furnaceModel = new BlockModel(EnumBlock.FURNACE, shader, true);
            var bedrockModel = new BlockModel(EnumBlock.BEDROCK, shader, false);
            var rareModel = new BlockModel(EnumBlock.RARE, shader, false);
            var rareModelUnlit = new BlockModel(EnumBlock.RARE, shader_unlit, false);
            var glassModel = new BlockModel(EnumBlock.GLASS, shader, false);

            ModelManager.registerBlockModel(stoneModel,
                0); //TODO - set model for block using metadata here, dont sore it in the model
            ModelManager.registerBlockModel(grassModel, 0);
            ModelManager.registerBlockModel(dirtModel, 0);
            ModelManager.registerBlockModel(cobblestoneModel, 0);
            ModelManager.registerBlockModel(planksModel, 0);
            ModelManager.registerBlockModel(craftingTableModel, 0);
            ModelManager.registerBlockModel(furnaceModel, 0);
            ModelManager.registerBlockModel(bedrockModel, 0);
            ModelManager.registerBlockModel(rareModel, 0);
            ModelManager.registerBlockModel(rareModelUnlit, 1);
            ModelManager.registerBlockModel(glassModel, 0);

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

                var playerPos = new BlockPos(-100 + (float)r.NextDouble() * 200, 0,
                    -100 + (float)r.NextDouble() * 200);

                world = new World(0);
                world.generateChunk(playerPos, true);

                player = new EntityPlayerSP(new Vector3(playerPos.x, world.getHeightAtPos(playerPos.x, playerPos.z),
                    playerPos.z));

                world.addEntity(player);

                player.setItemStackInHotbar(0, new ItemStack(new ItemBlock(EnumBlock.CRAFTING_TABLE)));
                player.setItemStackInHotbar(1, new ItemStack(new ItemBlock(EnumBlock.FURNACE)));
                player.setItemStackInHotbar(2, new ItemStack(new ItemBlock(EnumBlock.COBBLESTONE)));
                player.setItemStackInHotbar(3, new ItemStack(new ItemBlock(EnumBlock.PLANKS)));
                player.setItemStackInHotbar(4, new ItemStack(new ItemBlock(EnumBlock.GLASS)));
            }
            else
            {
                world = loadedWorld;
            }

            resetMouse();

            var state = OpenTK.Input.Mouse.GetState();
            mouseLast = new Point(state.X, state.Y);

            gameRenderer.setCamera(player.camera);

            runUpdateThreads();

            ShaderManager.updateProjectionMatrix();

            world.setBlock(new BlockPos(player.pos), EnumBlock.RARE, 1, true);
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

                        Thread.Sleep(5);
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
                openGuiScreen(new GuiScreenIngameMenu());

            var wheelValue = Mouse.WheelPrecise;

            if (player != null && guiScreen == null)
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
                            ThreadPool.RunTask(false, () =>
                            {
                                world.generateChunk(pos, true);

                                //WorldRegionManager.saveChunk(world.getChunkFromPos(pos));
                            });

                            Console.WriteLine("generated a chunk!");
                        }
                        else if (!world.doesChunkHaveModel(pos))
                        {
                            ThreadPool.RunTask(false, () => world.updateModelForChunk(pos));
                        }

                        Thread.Sleep(5);
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
            if (guiScreen == null)
            {
                closeGuiScreen();
                return;
            }

            this.guiScreen = guiScreen;

            var middle = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
            middle = PointToScreen(middle);

            OpenTK.Input.Mouse.SetPosition(middle.X, middle.Y);
        }

        public float getRenderPartialTicks()
        {
            return (float)timer.Elapsed.TotalMilliseconds / 50f;
        }

        private void getMouseOverObject()
        {
            int radius = 5;

            MouseOverObject final = new MouseOverObject();

            float dist = float.MaxValue;

            var camPos = Vector3.One * 0.5f + player.camera.pos;

            for (int z = -radius; z <= radius; z++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        var vec = new Vector3(x, y, z) + camPos;
                        float f = (vec - player.camera.pos).LengthFast;

                        if (f <= radius)
                        {
                            var pos = new BlockPos(vec);
                            var block = world.getBlock(pos);

                            if (block != EnumBlock.AIR)
                            {
                                var model = ModelManager.getModelForBlock(block, world.getMetadata(pos));
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
                                    
                                    var p = new BlockPos(hitPos - normal * 0.5f);

                                    var l = Math.Abs((player.camera.pos - (p.vector + Vector3.One * 0.5f)).Length);

                                    if (l < dist)
                                    {
                                        dist = l;

                                        final = new MouseOverObject()
                                        {
                                            hit = block,
                                            hitVec = hitPos,
                                            blockPos = p,
                                            normal = normal,
                                            sideHit = sideHit
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
            }

            mouseOverObject = final;
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
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Key.Escape))
            {
                if (guiScreen is GuiScreenMainMenu)
                    return;

                if (guiScreen != null)
                    closeGuiScreen();
                else
                {
                    openGuiScreen(new GuiScreenIngameMenu());

                    ThreadPool.RunTask(false, () => { WorldLoader.saveWorld(world); });
                }
            }

            if (guiScreen == null)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (state.IsKeyDown(Key.Number1 + i))
                    {
                        player?.setSelectedSlot(i);

                        break;
                    }
                }
            }

            if (state.IsKeyDown(Key.LAlt | Key.F4))
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
                                player.setItemStackInSelectedSlot(new ItemStack(new ItemBlock(clickedBlock), 1,
                                    world.getMetadata(pos)));
                            }
                        }

                        //place/interact
                        if (e.Button == MouseButton.Right)
                        {
                            var block = world.getBlock(pos);
                            var model = ModelManager.getModelForBlock(block, world.getMetadata(pos));

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
                            else if (player.getEquippedItemStack()?.Item is ItemBlock itemBlock)
                            {
                                pos = pos.offset(mouseOverObject.sideHit);

                                var blockAtPos = world.getBlock(pos);

                                var heldBlock = itemBlock.getBlock();
                                var blockBB = ModelManager.getModelForBlock(heldBlock, world.getMetadata(pos))
                                    .boundingBox.offset(pos.vector);

                                if (blockAtPos == EnumBlock.AIR && world.getIntersectingEntitiesBBs(blockBB).Count == 0)
                                {
                                    var posUnder = pos.offset(EnumFacing.DOWN);

                                    var blockUnder = world.getBlock(posUnder);
                                    var blockAbove = world.getBlock(pos.offset(EnumFacing.UP));

                                    if (blockUnder == EnumBlock.GRASS)
                                        world.setBlock(posUnder, EnumBlock.DIRT, 0, false);
                                    if (blockAbove != EnumBlock.AIR && heldBlock == EnumBlock.GRASS)
                                        world.setBlock(pos, EnumBlock.DIRT, 0, true);
                                    else
                                        world.setBlock(pos, heldBlock, player.getEquippedItemStack().Meta, true);
                                }
                            }
                        }

                        //break
                        if (e.Button == MouseButton.Left)
                            world.setBlock(pos, EnumBlock.AIR, 0, true);
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
            GL.Ortho(0, ClientRectangle.Width, ClientRectangle.Height, 0, gameRenderer.NEAR_PLANE,
                gameRenderer.FAR_PLANE);

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

            openGuiScreen(new GuiScreen());

            WorldLoader.saveWorld(world);
            SettingsManager.save();
        }

        /*
        // false if fully outside, true if inside or intersects
        bool boxInFrustum(AxisAlignedBB box)
        {
            // check box outside/inside of frustum
            for( int i=0; i<6; i++ )
            {
                int j = 0;

                    j += ((Vector4.Dot( fru.mPlane[i], new Vector4(box.min.X, box.min.Y, box.min.Z, 1.0f)) < 0.0 )?1:0);
                    j += ((Vector4.Dot( fru.mPlane[i], new Vector4(box.max.X, box.min.Y, box.min.Z, 1.0f) ) < 0.0 )?1:0);
                    j += ((Vector4.Dot( fru.mPlane[i], new Vector4(box.min.X, box.max.Y, box.min.Z, 1.0f) ) < 0.0 )?1:0);
                    j += ((Vector4.Dot( fru.mPlane[i], new Vector4(box.max.X, box.max.Y, box.min.Z, 1.0f) ) < 0.0 )?1:0);
                    j += ((Vector4.Dot( fru.mPlane[i], new Vector4(box.min.X, box.min.Y, box.max.Z, 1.0f) ) < 0.0 )?1:0);
                    j += ((Vector4.Dot( fru.mPlane[i], new Vector4(box.max.X, box.min.Y, box.max.Z, 1.0f) ) < 0.0 )?1:0);
                    j += ((Vector4.Dot( fru.mPlane[i], new Vector4(box.min.X, box.max.Y, box.max.Z, 1.0f) ) < 0.0 )?1:0);
                    j += ((Vector4.Dot( fru.mPlane[i], new Vector4(box.max.X, box.max.Y, box.max.Z, 1.0f) ) < 0.0 )?1:0);
                if( j==8 ) return false;
            }

            // check frustum outside/inside box
            int k;
                k=0; for( int i=0; i<8; i++ ) k += ((fru.mPoints[i].x > box.max.X)?1:0); if( k==8 ) return false;
                k=0; for( int i=0; i<8; i++ ) k += ((fru.mPoints[i].x < box.min.X)?1:0); if( k==8 ) return false;
                k=0; for( int i=0; i<8; i++ ) k += ((fru.mPoints[i].y > box.max.Y)?1:0); if( k==8 ) return false;
                k=0; for( int i=0; i<8; i++ ) k += ((fru.mPoints[i].y < box.min.Y)?1:0); if( k==8 ) return false;
                k=0; for( int i=0; i<8; i++ ) k += ((fru.mPoints[i].z > box.max.Z)?1:0); if( k==8 ) return false;
                k=0; for( int i=0; i<8; i++ ) k += ((fru.mPoints[i].z < box.min.Z)?1:0); if( k==8 ) return false;

            return true;
        }*/
    }

    internal class SettingsManager
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