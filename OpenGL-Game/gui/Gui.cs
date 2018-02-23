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
        protected static ModelLight GUI_BLOCK_LIGHT { get; }
        protected static GuiItemModel itemModel;

        static Gui()
        {
            itemModel = new GuiItemModel(new GuiItemShader("gui_item"));

            GUI_BLOCK_LIGHT = new ModelLight(new Vector3(10, 50, 10), Vector3.One);
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

        protected virtual void renderTexture(GuiShader shader, GuiTexture tex, int x, int y)
        {
            renderTexture(shader, tex, tex.scale, x, y);
        }

        protected virtual void renderTexture(GuiShader shader, GuiTexture tex, Vector2 scale, int x, int y)
        {
            shader.start();
            GL.BindVertexArray(GuiRenderer.GUIquad.vaoID);
            GL.EnableVertexAttribArray(0);
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
            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);
            shader.stop();
        }

        protected virtual void renderBlock(EnumBlock block, float scale, int x, int y)
        {
            var UVs = TextureManager.getUVsFromBlock(block);
            GraphicsManager.overrideModelUVsInVAO(itemModel.rawModel.bufferIDs[1], UVs.getUVForSide(EnumFacing.SOUTH).ToArray());

            var unit = new Vector2(1f / Game.INSTANCE.ClientSize.Width, 1f / Game.INSTANCE.ClientSize.Height);

            float width = 16;
            float height = 16;

            float scaledWidth = 16 * scale;
            float scaledHeight = 16 * scale;

            float posX = x + scaledWidth / 2;
            float posY = -y - scaledHeight / 2;

            var pos = new Vector2(posX, posY) * unit;

            var mat = MatrixHelper.createTransformationMatrix(pos * 2 - Vector2.UnitX + Vector2.UnitY, scale * new Vector2(width, height) * unit);

            itemModel.shader.start();
            itemModel.shader.loadTransformationMatrix(mat);

            GL.BindVertexArray(itemModel.rawModel.vaoID);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureManager.blockTextureAtlasID);
            GL.DrawArrays(itemModel.shader.renderType, 0, 4);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);

            GL.BindVertexArray(0);
            itemModel.shader.stop();
        }
    }
}
