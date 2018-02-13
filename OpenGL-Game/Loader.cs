using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    class Loader
    {
        List<int> VAOs = new List<int>();
        List<int> VBOs = new List<int>();

        public RawModel loadToVAO(float[] positions, int[] indices)
        {
            int vaoID = createVAO();

            bindIndicesBuffer(indices);
            storeDataInAttributeList(0, positions);
            unbindVAO();

            return new RawModel(vaoID, indices.Length);
        }

        private int createVAO()
        {
            int vaoID = GL.GenVertexArray();

            VAOs.Add(vaoID);
            GL.BindVertexArray(vaoID);

            return vaoID;
        }

        private void bindIndicesBuffer(int[] indices)
        {
            int vboID = GL.GenBuffer();
            VBOs.Add(vboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticDraw);
        }

        private void storeDataInAttributeList(int attrib, float[] data)
        {
            int vboID = GL.GenBuffer();
            VBOs.Add(vboID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attrib, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void unbindVAO()
        {
            GL.BindVertexArray(0);
        }
    }
}
