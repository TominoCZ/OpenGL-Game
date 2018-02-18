namespace OpenGL_Game
{
    class GuiScreen : Gui
    {
        public GuiScreen()
        {
            
        }

        public override void render(GuiShader shader)
        {
            drawBackground();

            //render stuff
        }

        protected virtual void drawBackground()
        {

        }

        public virtual void onClose()
        {

        }
    }
}
