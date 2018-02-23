using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace OpenGL_Game
{
    class WorldLoader
    {
        static string dir = "SharpCraft_Data/saves/world";

        public static void saveWorld(World w)
        {
            if (w == null)
                return;

            new Thread(() =>
            {
                var bf = new BinaryFormatter();

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
                    var wcn = new WorldChunksNode(caches);

                    using (var fs = File.OpenWrite(dir + "/chunks.dat"))
                    {
                        bf.Serialize(fs, wcn);
                    }

                    var wpn = new WorldPlayerNode(Game.INSTANCE.player);

                    using (var fs = File.OpenWrite(dir + "/player.dat"))
                    {
                        bf.Serialize(fs, wpn);
                    }
                }
                catch
                {

                }
            }).Start();
        }

        public static World loadWorld()
        {
            var bf = new BinaryFormatter();

            if (!Directory.Exists(dir))
                return null;

            try
            {
                WorldChunksNode wcn;
                WorldPlayerNode wpn;

                using (var fs = File.OpenRead(dir + "/chunks.dat"))
                {
                    wcn = (WorldChunksNode)bf.Deserialize(fs);
                }

                using (var fs = File.OpenRead(dir + "/player.dat"))
                {
                    wpn = (WorldPlayerNode)bf.Deserialize(fs);
                }

                var world = World.Create(wcn.caches);

                var player = new EntityPlayerSP(wpn.pos);
                player.camera.pitch = wpn.pitch;
                player.camera.yaw = wpn.yaw;

                for (int i = 0; i < wpn.hotbar.Length; i++)
                {
                    player.hotbar[i] = wpn.hotbar[i];
                }

                Game.INSTANCE.player = player;

                world.addEntity(player);

                return world;
            }
            catch
            {

            }

            return null;
        }
    }
}
