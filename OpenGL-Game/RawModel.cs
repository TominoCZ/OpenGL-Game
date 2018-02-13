namespace OpenGL_Game
{
    class RawModel
    {
        private int vaoID;
        private int vertexes;

        public RawModel(int vaoID, int vertexes)
        {
            this.vaoID = vaoID;
            this.vertexes = vertexes;
        }

        public int getVertexCount()
        {
            return vertexes;
        }

        public int getVaoID()
        {
            return vaoID;
        }
    }
}
