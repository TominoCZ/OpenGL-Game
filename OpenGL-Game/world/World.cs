﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace OpenGL_Game
{
    class World
    {
        private Dictionary<BlockPos, ChunkData> _chunks;

        public List<Entity> _entities;

        public World()
        {
            _chunks = new Dictionary<BlockPos, ChunkData>();
            _entities = new List<Entity>();
        }

        public World(int sizeInChunks) : this()
        {
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

        private World(List<ChunkCache> caches) : this()
        {
            foreach (var cache in caches)
            {
                var pos = cache.chunkPos;

                var chunk = Chunk.CreateFromCache(cache);
                var model = new ChunkModel();

                _chunks.Add(pos, new ChunkData(chunk, model));
            }
        }

        public static World Create(List<ChunkCache> caches)
        {
            return new World(caches);
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

        public void setBlock(BlockPos pos, EnumBlock blockType, bool redraw)
        {
            var chunk = getChunkFromPos(pos);
            if (chunk == null)
            {
                var chp = pos.ChunkPos;

                chunk = new Chunk(chp);
                _chunks.Add(chp, new ChunkData(chunk, new ChunkModel()));
            }

            chunk.setBlock(pos - chunk.chunkPos, blockType);

            if (redraw)
            {
                var sw = new Stopwatch();
                sw.Start();

                updateModelForChunk(chunk);

                var sides = (EnumFacing[])Enum.GetValues(typeof(EnumFacing));

                int chunksUpdated = 1;

                for (var index = 0; index < sides.Length; index++)
                {
                    EnumFacing side = sides[index];

                    var p = pos.offset(side);
                    var ch = getChunkFromPos(p);

                    p = p.ChunkPos;

                    if (ch == null && p.y >= 0)
                    {
                        ch = new Chunk(p);
                        _chunks.Add(p, new ChunkData(ch, new ChunkModel()));
                    }

                    if (ch != chunk && ch != null)
                    {
                        updateModelForChunk(ch);
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

        public bool isBlockAbove(BlockPos pos)
        {
            var chunk = getChunkFromPos(pos);

            return chunk.isBlockAbove(pos - chunk.chunkPos);
        }

        public void generateChunk(BlockPos pos)
        {
            var chunkPos = pos.ChunkPos;
            if (chunkPos.y >= 1)
                return;

            var noise = new FastNoise(0);
            noise.SetFractalType(FastNoise.FractalType.FBM);

            var chunk = new Chunk(chunkPos);

            //if (_chunks.ContainsKey(chunkPos))
            _chunks.Remove(chunkPos);

            _chunks.Add(chunkPos, new ChunkData(chunk, new ChunkModel()));

            var r = new Random();

            for (int z = 0; z < 16; z++)
            {
                for (int x = 0; x < 16; x++)
                {
                    int peakY = (int)Math.Abs((0.5f + noise.GetPerlinFractal((x + chunkPos.x), (z + chunkPos.z))) * 31);

                    for (int y = peakY; y >= 0; y--)
                    {
                        var p = new BlockPos(x, y, z) + chunkPos;

                        if (y == peakY)
                            setBlock(p, EnumBlock.GRASS, false);
                        else if (peakY - y > 0 && peakY - y < 3) // for 2 blocks
                            setBlock(p, EnumBlock.DIRT, false);
                        else if (y == 0)
                            setBlock(p, EnumBlock.BEDROCK, false);
                        else
                            setBlock(p, r.NextDouble() > 0.98 ? EnumBlock.RARE : EnumBlock.STONE, false);
                    }
                }
            }
        }

        public void generateChunkModels()
        {
            new Thread(() =>
            {
                var chunkDatas = _chunks.Values.ToArray();

                for (var index = 0; index < chunkDatas.Length; index++)
                {
                    var chunkData = chunkDatas[index];

                    var model = chunkData.chunk.generateModel(chunkData.model);
                    chunkData.model = model;
                }
            }).Start();
        }

        private void updateModelForChunk(Chunk chunk)
        {
            new Thread(() =>
               {
                   var dataNodes = getChunkDataNodes();

                   for (var index = 0; index < dataNodes.Length; index++)
                   {
                       var node = dataNodes[index];

                       if (node.chunk == chunk)
                       {
                           var model = node.chunk.generateModel(node.model);

                           node.model = model;
                           break;
                       }
                   }
               }).Start();
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