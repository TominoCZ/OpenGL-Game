using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    class Loader
    {
        static List<int> VAOs = new List<int>();
        static List<int> VBOs = new List<int>();

        public static RawBlockModel loadBlockModelToVAO(Dictionary<EnumFacing, RawQuad> quads)
        {
            int vaoID = createVAO();

            List<float> vertices = new List<float>();
            List<float> normals = new List<float>();
            List<float> UVs = new List<float>();

            foreach (var q in quads)
            {
                vertices.AddRange(q.Value.vertices);
                normals.AddRange(q.Value.normal);
                UVs.AddRange(q.Value.UVs);
            }

            storeDataInAttributeList(0, 3, vertices.ToArray());
            storeDataInAttributeList(1, 2, UVs.ToArray());
            storeDataInAttributeList(2, 3, normals.ToArray());

            unbindVAO();

            return new RawBlockModel(vaoID, quads);
        }

        public static RawChunkModel loadChunkModelToVAO(List<RawQuad> model)
        {
            int vaoID = createVAO();

            List<float> vertices = new List<float>();
            List<float> normals = new List<float>();
            List<float> UVs = new List<float>();

            foreach (var quad in model)
            {
                vertices.AddRange(quad.vertices);
                normals.AddRange(quad.normal);
                UVs.AddRange(quad.UVs);
            }

            storeDataInAttributeList(0, 3, vertices.ToArray());
            storeDataInAttributeList(1, 2, UVs.ToArray());
            storeDataInAttributeList(2, 3, normals.ToArray());

            unbindVAO();

            return new RawChunkModel(vaoID, model);
        }

        private static void storeDataInAttributeList(int attrib, int coordSize, float[] data)
        {
            int vboID = GL.GenBuffer();

            VBOs.Add(vboID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attrib, coordSize, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private static int createVAO()
        {
            int vaoID = GL.GenVertexArray();

            VAOs.Add(vaoID);
            GL.BindVertexArray(vaoID);

            return vaoID;
        }

        private static void unbindVAO()
        {
            GL.BindVertexArray(0);
        }

        public static void cleanUp()
        {
            foreach (var item in VAOs)
            {
                deleteVAO(item);
            }
            foreach (var item in VBOs)
            {
                GL.DeleteBuffer(item);
            }

            GL.DeleteTexture(TextureRegistry.textureAtlasID);
        }

        public static void deleteVAO(int vaoID)
        {
            GL.DeleteVertexArray(vaoID);
        }
    }
}
