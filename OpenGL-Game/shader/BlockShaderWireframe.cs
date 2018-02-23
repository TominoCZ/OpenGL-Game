using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game
{
    class BlockShaderWireframe : BlockShaderUnlit
    {
        private int loc_color;

        public BlockShaderWireframe() : base("color", PrimitiveType.Quads)
        {

        }

        protected override void bindAttributes()
        {
            bindAttributes(0, "position");
        }

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();

            loc_color = getUniformLocation("colorIn");
        }

        public void loadColor(Vector4 color)
        {
            loadVector(loc_color, color);
        }
    }
}