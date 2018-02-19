using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class TextureManager
    {
        public static int blockTextureAtlasID;

        private static Dictionary<EnumBlock, BlockTextureUV> UVs = new Dictionary<EnumBlock, BlockTextureUV>();

        public static void stitchTextures()
        {
            blockTextureAtlasID = GraphicsManager.loadTexture(generateTextureMap());
        }

        private static Bitmap generateTextureMap()
        {
            Bitmap map = new Bitmap(256, 256);

            var blocks = Enum.GetValues(typeof(EnumBlock));
            var sides = Enum.GetValues(typeof(EnumFacing));

            var dir = "assets/textures/blocks/";
            var files = Directory.GetFiles("assets/textures/blocks", "*.png");

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]).ToLower();
            }

            int countX = 0;
            int countY = 0;

            var size = new Vector2(16f / map.Size.Width, 16f / map.Size.Height);

            using (map)
            {
                foreach (EnumBlock block in blocks)
                {
                    var name = block.ToString().ToLower();

                    if (containsContaining(files, name))
                    {
                        var uvs = new BlockTextureUV();

                        if (files.Contains(name))
                        {
                            if (countX * 16 >= map.Size.Width)
                            {
                                countX = 0;
                                countY++;
                            }

                            var pos = new Vector2(countX * size.X, countY * size.Y);
                            var end = pos + size;

                            uvs.fill(pos, end);

                            using (var bmp = Image.FromFile(dir + name + ".png"))
                            {
                                using (var g = Graphics.FromImage(map))
                                {
                                    g.DrawImage(bmp, countX * 16, countY * 16, 16, 16);
                                }
                            }

                            countX++;
                        }

                        var textureName = "";

                        if (files.Contains(textureName = name + "_side"))
                        {
                            if (countX * 16 >= map.Size.Width)
                            {
                                countX = 0;
                                countY++;
                            }

                            var pos = new Vector2(countX * size.X, countY * size.Y);
                            var end = pos + size;

                            uvs.setUVForSide(EnumFacing.NORTH, pos, end);
                            uvs.setUVForSide(EnumFacing.SOUTH, pos, end);
                            uvs.setUVForSide(EnumFacing.WEST, pos, end);
                            uvs.setUVForSide(EnumFacing.EAST, pos, end);

                            using (var bmp = Image.FromFile(dir + textureName + ".png"))
                            {
                                using (var g = Graphics.FromImage(map))
                                {
                                    g.DrawImage(bmp, countX * 16, countY * 16, 16, 16);
                                }
                            }

                            countX++;
                        }

                        foreach (EnumFacing side in sides)
                        {
                            var sideName = side.ToString().ToLower();

                            if (files.Contains(textureName = name + "_" + sideName))
                            {
                                if (countX * 16 >= map.Size.Width)
                                {
                                    countX = 0;
                                    countY++;
                                }

                                var pos = new Vector2(countX * size.X, countY * size.Y);
                                var end = pos + size;

                                uvs.setUVForSide(side, pos, end);

                                using (var bmp = Image.FromFile(dir + textureName + ".png"))
                                {
                                    using (var g = Graphics.FromImage(map))
                                    {
                                        g.DrawImage(bmp, countX * 16, countY * 16, 16, 16);
                                    }
                                }

                                countX++;
                            }
                        }

                        UVs.Add(block, uvs);
                    }
                }

                map.Save("terrain_debug.png");

                return (Bitmap)map.Clone();
            }
        }

        public static BlockTextureUV getUVsFromBlock(EnumBlock block)
        {
            BlockTextureUV uv;

            UVs.TryGetValue(block, out uv);

            if (uv == null)
                UVs.TryGetValue(EnumBlock.MISSING, out uv);

            return uv;
        }

        private static bool containsContaining(Array a, string s)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (((string)a.GetValue(i)).Contains(s))
                    return true;
            }

            return false;
        }

        public static void cleanUp()
        {
            GL.DeleteTexture(blockTextureAtlasID);
        }
    }
}
