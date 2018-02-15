using System.Collections.Generic;

namespace OpenGL_Game
{
    class RawChunkModel : IRawModel
    {
        public int vaoID { get; }
        public int vertexCount { get; }

        public Dictionary<ShaderProgram, List<RawQuad>> model;

        public RawChunkModel(int vaoID, Dictionary<ShaderProgram, List<RawQuad>> model)
        {
            this.vaoID = vaoID;
            this.model = model;

            foreach (var list in model.Values)
            {
                foreach (var quad in list)
                {
                    vertexCount += quad.vertices.Length / 3;
                }
            }
        }
    }
}