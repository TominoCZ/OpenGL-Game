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
using System.Xml.Schema;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class TextureManager
    {
        private static Dictionary<EnumBlock, BlockTextureUV> UVs;

        static TextureManager()
        {
            UVs = new Dictionary<EnumBlock, BlockTextureUV>();
        }

        public static int blockTextureAtlasID;

        public static void stitchTextures()
        {
            blockTextureAtlasID = GraphicsManager.loadTexture(generateTextureMap(), false);
        }

        private static Bitmap generateTextureMap()
        {
            Bitmap map = new Bitmap(256, 256);

            var blocks = Enum.GetValues(typeof(EnumBlock));
            var sides = Enum.GetValues(typeof(EnumFacing));

            var dir = "assets/textures/blocks";

            var files = new string[0];
            if (Directory.Exists(dir))
                files = Directory.GetFiles("assets/textures/blocks", "*.png");

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]).ToLower();
            }

            int countX = 0;
            int countY = 0;

            var size = new Vector2(16f / map.Size.Width, 16f / map.Size.Height);

            using (map)
            {
                var faces = (EnumFacing[])Enum.GetValues(typeof(EnumFacing));

                foreach (EnumBlock block in blocks)
                {
                    var texName = block.ToString().ToLower();
                    var uvs = new BlockTextureUV();

                    if (containsContaining(files, texName) > 0)
                    {
                        //found

                        //found texture for all 6 sides
                        if (files.Contains(texName))
                        {
                            var pos = new Vector2(countX * size.X, countY * size.Y);
                            var end = pos + size;

                            uvs.fill(pos, end);

                            drawToBitmap(map, countX * 16, countY * 16, $"{dir}/{texName}.png");

                            countX++;

                            //check
                            if (countX * 16 >= map.Size.Width)
                            {
                                countX = 0;
                                countY++;
                            }
                        }

                        //found texture for the 4 sides
                        if (files.Contains($"{texName}_side"))
                        {
                            var pos = new Vector2(countX * size.X, countY * size.Y);
                            var end = pos + size;

                            uvs.setUVForSide(EnumFacing.NORTH, pos, end);
                            uvs.setUVForSide(EnumFacing.EAST, pos, end);
                            uvs.setUVForSide(EnumFacing.SOUTH, pos, end);
                            uvs.setUVForSide(EnumFacing.WEST, pos, end);

                            drawToBitmap(map, countX * 16, countY * 16, $"{dir}/{texName}_side.png");

                            countX++;

                            //check
                            if (countX * 16 >= map.Size.Width)
                            {
                                countX = 0;
                                countY++;
                            }
                        }

                        for (var index = 0; index < faces.Length; index++)
                        {
                            var face = faces[index];

                            var faceName = face.ToString().ToLower();

                            if (files.Contains($"{texName}_{faceName}"))
                            {
                                var pos = new Vector2(countX * size.X, countY * size.Y);
                                var end = pos + size;

                                uvs.setUVForSide(face, pos, end);

                                drawToBitmap(map, countX * 16, countY * 16, $"{dir}/{texName}_{faceName}.png");

                                countX++;

                                //check
                                if (countX * 16 >= map.Size.Width)
                                {
                                    countX = 0;
                                    countY++;
                                }
                            }
                        }
                    }

                    UVs.Add(block, uvs);
                }

                var missingUVs = getUVsFromBlock(EnumBlock.MISSING);

                if (missingUVs != null)
                {
                    foreach (var uv in UVs)
                    {
                        if (uv.Key != EnumBlock.AIR)
                            uv.Value.fillEmptySides(missingUVs.getUVForSide(EnumFacing.DOWN));
                    }
                }

                #region fuj
                /*
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

                        Uv.Add(block, uvs);
                    }
                }*/
                #endregion
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

        private static void drawToBitmap(Bitmap to, int x, int y, string file)
        {
            using (var bmp = Image.FromFile(file))
            {
                using (var g = Graphics.FromImage(to))
                {
                    g.DrawImage(bmp, x, y, 16, 16);
                }
            }
        }

        private static int containsContaining(Array a, string s)
        {
            int res = 0;

            for (int i = 0; i < a.Length; i++)
            {
                if (((string)a.GetValue(i)).Contains(s))
                    res++;
            }

            return res;
        }

        public static void cleanUp()
        {
            GL.DeleteTexture(blockTextureAtlasID);
        }
    }
}
