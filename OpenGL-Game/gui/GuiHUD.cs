using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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
                slot = new GuiTexture(slot_texture.textureID, slot_texture.textureSize, Vector2.Zero, Vector2.One * 2);
            if (slot_selected_texture != null)
                slot_selected = new GuiTexture(slot_selected_texture.textureID, slot_selected_texture.textureSize, Vector2.Zero, Vector2.One * 2);
        }

        public override void render(GuiShader shader, int mouseX, int mouseY)
        {
            var size = Game.INSTANCE.ClientSize;

            int space = 5;

            int scaledWidth = (int)(slot.textureSize.Width * slot.scale.X);
            int scaledHeight = (int)(slot.textureSize.Height * slot.scale.Y);

            int totalHotbarWidth = 9 * scaledWidth + 8 * space;

            int startPos = size.Width / 2 - totalHotbarWidth / 2;

            for (int i = 0; i < 9; i++)
            {
                var b = i == Game.INSTANCE.player.equippedItemHotbarIndex;

                renderTexture(shader, b ? slot_selected : slot, startPos + i * (scaledWidth + space), size.Height - 20 - scaledHeight);

                var item = Game.INSTANCE.player.getEquippedItem();

                if (item != null && item is ItemBlock itemBlock)
                {
                    //TODO render preview
                }
            }
        }
    }
}
