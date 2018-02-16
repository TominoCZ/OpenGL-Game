using System.Collections.Generic;

namespace OpenGL_Game
{
    class RawBlockModel : IRawModel
    {
        public int vaoID { get; }
        public int vertexCount { get; }

        private Dictionary<EnumFacing, RawQuad> quads;

        public RawBlockModel(int vaoID, Dictionary<EnumFacing, RawQuad> quads)
        {
            this.vaoID = vaoID;
            this.quads = quads;

            foreach (var value in quads)
            {
                vertexCount += value.Value.vertices.Length / 3;
            }
        }

        public RawQuad getQuadForSide(EnumFacing side)
        {
            quads.TryGetValue(side, out var quad);

            return quad;
        }
    }
}
