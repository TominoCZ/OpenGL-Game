using System.Collections.Generic;
using OpenGL_Game.gui;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class GuiRenderer
    {
        private RawModel quad;

        private GuiShader shader;

        private GuiCrosshair crosshairGui;
        private GuiHUD hudGui;

        public GuiRenderer()
        {
            var rawQuad = new RawQuad(new float[] {
                -1,  1,
                -1, -1,
                 1, 1,
                 1, -1 });

            quad = GraphicsManager.loadModelToVAO(new List<RawQuad> { rawQuad }, 2);

            shader = new GuiShader("gui");

            var texture = GraphicsManager.loadTexture("gui/cross");

            if (texture != null)
            {
                var tex = new GuiTexture(texture.textureID, texture.textureSize, Vector2.Zero, Vector2.One * 1.4f);
                crosshairGui = new GuiCrosshair(tex);
            }

            hudGui = new GuiHUD();
        }

        public void render(Gui gui)
        {
            if (gui == null)
                return;

            shader.start();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.DepthTest);

            GL.BindVertexArray(quad.vaoID);
            GL.EnableVertexAttribArray(0);

            gui.render(shader);

            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.DepthTest);

            shader.stop();
        }

        public void renderCrosshair()
        {
            render(crosshairGui);
        }

        public void renderHUD()
        {
            render(hudGui);
        }

        public void cleanUp()
        {
            shader.stop();
            shader.cleanUp();
        }
    }
}
