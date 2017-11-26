using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inferno {
    [System.Serializable]
    public class WorldGenSettings {
        [System.Serializable]
        public struct WSeed {
            public float frequency;
            public float amplitude;
            public float octave;
            public Reigon[] reigons;
            public Item[] items;
        }
        public List<Chunk> chunks = new List<Chunk>();
        public WSeed seed;
        public string worldName;
        public int maxChunkSize;
        public WorldGenSettings(string _name, WSeed _seed, int _maxChunkSize) {
            seed = _seed;
            maxChunkSize = _maxChunkSize;
            worldName = _name;
        }
    }
    public class WorldGen : MonoBehaviour {
        public bool genWorld,saveWorld, loadWorld;
        public string worldName;
        public GameObject chunkPrefab;
        public GameObject pickupPrefab;
        public static List<GameObject> chunkPool;
        private List<GameObject> lootPool = new List<GameObject>();
        public int viewDistance = 10;
        public WorldSeed seed;
        const float CTICK = 0.01f;
        float chunkTick;
        int iterator = 0;
        static WorldGenSettings.WSeed _WSeed;

        public void Update() {
            if(genWorld) {
                genWorld = false;
                Global.SetChunks(viewDistance, seed);
                UpdateAllChunks();
            }
            if(saveWorld) {
                saveWorld = false;
                SaveChunks(chunkPool, worldName, seed);
            }
            if(loadWorld) {
               
                foreach(Transform c in transform) {
                    Destroy(c.gameObject);
                }
                chunkPool.Clear();
                InitChunks(transform.position, viewDistance);
                LoadWorld(worldName);
                UpdateAllChunks();
                loadWorld = false;
            }
           
        }
        public void Start() {
            Global.worldGen = this;
            Global.worldName = worldName;
            Global.SetChunks(viewDistance, seed);
            InitChunks(transform.position, 5);
            UpdateAllChunks();
        }
      
        public static void ChunkUpdate(MeshBuilder meshBuilder) {
            meshBuilder.BuildMesh();
            meshBuilder.UpdateMesh();
            
        }
        public static void UpdateAllChunks() {
            for(int i = 0; i < chunkPool.Count; i++) {
                MeshBuilder meshBuilder = chunkPool[i].GetComponent<MeshBuilder>();
                meshBuilder.FindSurroundingChunks();
                meshBuilder.GetComponent<MeshBuilder>().InitChunk();
                meshBuilder.GetComponent<MeshBuilder>().BuildMesh();
                meshBuilder.GetComponent<MeshBuilder>().UpdateMesh();
            }
        }
       public static void SaveChunks(List<GameObject> _pool, string _worldName, WorldSeed _seed) {
            _WSeed.amplitude = _seed.amplitude;
            _WSeed.frequency = _seed.frequency;
            _WSeed.octave = _seed.octave;
            _WSeed.reigons = _seed.reigons.ToArray();
            _WSeed.items = _seed.randomLoot.ToArray();
            WorldGenSettings wgs = new WorldGenSettings(_worldName, _WSeed, Global.maxChunkSize);
            IOChunks.SaveWorldGenSettings(wgs);
            foreach(GameObject chunk in _pool) {
                chunk.GetComponent<MeshBuilder>().SaveChunk();
            }
        }
        public void LoadWorld(string worldName) {
            WorldGenSettings wgs = IOChunks.LoadWorldGenSettings(worldName);
            seed.amplitude = wgs.seed.amplitude;
            seed.frequency = wgs.seed.frequency;
            seed.octave = wgs.seed.octave;
            seed.reigons.Clear();
            for(int i = 0; i < wgs.seed.reigons.Length; i++) {
                seed.reigons.Add(wgs.seed.reigons[i]);
            }
            seed.randomLoot.Clear();
            for(int i = 0; i < wgs.seed.items.Length; i++) {
                seed.randomLoot.Add(wgs.seed.items[i]);
            }
            Global.chunks = IOChunks.LoadChunks(worldName);
        }
        public void InitChunks(Vector3 centerPos, int dist) {
            chunkPool = new List<GameObject>();
            chunkPool.Clear();
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
                    meshBuilder.currentChunk.x = x;
                    meshBuilder.currentChunk.z = z;
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

            UpdateAllChunks();
            
        }
    }
}