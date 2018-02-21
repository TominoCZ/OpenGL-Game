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

        private int _renderDistance;

        /// <summary>
        /// Render distance radius in chunks
        /// </summary>
        public int RenderDistance
        {
            get => _renderDistance;
            set => _renderDistance = MathHelper.Clamp(value, 2, int.MaxValue);
        }

        public WorldRenderer()
        {
            modelLight = new ModelLight(new Vector3(-25, 100, -75) * 5, Vector3.One);

            RenderDistance = 16;
        }

        private void beginRendering(IModel m)
        {
            GL.BindVertexArray(m.rawModel.vaoID);

            GL.EnableVertexAttribArray(0);

            if (m.rawModel.hasUVs)
                GL.EnableVertexAttribArray(1);
            if (m.rawModel.hasNormals)
                GL.EnableVertexAttribArray(2);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureManager.blockTextureAtlasID);
        }

        private void finishRendering(IModel m)
        {
            GL.DisableVertexAttribArray(0);

            if (m.rawModel.hasUVs)
                GL.DisableVertexAttribArray(1);
            if (m.rawModel.hasNormals)
                GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public void render(Matrix4 viewMatrix)
        {
            var nodes = Game.INSTANCE.world.getChunkDataNodes();

            for (var index = 0; index < nodes.Length; index++)
            {
                var node = nodes[index];

                var dist = (Game.INSTANCE.player.camera.pos - (node.chunk.chunkPos.vector + Vector3.UnitX * 8 + Vector3.UnitZ * 8)).Length;
                
                if (dist > RenderDistance * 16)
                    continue;

                var shaders = node.model.getShadersPresent();

                for (int j = 0; j < shaders.Length; j++)
                {
                    var shader = shaders[j];

                    if (node.model.getFragmentModelWithShader(shader, out var chunkFragmentModel))
                    {
                        beginRendering(chunkFragmentModel);

                        shader.start();

                        shader.loadLight(modelLight);
                        shader.loadViewMatrix(viewMatrix);

                        shader.loadTransformationMatrix(MatrixHelper.createTransformationMatrix(node.chunk.chunkPos.vector));

                        GL.DrawArrays(PrimitiveType.Quads, 0, chunkFragmentModel.rawModel.vertexCount);

                        finishRendering(chunkFragmentModel);
                        shader.stop();
                    }
                }
            }
        }
    }
}
