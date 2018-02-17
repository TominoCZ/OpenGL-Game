using System;

namespace OpenGL_Game
{
    class WorldGenerator
    {
        public static World generate(int seed)
        {
            int size = (int)Math.Pow(2, 6);
            int half = size / 2;

            int totalHeight = 10;

            World world = new World(size / 16);

            Random rand = new Random();

            for (int y = 0; y < totalHeight; y++)
            {
                for (int z = -half; z < half; z++)
                {
                    for (int x = -half; x < half; x++)
                    {
                        var pos = new BlockPos(x, y, z);

                        EnumBlock block;

                        if (y == 0)
                            block = EnumBlock.BEDROCK;
                        else if (totalHeight - y <= 3)
                            block = totalHeight - y > 1 ? EnumBlock.DIRT : EnumBlock.GRASS;
                        else
                            block = (rand.NextDouble() >= 0.95 && y != 0 && y != 4) ? EnumBlock.RARE : EnumBlock.STONE;

                        if (block == EnumBlock.AIR)
                            continue;

                        world.setBlock(block, pos, false);
                    }
                }
            }

            return world;
        }
    }
}
