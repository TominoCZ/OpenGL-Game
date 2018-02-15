namespace OpenGL_Game
{
    class RawModel
    {
        public int vaoID { get; }

        public float[] postitions { get; }
        public float[] UVs { get; }
        public int[] indices { get; }
        public float[] normals { get; }

        public RawModel(int vaoID, float[] postitions, float[] UVs, int[] indices, float[] normals)
        {
            this.vaoID = vaoID;

            this.postitions = postitions;
            this.UVs = UVs;
            this.indices = indices;
            this.normals = normals;
        }
    }
}
