using System;
using System.Collections.Generic;

namespace OpenGL_Game
{
    [Serializable]
    class WorldChunksNode
    {
        public List<ChunkCache> caches { get; }

        public WorldChunksNode(List<ChunkCache> caches)
        {
            this.caches = caches;
        }
    }
}