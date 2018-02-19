using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_Game.world
{
    class WorldLoader //TODO: MAKE A PLAYERS FOLDER AND STORE PLAYERS THERE
    {
        static string dir = "CSharpMC_World";

        public static void saveWorld(World w)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var nodes = w.getChunkDataNodes();

            List<ChunkCache> caches = new List<ChunkCache>();

            foreach (var node in nodes)
            {
                var cache = node.chunk.createChunkCache();

                caches.Add(cache);
            }

            try
            {
                var wcn = new WorldCacheNode(caches, Game.INSTANCE.player.pos);

                var bf = new BinaryFormatter();
                using (var fs = File.OpenWrite(dir + "/chunks.dat"))
                {
                    bf.Serialize(fs, wcn);
                }
            }
            catch
            {

            }
        }

        public static World loadWorld()
        {
            var bf = new BinaryFormatter();

            if (!Directory.Exists(dir))
                return null;

            try
            {
                WorldCacheNode wcn;

                using (var fs = File.OpenRead(dir + "/chunks.dat"))
                {
                    wcn = (WorldCacheNode)bf.Deserialize(fs);
                }

                var world = World.Create(wcn.caches);
                world.addEntity(new EntityPlayerSP(wcn.lastPlayerPos));
            }
            catch
            {

            }

            return null;
        }
    }

    [Serializable]
    class WorldCacheNode
    {
        public List<ChunkCache> caches { get; }
        public Vector3 lastPlayerPos { get; }

        public WorldCacheNode(List<ChunkCache> caches, Vector3 lastPlayerPos)
        {
            this.caches = caches;
            this.lastPlayerPos = lastPlayerPos;
        }
    }
}
