using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace OpenGL_Game
{
    enum EnumBlock
    {
        AIR,
        STONE,
        DIRT,
        BEDROCK,
        RARE,
        SELECTION
    }

    class ModelRegistry
    {
        private static Dictionary<EnumBlock, BlockModel> models = new Dictionary<EnumBlock, BlockModel>();

        public static CubeVertexData CUBE = new CubeVertexData();

        public static void setModelForBlock(EnumBlock blockType, BlockModel model)
        {
            if (models.ContainsKey(blockType))
                models.Remove(blockType);

            models.Add(blockType, model);
        }

        public static BlockModel getModelForBlock(EnumBlock blockType)
        {
            models.TryGetValue(blockType, out var model);

            return model;
        }
    }

    class CubeVertexData
    {
        public float[] vertices { get; }
        public int[] indices { get; }
        public float[] normals { get; }
        public float[] UVs { get; }

        public CubeVertexData()
        {
            vertices = new float[]{
                0, 1, 1,
                0, 0, 1,
                1, 0, 1,
                1, 1, 1,

                1, 1, 0,
                1, 0, 0,
                0, 0, 0,
                0, 1, 0,

                0, 1, 0,
                0, 1, 1,
                1, 1, 1,
                1, 1, 0,

                0, 0, 1,
                0, 0, 0,
                1, 0, 0,
                1, 0, 1,

                0, 1, 0,
                0, 0, 0,
                0, 0, 1,
                0, 1, 1,

                1, 1, 1,
                1, 0, 1,
                1, 0, 0,
                1, 1, 0};

            List<int> indices = new List<int>();
            List<float> UVs = new List<float>();

            for (int i = 0; i < 24; i += 4)
            {
                indices.AddRange(new[]
                {
                    i, i + 1, i + 3,
                    i + 3, i + 1, i + 2
                });

                UVs.AddRange(new float[]
                {
                    0, 0,
                    0, 1,
                    1, 1,
                    1, 0
                });
            }

            this.indices = indices.ToArray();
            this.UVs = UVs.ToArray();

            normals = ModelHelper.calculateNormals(vertices, this.indices);
        }
    }
}