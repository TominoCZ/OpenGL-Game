using OpenTK;
using OpenTK.Input;

namespace OpenGL_Game
{
    class EntityPlayerSP : Entity
    {
        public Camera camera;

        public float maxMoveSpeed = 0.275f;
        public float moveSpeed;

        private bool wasSpaceDown;

        private Item equipped;

        public EntityPlayerSP(Vector3 pos) : base(pos)
        {
            camera = new Camera();
            camera.pos = pos;

            boundingBox = new AxisAlignedBB(new Vector3(-0.25f, 0, -0.25f), new Vector3(0.25f, 2, 0.25f)).offset(pos);
        }

        public override void Update()
        {
            if (Game.INSTANCE.Focused)
                UpdateCamera();

            base.Update();
        }

        public override void Render(float particalTicks)
        {
            var interpolatedPos = lastPos + (pos - lastPos) * particalTicks;

            camera.pos = interpolatedPos + Vector3.UnitY * 1.725f;

            var state = Keyboard.GetState();

            if (state.IsKeyDown(Key.Space) && !wasSpaceDown && onGround)
            {
                wasSpaceDown = true;
                motion.Y = 0.45F;
            }
            else if ((!state.IsKeyDown(Key.Space) || onGround) && wasSpaceDown)
                wasSpaceDown = false;
        }

        private void UpdateCamera()
        {
            if (Game.INSTANCE.guiScreen != null)
                return;

            var state = Keyboard.GetState();

            Vector2 vec = Vector2.Zero;

            if (state.IsKeyDown(Key.W))
                vec += camera.forward;
            if (state.IsKeyDown(Key.S))
                vec += -camera.forward;

            if (state.IsKeyDown(Key.A))
                vec += camera.left;
            if (state.IsKeyDown(Key.D))
                vec += -camera.left;

            if (vec != Vector2.Zero)
            {
                moveSpeed = MathHelper.Clamp(moveSpeed + 0.0875f, 0, maxMoveSpeed);
                motion.Xz = vec.Normalized() * moveSpeed;
            }
            else
                moveSpeed = 0;
        }

        public Item getEquippedItem()
        {
            return equipped;
        }

        public void setEquippedItem(Item i)
        {
            equipped = i;
        }
    }

    abstract class Item
    {
        protected object item { get; }

        string displayName { get; }

        protected Item(string displayName, object item)
        {
            this.item = item;
            this.displayName = displayName;
        }
    }

    class ItemBlock : Item
    {
        public ItemBlock(EnumBlock block) : base(block.ToString(), block)
        {

        }

        public EnumBlock getBlock()
        {
            return (EnumBlock)item;
        }
    }

    enum EnumItem
    {

    }
}
