using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    public class World
    {
        public Dictionary<Chunk, Dictionary<ShaderProgram, ChunkFragmentModel>> loadedChunks;

        public World()
        {
            loadedChunks = new Dictionary<Chunk, Dictionary<ShaderProgram, ChunkFragmentModel>>();

            loadedChunks.Add(new Chunk(new BlockPos(0, 0, 0)), new Dictionary<ShaderProgram, ChunkFragmentModel>());
        }

        public Chunk getChunkFromPos(BlockPos pos)
        {
            var x = Math.Floor(pos.x / 16.0);
            var z = Math.Floor(pos.z / 16.0);

            lock (loadedChunks)
            {
                foreach (var chunk in loadedChunks)
                {
                    var chunkX = (int)Math.Floor(chunk.Key.chunkPos.x / 16.0);
                    var chunkZ = (int)Math.Floor(chunk.Key.chunkPos.z / 16.0);

                    if (x == chunkX && z == chunkZ)
                        return chunk.Key;
                }
            }

            return null;
        }

        public void setBlock(EnumBlock blockType, BlockPos pos, bool redraw)
        {
            var chunk = getChunkFromPos(pos);
            if (chunk == null)
                return;
            
            chunk.setBlock(pos - chunk.chunkPos, blockType, redraw);

            if (!redraw)
                return;

            updateModelForChunk(chunk);
        }

        public EnumBlock getBlock(BlockPos pos)
        {
            var chunk = getChunkFromPos(pos);
            if (chunk == null)
                return EnumBlock.AIR;

            return chunk.getBlock(pos - chunk.chunkPos);
        }

        private void updateModelForChunk(Chunk chunk)
        {
            if (loadedChunks.ContainsKey(chunk))
                loadedChunks.Remove(chunk);

            var model = chunk.generateModel();

            loadedChunks.Add(chunk, model);
        }

        public void generateChunkModels()
        {
            var keys = loadedChunks.Keys.ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                updateModelForChunk(keys[i]);
            }
        }
    }
}