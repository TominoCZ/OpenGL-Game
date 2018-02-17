using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ClearBufferMask = OpenTK.Graphics.OpenGL.ClearBufferMask;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    class Renderer
    {
        private GameWindow window;

        private Camera camera;
        
        private Matrix4 projectionMatrix;

        public WorldRenderer worldRenderer;
        public EntityRenderer entityRenderer;

        public float NEAR_PLANE = 0.1f;
        public float FAR_PLANE = 100f;

        private int FOV = 70;

        public Renderer(GameWindow window, StaticShader sp, Camera camera)
        {
            this.window = window;
            this.camera = camera;

            worldRenderer = new WorldRenderer();
            entityRenderer = new EntityRenderer();

            window.Resize += (s, e) =>
            {
                createProjectionMatrix();
                sp.start();
                sp.loadProjectionMatrix(projectionMatrix);
                sp.stop();
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

            //prepare();
            var viewMatrix = MatrixHelper.createViewMatrix(camera);

            worldRenderer.render(viewMatrix);

            entityRenderer.render(partialTicks);
        }

        private void createProjectionMatrix()
        {
            projectionMatrix = new Matrix4();

            float aspectRatio = (float)window.Width / window.Height;
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
