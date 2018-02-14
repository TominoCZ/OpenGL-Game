using System.Collections.Generic;
using OpenTK;

namespace OpenGL_Game
{
    class BlockModel
    {
        public RawModel rawModel{ get; }

        public ModelTexture texture { get; }

        public ShaderProgram shader { get; }

        public BlockModel(ModelTexture texture, ShaderProgram shader)
        {
            this.texture = texture;
            this.shader = shader;

            rawModel = Loader.loadToVAO(ModelRegistry.CUBE.vertices, ModelRegistry.CUBE.UVs, ModelRegistry.CUBE.indices);
        }
    }
}
