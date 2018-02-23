using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ClearBufferMask = OpenTK.Graphics.OpenGL.ClearBufferMask;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    class GameRenderer
    {
        private Camera camera;

        public WorldRenderer worldRenderer;
        public EntityRenderer entityRenderer;
        public GuiRenderer guiRenderer;

        public float NEAR_PLANE = 0.1f;
        public float FAR_PLANE = 1000f;

        public float FOV = 65;

        public GameRenderer(Camera camera)
        {
            this.camera = camera;

            worldRenderer = new WorldRenderer();
            entityRenderer = new EntityRenderer();
            guiRenderer = new GuiRenderer();

            prepare();
        }

        public GameRenderer() : this(null)
        {

        }

        public void prepare()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        public void render(float partialTicks)
        {
            if (camera == null)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            prepare();

            var viewMatrix = MatrixHelper.createViewMatrix(camera);

            if (Game.INSTANCE.world != null)
            {
                worldRenderer.render(viewMatrix);
                entityRenderer.render(partialTicks);
            }
            
            //render other gui
            if (Game.INSTANCE.player != null)
            {
                guiRenderer.renderCrosshair();
                guiRenderer.renderHUD();
            }

            //render gui screen
            if (Game.INSTANCE.guiScreen != null)
            {
                Game.INSTANCE.CursorVisible = true;
                guiRenderer.render(Game.INSTANCE.guiScreen);
            }
        }

        public void setCamera(Camera camera)
        {
            this.camera = camera;
        }

        public Matrix4 createProjectionMatrix()
        {
            var matrix = new Matrix4();

            float aspectRatio = (float)Game.INSTANCE.Width / Game.INSTANCE.Height;
            float y_scale = (float)(1f / Math.Tan(MathHelper.DegreesToRadians(FOV / 2f)));
            float x_scale = y_scale / aspectRatio;
            float frustum_length = FAR_PLANE - NEAR_PLANE;

            matrix.M11 = x_scale;
            matrix.M22 = y_scale;
            matrix.M33 = -((FAR_PLANE + NEAR_PLANE) / frustum_length);
            matrix.M34 = -1;
            matrix.M43 = -((2 * NEAR_PLANE * FAR_PLANE) / frustum_length);
            matrix.M44 = 0;

            return matrix;
        }
    }
}
