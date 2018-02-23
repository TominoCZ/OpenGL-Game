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

        public void setFragmentModelWithShader(ShaderProgram shader, ChunkFragmentModel model)
        {
            //if (fragmentPerShader.ContainsKey(shader))
            fragmentPerShader.Remove(shader);

            fragmentPerShader.Add(shader, model);
        }

        public ChunkFragmentModel getFragmentModelWithShader(ShaderProgram shader)
        {
            return fragmentPerShader[shader];
        }

        public ShaderProgram[] getShadersPresent()
        {
            return fragmentPerShader.Keys.ToArray();
        }
    }
}
