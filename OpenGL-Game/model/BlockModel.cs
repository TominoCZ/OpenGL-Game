using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace OpenGL_Game
{
    class BlockModel : IModel
    {
        public IRawModel rawModel { get; }

        public StaticShader shader { get; }

        public EnumBlock block { get; }

        public BlockModel(EnumBlock block, StaticShader shader)
        {
            this.shader = shader;
            this.block = block;

            var cube = ModelRegistry.createCubeModel(block);

            rawModel = Loader.loadBlockModelToVAO(cube.Values.ToList());
        }
    }
}