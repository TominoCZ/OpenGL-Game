using System;
using System.Collections.Generic;
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
        private Dictionary<EnumBlock, List<BlockPos>> blocks = new Dictionary<EnumBlock, List<BlockPos>>();
        private EnumBlock[] keys;

        private ModelLight modelLight;//, selectionLight;

        //private BlockModel PreviewModel;

        public BlockPos PreviewModelPos;

        public WorldRenderer()
        {
            modelLight = new ModelLight(new Vector3(-25, 120, -100), Vector3.One);
            //selectionLight = new ModelLight(Vector3.Zero, Vector3.One);

            keys = new EnumBlock[1];
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
            var chunks = Game.INSTANCE.world.loadedChunks.Keys.ToArray();

            for (int i = 0; i < Game.INSTANCE.world.loadedChunks.Count; i++)
            {
                var chunk = chunks[i];

                Game.INSTANCE.world.loadedChunks.TryGetValue(chunk, out var model);

                var shaders = model.Keys.ToArray();

                for (int j = 0; j < shaders.Length; j++)
                {
                    var shader = shaders[j];

                    if (model.TryGetValue(shader, out var chunkModel))
                    {
                        beginRendering(chunkModel);

                        shader.start();
                        shader.loadLight(modelLight);
                        shader.loadViewMatrix(viewMatrix);

                        shader.loadTransformationMatrix(MatrixHelper.createTransformationMatrix(chunk.chunkPos.vector));
                        GL.DrawArrays(PrimitiveType.Quads, 0, chunkModel.rawModel.vertexCount);

                        finishRendering();
                        shader.stop();
                    }
                }
            }

            
            /*for (int i = 0; i < keys.Length; i++)
            {
                var blockType = keys[i];

                var model = ModelRegistry.getModelForBlock(blockType);

                if (model == null)
                    continue;

                beginRendering(model);

                model.shader.start();
                model.shader.loadLight(modelLight);
                model.shader.loadViewMatrix(viewMatrix);

                if (blocks.TryGetValue(blockType, out var positions))
                {
                    foreach (var pos in positions)
                    {
                        model.shader.loadTransformationMatrix(MatrixHelper.createTransformationMatrix(pos.vector));

                        GL.DrawArrays(PrimitiveType.Quads, 0, model.rawModel.vertexCount);
                    }
                }

                finishRendering();
                model.shader.stop();
            }*/
        }

        [Obsolete]
        public void setBlock(EnumBlock blockType, BlockPos pos)
        {
            //if (blockType == EnumBlock.SELECTION)
            //return;

            lock (blocks)
            {
                //remove block at position
                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];

                    if (blocks.TryGetValue(key, out var positions))
                    {
                        if (positions.Contains(pos))
                            positions.Remove(pos);
                    }
                }

                if (blockType == EnumBlock.AIR)
                    return;

                if (!blocks.ContainsKey(blockType))
                    blocks.Add(blockType, new List<BlockPos>());

                if (blocks.TryGetValue(blockType, out var list))
                {
                    list.Add(pos);
                }

                lock (keys)
                {
                    this.keys = blocks.Keys.ToArray();
                }
            }
        }

        [Obsolete]
        public EnumBlock getBlock(BlockPos pos)
        {
            lock (blocks)
            {
                //remove block at position
                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];

                    if (blocks.TryGetValue(key, out var positions))
                    {
                        if (positions.Contains(pos))
                            return key;
                    }
                }

                return EnumBlock.AIR;
            }
        }
    }
}
