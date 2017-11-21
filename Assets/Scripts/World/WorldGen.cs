using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inferno {
    public class WorldGen : MonoBehaviour {
        public GameObject chunkPrefab;
        public GameObject pickupPrefab;
        public GameObject[,] chunkPool;
        public int viewDistance = 10;
        public WorldSeed[] seed;
       
        
        public void Start() {
            Global.SetChunks(20, seed[0]);
            
            LoadChunks(transform.position);
        }
        public void LoadChunks(Vector3 centerPos) {
            chunkPool = new GameObject[viewDistance, viewDistance];
            for(int z = 0; z < chunkPool.GetLength(1); z++) {
                for(int x = 0, i = 0; x < chunkPool.GetLength(0); x++, i++) {
                        int posX, posZ;
                        posX = Mathf.RoundToInt(centerPos.x + Global.maxChunkSize * x);
                        posZ = Mathf.RoundToInt(centerPos.z + Global.maxChunkSize * z);
                        GameObject newChunk = Instantiate(chunkPrefab, transform);
                        newChunk.transform.position = new Vector3(x * Global.maxChunkSize, 0, z * Global.maxChunkSize);
                        newChunk.name = "Chunk_" + x + "," + z;
                        newChunk.GetComponent<MeshBuilder>().chunkPosX = x;
                        newChunk.GetComponent<MeshBuilder>().chunkPosZ = z;
                        newChunk.GetComponent<MeshBuilder>().blocks = Global.chunks[x, z].blocks2;
                        chunkPool[x,z] = newChunk;

                    //Random Loot
                    for(int l = 0; l < Random.Range(0, 30); l++) { 
                        if(Random.Range(0, 1000) > 500 && newChunk.GetComponent<MeshBuilder>().blocks[x, z].isFloor) {
                            GameObject loot = Instantiate(pickupPrefab);
                            loot.transform.position = new Vector3(x + posX, 0, z + posZ);
                            loot.GetComponent<Pickup>().item = seed[0].randomLoot[Random.Range(0, seed[0].randomLoot.Count - 1)];

                        }
                    }

                }
            }

            for(int z = 0; z < chunkPool.GetLength(0); z++) {
                for(int x = 0; x < chunkPool.GetLength(0); x++) {
                    if(chunkPool[x, z] != null) {
                        chunkPool[x, z].GetComponent<MeshBuilder>().InitChunk();
                        chunkPool[x, z].GetComponent<MeshBuilder>().BuildMesh();
                        chunkPool[x, z].GetComponent<MeshBuilder>().UpdateMesh();
                    }
                }
            }
        }
    }
}