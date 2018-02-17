using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace OpenGL_Game
{
    class EntityPlayerSP : Entity
    {
        public Camera camera;
        public float moveSpeed = 0.25f;

        private bool wasSpaceDown;

        public EntityPlayerSP(Vector3 pos) : base(pos)
        {
            camera = new Camera();
            camera.pos = pos;

            boundingBox = new AxisAlignedBB(Vector3.Zero, new Vector3(0.25f, 2, 0.25f));
        }

        public override void Update()
        {
            lastPos = new Vector3(pos);

            if (Game.INSTANCE.Focused)
                UpdateCamera();

            motion.Xz *= 0.9124021f;

            Move();

            if (!OnGround)
                motion.Y -= 0.075f;

            if (OnGround)
            {
                pos.Y = (int)pos.Y;
                motion.Xz *= 0.4221f;
            }
        }

        public override void Render(float particalTicks)
        {
            var interpolatedPos = lastPos + (pos - lastPos) * particalTicks;

            camera.pos = interpolatedPos + Vector3.UnitY * 1.725f;

            var state = Keyboard.GetState();

            if (state.IsKeyDown(Key.Space) && !wasSpaceDown && OnGround)
            {
                wasSpaceDown = true;
                motion.Y = 0.475F;
            }
            else if ((!state.IsKeyDown(Key.Space) || OnGround) && wasSpaceDown)
                wasSpaceDown = false;
        }

        private void UpdateCamera()
        {
            var state = Keyboard.GetState();

            Vector2 vec = Vector2.Zero;

            if (state.IsKeyDown(Key.W))
            {
                var delta = camera.forward;

                vec.X += delta.X * moveSpeed;
                vec.Y += delta.Y * moveSpeed;
            }
            else if (state.IsKeyDown(Key.S))
            {
                var delta = -camera.forward;

                vec.X += delta.X * moveSpeed;
                vec.Y += delta.Y * moveSpeed;
            }

            if (state.IsKeyDown(Key.A))
            {
                var delta = camera.left;

                vec.X += delta.X * moveSpeed;
                vec.Y += delta.Y * moveSpeed;
            }
            else if (state.IsKeyDown(Key.D))
            {
                var delta = -camera.left;

                vec.X += delta.X * moveSpeed;
                vec.Y += delta.Y * moveSpeed;
            }

            if (vec != Vector2.Zero)
                motion.Xz = vec;

            //check for block collisions
            for (int yOffset = 0; yOffset <= 1; yOffset++)
            {
                if (Game.INSTANCE.world.getBlock(new BlockPos(pos + new Vector3(motion.X + (motion.X > 0 ? 0.25f : -0.25f), yOffset, 0))) != EnumBlock.AIR)
                {
                    if (motion.X < 0)
                        pos.X = (float)Math.Floor(pos.X);
                    else
                        pos.X = (float)Math.Ceiling(pos.X);

                    pos.X += (motion.X > 0 ? -0.25f : 0.25f);

                    motion.X = 0;
                }

                if (Game.INSTANCE.world.getBlock(new BlockPos(pos + new Vector3(0, yOffset, motion.Z + (motion.Z > 0 ? 0.25f : -0.25f)))) != EnumBlock.AIR)
                {
                    if (motion.Z < 0)
                        pos.Z = (float)Math.Floor(pos.Z);
                    else
                        pos.Z = (float)Math.Ceiling(pos.Z);

                    pos.Z += (motion.Z > 0 ? -0.25f : 0.25f);

                    motion.Z = 0;
                }
            }

            if (Game.INSTANCE.world.getBlock(new BlockPos(pos + Vector3.UnitY * (motion.Y + 1.7f))) !=
                    EnumBlock.AIR && motion.Y > 0)
            {
                pos.Y = (int)Math.Ceiling(pos.Y);
                motion.Y = 0;
            }

            /*if (state.IsKeyDown(Key.LControl) || state.IsKeyDown(Key.LShift))
            {
                motion.Y = -moveSpeed;
            }*/
        }

        public Item getEquippedItem()
        {
            return new ItemBlock(EnumBlock.STONE);
        }

        public void setEquippedItem()
        {

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
