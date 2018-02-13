using System.Collections.Generic;

namespace OpenGL_Game
{
    class Model
    {
        private List<float> vertices;
        private List<int> indices;

        private RawModel model;

        private bool baked;

        public Model()
        {
            vertices = new List<float>();
            indices = new List<int>();
        }

        public void addVertices(params float[] vertices)
        {
            this.vertices.AddRange(vertices);
        }

        public void addIndices(params int[] indices)
        {
            this.indices.AddRange(indices);
        }

        public bool bake(Loader l)
        {
            if (baked || vertices.Count < 3 || indices.Count < 3)
                return false;

            model = l.loadToVAO(vertices.ToArray(), indices.ToArray());

            return baked = true;
        }

        public RawModel getRaw()
        {
            return model;
        }
    }
}
