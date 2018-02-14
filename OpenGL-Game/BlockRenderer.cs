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
        private Dictionary<EnumBlock, List<Vector3>> blocks = new Dictionary<EnumBlock, List<Vector3>>();

        private ModelLight modelLight, selectionLight;

        private BlockModel PreviewModel;

        public Vector3 PreviewModelVector;

        public BlockRenderer()
        {
            modelLight = new ModelLight(new Vector3(-25, 120, -100), Vector3.One);
            selectionLight = new ModelLight(Vector3.Zero, Vector3.One);
        }

        private void beginRendering(BlockModel m)
        {
            GL.BindVertexArray(m.rawModel.vaoID);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, m.texture.textureID);
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
            var keys = blocks.Keys.ToArray();
            var viewMatrix = MatrixHelper.createViewMatrix(c);

            if (PreviewModel != null)
            {
                beginRendering(PreviewModel);

                selectionLight.pos = Camera.INSTANCE.pos;

                PreviewModel.shader.start();
                PreviewModel.shader.loadViewMatrix(viewMatrix);
                PreviewModel.shader.loadLight(selectionLight);
                PreviewModel.shader.loadTransformationMatrix(MatrixHelper.createTransformationMatrix(PreviewModelVector + new Vector3(-0.0125f), 0, 0, 0, 1.025f));

                GL.Disable(EnableCap.CullFace);
                GL.DrawElements(BeginMode.Triangles, PreviewModel.rawModel.vertexes, DrawElementsType.UnsignedInt, 0);
                GL.Enable(EnableCap.CullFace);

                finishRendering();
                PreviewModel.shader.stop();
            }
            else
            {
                PreviewModel = ModelRegistry.getModelForBlock(EnumBlock.SELECTION);
            }

            for (int i = 0; i < keys.Length; i++)
            {
                EnumBlock blockType = keys[i];
                BlockModel model = ModelRegistry.getModelForBlock(blockType);

                beginRendering(model);

                model.shader.start();
                model.shader.loadLight(modelLight);
                model.shader.loadViewMatrix(viewMatrix);

                if (blocks.TryGetValue(blockType, out var positions))
                {
                    for (int j = 0; j < positions.Count; j++)
                    {
                        Vector3 pos = positions[j];

                        model.shader.loadTransformationMatrix(MatrixHelper.createTransformationMatrix(pos, 0, 0, 0, 1));

                        GL.DrawElements(BeginMode.Triangles, model.rawModel.vertexes, DrawElementsType.UnsignedInt, 0);
                    }
                }

                finishRendering();
                model.shader.stop();
            }
        }

        public void setBlock(EnumBlock blockType, Vector3 pos)
        {
            if (blockType == EnumBlock.SELECTION)
                return;

            lock (blocks)
            {
                var keys = blocks.Keys.ToArray();

                var vecI = new Vector3((float)Math.Floor(pos.X), (float)Math.Floor(pos.Y), (float)Math.Floor(pos.Z));

                //remove block at position
                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];

                    if (blocks.TryGetValue(key, out var positions))
                    {
                        if (positions.Contains(vecI))
                            positions.Remove(vecI);
                    }
                }

                if (blockType == EnumBlock.AIR)
                    return;

                if (!blocks.ContainsKey(blockType))
                    blocks.Add(blockType, new List<Vector3>());

                if (blocks.TryGetValue(blockType, out var list))
                {
                    list.Add(vecI);
                }
            }
        }

        public EnumBlock getBlock(Vector3 pos)
        {
            lock (blocks)
            {
                var keys = blocks.Keys.ToArray();

                var vecI = new Vector3((float)Math.Floor(pos.X), (float)Math.Floor(pos.Y), (float)Math.Floor(pos.Z));

                //remove block at position
                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];

                    if (blocks.TryGetValue(key, out var positions))
                    {
                        if (positions.Contains(vecI))
                            return key;
                    }
                }

                return EnumBlock.AIR;
            }
        }
    }
}
