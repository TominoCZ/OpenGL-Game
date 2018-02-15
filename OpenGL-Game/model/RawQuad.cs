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
    }
}
