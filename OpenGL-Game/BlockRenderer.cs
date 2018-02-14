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

        private void beginRendering(BlockModel m)
        {
            GL.BindVertexArray(m.rawModel.vaoID);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, m.texture.textureID);
        }

        private void finishRendering()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }
        
        public void render(Camera c)
        {
            var keys = blocks.Keys.ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                EnumBlock blockType = keys[i];
                BlockModel model = ModelRegistry.getModelForBlock(blockType);

                beginRendering(model);

                model.shader.start();
                model.shader.loadViewMatrix(c);

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
            lock (blocks)
            {
                var keys = blocks.Keys.ToArray();

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

                if (!blocks.ContainsKey(blockType))
                    blocks.Add(blockType, new List<Vector3>());

                if (blocks.TryGetValue(blockType, out var list))
                {
                    list.Add(pos);
                }
            }
        }
    }
}
