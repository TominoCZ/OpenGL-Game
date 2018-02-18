using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    class Chunk
    {
        private int[,,] _blocks;

        public BlockPos chunkPos { get; }

        private List<int> modelVaoIDs;

        public bool unloaded = false;

        public Chunk(BlockPos chunkPos)
        {
            this.chunkPos = chunkPos;

            _blocks = new int[16, 256, 16];

            modelVaoIDs = new List<int>();
        }

        public void setBlock(BlockPos pos, EnumBlock blockType, bool redraw)
        {
            _blocks[pos.x, pos.y, pos.z] = (int)blockType;

            if (redraw)
                generateModel();
        }

        public EnumBlock getBlock(BlockPos pos)
        {
            var thisChunk = pos.x >= 0 && pos.x < _blocks.GetLength(0) &&
                            pos.y >= 0 && pos.y < _blocks.GetLength(1) &&
                            pos.z >= 0 && pos.z < _blocks.GetLength(2);

            if (thisChunk)
                return (EnumBlock)_blocks[pos.x, pos.y, pos.z];

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

            Dictionary<ShaderProgram, List<RawQuad>> MODEL_RAW = new Dictionary<ShaderProgram, List<RawQuad>>();

            var possibleDirections = (EnumFacing[]) Enum.GetValues(typeof(EnumFacing));
            var pos = new BlockPos();
            List<RawQuad> quads;

            for (int y = 0; y < _blocks.GetLength(1); y++)
            {
                for (int x = 0; x < _blocks.GetLength(0); x++)
                {
                    for (int z = 0; z < _blocks.GetLength(2); z++)
                    {
                        pos.setPos(x, y, z);

                        var block = getBlock(pos);

                        if (block == EnumBlock.AIR)
                            continue;

                        var blockModel = ModelManager.getModelForBlock(block);

                        if (!MODEL_RAW.ContainsKey(blockModel.shader))
                            MODEL_RAW.Add(blockModel.shader, quads = new List<RawQuad>());
                        else
                            MODEL_RAW.TryGetValue(blockModel.shader, out quads);

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
    }
}
