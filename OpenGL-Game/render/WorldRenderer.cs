using System.Linq;
using OpenTK;
using GL = OpenTK.Graphics.OpenGL.GL;
using TextureUnit = OpenTK.Graphics.OpenGL.TextureUnit;
using TextureTarget = OpenTK.Graphics.OpenGL.TextureTarget;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class WorldRenderer
    {
        private ModelLight modelLight;

        public WorldRenderer()
        {
            modelLight = new ModelLight(new Vector3(-25, 120, -100), Vector3.One);
        }

        private void beginRendering(IModel m)
        {
            GL.BindVertexArray(m.rawModel.vaoID);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureRegistry.textureAtlasID);
        }

        private void finishRendering()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public void render(Matrix4 viewMatrix)
        {
            var nodes = Game.INSTANCE.world.getChunkDataNodes();

            foreach (var node in nodes)
            {
                var shaders = node.model.getShadersPresent();

                for (int j = 0; j < shaders.Length; j++)
                {
                    var shader = shaders[j];

                    if (node.model.getFragmentModelWithShader(shader, out var chunkModel))
                    {
                        beginRendering(chunkModel);

                        shader.start();
                        shader.loadLight(modelLight);
                        shader.loadViewMatrix(viewMatrix);

                        shader.loadTransformationMatrix(MatrixHelper.createTransformationMatrix(node.chunk.chunkPos.vector));
                        GL.DrawArrays(PrimitiveType.Quads, 0, chunkModel.rawModel.vertexCount);

                        finishRendering();
                        shader.stop();
                    }
                }
            }
        }
    }
}
