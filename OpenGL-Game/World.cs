using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    class World
    {
        private List<Chunk> loadedChunks;

        public World()
        {
            loadedChunks = new List<Chunk>();
        }

        public Chunk getChunkFromPos(BlockPos pos)
        {
            var x = Math.Floor(pos.x / 16.0);
            var z = Math.Floor(pos.z / 16.0);

            lock (loadedChunks)
            {
                for (int i = 0; i < loadedChunks.Count; i++)
                {
                    var chunk = loadedChunks[i];

                    var chunkX = Math.Floor(chunk.pos.x / 16.0);
                    var chunkZ = Math.Floor(chunk.pos.z / 16.0);

                    if (x == chunkX && z == chunkZ)
                        return chunk;
                }
            }

            return null;
        }

        public void setBlock(BlockPos pos, EnumBlock blockType)
        {
            var chunk = getChunkFromPos(pos);
            chunk.setBlock(pos - chunk.pos, blockType);
        }
    }
}
