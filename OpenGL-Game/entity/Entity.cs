using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;

namespace OpenGL_Game
{
    class Entity
    {
        protected AxisAlignedBB boundingBox;

        public Vector3 pos;
        public Vector3 lastPos;

        public Vector3 motion;

        public bool onGround;

        protected Entity(Vector3 pos)
        {
            this.pos = pos;
            boundingBox = AxisAlignedBB.BLOCK_FULL.offset(Vector3.One * -0.5f);
        }

        public virtual void Update()
        {
            lastPos = new Vector3(pos);

            motion.Y -= 0.0585f;
            motion.Xz *= 0.8664021f;

            Move();

            if (onGround)
            {
                motion.Xz *= 0.6776801f;
            }
        }

        public virtual void Move()
        {
            var bb = getBoundingBox();
            var bb_o = bb.union(bb.offset(motion));

            List<AxisAlignedBB> list = Game.INSTANCE.world.getBlockCollisionBoxes(bb_o);

            var m_orig = motion;
            var m = motion;

            for (int i = 0; i < list.Count; i++)
            {
                var blockBB = list[i];
                m = blockBB.calculateOffset(bb, m);
            }

            pos += m;

            onGround = Math.Abs(m_orig.Y - m.Y) > 0.0001f && m_orig.Y < 0.0D;

            if (Math.Abs(m_orig.X - m.X) > 0.0000000001f)
                motion.X = 0;

            if (Math.Abs(m_orig.Z - m.Z) > 0.0000000001f)
                motion.Z = 0;

            if (onGround && motion.Y < 0)
                motion.Y = 0;
        }

        public virtual void Render(float particalTicks)
        {

        }

        public AxisAlignedBB getBoundingBox()
        {
            return boundingBox.offset(pos);
        }
    }
}