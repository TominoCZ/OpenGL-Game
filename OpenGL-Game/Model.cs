using System.Collections.Generic;

namespace OpenGL_Game
{
    class Model
    {
        private List<float> vertices;
        private List<int> indices;
        private List<float> UVs;

        public RawModel rawModel{ get; private set; }

        public ModelTexture texture { get; }

        public ShaderProgram shader { get; }

        private bool baked;

        public Model(ModelTexture texture, ShaderProgram shader)
        {
            UVs = new List<float>();

            vertices = new List<float>();
            indices = new List<int>();

            this.texture = texture;
            this.shader = shader;
        }

        public void addVertices(params float[] vertices)
        {
            this.vertices.AddRange(vertices);
        }

        public void addIndices(params int[] indices)
        {
            this.indices.AddRange(indices);
        }

        public void addUVs(params float[] UVs)
        {
            this.UVs.AddRange(UVs);
        }

        public bool bake(Loader l)
        {
            if (baked || vertices.Count < 3 || indices.Count < 3)
                return false;

            rawModel = l.loadToVAO(vertices.ToArray(), UVs.ToArray(), indices.ToArray());

            return baked = true;
        }
    }
}
