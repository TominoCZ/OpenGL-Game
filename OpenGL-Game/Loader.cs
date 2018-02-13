using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    class Loader
    {
        List<int> VAOs = new List<int>();
        List<int> VBOs = new List<int>();
        List<int> textures = new List<int>();

        public RawModel loadToVAO(float[] positions, float[] UVs, int[] indices)
        {
            int vaoID = createVAO();

            bindIndicesBuffer(indices);
            storeDataInAttributeList(0, 3, positions);
            storeDataInAttributeList(1, 2, UVs);
            unbindVAO();

            return new RawModel(vaoID, indices.Length);
        }

        public int loadTexture(string file)
        {
            int texID;

            var bmp = new Bitmap("res/" + file + ".png");

            texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);

            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Size.Width, bmp.Size.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, bmp.Size.Width, bmp.Size.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, bmpData.Scan0);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new[] { (int)TextureMagFilter.Linear });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new[] { (int)TextureMagFilter.Nearest });

            textures.Add(texID);

            return texID;
        }

        public void cleanUp()
        {
            foreach (var item in VAOs)
            {
                GL.DeleteVertexArray(item);
            }
            foreach (var item in VBOs)
            {
                GL.DeleteBuffer(item);
            }
            foreach (var item in textures)
            {
                GL.DeleteTexture(item);
            }
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

        private void storeDataInAttributeList(int attrib, int coordSize, float[] data)
        {
            int vboID = GL.GenBuffer();

            VBOs.Add(vboID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attrib, coordSize, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void unbindVAO()
        {
            GL.BindVertexArray(0);
        }
    }
}
