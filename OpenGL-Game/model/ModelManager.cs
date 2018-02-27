using System.Collections.Generic;
using System.Net.Configuration;
using System.Resources;
using System.Runtime.CompilerServices;

namespace OpenGL_Game
{
    class ModelManager
    {
        private static Dictionary<EnumBlock, List<BlockState>> models = new Dictionary<EnumBlock, List<BlockState>>();

        private static Dictionary<EnumFacing, float[]> CUBE = new Dictionary<EnumFacing, float[]>();

        static ModelManager()
        {
            CUBE.Add(EnumFacing.NORTH, new float[]
            {
                1, 1, 0,
                1, 0, 0,
                0, 0, 0,
                0, 1, 0
            });
            CUBE.Add(EnumFacing.SOUTH, new float[]
            {
                0, 1, 1,
                0, 0, 1,
                1, 0, 1,
                1, 1, 1
            });
            CUBE.Add(EnumFacing.EAST, new float[]
            {
                1, 1, 1,
                1, 0, 1,
                1, 0, 0,
                1, 1, 0
            });
            CUBE.Add(EnumFacing.WEST, new float[]
            {
                0, 1, 0,
                0, 0, 0,
                0, 0, 1,
                0, 1, 1
            });
            CUBE.Add(EnumFacing.UP, new float[]
            {
                0, 1, 0,
                0, 1, 1,
                1, 1, 1,
                1, 1, 0
            });
            CUBE.Add(EnumFacing.DOWN, new float[]
            {
                0, 0, 1,
                0, 0, 0,
                1, 0, 0,
                1, 0, 1
            });
        }

        public static void registerBlockModel(BlockModel model, int meta)
        {
            List<BlockState> states;
            
            //if already contains state with this meta tag, remove and set a new one
            if (models.TryGetValue(model.block, out states))
            {
                for (int i = 0; i < states.Count; i++)
                {
                    var state = states[i];
                    
                    if (state.meta == meta)
                    {
                        states.Remove(state);
                        break;
                    }
                }
            }
            else
            {
                models.Add(model.block, states = new List<BlockState>());
            }
            
            states.Add(new BlockState(model, meta));
        }

        public static BlockModel getModelForBlock(EnumBlock blockType, int meta)
        {
            models.TryGetValue(blockType, out var states);

            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];

                if (state.meta == meta)
                    return state.Model;
            }
            
            return null;
        }

        public static Dictionary<EnumFacing, RawQuad> createTexturedCubeModel(EnumBlock block)
        {
            var quads = new Dictionary<EnumFacing, RawQuad>();
            var uvs = TextureManager.getUVsFromBlock(block);

            foreach (var face in CUBE.Keys)
            {
                if (CUBE.TryGetValue(face, out var data))
                {
                    var uvNode = uvs.getUVForSide(face);

                    if (uvNode != null)
                        quads.Add(face, new RawQuad(data, uvNode.ToArray(), ModelHelper.calculateNormals(data)));
                }
            }

            return quads;
        }

        public static List<RawQuad> createCubeModel()
        {
            var quads = new List<RawQuad>();

            foreach (var face in CUBE.Keys)
            {
                if (CUBE.TryGetValue(face, out var vertices))
                {
                    quads.Add(new RawQuad(vertices));
                }
            }

            return quads;
        }

        public static List<RawQuad> createCubeModelFace(EnumFacing face)
        {
            var quads = new List<RawQuad>();

            if (CUBE.TryGetValue(face, out var vertices))
            {
                quads.Add(new RawQuad(vertices));
            }

            return quads;
        }
    }

    class BlockState
    {
        public BlockModel Model { get; }
        public int meta { get; }

        public BlockState(BlockModel model, int meta)
        {
            Model = model;
            this.meta = meta;
        }
    }
}