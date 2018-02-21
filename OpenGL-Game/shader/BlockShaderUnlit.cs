using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class BlockShaderUnlit : ShaderProgram
    {
        private int loc_transformationMatrix;
        private int loc_projectionMatrix;
        private int loc_viewMatrix;

        public BlockShaderUnlit(string shaderName, PrimitiveType renderType) : base(shaderName, renderType)
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
            loadMatrix(loc_transformationMatrix, mat);
        }

        public override void loadProjectionMatrix(Matrix4 mat)
        {
            loadMatrix(loc_projectionMatrix, mat);
        }

        public override void loadViewMatrix(Matrix4 mat)
        {
            loadMatrix(loc_viewMatrix, mat);
        }

        public override void loadLight(ModelLight light)
        {

        }
    }
}
