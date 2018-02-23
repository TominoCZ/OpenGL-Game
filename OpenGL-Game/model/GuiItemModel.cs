using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    class GuiItemModel : IModel//TODO temporary
    {
        public IRawModel rawModel { get; }
        public ShaderProgram shader { get; }

        public GuiItemModel(ShaderProgram shader)
        {
            this.shader = shader;

            var rawQuad = new RawQuad(new float[] {
                -1,  1,
                -1, -1,
                1, -1,
                1, 1 }, 
                new float[] {
                0, 0,
                1, 0,
                1, 1,
                0, 1});

            rawModel = GraphicsManager.loadModelToVAO(new List<RawQuad> { rawQuad }, 2);
        }
    }
}
