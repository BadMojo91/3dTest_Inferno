using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inferno {
    public class WorldGen : MonoBehaviour {
        public GameObject chunkPrefab;
        public GameObject pickupPrefab;
        public List<GameObject> chunkPool;
        private List<GameObject> lootPool = new List<GameObject>();
        public int viewDistance = 10;
        public WorldSeed[] seed;
       
        
        public void Start() { 
            Global.SetChunks(5, seed[0]);
            LoadChunks(transform.position, 2);
        }
        public void LoadChunks(Vector3 centerPos, int dist) {
            for(int z = 0; z < dist; z++) {
                for(int x = 0, i = 0; x < dist; x++, i++) {
                    int posX, posZ;
                    posX = Mathf.RoundToInt(centerPos.x + Global.maxChunkSize * x);
                    posZ = Mathf.RoundToInt(centerPos.z + Global.maxChunkSize * z);
                    GameObject newChunk = Instantiate(chunkPrefab, transform);
                    newChunk.transform.position = new Vector3(x * Global.maxChunkSize, 0, z * Global.maxChunkSize);
                    newChunk.name = "Chunk_" + x + "," + z;

                    MeshBuilder meshBuilder = newChunk.GetComponent<MeshBuilder>();
                    meshBuilder.chunkPosX = x;
                    meshBuilder.chunkPosZ = z;
                    meshBuilder.meshFilter = newChunk.GetComponent<MeshFilter>();
                    meshBuilder.meshRenderer = newChunk.GetComponent<MeshRenderer>();
                    meshBuilder.meshCollider = newChunk.GetComponent<MeshCollider>();
                    meshBuilder.meshRenderer.sharedMaterials = Global.materials;

                    for(int c = 0; c < Global.chunks.Count; c++) {
                        if(Global.chunks[c].x == x && Global.chunks[c].z == z) {
                            chunkPool.Add(newChunk);
                        }
                    }
                }
            }
            for(int i = 0; i < chunkPool.Count; i++) {
                MeshBuilder meshBuilder = chunkPool[i].GetComponent<MeshBuilder>();
                meshBuilder.FindSurroundingChunks();
                meshBuilder.GetComponent<MeshBuilder>().InitChunk();
                meshBuilder.GetComponent<MeshBuilder>().BuildMesh();
                meshBuilder.GetComponent<MeshBuilder>().UpdateMesh();
            }
        }
    }
}