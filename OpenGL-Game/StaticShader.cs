namespace OpenGL_Game
{
    class StaticShader : ShaderProgram
    {
        public StaticShader(string fileName) : base("shaders/" + fileName)
        {

        }

        protected override void bindAttributes()
        {
            bindAttributes(0, "position");
            bindAttributes(1, "textureCoords");
        }
    }
}
