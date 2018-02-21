using System.Linq;
using OpenTK;
using GL = OpenTK.Graphics.OpenGL.GL;
using TextureUnit = OpenTK.Graphics.OpenGL.TextureUnit;
using TextureTarget = OpenTK.Graphics.OpenGL.TextureTarget;
using OpenTK.Graphics.OpenGL;
using System;

namespace OpenGL_Game
{
    class WorldRenderer
    {
        private CubeOutlineModel selectionOutlineModel;

        private ModelLight modelLight;

        /// <summary>
        /// Render distance radius in chunks
        /// </summary>
        private int _renderDistance;

        public int RenderDistance
        {
            get => _renderDistance;
            set => _renderDistance = MathHelper.Clamp(value, 2, int.MaxValue);
        }

        public WorldRenderer()
        {
            modelLight = new ModelLight(new Vector3(-25, 100, -75) * 5, Vector3.One);
            selectionOutlineModel = new CubeOutlineModel(new BlockShaderWireframe());

            RenderDistance = 3;
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
            var hit = Game.INSTANCE.mouseOverObject;

            if (hit.hit != null && hit.hit is EnumBlock block && block != EnumBlock.AIR)
                renderBlockSelectionOutline(viewMatrix, block, hit.blockPos);

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

                        GL.DrawArrays(shader.renderType, 0, chunkFragmentModel.rawModel.vertexCount);

                        finishRendering(chunkFragmentModel);
                        shader.stop();
                    }
                }
            }
        }

        private void renderBlockSelectionOutline(Matrix4 viewMatrix, EnumBlock block, BlockPos pos)
        {
            var shader = (BlockShaderWireframe)selectionOutlineModel.shader;
            var size = ModelManager.getModelForBlock(block).boundingBox.size + Vector3.One * 0.005f;

            beginRendering(selectionOutlineModel);
            shader.start();

            shader.loadColor(Vector4.One);
            shader.loadViewMatrix(viewMatrix);
            shader.loadTransformationMatrix(MatrixHelper.createTransformationMatrix(pos.vector - Vector3.One * 0.0025f, size));

            GL.Disable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.DrawArrays(shader.renderType, 0, selectionOutlineModel.rawModel.vertexCount);

            GL.Enable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            finishRendering(selectionOutlineModel);
            shader.stop();
        }
    }
}
