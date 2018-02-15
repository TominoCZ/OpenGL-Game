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
    class BlockRenderer
    {
        private Dictionary<EnumBlock, List<BlockPos>> blocks = new Dictionary<EnumBlock, List<BlockPos>>();
        private EnumBlock[] keys;

        private ModelLight modelLight;//, selectionLight;

        //private BlockModel PreviewModel;

        public BlockPos PreviewModelPos;

        public BlockRenderer()
        {
            modelLight = new ModelLight(new Vector3(-25, 120, -100), Vector3.One);
            //selectionLight = new ModelLight(Vector3.Zero, Vector3.One);

            keys = new EnumBlock[1];
        }

        private void beginRendering(BlockModel m)
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

        public void render(Camera c)
        {
            var viewMatrix = MatrixHelper.createViewMatrix(c);
            /*
            if (PreviewModel != null)
            {
                beginRendering(PreviewModel);

                selectionLight.pos = Camera.INSTANCE.pos;

                PreviewModel.shader.start();
                PreviewModel.shader.loadViewMatrix(viewMatrix);
                PreviewModel.shader.loadLight(selectionLight);
                PreviewModel.shader.loadTransformationMatrix(MatrixHelper.createTransformationMatrix(PreviewModelPos.vector + Vector3.One * -0.0125f, 1.025f));

                GL.Disable(EnableCap.CullFace);
                GL.DrawArrays(PrimitiveType.Quads, 0, PreviewModel.RawBlockModel.vertexCount);
                GL.Enable(EnableCap.CullFace);

                finishRendering();
                PreviewModel.shader.stop();
            }
            else
            {
                PreviewModel = ModelRegistry.getModelForBlock(EnumBlock.SELECTION);
            }*/

            for (int i = 0; i < keys.Length; i++)
            {
                var blockType = keys[i];
                var model = ModelRegistry.getModelForBlock(blockType);

                beginRendering(model);

                model.shader.start();
                model.shader.loadLight(modelLight);
                model.shader.loadViewMatrix(viewMatrix);

                if (blocks.TryGetValue(blockType, out var positions))
                {
                    foreach (var pos in positions)
                    {
                        model.shader.loadTransformationMatrix(MatrixHelper.createTransformationMatrix(pos.vector));
                        
                        GL.DrawArrays(PrimitiveType.Quads, 0, 24);
                    }
                }

                finishRendering();
                model.shader.stop();
            }
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
