using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Game
{
    class World
    {
        private List<Chunk> loadedChunks;

        public World()
        {
            loadedChunks = new List<Chunk>();
        }
    }
}
