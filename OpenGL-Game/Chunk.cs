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

        public BlockPos pos { get; }

        public Chunk(BlockPos pos)
        {
            this.pos = pos;

            blocks = new int[32, 32, 32];
        }

        public void setBlock(BlockPos pos, EnumBlock blockType)
        {
            blocks[pos.z, pos.y, pos.x] = (int)blockType;
        }

        public EnumBlock getBlock(BlockPos pos)
        {
            return (EnumBlock)blocks[pos.z, pos.y, pos.x];
        }

        public EnumBlock getBlock(int x, int y, int z)
        {
            return (EnumBlock)blocks[pos.z, pos.y, pos.x];
        }

        private RawQuad getRenderedQuad()
        {
            return null;
        }

        private void generateModel()
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int z = 0; z < blocks.GetLength(0); z++)
                {
                    for (int x = 0; x < blocks.GetLength(2); x++)
                    {
                        var block = getBlock(x, y, z);
                        var model = ModelRegistry.getModelForBlock(block);

                        //for (int i = 0; i < model.rawModel.setQuadForSide();
                    }
                }
            }
        }
    }
}
