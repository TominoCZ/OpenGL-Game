namespace OpenGL_Game
{
    internal class ChunkData
    {
        public Chunk chunk;
        public ChunkModel model;

        public bool modelGenerated;
        public bool chunkGenerated;

        public ChunkData(Chunk chunk, ChunkModel model)
        {
            this.chunk = chunk;
            this.model = model;
        }
    }
}