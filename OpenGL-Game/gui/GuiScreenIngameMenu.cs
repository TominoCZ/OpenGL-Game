using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Game.gui;
using OpenTK;

namespace OpenGL_Game
{
    class GuiScreenIngameMenu : GuiScreen
    {
        private GuiTexture background;

        public GuiScreenIngameMenu()
        {
            background = new GuiTexture(GraphicsManager.loadTexture("gui/bg_transparent", false), Vector2.Zero, Vector2.One * 4);
        }

        public override void render(GuiShader shader, int mouseX, int mouseY)
        {
            drawBackground(shader, background);

            base.render(shader, mouseX, mouseY);
        }

        public override void onClose()
        {
            GraphicsManager.deleteTexture(background.textureID);
        }
    }
}
