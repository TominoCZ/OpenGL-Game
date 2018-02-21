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

        public virtual void render(GuiShader shader, int mouseX, int mouseY)
        {

        }

        protected virtual void renderTexture(GuiShader shader, GuiTexture tex)
        {
            var ratio = Vector2.UnitX * ((float)tex.textureSize.Width / Game.INSTANCE.ClientSize.Width) + Vector2.UnitY * ((float)tex.textureSize.Height / Game.INSTANCE.ClientSize.Height);

            var mat = MatrixHelper.createTransformationMatrix(tex.pos, tex.scale * ratio);
            shader.loadTransformationMatrix(mat);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex.textureID);
            GL.DrawArrays(shader.renderType, 0, 4);
        }

        protected virtual void renderTexture(GuiShader shader, GuiTexture tex, int x, int y)
        {
            var ratio = Vector2.UnitX * ((float)tex.textureSize.Width / Game.INSTANCE.ClientSize.Width) + Vector2.UnitY * ((float)tex.textureSize.Height / Game.INSTANCE.ClientSize.Height);

            var posX = (x + tex.textureSize.Width) / (float)tex.textureSize.Width;
            var posY = (-y - tex.textureSize.Height) / (float)tex.textureSize.Height;

            var pos = (tex.scale * new Vector2(posX, posY) * ratio - Vector2.UnitX + Vector2.UnitY);

            var mat = MatrixHelper.createTransformationMatrix(pos, tex.scale * ratio);
            shader.loadTransformationMatrix(mat);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex.textureID);
            GL.DrawArrays(shader.renderType, 0, 4);
        }

        protected virtual void renderTexture(GuiShader shader, GuiTexture tex, Vector2 scale, int x, int y)
        {
            var ratio = Vector2.UnitX * ((float)tex.textureSize.Width / Game.INSTANCE.ClientSize.Width) + Vector2.UnitY * ((float)tex.textureSize.Height / Game.INSTANCE.ClientSize.Height);

            var posX = (x + tex.textureSize.Width) / (float)tex.textureSize.Width;
            var posY = (-y - tex.textureSize.Height) / (float)tex.textureSize.Height;

            var pos = (scale * new Vector2(posX, posY) * ratio - Vector2.UnitX + Vector2.UnitY);

            var mat = MatrixHelper.createTransformationMatrix(pos, scale * ratio);
            shader.loadTransformationMatrix(mat);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex.textureID);
            GL.DrawArrays(shader.renderType, 0, 4);
        }

        protected virtual void renderTextureCentered(GuiShader shader, GuiTexture tex, Vector2 scale, int y)
        {
            var ratio = Vector2.UnitX * ((float)tex.textureSize.Width / Game.INSTANCE.ClientSize.Width) + Vector2.UnitY * ((float)tex.textureSize.Height / Game.INSTANCE.ClientSize.Height);

            var posY = ((-y - tex.textureSize.Height) * scale.Y) / tex.textureSize.Height;

            var pos = new Vector2(0, posY) * ratio.Y + Vector2.UnitY;

            var mat = MatrixHelper.createTransformationMatrix(pos, scale * ratio);
            shader.loadTransformationMatrix(mat);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex.textureID);
            GL.DrawArrays(shader.renderType, 0, 4);
        }
    }
}
