using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    public class ChunkFragmentModel : IModel
    {
        public IRawModel rawModel { get; }
        public ShaderProgram shader { get; }

        public ChunkFragmentModel(ShaderProgram shader, List<RawQuad> model)
        {
            this.shader = shader;

            rawModel = Loader.loadChunkModelToVAO(model);
        }
    }
}
