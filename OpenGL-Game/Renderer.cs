using OpenTK.Graphics.OpenGL;
using ClearBufferMask = OpenTK.Graphics.OpenGL.ClearBufferMask;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    class Renderer
    {
        public void prepare()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(1, 0, 0, 1);
        }

        public void render(Model model)
        {
            render(model.getRaw());
        }

        private void render(RawModel model)
        {
            GL.BindVertexArray(model.getVaoID());
            GL.EnableVertexAttribArray(0);
            GL.DrawElements(BeginMode.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);
            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }
    }
}
