using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OpenGL_Game.Properties;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL = OpenTK.Graphics.OpenGL.GL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace OpenGL_Game
{
    class GraphicsManager
    {
        static List<int> VAOs = new List<int>();
        static ThreadSafeList<int> VBOs = new ThreadSafeList<int>();
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

            var buff0 = storeDataInAttributeList(0, coordSize, vertices.ToArray());
            var buff1 = storeDataInAttributeList(1, 2, UVs.ToArray());
            var buff2 = storeDataInAttributeList(2, 3, normals.ToArray());

            unbindVAO();

            return new RawModel(vaoID, new[] { buff0, buff1, buff2 }, coordSize, quads);
        }

        public static RawModel overrideModelInVAO(int ID, int[] buffers, List<RawQuad> quads, int coordSize)
        {
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

            overrideDataInAttributeList(buffers[0], 0, coordSize, vertices.ToArray());
            overrideDataInAttributeList(buffers[1], 1, 2, UVs.ToArray());
            overrideDataInAttributeList(buffers[2], 2, 3, normals.ToArray());

            return new RawModel(ID, buffers, coordSize, quads);
        }

        public static void overrideModelUVsInVAO(int bufferID, float[] UVs)
        {
            overrideDataInAttributeList(bufferID, 1, 2, UVs);
        }

        private static void overrideDataInAttributeList(int ID, int attrib, int coordSize, float[] data)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(attrib, coordSize, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private static int storeDataInAttributeList(int attrib, int coordSize, float[] data)
        {
            int vboID = GL.GenBuffer();

            VBOs.Add(vboID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(attrib, coordSize, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return vboID;
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
                (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)TextureWrapMode.ClampToEdge);

            return texID;
        }

        public static Texture loadTexture(string textureName, bool smooth)
        {
            try
            {
                var bmp = (Bitmap)Image.FromFile($"SharpCraft_Data/assets/textures/{textureName}.png");

                int id = loadTexture(bmp, smooth);

                return new Texture(id, bmp.Size);
            }
            catch
            {
                Console.WriteLine($"Error: the texture '{textureName}' failed to load!");
            }

            return new Texture(loadTexture(Resources.missing, smooth), Resources.missing.Size);
        }

        public static int loadCubeMap()
        {
            int texID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, texID);

            var cubeMapTextures = loadCubeMapTextures();

            foreach (var dictValues in cubeMapTextures)
            {
                var target = TextureTarget.Texture2D;

                switch (dictValues.Key)
                {
                    case EnumFacing.NORTH:
                        target = TextureTarget.TextureCubeMapNegativeZ;
                        break;
                    case EnumFacing.SOUTH:
                        target = TextureTarget.TextureCubeMapPositiveZ;
                        break;
                    case EnumFacing.EAST:
                        target = TextureTarget.TextureCubeMapPositiveX;
                        break;
                    case EnumFacing.WEST:
                        target = TextureTarget.TextureCubeMapNegativeX;
                        break;
                    case EnumFacing.UP:
                        target = TextureTarget.TextureCubeMapPositiveY;
                        break;
                    case EnumFacing.DOWN:
                        target = TextureTarget.TextureCubeMapNegativeY;
                        break;
                }

                var bmp = (Bitmap)dictValues.Value.Clone();
                var size = bmp.Size;

                BitmapData data = bmp.LockBits(new Rectangle(0, 0, size.Width, size.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(target, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, PixelFormat.Bgra,
                        PixelType.UnsignedByte, data.Scan0);

                bmp.UnlockBits(data);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            return texID;
        }

        private static Dictionary<EnumFacing, Bitmap> loadCubeMapTextures()
        {
            Dictionary<EnumFacing, Bitmap> bitmaps = new Dictionary<EnumFacing, Bitmap>();

            string[] files = new string[0];

            var dir = "SharpCraft_Data/assets/textures/skybox";

            if (Directory.Exists(dir))
                files = Directory.GetFiles(dir, "*.png");

            var sides = (EnumFacing[])Enum.GetValues(typeof(EnumFacing));

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i])?.ToLower();
            }

            foreach (var side in sides)
            {
                var sideName = side.ToString().ToLower();

                if (files.Contains($"sky_{sideName}"))
                {
                    var file = $"{dir}/sky_{sideName}.png";

                    bitmaps.Add(side, (Bitmap)Image.FromFile(file));
                }
                else
                {
                    bitmaps.Add(side, Resources.missing);
                }
            }

            return bitmaps;
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
            for (int i = 0; i < VBOs.Count; i++)
            {
                GL.DeleteBuffer(VBOs[i]);
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

        public static void deleteTexture(int vaoID)
        {
            GL.DeleteTexture(vaoID);
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
