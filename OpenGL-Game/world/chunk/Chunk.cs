using System;
using System.Collections.Generic;

namespace OpenGL_Game
{
    class Chunk
    {
        private int[,,] chunkBlocks;

        public BlockPos chunkPos { get; }

        private List<int> modelVaoIDs;

        public bool unloaded = false;

        public Chunk(BlockPos chunkPos)
        {
            this.chunkPos = chunkPos;

            chunkBlocks = new int[16, 16, 16];

            modelVaoIDs = new List<int>();
        }

        private Chunk(ChunkCache cache)
        {
            modelVaoIDs = new List<int>();

            chunkPos = cache.chunkPos;
            chunkBlocks = cache.chunkBlocks;
        }

        public static Chunk CreateFromCache(ChunkCache cache)
        {
            return new Chunk(cache);
        }

        public void setBlock(BlockPos pos, EnumBlock blockType, bool redraw)
        {
            chunkBlocks[pos.x, pos.y, pos.z] = (int)blockType;

            if (redraw)
                generateModel();
        }

        public EnumBlock getBlock(BlockPos pos)
        {
            var thisChunk = pos.x >= 0 && pos.x < chunkBlocks.GetLength(0) &&
                            pos.y >= 0 && pos.y < chunkBlocks.GetLength(1) &&
                            pos.z >= 0 && pos.z < chunkBlocks.GetLength(2);

            if (thisChunk)
                return (EnumBlock)chunkBlocks[pos.x, pos.y, pos.z];

            var block = Game.INSTANCE.world.getBlock(pos + chunkPos);

            return block;
        }

        public ChunkModel generateModel()
        {
            for (var index = 0; index < modelVaoIDs.Count; index++)
            {
                var id = modelVaoIDs[index];
                GraphicsManager.deleteVAO(id);
            }

            modelVaoIDs.Clear();

            var MODEL_RAW = new Dictionary<ShaderProgram, List<RawQuad>>();

            var possibleDirections = (EnumFacing[])Enum.GetValues(typeof(EnumFacing));
            var pos = new BlockPos(0, 0, 0);
            List<RawQuad> quads;

            var l_x = chunkBlocks.GetLength(0);
            var l_y = chunkBlocks.GetLength(1);
            var l_z = chunkBlocks.GetLength(2);

            for (int z = 0; z < l_z; z++)
            {
                for (int y = 0; y < l_y; y++)
                {
                    for (int x = 0; x < l_x; x++)
                    {
                        pos.setPos(x, y, z);

                        var block = getBlock(pos);

                        if (block == EnumBlock.AIR)
                            continue;

                        var blockModel = ModelManager.getModelForBlock(block);

                        if (!MODEL_RAW.TryGetValue(blockModel.shader, out quads))
                            MODEL_RAW.Add(blockModel.shader, quads = new List<RawQuad>());

                        for (int i = 0; i < possibleDirections.Length; i++)
                        {
                            var dir = possibleDirections[i];

                            if (getBlock(pos.offset(dir)) == EnumBlock.AIR)
                            {
                                quads?.Add(((RawBlockModel)blockModel.rawModel).getQuadForSide(dir).offset(pos));
                            }
                        }
                    }
                }
            }

            ChunkModel model = new ChunkModel();

            foreach (var m in MODEL_RAW)
            {
                var bakedModel = new ChunkFragmentModel(m.Key, m.Value);

                model.addFragmentModelWithShader(m.Key, bakedModel);

                modelVaoIDs.Add(bakedModel.rawModel.vaoID);
            }

            return model;
        }

        public ChunkCache createChunkCache()
        {
            return new ChunkCache(chunkPos, chunkBlocks);
        }
    }

    [Serializable]
    class ChunkCache
    {
        private readonly BlockPos _chunkPos;
        private readonly int[,,] _chunkBlocks;

        public BlockPos chunkPos => _chunkPos;
        public int[,,] chunkBlocks => _chunkBlocks;

        public ChunkCache(BlockPos chunkPos, int[,,] chunkBlocks)
        {
            _chunkPos = chunkPos;
            _chunkBlocks = chunkBlocks;
        }
    }
}
