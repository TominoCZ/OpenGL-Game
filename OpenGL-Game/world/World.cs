using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
            if (!_entities.Contains(e))
                _entities.Add(e);
        }

        public void updateEntities()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Update();
            }
        }

        public List<AxisAlignedBB> getIntersectingEntitiesBBs(AxisAlignedBB with)
        {
            List<AxisAlignedBB> bbs = new List<AxisAlignedBB>();

            for (int i = 0; i < _entities.Count; i++)
            {
                var bb = _entities[i].getBoundingBox();

                if (bb.intersectsWith(with))
                    bbs.Add(bb);
            }

            return bbs;
        }

        public List<AxisAlignedBB> getBlockCollisionBoxes(AxisAlignedBB box)
        {
            List<AxisAlignedBB> blocks = new List<AxisAlignedBB>();

            var bb = box.union(box);

            for (int x = (int)bb.min.X, maxX = (int)bb.max.X; x < maxX; x++)
            {
                for (int y = (int)bb.min.Y, maxY = (int)bb.max.Y; y < maxY; y++)
                {
                    for (int z = (int)bb.min.Z, maxZ = (int)bb.max.Z; z < maxZ; z++)
                    {
                        var pos = new BlockPos(x, y, z);
                        var block = Game.INSTANCE.world.getBlock(pos);
                        if (block == EnumBlock.AIR)
                            continue;

                        blocks.Add(ModelManager.getModelForBlock(block).boundingBox.offset(pos.vector));
                    }
                }
            }

            return blocks;
        }

        public Chunk getChunkFromPos(BlockPos pos)
        {
            _chunks.TryGetValue(pos.ChunkPos, out var chunkData);

            return chunkData?.chunk;
        }

        public ChunkData[] getChunkDataNodes()
        {
            return _chunks.Values.ToArray();
        }

        public void setBlock(EnumBlock blockType, BlockPos pos, bool redraw)
        {
            var sw = new Stopwatch();
            sw.Start();
            var chunk = getChunkFromPos(pos);
            if (chunk == null)
                return;

            chunk.setBlock(pos - chunk.chunkPos, blockType, redraw);

            if (redraw)
            {
                updateModelForChunk(chunk);

                var sides = (EnumFacing[])Enum.GetValues(typeof(EnumFacing));

                for (var index = 0; index < sides.Length; index++)
                {
                    EnumFacing side = sides[index];

                    var p = pos.offset(side);

                    var ch = getChunkFromPos(p);

                    if (ch != chunk)
                        updateModelForChunk(ch);
                }
            }
            sw.Stop();

            Console.WriteLine($"DEBUG: rebuilding terrain took {sw.ElapsedMilliseconds}ms");
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

            for (var index = 0; index < dataNodes.Length; index++)
            {
                var node = dataNodes[index];

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

            for (var index = 0; index < chunkDatas.Length; index++)
            {
                var chunkData = chunkDatas[index];

                if (chunkData.chunk.unloaded)
                    continue;

                chunkData.model = chunkData.chunk.generateModel();
            }
        }
    }
}