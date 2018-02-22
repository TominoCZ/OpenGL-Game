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
            var ratio = new Vector2((float)tex.textureSize.Width / Game.INSTANCE.ClientSize.Width, (float)tex.textureSize.Height / Game.INSTANCE.ClientSize.Height);

            var mat = MatrixHelper.createTransformationMatrix(tex.pos * 2, tex.scale * ratio);
            shader.loadTransformationMatrix(mat);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex.textureID);
            GL.DrawArrays(shader.renderType, 0, 4);
        }

        protected virtual void renderTexture(GuiShader shader, GuiTexture tex, float x, float y)
        {
            renderTexture(shader, tex, tex.scale, x, y);
        }

        protected virtual void renderTexture(GuiShader shader, GuiTexture tex, Vector2 scale, float x, float y)
        {
            var unit = new Vector2(1f / Game.INSTANCE.ClientSize.Width, 1f / Game.INSTANCE.ClientSize.Height);

            float width = tex.textureSize.Width;
            float height = tex.textureSize.Height;

            float scaledWidth = width * scale.X;
            float scaledHeight = height * scale.Y;

            float posX = x + scaledWidth / 2;
            float posY = -y - scaledHeight / 2;

            var pos = new Vector2(posX, posY) * unit;

            var mat = MatrixHelper.createTransformationMatrix(pos * 2 - Vector2.UnitX + Vector2.UnitY, scale * new Vector2(width, height) * unit);
            shader.loadTransformationMatrix(mat);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex.textureID);
            GL.DrawArrays(shader.renderType, 0, 4);
        }
    }
}
