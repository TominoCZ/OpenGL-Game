using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

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
            blocks[pos.x, pos.y, pos.z] = (int)blockType;
        }

        public EnumBlock getBlock(BlockPos pos)
        {
            return (EnumBlock)blocks[pos.x, pos.y, pos.z];
        }
    }

    struct BlockPos
    {
        public int x, y, z;
        public Vector3 vector => new Vector3(x, y, z);

        public static BlockPos operator +(BlockPos p1, BlockPos p2)
        {
            return new BlockPos(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }

        public BlockPos(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public BlockPos(Vector3 vec)
        {
            x = (int)Math.Floor(vec.X);
            y = (int)Math.Floor(vec.Y);
            z = (int)Math.Floor(vec.Z);
        }

        public BlockPos left()
        {
            return new BlockPos(x - 1, y, z);
        }

        public BlockPos right()
        {
            return new BlockPos(x + 1, y, z);
        }

        public BlockPos front()
        {
            return new BlockPos(x, y, z - 1);
        }

        public BlockPos back()
        {
            return new BlockPos(x, y, z + 1);
        }

        public BlockPos above()
        {
            return new BlockPos(x, y + 1, z);
        }

        public BlockPos under()
        {
            return new BlockPos(x, y - 1, z);
        }
    }
}
