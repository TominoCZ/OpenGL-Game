﻿using System.Collections.Generic;

namespace OpenGL_Game
{
    public enum EnumBlock
    {
        AIR,
        STONE,
        GRASS,
        DIRT,
        BEDROCK,
        RARE,
        MISSING
    }

    class ModelRegistry
    {
        private static Dictionary<EnumBlock, BlockModel> models = new Dictionary<EnumBlock, BlockModel>();

        private static Dictionary<EnumFacing, float[]> CUBE = new Dictionary<EnumFacing, float[]>();

        static ModelRegistry()
        {
            CUBE.Add(EnumFacing.NORTH, new float[]
            {
                1, 1, 0,
                1, 0, 0,
                0, 0, 0,
                0, 1, 0
            });
            CUBE.Add(EnumFacing.SOUTH, new float[]
            {
                0, 1, 1,
                0, 0, 1,
                1, 0, 1,
                1, 1, 1
            });
            CUBE.Add(EnumFacing.EAST, new float[]
            {
                1, 1, 1,
                1, 0, 1,
                1, 0, 0,
                1, 1, 0
            });
            CUBE.Add(EnumFacing.WEST, new float[]
            {
                0, 1, 0,
                0, 0, 0,
                0, 0, 1,
                0, 1, 1
            });
            CUBE.Add(EnumFacing.UP, new float[]
            {
                0, 1, 0,
                0, 1, 1,
                1, 1, 1,
                1, 1, 0
            });
            CUBE.Add(EnumFacing.DOWN, new float[]
            {
                0, 0, 1,
                0, 0, 0,
                1, 0, 0,
                1, 0, 1
            });
        }

        public static void registerBlockModel(BlockModel model)
        {
            if (models.ContainsKey(model.block))
                models.Remove(model.block);

            models.Add(model.block, model);
        }

        public static BlockModel getModelForBlock(EnumBlock blockType)
        {
            models.TryGetValue(blockType, out var model);

            return model;
        }

        public static Dictionary<EnumFacing, RawQuad> createCubeModel(EnumBlock block)
        {
            var quads = new Dictionary<EnumFacing, RawQuad>();
            var uvs = TextureRegistry.getUVsFromBlock(block);

            foreach (var face in CUBE.Keys)
            {
                if (CUBE.TryGetValue(face, out var data))
                {
                    var uvNode = uvs.getUVForSide(face);

                    if (uvNode != null)
                        quads.Add(face, new RawQuad(data, uvNode.ToArray(), ModelHelper.calculateNormals(data)));
                }
            }

            return quads;
        }
    }
}