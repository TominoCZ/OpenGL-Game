using OpenTK;

namespace OpenGL_Game
{
    class RawQuad
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

        public RawQuad(float[] vertices, float[] UVs, float nx, float ny, float nz) : this(vertices, UVs, new[] { nx, ny, nz })
        {
        }

        public RawQuad(float[] vertices, float[] UVs, Vector3 normal) : this(vertices, UVs, new[] { normal.X, normal.Y, normal.Z })
        {
        }
    }
}
