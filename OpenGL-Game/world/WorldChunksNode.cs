﻿using System;
using System.Collections.Generic;

namespace OpenGL_Game
{
    [Serializable]
    class WorldChunksNode
    {
        public List<ChunkCache> caches { get; }
        public int seed { get; }

        public WorldChunksNode(World w)
        {
            var nodes = w.getChunkDataNodes();

            caches = new List<ChunkCache>();

            foreach (var node in nodes)
            {
                var cache = node.chunk.createChunkCache();

                caches.Add(cache);
            }

            seed = w.seed;
        }
    }
}