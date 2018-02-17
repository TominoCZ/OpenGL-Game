﻿namespace OpenGL_Game
{
    class BlockModel : IModel
    {
        public IRawModel rawModel { get; }

        public ShaderProgram shader { get; }

        public EnumBlock block { get; }

        public BlockModel(EnumBlock block, ShaderProgram shader)
        {
            this.shader = shader;
            this.block = block;

            var cube = ModelRegistry.createCubeModel(block);

            rawModel = Loader.loadBlockModelToVAO(cube);
        }
    }
}