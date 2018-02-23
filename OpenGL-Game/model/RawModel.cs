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

        public bool hasUVs { get; }

        public bool hasNormals { get; }

        public int[] bufferIDs { get; }

        public List<RawQuad> quads;

        public RawModel(int vaoID, int[] bufferIDs, int valuesPerVertice, List<RawQuad> quads)
        {
            this.vaoID = vaoID;
            this.quads = quads;

            this.bufferIDs = bufferIDs;

            foreach (var quad in quads)
            {
                vertexCount += quad.vertices.Length / valuesPerVertice;

                var uv = quads[0].UVs.Length > 0;
                var normal = quads[0].normal.Length > 0;

                if (uv)
                    hasUVs = true;
                if (normal)
                    hasNormals = true;
            }
        }
    }
}
