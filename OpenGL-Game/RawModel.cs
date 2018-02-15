using System.Collections.Generic;

namespace OpenGL_Game
{
    class RawModel
    {
        public int vaoID { get; }

        private Dictionary<EnumFacing, RawQuad> quads;

        public int vertexCount;

        public RawModel(int vaoID)
        {
            this.vaoID = vaoID;

            quads = new Dictionary<EnumFacing, RawQuad>();
        }

        public void setQuadForSide(EnumFacing side, RawQuad quad)
        {
            if (quads.TryGetValue(side, out var q))
            {
                vertexCount -= q.vertices.Length / 3;
                quads.Remove(side);
            }

            quads.Add(side, quad);
            vertexCount += quad.vertices.Length / 3;
        }
    }

    enum EnumFacing
    {
        NORTH,
        SOUTH,
        EAST,
        WEST,
        UP,
        DOWN
    }
}
