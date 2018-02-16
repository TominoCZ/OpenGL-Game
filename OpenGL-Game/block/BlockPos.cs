using System;
using OpenTK;

namespace OpenGL_Game
{
    public struct BlockPos
    {
        public int x { get; }
        public int y { get; }
        public int z { get; }

        public Vector3 vector => new Vector3(x, y, z);

        public static BlockPos operator -(BlockPos p1, BlockPos p2)
        {
            return new BlockPos(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
        }

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

        public BlockPos offset(EnumFacing dir)
        {
            switch (dir)
            {
                case EnumFacing.NORTH:
                    return new BlockPos(x, y, z - 1);
                case EnumFacing.SOUTH:
                    return new BlockPos(x, y, z + 1);
                case EnumFacing.EAST:
                    return new BlockPos(x + 1, y, z);
                case EnumFacing.WEST:
                    return new BlockPos(x - 1, y, z);
                case EnumFacing.UP:
                    return new BlockPos(x, y + 1, z);
                case EnumFacing.DOWN:
                    return new BlockPos(x, y - 1, z);

                default: return this;
            }
        }
    }
}