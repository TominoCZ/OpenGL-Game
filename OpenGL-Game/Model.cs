using System.Collections.Generic;

namespace OpenGL_Game
{
    class Model
    {
        private List<float> UVs;

        private List<float> vertices;
        private List<int> indices;

        private RawModel model;
        private ModelTexture texture;
        
        private bool baked;

        public Model(ModelTexture texture)
        {
            UVs = new List<float>();

            vertices = new List<float>();
            indices = new List<int>();

            this.texture = texture;
        }

        public void addVertices(params float[] vertices)
        {
            this.vertices.AddRange(vertices);
        }

        public void addIndices(params int[] indices)
        {
            this.indices.AddRange(indices);
        }

        public void addUV(params float[] UVs)
        {
            this.UVs.AddRange(UVs);
        }

        public bool bake(Loader l)
        {
            if (baked || vertices.Count < 3 || indices.Count < 3)
                return false;

            model = l.loadToVAO(vertices.ToArray(), UVs.ToArray(), indices.ToArray());

            return baked = true;
        }

        public RawModel getRaw()
        {
            return model;
        }

        public ModelTexture getTexture()
        {
            return texture;
        }
    }
}
