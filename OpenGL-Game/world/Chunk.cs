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
    class Chunk
    {
        private int[,,] blocks;

        public BlockPos chunkPos { get; }

        public ChunkModel chunkModel { get; private set; }

        public Chunk(BlockPos chunkPos)
        {
            this.chunkPos = chunkPos;

            blocks = new int[16, 32, 16];
        }

        public void setBlock(BlockPos pos, EnumBlock blockType)
        {
            blocks[pos.z, pos.y, pos.x] = (int)blockType;

            generateModel();
        }

        public EnumBlock getBlock(BlockPos pos)
        {
            return (EnumBlock)blocks[pos.z, pos.y, pos.x];
        }

        private void generateModel()
        {
            Dictionary<ShaderProgram, List<RawQuad>> MODEL = new Dictionary<ShaderProgram, List<RawQuad>>();

            var possibleDirections = Enum.GetValues(typeof(EnumFacing));

            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int z = 0; z < blocks.GetLength(0); z++)
                {
                    for (int x = 0; x < blocks.GetLength(2); x++)
                    {
                        var pos = new BlockPos(x, y, z);
                        var block = getBlock(pos);
                        var model = ModelRegistry.getModelForBlock(block);

                        foreach (EnumFacing dir in possibleDirections)
                        {
                            if (getBlock(pos.offset(dir)) == EnumBlock.AIR)
                            {
                                List<RawQuad> quads;

                                if (!MODEL.ContainsKey(model.shader))
                                    MODEL.Add(model.shader, quads = new List<RawQuad>());
                                else
                                    MODEL.TryGetValue(model.shader, out quads);

                                quads?.Add(((RawBlockModel)model.rawModel).getQuadForSide(dir));
                            }
                        }
                    }
                }
            }

            if (chunkModel != null)
                Loader.deleteVAO(chunkModel.rawModel.vaoID);

            chunkModel = new ChunkModel(MODEL);
        }
    }
}
