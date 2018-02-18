using System;
using System.Collections.Generic;
using OpenTK;

namespace OpenGL_Game
{
    class BlockTextureUV
    {
        private Dictionary<EnumFacing, TextureUVNode> UVs;

        public BlockTextureUV()
        {
            UVs = new Dictionary<EnumFacing, TextureUVNode>();
        }

        public void setUVForSide(EnumFacing side, Vector2 from, Vector2 to)
        {
            if (UVs.ContainsKey(side))
                UVs.Remove(side);

            UVs.Add(side, new TextureUVNode(from, to));
        }

        public TextureUVNode getUVForSide(EnumFacing side)
        {
            UVs.TryGetValue(side, out var uv);

            return uv;
        }

        public void fill(Vector2 from, Vector2 to)
        {
            var values = Enum.GetValues(typeof(EnumFacing));

            foreach (EnumFacing side in values)
            {
                setUVForSide(side, from, to);
            }
        }
    }
}
