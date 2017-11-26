using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Inferno {

    public class IOChunks {

        public static void SaveChunk(Chunk chunk, string name) {
            Directory.CreateDirectory(Application.dataPath + "/Chunks/");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Create(Application.dataPath + "/Chunks/" + name + ".chnk");
            formatter.Serialize(stream, chunk);
            stream.Close();
        }

        public static Block[,] LoadChunk(string name) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Open(Application.dataPath + "/Chunks/" + name + ".chnk", FileMode.Open);
            Chunk chunk = (Chunk)formatter.Deserialize(stream);

            Block[,] blocks = new Block[Global.maxChunkSize, Global.maxChunkSize];
            for(int i = 0; i < chunk.blocks.Length; i++) {
                blocks[chunk.blocks[i].posX, chunk.blocks[i].posZ] = chunk.blocks[i];
            }

            Debug.Log(blocks[0, 0].isFloor);
            stream.Close();
            return blocks;
        }

    }

}