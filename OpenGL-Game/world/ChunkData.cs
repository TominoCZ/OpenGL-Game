namespace OpenGL_Game
{
    class ChunkData
    {
        public Chunk chunk;
        public ChunkModel model;

        public ChunkData(Chunk chunk, ChunkModel model)
        {
            this.chunk = chunk;
            this.model = model;
        }
    }
}