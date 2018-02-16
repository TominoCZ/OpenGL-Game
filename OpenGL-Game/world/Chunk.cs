using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    public class Chunk
    {
        private int[,,] blocks;

        public BlockPos chunkPos { get; }

        private List<int> modelVaoIDs;

        public Chunk(BlockPos chunkPos)
        {
            this.chunkPos = chunkPos;

            blocks = new int[16, 32, 16];

            modelVaoIDs = new List<int>();
            //chunkModel = new Dictionary<ShaderProgram, ChunkFragmentModel>();
        }

        public void setBlock(BlockPos pos, EnumBlock blockType, bool redraw)
        {
            blocks[pos.x, pos.y, pos.z] = (int)blockType;

            if (redraw)
                generateModel();
        }

        public EnumBlock getBlock(BlockPos pos)
        {
            var thisChunk = pos.x >= 0 && pos.x < 16 &&
                            pos.y >= 0 && pos.y < 32 &&
                            pos.z >= 0 && pos.z < 16;

            if (thisChunk)
                return (EnumBlock)blocks[pos.x, pos.y, pos.z];

            //var chunk = Game.INSTANCE.world.getChunkFromPos(chunkPos + pos);

            return EnumBlock.AIR; //chunk?.getBlock(chunkPos + pos) ?? EnumBlock.AIR;
        }

        public Dictionary<ShaderProgram, ChunkFragmentModel> generateModel()
        {
            Dictionary<ShaderProgram, List<RawQuad>> MODEL_RAW = new Dictionary<ShaderProgram, List<RawQuad>>();

            var possibleDirections = Enum.GetValues(typeof(EnumFacing));

            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int x = 0; x < blocks.GetLength(0); x++)
                {
                    for (int z = 0; z < blocks.GetLength(2); z++)
                    {
                        var pos = new BlockPos(x, y, z);
                        var block = getBlock(pos);

                        if (block == EnumBlock.AIR)
                            continue;

                        var blockModel = ModelRegistry.getModelForBlock(block);
                        
                        foreach (EnumFacing dir in possibleDirections)
                        {
                            if (getBlock(pos.offset(dir)) == EnumBlock.AIR)
                            {
                                List<RawQuad> quads;

                                if (!MODEL_RAW.ContainsKey(blockModel.shader))
                                    MODEL_RAW.Add(blockModel.shader, quads = new List<RawQuad>());
                                else
                                    MODEL_RAW.TryGetValue(blockModel.shader, out quads);

                                quads?.Add(((RawBlockModel)blockModel.rawModel).getQuadForSide(dir).offset(x,y,z));
                            }
                        }
                    }
                }
            }

            foreach (var id in modelVaoIDs)
            {
                Loader.deleteVAO(id);
            }

            modelVaoIDs.Clear();

            Dictionary<ShaderProgram, ChunkFragmentModel> model = new Dictionary<ShaderProgram, ChunkFragmentModel>();

            foreach (var m in MODEL_RAW)
            {
                var bakedModel = new ChunkFragmentModel(m.Key, m.Value);
                
                model.Add(m.Key, bakedModel);

                modelVaoIDs.Add(bakedModel.rawModel.vaoID);
            }

            return model;
        }
    }
}
