using OpenTK;

namespace OpenGL_Game
{
    class StaticShader : ShaderProgram
    {
        private int loc_transformationMatrix;
        private int loc_projectionMatrix;
        private int loc_viewMatrix;
        private int loc_lightPosition;
        private int loc_lightColor;

        public StaticShader(string shaderName) : base(shaderName)
        {

        }

        protected override void bindAttributes()
        {
            bindAttributes(0, "position");
            bindAttributes(1, "textureCoords");
            bindAttributes(2, "normal");
        }

        protected override void getAllUniformLocations()
        {
            loc_transformationMatrix = getUniformLocation("transformationMatrix");
            loc_projectionMatrix = getUniformLocation("projectionMatrix");
            loc_viewMatrix = getUniformLocation("viewMatrix");
            loc_lightPosition = getUniformLocation("lightPosition");
            loc_lightColor = getUniformLocation("lightColor");
        }

        public override void loadTransformationMatrix(Matrix4 mat)
        {
            loadMatrix4(loc_transformationMatrix, mat);
        }

        public void loadProjectionMatrix(Matrix4 mat)
        {
            loadMatrix4(loc_projectionMatrix, mat);
        }

        public override void loadViewMatrix(Matrix4 mat)
        {
            loadMatrix4(loc_viewMatrix, mat);
        }

        public override void loadLight(ModelLight light)
        {
            loadVector(loc_lightPosition, light.pos);
            loadVector(loc_lightColor, light.color);
        }
    }
}
