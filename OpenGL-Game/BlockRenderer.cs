using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GL = OpenTK.Graphics.OpenGL.GL;
using TextureUnit = OpenTK.Graphics.OpenGL.TextureUnit;
using TextureTarget = OpenTK.Graphics.OpenGL.TextureTarget;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class BlockRenderer
    {
        private Dictionary<Model, List<BlockNode>> blocks = new Dictionary<Model, List<BlockNode>>();

        private void prepareModel(Model m)
        {
            GL.BindVertexArray(m.rawModel.vaoID);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, m.texture.textureID);
        }

        private void finishModel(Model m)
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
                var model = keys[i];

                prepareModel(model);

                model.shader.start();
                model.shader.loadViewMatrix(c);

                if (blocks.TryGetValue(model, out var list))
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        var node = list[j];

                        var mat = MatrixHelper.createTransformationMatrix(node.translation, node.rx, node.ry, node.rz, node.scale);
                        model.shader.loadTransformationMatrix(mat);

                        GL.DrawElements(BeginMode.Triangles, model.rawModel.vertexes, DrawElementsType.UnsignedInt, 0);
                    }
                }

                finishModel(model);
                model.shader.stop();
            }
        }

        public void addBlockNodes(params BlockNode[] nodes)
        {
            lock (blocks)
            {
                foreach (var n in nodes)
                {
                    blocks.TryGetValue(n.model, out var list);

                    if (list == null)
                        blocks.Add(n.model, list = new List<BlockNode>());

                    lock (list)
                    {
                        list.Add(n);
                    }
                }
            }
        }
    }
}
