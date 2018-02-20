using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_Game.gui
{
    class GuiHUD : Gui
    {
        private GuiTexture slot;
        private GuiTexture slot_selected;

        public GuiHUD()
        {
            var slot_texture = GraphicsManager.loadTexture("gui/slot", false);
            var slot_selected_texture = GraphicsManager.loadTexture("gui/slot_selected", false);

            if (slot_texture != null)
                slot = new GuiTexture(slot_texture.textureID, slot_texture.textureSize, Vector2.Zero);
            if (slot_selected_texture != null)
                slot_selected = new GuiTexture(slot_selected_texture.textureID, slot_selected_texture.textureSize, Vector2.UnitX * 1 / 16f);
        }

        public override void render(GuiShader shader, int mouseX, int mouseY)
        {
            renderTexture(shader, slot);
            renderTexture(shader, slot_selected);
        }
    }
}
