using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    class ChunkModel : IModel
    {
        public IRawModel rawModel { get; }

        public ChunkModel(Dictionary<ShaderProgram, List<RawQuad>> model)
        {
            rawModel = Loader.loadChunkModelToVAO(model);
        }
    }
}
