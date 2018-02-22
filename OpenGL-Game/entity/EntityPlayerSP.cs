using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Input;

namespace OpenGL_Game
{
    class EntityPlayerSP : Entity
    {
        public Camera camera;

        public float maxMoveSpeed = 0.25f;
        public float moveSpeed;

        public int equippedItemHotbarIndex { get; private set; }

        public Item[] hotbar { get; }

        public EntityPlayerSP(Vector3 pos) : base(pos)
        {
            camera = new Camera();
            camera.pos = pos;

            boundingBox = new AxisAlignedBB(Vector3.Zero, new Vector3(0.5f, 1.75f, 0.5f)).offset(pos);

            hotbar = new Item[9];
        }

        public EntityPlayerSP() : this(Vector3.Zero)
        {

        }

        public override void Update()
        {
            if (Game.INSTANCE.Focused)
                UpdateCameraMovement();

            base.Update();
        }

        public override void Render(float particalTicks)
        {
            var interpolatedPos = lastPos + (pos - lastPos) * particalTicks;

            camera.pos = interpolatedPos + Vector3.UnitY * 1.625f;
        }

        private void UpdateCameraMovement()
        {
            if (Game.INSTANCE.guiScreen != null)
                return;

            var state = Keyboard.GetState();

            Vector2 dirVec = Vector2.Zero;

            if (state.IsKeyDown(Key.W))
                dirVec += camera.forward;
            if (state.IsKeyDown(Key.S))
                dirVec += -camera.forward;

            if (state.IsKeyDown(Key.A))
                dirVec += camera.left;
            if (state.IsKeyDown(Key.D))
                dirVec += -camera.left;

            float mult = 1;

            if (state.IsKeyDown(Key.LShift))
                mult = 1.5f;

            if (dirVec != Vector2.Zero)
            {
                moveSpeed = MathHelper.Clamp(moveSpeed + 0.095f, 0, maxMoveSpeed) * mult;

                motion.Xz = dirVec.Normalized() * moveSpeed;
            }
            else
                moveSpeed = 0;
        }

        public void setItemInHotbar(int index, Item item)
        {
            hotbar[index % 8] = item;
        }

        public void setItemInSelectedSlot(Item item)
        {
            hotbar[equippedItemHotbarIndex] = item;
        }

        public Item getEquippedItem()
        {
            return hotbar[equippedItemHotbarIndex];
        }

       /* public void setEquippedItem(Item i)
        {
            var index = hotbar.ToList().IndexOf(i);

            if (index != -1)
                equippedItemHotbarIndex = index;
        }*/

        public void selectNextItem()
        {
            equippedItemHotbarIndex = (equippedItemHotbarIndex + 1) % 9;
        }

        public void selectPreviousItem()
        {
            if (equippedItemHotbarIndex <= 0)
                equippedItemHotbarIndex = 8;
            else
                equippedItemHotbarIndex = equippedItemHotbarIndex - 1;
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
