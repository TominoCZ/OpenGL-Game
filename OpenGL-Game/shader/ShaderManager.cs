using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_Game
{
    class ShaderManager
    {
        private static ThreadSafeList<ShaderProgram> shaders = new ThreadSafeList<ShaderProgram>();

        private static Matrix4 projectionMatrix;

        public static void registerShader(ShaderProgram shader)
        {
            shader.start();
            shader.loadProjectionMatrix(projectionMatrix);
            shader.stop();

            shaders.Add(shader);
        }

        public static void updateProjectionMatrix()
        {
            projectionMatrix = Game.INSTANCE.gameRenderer.createProjectionMatrix();

            for (int i = 0; i < shaders.Count; i++)
            {
                var shader = shaders[i];

                shader.start();
                shader.loadProjectionMatrix(projectionMatrix);
                shader.stop();
            }
        }

        public static void cleanUp()
        {
            for (int i = 0; i < shaders.Count; i++)
            {
                shaders[i].cleanUp();
            }
        }
    }
}
