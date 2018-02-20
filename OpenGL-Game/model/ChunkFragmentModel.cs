
using System.Collections.Generic;

namespace OpenGL_Game
{
    class ChunkFragmentModel : IModel
    {
        public IRawModel rawModel { get; private set; }
        public ShaderProgram shader { get; }

        public bool hasUVs { get; }

        public bool hasNormals { get; }

        public ChunkFragmentModel(ShaderProgram shader, List<RawQuad> quads)
        {
            this.shader = shader;

            rawModel = GraphicsManager.loadModelToVAO(quads, 3);

            hasUVs = quads.Count > 0 && quads[0].UVs.Length > 0;
            hasNormals = quads.Count > 0 && quads[0].UVs.Length > 0;
        }

        public void overrideData(List<RawQuad> quads)
        {
            rawModel = GraphicsManager.overrideModelInVAO(rawModel.vaoID, quads, 3);
        }
    }
}
