using System;
using OpenTK;

namespace OpenGL_Game
{
    [Serializable]
    class WorldPlayerNode
    {
        public float pitch;
        public float yaw;

        public Vector3 pos;

        public ItemStack[] hotbar;

        public WorldPlayerNode(EntityPlayerSP player)
        {
            pitch = player.camera.pitch;
            yaw = player.camera.yaw;
            pos = player.pos;
            hotbar = player.hotbar;
        }
    }
}