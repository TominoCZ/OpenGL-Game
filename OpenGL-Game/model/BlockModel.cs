namespace OpenGL_Game
{
    class BlockModel : IModel
    {
        public IRawModel rawModel { get; }

        public ShaderProgram shader { get; }

        public EnumBlock block { get; }

        public AxisAlignedBB boundingBox { get; }

        public BlockModel(EnumBlock block, ShaderProgram shader)
        {
            this.shader = shader;
            this.block = block;

            var cube = ModelManager.createTexturedCubeModel(block);

            rawModel = GraphicsManager.loadBlockModelToVAO(cube);

            boundingBox = AxisAlignedBB.BLOCK_FULL;
        }

        public BlockModel(EnumBlock block, ShaderProgram shader, AxisAlignedBB bb) : this(block, shader)
        {
            boundingBox = bb;
        }
    }
}