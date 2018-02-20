namespace OpenGL_Game
{
    class BlockModel : IModel
    {
        public IRawModel rawModel { get; }

        public ShaderProgram shader { get; }

        public EnumBlock block { get; }

        public AxisAlignedBB boundingBox { get; }

        public bool canBeInteractedWith { get; }

        public BlockModel(EnumBlock block, ShaderProgram shader, bool canBeInteractedWith)
        {
            this.shader = shader;
            this.block = block;
            this.canBeInteractedWith = canBeInteractedWith;

            var cube = ModelManager.createTexturedCubeModel(block);

            rawModel = GraphicsManager.loadBlockModelToVAO(cube);

            boundingBox = AxisAlignedBB.BLOCK_FULL;
        }

        public BlockModel(EnumBlock block, ShaderProgram shader, AxisAlignedBB bb, bool canBeInteractedWith) : this(block, shader, canBeInteractedWith)
        {
            boundingBox = bb;
        }
    }
}