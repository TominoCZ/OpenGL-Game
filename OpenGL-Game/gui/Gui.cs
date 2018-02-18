using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class Gui
    {
        protected Gui()
        {

        }

        public virtual void render(GuiShader shader)
        {

        }

        protected virtual void renderTexture(GuiShader shader, GuiTexture tex)
        {
            var ratio = Vector2.UnitX * ((float)tex.textureSize.Width / Game.INSTANCE.ClientSize.Width) + Vector2.UnitY * ((float)tex.textureSize.Height / Game.INSTANCE.ClientSize.Height);

            var mat = MatrixHelper.createTransformationMatrix(tex.pos, tex.scale * ratio);
            shader.loadTransformationMatrix(mat);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex.textureID);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }
    }
}
