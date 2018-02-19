using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenGL_Game
{
    class GraphicsManager
    {
        static List<int> VAOs = new List<int>();
        static List<int> VBOs = new List<int>();
        static List<int> textures = new List<int>();

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

        public static RawModel loadModelToVAO(List<RawQuad> quads, int coordSize)
        {
            int vaoID = createVAO();

            List<float> vertices = new List<float>();
            List<float> normals = new List<float>();
            List<float> UVs = new List<float>();

            for (var index = 0; index < quads.Count; index++)
            {
                var quad = quads[index];

                vertices.AddRange(quad.vertices);
                normals.AddRange(quad.normal);
                UVs.AddRange(quad.UVs);
            }

            storeDataInAttributeList(0, coordSize, vertices.ToArray());

            if (UVs.Count > 0)
                storeDataInAttributeList(1, 2, UVs.ToArray());
            if (normals.Count > 0)
                storeDataInAttributeList(2, 3, normals.ToArray());

            unbindVAO();

            return new RawModel(vaoID, coordSize, quads);
        }

        public static int loadTexture(Bitmap textureMap, bool smooth)
        {
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            int texID = GL.GenTexture();
            textures.Add(texID);

            GL.BindTexture(TextureTarget.Texture2D, texID);

            BitmapData data = textureMap.LockBits(new Rectangle(0, 0, textureMap.Width, textureMap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            textureMap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)(smooth ? TextureMinFilter.Linear : TextureMinFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)(smooth ? TextureMagFilter.Linear : TextureMagFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int)TextureWrapMode.MirroredRepeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)TextureWrapMode.MirroredRepeat);

            return texID;
        }

        public static Texture loadTexture(string textureName, bool smooth)
        {
            try
            {
                var bmp = (Bitmap)Image.FromFile($"assets/textures/{textureName}.png");

                int id = loadTexture(bmp, smooth);

                return new Texture(id, bmp.Size);
            }
            catch
            {

            }

            Console.WriteLine($"Error: the texture '{textureName}' failed to load!");
            return null;
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
            foreach (var item in textures)
            {
                GL.DeleteTexture(item);
            }

            GL.DeleteTexture(TextureManager.blockTextureAtlasID);
        }

        public static void deleteVAO(int vaoID)
        {
            GL.DeleteVertexArray(vaoID);
        }
    }

    class Texture
    {
        public int textureID { get; }
        public Size textureSize { get; }

        public Texture(int textureID, Size textureSize)
        {
            this.textureID = textureID;
            this.textureSize = textureSize;
        }
    }
}
