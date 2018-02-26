using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenTK;

namespace OpenGL_Game
{
    class World
    {
        private ConcurrentDictionary<BlockPos, ChunkData> _chunks;

        public List<Entity> _entities;

        public readonly int seed;

        private FastNoise noise;

        public World(int seed)
        {
            _chunks = new ConcurrentDictionary<BlockPos, ChunkData>();
            _entities = new List<Entity>();

            noise = new FastNoise(seed);
            noise.SetFractalType(FastNoise.FractalType.FBM);

            this.seed = seed;
        }

        private World(int seed, List<ChunkCache> caches) : this(seed)
        {
            foreach (var cache in caches)
            {
                var pos = cache.chunkPos;

                var chunk = Chunk.CreateFromCache(cache);
                var model = new ChunkModel();

                var data = new ChunkData(chunk, model);

                _chunks.TryAdd(pos, data);
            }
        }

        public static World Create(int seed, List<ChunkCache> caches)
        {
            return new World(seed, caches);
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
            if (!_chunks.TryGetValue(pos.ChunkPos(), out var chunkData))
                return null;

            return chunkData?.chunk;
        }

        public ChunkData[] getChunkDataNodes()
        {
            return _chunks.Values.ToArray();
        }

        public void setBlock(BlockPos pos, EnumBlock blockType, bool redraw)
        {
            var chunk = getChunkFromPos(pos);
            if (chunk == null)
            {
                var chp = pos.ChunkPos();

                new Thread(() =>
                {
                    generateChunk(chp, false);
                    setBlock(pos, blockType, redraw);
                }).Start();

                //chunk = new Chunk(chp);
                //_chunks.TryAdd(chp, new ChunkData(chunk, new ChunkModel()));
            }

            chunk.setBlock(pos - chunk.chunkPos, blockType, 0);

            if (redraw)
            {
                var sw = new Stopwatch();
                sw.Start();

                updateModelForChunk(chunk.chunkPos);

                var sides = (EnumFacing[])Enum.GetValues(typeof(EnumFacing));

                int chunksUpdated = 1;

                for (var index = 0; index < sides.Length; index++)
                {
                    EnumFacing side = sides[index];

                    var p = pos.offset(side);
                    var ch = getChunkFromPos(p);

                    if (ch != chunk && ch != null)
                    {
                        updateModelForChunk(ch.chunkPos);
                        chunksUpdated++;
                    }
                }

                sw.Stop();

                Console.WriteLine($"DEBUG: built terrain model [{sw.Elapsed.TotalMilliseconds:##.000}ms] ({chunksUpdated} {(chunksUpdated > 1 ? "chunks" : "chunk")})");
            }
        }

        public EnumBlock getBlock(BlockPos pos)
        {
            var chunk = getChunkFromPos(pos);
            if (chunk == null)
                return EnumBlock.AIR;

            return chunk.getBlock(this, pos - chunk.chunkPos);
        }

        public int getHeightAtPos(int x, int z)
        {
            for (int y = 255; y >= 0; y--)
            {
                var pos = new BlockPos(x, y, z);

                var chunk = getChunkFromPos(pos);

                if (chunk == null)
                    generateChunk(pos, false);

                var block = getBlock(pos);

                if (block != EnumBlock.AIR)
                    return y + 1;
            }

            return 0;
        }

        public bool isBlockAbove(BlockPos pos)
        {
            var chunk = getChunkFromPos(pos);

            return chunk.isBlockAbove(this, pos - chunk.chunkPos);
        }

        public void generateChunk(BlockPos pos, bool redraw)
        {
            var chunkPos = pos.ChunkPos();

            if (_chunks.ContainsKey(chunkPos))
                return;

            var chunk = new Chunk(chunkPos);

            _chunks.TryAdd(chunkPos, new ChunkData(chunk, new ChunkModel()));

            for (int z = 0; z < 16; z++)
            {
                for (int x = 0; x < 16; x++)
                {
                    int peakY = 32 + (int)Math.Abs(MathHelper.Clamp(0.35f + noise.GetPerlinFractal((x + chunkPos.x) / 1.25f, (z + chunkPos.z) / 1.25f), 0, 1) * 30);

                    for (int y = peakY; y >= 0; y--)
                    {
                        var p = new BlockPos(x, y, z);

                        if (y == peakY)
                            chunk.setBlock(p, EnumBlock.GRASS, 0);
                        else if (y > 0 && peakY - y > 0 && peakY - y < 3) // for 2 blocks
                            chunk.setBlock(p, EnumBlock.DIRT, 0);
                        else if (y == 0)
                            chunk.setBlock(p, EnumBlock.BEDROCK, 0);
                        else
                            chunk.setBlock(p, noise.GetCubic(x, y) > 0.98 ? EnumBlock.RARE : EnumBlock.STONE, 0);
                    }
                }
            }

            if (redraw)
                updateModelForChunk(chunk.chunkPos);

            var sides = (EnumFacing[])Enum.GetValues(typeof(EnumFacing));

            for (var index = 0; index < sides.Length - 2; index++)
            {
                var side = sides[index];

                var vec = new BlockPos().offset(side).vector;
                var offset = new BlockPos(vec * 16);

                var c = getChunkFromPos(offset + chunkPos);

                if (c != null)
                    updateModelForChunk(c.chunkPos);
            }
        }

        public void updateModelForChunk(BlockPos pos)
        {
            if (_chunks.TryGetValue(pos.ChunkPos(), out var node))
            {
                node.modelGenerated = true;

                new Thread(() =>
                {
                    var model = node.chunk.generateModel(this, node.model);
                    node.model = model;
                }).Start();
            }
        }

        public bool doesChunkHaveModel(BlockPos pos)
        {
            return _chunks[pos.ChunkPos()].modelGenerated;
        }
    }

    class ThreadLock
    {
        private bool locked;

        public void Lock() => locked = true;
        public void Unlock() => locked = false;

        public delegate void Method();

        private Method method;

        public ThreadLock(Method m)
        {
            method = m;
        }

        public void WaitFor()
        {
            while (locked)
            {
                Thread.Sleep(1);
            }
        }

        public void ExecuteCode()
        {
            method();
            Unlock();
        }
    }
}