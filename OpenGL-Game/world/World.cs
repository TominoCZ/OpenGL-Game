using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace OpenGL_Game
{
    class World
    {
        private Dictionary<BlockPos, ChunkData> _chunks;

        public List<Entity> _entities;

        public World(int sizeInChunks)
        {
            _chunks = new Dictionary<BlockPos, ChunkData>();

            _entities = new List<Entity>();

            int half = sizeInChunks / 2;

            for (int z = -half; z < half; z++)
            {
                for (int x = -half; x < half; x++)
                {
                    var pos = new BlockPos(x * 16, 0, z * 16);
                    var chunk = new Chunk(pos);
                    var model = new ChunkModel();

                    _chunks.Add(pos, new ChunkData(chunk, model));
                }
            }
        }

        public void addEntity(Entity e)
        {
            lock (_entities)
            {
                if (!_entities.Contains(e))
                    _entities.Add(e);
            }
        }

        public void updateEntities()
        {
            lock (_entities)
            {
                for (int i = 0; i < _entities.Count; i++)
                {
                    _entities[i].Update();
                }
            }
        }

        public List<AxisAlignedBB> getIntersectingEntityBBs(AxisAlignedBB with)
        {
            List<AxisAlignedBB> bbs = new List<AxisAlignedBB>();

            lock (_entities)
            {
                for (int i = 0; i < _entities.Count; i++)
                {
                    var bb = _entities[i].getBoundingBox();

                    if (bb.isIntersectingWith(with))
                        bbs.Add(bb);
                }
            }

            return bbs;
        }

        public Chunk getChunkFromPos(BlockPos pos)
        {
            if (_chunks.TryGetValue(pos.ChunkPos, out var chunkData))
                return chunkData.chunk;

            return null;
        }

        public Chunk[] getChunks()
        {
            var positions = _chunks.Keys.ToArray();

            List<Chunk> chunks = new List<Chunk>();

            foreach (var position in positions)
            {
                if (_chunks.TryGetValue(position, out var chunk))
                    chunks.Add(chunk.chunk);
            }

            return chunks.ToArray();
        }

        public ChunkData[] getChunkDataNodes()
        {
            return _chunks.Values.ToArray();
        }

        public void setBlock(EnumBlock blockType, BlockPos pos, bool redraw)
        {
            var chunk = getChunkFromPos(pos);
            if (chunk == null)
                return;

            chunk.setBlock(pos - chunk.chunkPos, blockType, redraw);

            if (redraw)
                updateModelForChunk(chunk);
        }

        public EnumBlock getBlock(BlockPos pos)
        {
            var chunk = getChunkFromPos(pos);
            if (chunk == null)
                return EnumBlock.AIR;

            return chunk.getBlock(pos - chunk.chunkPos);
        }

        public int getHeightAtPos(int x, int z)
        {
            for (int y = 255; y >= 0; y--)
            {
                var block = getBlock(new BlockPos(x, y, z));

                if (block != EnumBlock.AIR)
                    return y;
            }

            return -1;
        }

        private void updateModelForChunk(Chunk chunk)
        {
            var dataNodes = getChunkDataNodes();

            foreach (var node in dataNodes)
            {
                if (node.chunk == chunk)
                {
                    node.model = node.chunk.generateModel();
                    break;
                }
            }
        }

        public void generateChunkModels()
        {
            var chunkDatas = _chunks.Values.ToArray();

            for (int i = 0; i < chunkDatas.Length; i++)
            {
                var chunkData = chunkDatas[i];

                if (chunkData.chunk.unloaded)
                    continue;

                chunkData.model = chunkData.chunk.generateModel();
            }
        }
    }
}