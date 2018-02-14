using OpenTK;

namespace OpenGL_Game
{
    class StaticShader : ShaderProgram
    {
        private int loc_transformationMatrix;
        private int loc_projectionMatrix;
        private int loc_viewMatrix;

        public StaticShader(string shaderName) : base(shaderName)
        {

        }

        protected override void bindAttributes()
        {
            bindAttributes(0, "position");
            bindAttributes(1, "textureCoords");
        }

        protected override void getAllUniformLocations()
        {
            loc_transformationMatrix = getUniformLocation("transformationMatrix");
            loc_projectionMatrix = getUniformLocation("projectionMatrix");
            loc_viewMatrix = getUniformLocation("viewMatrix");
        }

        public override void loadTransformationMatrix(Matrix4 mat)
        {
            loadMatrix4(loc_transformationMatrix, mat);
        }

        public override void loadProjectionMatrix(Matrix4 mat)
        {
            loadMatrix4(loc_projectionMatrix, mat);
        }

        public override void loadViewMatrix(Camera c)
        {
            var mat = MatrixHelper.createViewMatrix(c);
            loadMatrix4(loc_viewMatrix, mat);
        }
    }
}
