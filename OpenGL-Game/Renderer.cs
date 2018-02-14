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
        public float FAR_PLANE = 1000f;

        private int FOV = 65;

        private Matrix4 projectionMatrix;
        
        public BlockRenderer blockRenderer;

        private Camera camera;

        public Renderer(GameWindow window, ShaderProgram sp, Camera camera)
        {
            this.window = window;
            this.camera = camera;

            blockRenderer = new BlockRenderer();

            createProjectionMatrix();
            sp.start();
            sp.loadProjectionMatrix(projectionMatrix);
            sp.stop();

            prepare();
        }

        public void prepare()
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            GL.Enable(EnableCap.DepthTest);
        }

        public void render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.ClearColor(0, 0, 0, 0);

            blockRenderer.render(camera);
            /*
            //prepare for rendering
            GL.BindVertexArray(e.blockModel.rawModel.vaoID);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, e.blockModel.texture.textureID);

            //render
            var mat = MatrixHelper.createTransformationMatrix(e.pos, e.rx, e.ry, e.rz, e.scale);
            shader.loadTransformationMatrix(mat);

            GL.DrawElements(BeginMode.Triangles, e.blockModel.rawModel.vertices, DrawElementsType.UnsignedInt, 0);

            //blockModel 2
            mat = MatrixHelper.createTransformationMatrix(e.pos, e.rx, e.ry, e.rz, e.scale);
            shader.loadTransformationMatrix(mat);
            
            GL.DrawElements(BeginMode.Triangles, e.blockModel.rawModel.vertices, DrawElementsType.UnsignedInt, 0);

            //cleanup
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);

            GL.BindVertexArray(0);*/
        }

        private void createProjectionMatrix()
        {
            //works for fullscreen
            float aspectRatio = window.ClientSize.Width / window.ClientSize.Height;

            float y_scale = (float)(1f / Math.Tan(MathHelper.DegreesToRadians(FOV / 2f))) * aspectRatio;
            float x_scale = y_scale / aspectRatio;
            float frustrum_length = FAR_PLANE - NEAR_PLANE;

            projectionMatrix = new Matrix4();
            projectionMatrix.M11 = x_scale;
            projectionMatrix.M22 = y_scale;
            projectionMatrix.M33 = -((FAR_PLANE + NEAR_PLANE) / frustrum_length);
            projectionMatrix.M34 = -1;
            projectionMatrix.M43 = -((2 * FAR_PLANE * NEAR_PLANE) / frustrum_length);
            projectionMatrix.M44 = 0;
        }
    }
}
