using System.Collections.Generic;

namespace OpenGL_Game
{
    internal class ChunkModel
    {
        private Dictionary<ShaderProgram, ChunkFragmentModel> fragmentPerShader;

        private List<ShaderProgram> shaders;

        public ChunkModel()
        {
            fragmentPerShader = new Dictionary<ShaderProgram, ChunkFragmentModel>();
            shaders = new List<ShaderProgram>();
        }

        public void setFragmentModelWithShader(ShaderProgram shader, ChunkFragmentModel model)
        {
            //if (fragmentPerShader.ContainsKey(shader))
            fragmentPerShader.Remove(shader);

            fragmentPerShader.Add(shader, model);
            shaders.Add(shader);
        }

        public ChunkFragmentModel getFragmentModelWithShader(ShaderProgram shader)
        {
            return fragmentPerShader[shader];
        }

        public List<ShaderProgram> getShadersPresent()
        {
            return shaders;
        }
    }
}