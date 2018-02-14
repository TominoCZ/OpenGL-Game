using System;
using OpenTK;

namespace OpenGL_Game
{
    class BlockNode
    {
        public Model model { get; }

        public Vector3 translation;

        public float rx, ry, rz, scale;

        public BlockNode(Model model, Vector3 translation, float scale)
        {
            this.model = model;

            this.translation = translation;
            this.scale = scale;
        }
    }
}