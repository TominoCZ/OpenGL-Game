
using System.Collections.Generic;

namespace OpenGL_Game
{
    class ChunkFragmentModel : IModel
    {
        public IRawModel rawModel { get; }
        public ShaderProgram shader { get; }

        public ChunkFragmentModel(ShaderProgram shader, List<RawQuad> model)
        {
            this.shader = shader;

            rawModel = GraphicsManager.loadModelToVAO(model, 3);
        }
    }
}
