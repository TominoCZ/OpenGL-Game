using System.Collections.Generic;

namespace OpenGL_Game
{
    class RawChunkModel : IRawModel
    {
        public int vaoID { get; }
        public int vertexCount { get; }

        public List<RawQuad> model;

        public RawChunkModel(int vaoID, List<RawQuad> model)
        {
            this.vaoID = vaoID;
            this.model = model;

            foreach (var quad in model)
            {
                vertexCount += quad.vertices.Length / 3;
            }
        }
    }
}