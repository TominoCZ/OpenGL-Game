using System.Data;
using OpenTK;

namespace OpenGL_Game
{
    public class RawQuad
    {
        public float[] vertices { get; }
        public float[] normal { get; }
        public float[] UVs { get; }

        public RawQuad(float[] vertices, float[] UVs, float[] normal)
        {
            this.vertices = vertices;
            this.normal = normal;
            this.UVs = UVs;
        }

        public RawQuad offset(int x, int y, int z)
        {
            float[] newVertices = new float[vertices.Length];

            for (int i = 0; i < newVertices.Length; i += 3)
            {
                newVertices[i] = vertices[i] + x;
                newVertices[i + 1] = vertices[i + 1] + y;
                newVertices[i + 2] = vertices[i + 2] + z;
            }

            return new RawQuad(newVertices, UVs, normal);
        }
    }
}
