using OpenTK;

namespace OpenGL_Game
{
    class Entity
    {
        public Vector3 pos;
        public Vector3 lastPos;

        public Vector3 motion;

        public bool OnGround;

        protected Entity(Vector3 pos)
        {
            this.pos = pos;
        }

        public virtual void Update()
        {
            lastPos = new Vector3(pos);

            motion.Xz *= 0.8664021f;

            Move();

            if (!OnGround)
                motion.Y -= 0.075f;

            if (OnGround)
            {
                pos.Y = (int)pos.Y;
                motion.Xz *= 0.6776801f;
            }
        }

        public virtual void Move()
        {
            if (Game.INSTANCE.world.getBlock(new BlockPos(pos + motion)) != EnumBlock.AIR)
            {
                OnGround = motion.Y < 0;
            }
            else
            {
                OnGround = false;
            }

            if (OnGround)
                motion.Y = 0;
            else
                pos.Y += motion.Y;

            pos.Xz += motion.Xz;
        }

        public virtual void Render(float particalTicks)
        {

        }
    }
}