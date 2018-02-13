using OpenTK.Graphics.OpenGL;
using ClearBufferMask = OpenTK.Graphics.OpenGL.ClearBufferMask;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    class Renderer
    {
        public void prepare()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(1, 0, 0, 1);
        }

        public void render(Model model)
        {
            var rawModel = model.getRaw();

            GL.BindVertexArray(rawModel.getVaoID());

            GL.EnableVertexAttribArray(0);//enable position
            GL.EnableVertexAttribArray(1);//enable UV

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, model.getTexture().getID());

            GL.DrawElements(BeginMode.Triangles, rawModel.getVertexCount(), DrawElementsType.UnsignedInt, 0);
            GL.DisableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }
    }
}
