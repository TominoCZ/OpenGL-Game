using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    class RawModel : IRawModel
    {
        public int vaoID { get; }

        public int vertexCount { get; }

        public List<RawQuad> quads;

        public RawModel(int vaoID, int valuesPerVertice, List<RawQuad> quads)
        {
            this.vaoID = vaoID;
            this.quads = quads;

            foreach (var quad in quads)
            {
                vertexCount += quad.vertices.Length / valuesPerVertice;
            }
        }

        public bool hasNormals()
        {
            return quads.Count > 0 && quads[0].normal.Length > 0;
        }

        public bool hasUVs()
        {
            return quads.Count > 0 && quads[0].UVs.Length > 0;
        }
    }
}
