using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Inferno {

    public class IOChunks {
        public static void SaveWorldGenSettings(WorldGenSettings wgs) {
            Directory.CreateDirectory(Application.dataPath + "/Worlds/" + wgs.worldName);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Create(Application.dataPath + "/Worlds/" + wgs.worldName + "/settings.wgs");
            formatter.Serialize(stream, wgs);
            stream.Close();
        }
        public static WorldGenSettings LoadWorldGenSettings(string worldName) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Open(Application.dataPath + "/Worlds/" + worldName + "/settings.wgs", FileMode.Open);
            WorldGenSettings wgs = (WorldGenSettings)formatter.Deserialize(stream);
            stream.Close();
            return wgs;
        }

        public static void SaveChunk(SavedChunk chunk, string worldName,string name) {
            Directory.CreateDirectory(Application.dataPath + "/Worlds/" + worldName);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Create(Application.dataPath + "/Worlds/" + worldName + "/" + name + ".chnk");
            formatter.Serialize(stream, chunk);
            stream.Close();
        }

        public static Chunk LoadChunk(string worldName, string name) {
            string path = Application.dataPath + "/Worlds/" + worldName + "/" + name + ".chnk";
            Chunk _chunk = null;
            if(File.Exists(path)) {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = File.Open(path, FileMode.Open);
                SavedChunk chunk = (SavedChunk)formatter.Deserialize(stream);

                Block[,] blocks = new Block[Global.maxChunkSize, Global.maxChunkSize];
                for(int i = 0; i < chunk.blocks.Length; i++) {
                    blocks[chunk.blocks[i].posX, chunk.blocks[i].posZ] = chunk.blocks[i];
                }

                _chunk = new Chunk(blocks);
                _chunk.x = chunk.x;
                _chunk.z = chunk.z;
                stream.Close();
            }
            return _chunk;
        }

        public static List<Chunk> LoadChunks(string worldName) {
            List<Chunk> chunks = new List<Chunk>();
            string path = Application.dataPath + "/Worlds/" + worldName + "/";
            foreach(string file in Directory.GetFiles(path)) {
                

                if(Path.GetExtension(path + file) == ".chnk") {
                    string f = Path.GetFileNameWithoutExtension(file);
                    Chunk newChunk = LoadChunk(worldName, f);
                    Debug.Log(newChunk.x + " " + newChunk.z);
                    chunks.Add(newChunk);
                }
            }
            return chunks;
        }

    }

}