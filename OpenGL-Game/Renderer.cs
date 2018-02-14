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

        public float NEAR_PLANE = 0.1f;
        public float FAR_PLANE = 100f;

        private int FOV = 100;

        private Matrix4 projectionMatrix;

        public BlockRenderer blockRenderer;

        private Camera camera;

        public Renderer(GameWindow window, StaticShader sp, Camera camera)
        {
            this.window = window;
            this.camera = camera;

            blockRenderer = new BlockRenderer();

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

        public void render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            blockRenderer.render(camera);
        }

        private void createProjectionMatrix()
        {
            //works for fullscreen
            float aspectRatio = (float)window.Width / window.Height;

            float y_scale = (float)(1f / Math.Tan(MathHelper.DegreesToRadians(FOV / 2f)) * aspectRatio);
            float x_scale = y_scale / aspectRatio;
            float frustrum_length = FAR_PLANE - NEAR_PLANE;

            projectionMatrix = new Matrix4();
            projectionMatrix.Diagonal = new Vector4(x_scale, y_scale, -((FAR_PLANE + NEAR_PLANE) / frustrum_length), 0);

            projectionMatrix.M34 = -1;
            projectionMatrix.M43 = -((2 * FAR_PLANE * NEAR_PLANE) / frustrum_length);
        }
    }
}
