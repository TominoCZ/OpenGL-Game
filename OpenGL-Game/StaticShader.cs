namespace OpenGL_Game
{
    class StaticShader : ShaderProgram
    {
        public static string FileName = "Shaders/color";

        public StaticShader() : base(FileName)
        {

        }

        protected override void bindAttributes()
        {
            bindAttributes(0, "position");
        }
    }
}
