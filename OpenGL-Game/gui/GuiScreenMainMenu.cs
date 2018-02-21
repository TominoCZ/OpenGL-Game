using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Game;
using OpenGL_Game.gui;
using OpenTK;

namespace OpenGL_Game
{
    class GuiScreenMainMenu : GuiScreen
    {
        private GuiTexture background;

        public GuiScreenMainMenu()
        {
            buttons.Add(new GuiButton(0, 0, 200, Vector2.One * 2) { centered = true });
            background = new GuiTexture(GraphicsManager.loadTexture("gui/bg", false), Vector2.Zero, Vector2.One * 8);
        }

        public override void render(GuiShader shader, int mouseX, int mouseY)
        {
            drawBackground(shader, background);

            base.render(shader, mouseX, mouseY);
        }

        protected override void buttonClicked(GuiButton btn)
        {
            switch (btn.ID)
            {
                case 0:
                    Game.INSTANCE.closeGuiScreen();
                    Game.INSTANCE.startGame();
                    break;
            }
        }

        public override void onClose()
        {
            GraphicsManager.deleteTexture(background.textureID);
        }
    }
}
