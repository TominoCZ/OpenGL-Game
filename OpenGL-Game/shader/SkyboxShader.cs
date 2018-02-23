using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game.shader
{
    class SkyboxShader : ShaderProgram
    {
        private int loc_projectionMatrix;
        private int loc_viewMatrix;

        public SkyboxShader() : base("skybox", PrimitiveType.Triangles)
        {

        }

        protected override void bindAttributes()
        {
            bindAttributes(0, "position");
        }

        protected override void getAllUniformLocations()
        {
            loc_projectionMatrix = getUniformLocation("projectionMatrix");
            loc_viewMatrix = getUniformLocation("viewMatrix");
        }

        public override void loadTransformationMatrix(Matrix4 mat)
        {
        }

        public override void loadProjectionMatrix(Matrix4 mat)
        {
            loadMatrix(loc_projectionMatrix, mat);
        }

        public override void loadViewMatrix(Matrix4 mat)
        {
            var m = mat;

            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;

            loadMatrix(loc_viewMatrix, m);
        }

        public override void loadLight(ModelLight light)
        {

        }
    }
}
