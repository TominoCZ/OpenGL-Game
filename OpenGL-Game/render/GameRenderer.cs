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

        private Matrix4 projectionMatrix;

        public WorldRenderer worldRenderer;
        public EntityRenderer entityRenderer;
        public GuiRenderer guiRenderer;

        public float NEAR_PLANE = 0.1f;
        public float FAR_PLANE = 100f;

        private int FOV = 65;

        public GameRenderer(Camera camera)
        {
            this.camera = camera;

            worldRenderer = new WorldRenderer();
            entityRenderer = new EntityRenderer();
            guiRenderer = new GuiRenderer();

            Game.INSTANCE.Resize += (s, e) =>
            {
                var shaders = ModelManager.getAllRegisteredShaders();
                createProjectionMatrix();

                foreach (var shader in shaders)
                {
                    shader.start();
                    if (shader is BlockShader bShader)
                        bShader.loadProjectionMatrix(projectionMatrix);
                    shader.stop();
                }
            };

            prepare();
        }

        public void prepare()
        {
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        public void render(float partialTicks)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            prepare();

            var viewMatrix = MatrixHelper.createViewMatrix(camera);

            worldRenderer.render(viewMatrix);
            entityRenderer.render(partialTicks);

            //render other gui
            guiRenderer.renderCrosshair();
            //guiRenderer.renderHUD(); //TODO

            //render gui screen
            if (Game.INSTANCE.guiScreen != null)
            {
                Game.INSTANCE.CursorVisible = true;
                guiRenderer.render(Game.INSTANCE.guiScreen);
            }
        }

        private void createProjectionMatrix()
        {
            projectionMatrix = new Matrix4();

            float aspectRatio = (float)Game.INSTANCE.Width / Game.INSTANCE.Height;
            float y_scale = (float)(1f / Math.Tan(MathHelper.DegreesToRadians(FOV / 2f)));
            float x_scale = y_scale / aspectRatio;
            float frustum_length = FAR_PLANE - NEAR_PLANE;

            projectionMatrix.M11 = x_scale;
            projectionMatrix.M22 = y_scale;
            projectionMatrix.M33 = -((FAR_PLANE + NEAR_PLANE) / frustum_length);
            projectionMatrix.M34 = -1;
            projectionMatrix.M43 = -((2 * NEAR_PLANE * FAR_PLANE) / frustum_length);
            projectionMatrix.M44 = 0;
        }
    }
}
