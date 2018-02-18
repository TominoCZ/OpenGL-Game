using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    class GuiCrosshair : Gui
    {
        private GuiTexture crosshairTexture;

        public GuiCrosshair(GuiTexture crosshairTexture)
        {
            this.crosshairTexture = crosshairTexture;
        }

        public override void render(GuiShader shader)
        {
            renderTexture(shader, crosshairTexture);
        }
    }
}
