using System.Collections.Generic;

namespace OpenGL_Game
{
    class RawBlockModel : IRawModel
    {
        public int vaoID { get; }
        public int vertexCount { get; }
        public int[] bufferIDs { get; }

        public bool hasUVs { get; }
        public bool hasNormals { get; }

        private Dictionary<EnumFacing, RawQuad> quads;

        public RawBlockModel(int vaoID, Dictionary<EnumFacing, RawQuad> quads)
        {
            this.vaoID = vaoID;
            this.quads = quads;

            foreach (var value in quads)
            {
                vertexCount += value.Value.vertices.Length / 3;

                var uv = value.Value.UVs.Length > 0;
                var normal = value.Value.normal.Length > 0;

                if (uv)
                    hasUVs = true;
                if (normal)
                    hasNormals = true;
            }
        }

        public RawQuad getQuadForSide(EnumFacing side)
        {
            quads.TryGetValue(side, out var quad);

            return quad;
        }
    }
}
