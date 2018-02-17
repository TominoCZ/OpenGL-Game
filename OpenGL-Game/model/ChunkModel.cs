using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    class ChunkModel
    {
        private Dictionary<ShaderProgram, ChunkFragmentModel> fragmentPerShader;

        public ChunkModel()
        {
            fragmentPerShader = new Dictionary<ShaderProgram, ChunkFragmentModel>();
        }

        public void addFragmentModelWithShader(ShaderProgram shader, ChunkFragmentModel model)
        {
            fragmentPerShader.Add(shader, model);
        }

        public bool getFragmentModelWithShader(ShaderProgram shader, out ChunkFragmentModel model)
        {
            return fragmentPerShader.TryGetValue(shader, out model);
        }

        public ShaderProgram[] getShadersPresent()
        {
            return fragmentPerShader.Keys.ToArray();
        }
    }
}
