using System;
using System.Threading;

namespace OpenGL_Game
{
    class WorldGenerator
    {
        public static World generate(int seed)
        {
            int size = (int)Math.Pow(2, 8);
            int half = size / 2;

            int totalHeight = 10;

            World world = new World();

            Random rand = new Random(seed);

            var sizeInChunks = size / 16;
            var chunksHalf = sizeInChunks / 2;

            for (int x = -chunksHalf; x < chunksHalf; x++)
            {
                for (int z = -chunksHalf; z < chunksHalf; z++)
                {

                    var pos = new BlockPos(x * 16, 0, z * 16);
                    world.generateChunk(pos);

                }
            }

            if (Game.INSTANCE.player != null)
            {
                var playerPos = new BlockPos(Game.INSTANCE.player.pos);

                Game.INSTANCE.player.pos.X = world.getHeightAtPos(playerPos.x, playerPos.z) + 1;
            }

            return world;
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

                        world.setBlock(pos, block, false);
                    }
                }
            }

            return world;
        }
    }
}
