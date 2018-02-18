using OpenTK;

namespace OpenGL_Game
{
    class GuiShader : ShaderProgram
    {
        private int loc_transformationMatrix;

        public GuiShader(string shaderName) : base(shaderName)
        {

        }

        protected override void bindAttributes()
        {
            bindAttributes(0, "position");
        }

        protected override void getAllUniformLocations()
        {
            loc_transformationMatrix = getUniformLocation("transformationMatrix");
        }

        public override void loadTransformationMatrix(Matrix4 mat)
        {
            loadMatrix(loc_transformationMatrix, mat);
        }

        public override void loadViewMatrix(Matrix4 mat)
        {
            
        }

        public override void loadLight(ModelLight light)
        {

        }
    }
}
