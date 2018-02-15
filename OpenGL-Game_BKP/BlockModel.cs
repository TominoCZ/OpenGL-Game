using System.Collections.Generic;
using OpenTK;

namespace OpenGL_Game
{
    class BlockModel
    {
        public RawModel rawModel{ get; }

        public ModelTexture texture { get; }

        public StaticShader shader { get; }

        public BlockModel(ModelTexture texture, StaticShader shader)
        {
            this.texture = texture;
            this.shader = shader;

            rawModel = Loader.loadToVAO(ModelRegistry.CUBE.vertices, ModelRegistry.CUBE.UVs, ModelRegistry.CUBE.indices, ModelRegistry.CUBE.normals);
        }
    }
}
