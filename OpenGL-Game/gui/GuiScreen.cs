using System.Collections.Generic;
using OpenGL_Game.gui;
using OpenTK;

namespace OpenGL_Game
{
    class GuiScreen : Gui
    {
        private GuiTexture background;

        protected List<GuiButton> buttons = new List<GuiButton>();

        public GuiScreen()
        {
            background = new GuiTexture(GraphicsManager.loadTexture("gui/bg", false), Vector2.Zero, Vector2.One * 4);
        }

        public override void render(GuiShader shader, int mouseX, int mouseY)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].render(shader, mouseX, mouseY);
            }
            //render stuff
        }

        protected virtual void drawBackground(GuiShader shader, GuiTexture tex)
        {
            var sizeX = tex.textureSize.Width * 2f;
            var sizeY = tex.textureSize.Height * 2f;

            var countX = Game.INSTANCE.ClientSize.Width / sizeX;
            var countY = Game.INSTANCE.ClientSize.Height / sizeY;

            for (int x = 0; x <= countX; x++)
            {
                for (int y = 0; y <= countY; y++)
                {
                    renderTexture(shader, tex, tex.scale, (int)(x * sizeX), (int)(y * sizeY));
                }
            }
        }

        public virtual void onMouseClick(int x, int y)
        {
            for (int i = buttons.Count - 1; i >= 0; i--)
            {
                var btn = buttons[i];

                if (btn.isMouseOver(x, y))
                {
                    buttonClicked(btn);
                    break;
                }
            }
        }

        protected virtual void buttonClicked(GuiButton btn)
        {

        }

        public virtual void onClose()
        {
            GraphicsManager.deleteTexture(background.textureID);
        }
    }
}
