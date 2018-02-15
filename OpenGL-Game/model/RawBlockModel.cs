using System.Collections.Generic;

namespace OpenGL_Game
{
    class RawBlockModel : IRawModel
    {
        public int vaoID { get; }
        public int vertexCount { get; private set; }

        private Dictionary<EnumFacing, RawQuad> quads;

        public RawBlockModel(int vaoID)
        {
            this.vaoID = vaoID;

            quads = new Dictionary<EnumFacing, RawQuad>();
        }

        public RawQuad getQuadForSide(EnumFacing side)
        {
            quads.TryGetValue(side, out var quad);

            return quad;
        }
    }
}
