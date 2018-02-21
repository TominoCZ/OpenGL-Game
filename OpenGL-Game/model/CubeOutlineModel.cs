using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    class CubeOutlineModel : IModel
    {
        public IRawModel rawModel { get; }
        public ShaderProgram shader { get; }

        public CubeOutlineModel(ShaderProgram shader)
        {
            this.shader = shader;
            rawModel = GraphicsManager.loadModelToVAO(ModelManager.createCubeModel(), 3);
        }
    }
}
