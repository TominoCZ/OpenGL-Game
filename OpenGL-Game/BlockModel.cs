using System;
using System.Collections.Generic;
using OpenTK;

namespace OpenGL_Game
{
    class BlockModel
    {
        public RawModel rawModel { get; }

        public StaticShader shader { get; }

        public EnumBlock block { get; }

        public BlockModel(EnumBlock block, StaticShader shader)
        {
            this.shader = shader;
            this.block = block;

            var cube = ModelRegistry.createCube(block);

            List<float> vertices = new List<float>();
            List<float> normals = new List<float>();
            List<float> UVs = new List<float>();

            foreach (var v in cube)
            {
                vertices.AddRange(v.Value.vertices);
                normals.AddRange(v.Value.normal);
                UVs.AddRange(v.Value.UVs);
            }

            rawModel = Loader.loadToVAO(vertices.ToArray(), normals.ToArray(), UVs.ToArray());
        }
    }
}