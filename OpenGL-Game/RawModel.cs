namespace OpenGL_Game
{
    class RawModel
    {
        public int vaoID { get; }
        public int vertexes { get; }

        public RawModel(int vaoID, int vertexes)
        {
            this.vaoID = vaoID;
            this.vertexes = vertexes;
        }
    }
}
