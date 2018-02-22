using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class GuiItemShader : ShaderProgram
    {
        private int loc_transformationMatrix;

        public GuiItemShader(string shaderName) : base(shaderName, PrimitiveType.Quads)
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
        }

        public override void loadTransformationMatrix(Matrix4 mat)
        {
            loadMatrix(loc_transformationMatrix, mat);
        }

        public override void loadProjectionMatrix(Matrix4 mat)
        {

        }

        public override void loadViewMatrix(Matrix4 mat)
        {

        }

        public override void loadLight(ModelLight light)
        {

        }
    }
}
