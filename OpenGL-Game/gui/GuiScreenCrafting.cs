using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_Game
{
    class GuiScreenCrafting : GuiScreen
    {
        private GuiTexture gui;

        public GuiScreenCrafting()
        {
            gui = new GuiTexture(GraphicsManager.loadTexture("gui/crafting", false), Vector2.One * 0.5f);
        }

        public override void render(GuiShader shader, int mouseX, int mouseY)
        {
            renderTexture(shader, gui, 0, 0);
        }

        public override void onClose()
        {
            GraphicsManager.deleteTexture(gui.textureID);
        }
    }
}
