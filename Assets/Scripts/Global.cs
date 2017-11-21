using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Inferno {
    [System.Serializable]
    public class Global {
        //Global statics
        public static GameObject inventoryViewPrefab;
        public static GameObject pickupPrefab;
        public static int maxChunkSize = 32;
        public static Material[] materials;
        public enum Compass { North, South, East, West };
        public static Chunk[,] chunks;
        public static bool uiActive = false;
        public static GameObject activeUi;
        private static byte gameType = 2;
        /// <summary>
        /// Possible game types:
        /// 0 = Grid based platform no gravity (heartlight)
        /// 1 = Platform with gravity (commander keen)
        /// 2 = Grid based 2.5d restricted (hypercycles)
        /// 3 = 2.5d (doom, duke3d)
        /// 4 = full 3d (quake)
        /// 5 = flight (decent)
        /// </summary>
        public static byte GameType {
            get { return gameType; }
            set {
                if(value > 5) {
                    gameType = 5;
                    Debug.Log("Game Type " + value + " doesn't exist, Game Type changed to 5");
                }
                else {
                    gameType = value;
                    Debug.Log("Game Type changed to " + value);
                }
            }
        }
        //Global functions
        /// <summary>
        /// Finds all textures in "Resources/Flats" and updates global materials array.
        /// </summary>
        /// <returns></returns>
        public static IEnumerator GatherMaterials() {
            List<Material> textures = new List<Material>();
            foreach(Texture2D tex in Resources.LoadAll("Flats", typeof(Texture2D))) {
                string n = tex.name;
                n = n.Substring(n.Length - 2);
                //Debug.Log(n);
                if(n != "_n" && n != "_s" && n != "_h") {


                    Material mat = null;
                    if(tex.name == "00-Up")
                        mat = new Material(Shader.Find("Skybox/Procedural"));
                    else
                        mat = new Material(Shader.Find("Standard"));
                    mat.mainTexture = tex;
                    mat.name = tex.name;

                    Texture2D normalMap = null;
                    normalMap = (Texture2D)Resources.Load("Flats/" + tex.name + "_n", typeof(Texture2D));
                    if(normalMap) {
                        
                        //Debug.Log("Normal Map Found: " + normalMap);
                        mat.SetTexture("_BumpMap", normalMap);
                        mat.EnableKeyword("_NORMALMAP");
                    }

                    Texture2D heightMap = null;
                    heightMap = (Texture2D)Resources.Load("Flats/" + tex.name + "_h", typeof(Texture2D));
                    if(heightMap) {

                        //Debug.Log("Parallax Map Found: " + heightMap);
                        mat.SetTexture("_ParallaxMap", heightMap);
                        mat.EnableKeyword("_PARALLAXMAP");
                    }

                    Texture2D specularMap = null;
                    specularMap = (Texture2D)Resources.Load("Flats/" + tex.name + "_s", typeof(Texture2D));
                    if(specularMap) {

                        //Debug.Log("Specular Map Found: " + specularMap);
                        mat.SetTexture("_MetallicGlossMap", specularMap);
                        mat.EnableKeyword("_METALLICGLOSSMAP");
                        mat.SetFloat("_GlossMapScale", 0.5f);
                    }

                    textures.Add(mat);
                    //Debug.Log(tex.name);
                }
            }
            materials = textures.ToArray();
            yield return new WaitForEndOfFrame();
        }


        /// <summary>
        /// Rounds vector
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 RoundVector3(Vector3 v3) {
            Vector3 rVec = new Vector3(Mathf.Round(v3.x), Mathf.Round(v3.y), Mathf.Round(v3.z));
            return rVec;
        }


        public static Vector3 GridVector3(Vector3 v) {
            Vector3 vec = v;
            RoundVector3(v);
            if(v.x < 0)
                v.x = 0;
            if(v.x >= maxChunkSize)
                v.x = maxChunkSize - 1;
            if(v.z < 0)
                v.z = 0;
            if(v.z >= maxChunkSize)
                v.z = maxChunkSize - 1;
            return v;
        }
        public static int[] CubeMaterial(int up, int down, int left, int right, int front, int back) {
            int[] cMat = new int[6];
            cMat[0] = up;
            cMat[1] = down;
            cMat[2] = left;
            cMat[3] = right;
            cMat[4] = front;
            cMat[5] = back;

            return cMat;
        }
        public static Block[,] SetSurroundingBlocks(Block[,] blocks, int x, int z, int left, int right, int front, int back) {
            Block[,] bl = blocks;
            if(x + 1 < Global.maxChunkSize && bl[x + 1, z].isFloor) {
                int[] sub = bl[x + 1, z].subMesh;
                bl[x + 1, z].subMesh = CubeMaterial(sub[0], sub[1], left, sub[3], sub[4], sub[5]);
            }
            if(x - 1 >= 0 && bl[x - 1, z].isFloor) {
                int[] sub = bl[x - 1, z].subMesh;
                bl[x - 1, z].subMesh = CubeMaterial(sub[0], sub[1], sub[2], right, sub[4], sub[5]);
            }
            if(z + 1 < Global.maxChunkSize && bl[x, z + 1].isFloor) {
                int[] sub = bl[x, z + 1].subMesh;
                bl[x, z + 1].subMesh = CubeMaterial(sub[0], sub[1], sub[2], sub[3], front, sub[5]);
            }
            if(z - 1 >= 0 && bl[x, z - 1].isFloor) {
                int[] sub = bl[x, z - 1].subMesh;
                bl[x, z - 1].subMesh = CubeMaterial(sub[0], sub[1], sub[2], sub[3], sub[4], back);
            }
            return bl;
        }
        public static Block[,] RandomizeChunk(WorldSeed seed) {
            Block[,] blocks = new Block[maxChunkSize, maxChunkSize];
            //First pass
            for(int z = 0; z < maxChunkSize; z++) {
                for(int x = 0; x < maxChunkSize; x++) {
                    float perlin = Mathf.PerlinNoise(x * seed.frequency, z * seed.frequency) * seed.amplitude + seed.octave;
                    blocks[x, z] = new Block(x, 0, z, CubeMaterial(0, 1, 2, 3, 4, 5));
                    blocks[x, z].isFloor = perlin > seed.reigons[0].level ? true : false;

                    

                }
            }
            blocks = SecondPass(seed, blocks);
            return blocks;
        }

        public static Block[,] SecondPass(WorldSeed seed, Block[,] blocks) {
            Block[,] bl = blocks;
            //Second pass
            for(int z = 0; z < maxChunkSize; z++) {
                for(int x = 0; x < maxChunkSize; x++) {
                    if(!bl[x, z].isFloor) {
                        int[] s = seed.reigons[0].subMeshes;
                        bl = SetSurroundingBlocks(bl, x, z, s[2], s[3], s[4], s[5]);
                    }
                }
            }
            return bl;
        }

        public static void SetChunks(int count, WorldSeed seed) {
            chunks = new Chunk[count, count];
            for(int x = 0; x < count; x++) {
                for(int z = 0; z < count; z++) {
                    chunks[x, z] = new Chunk(RandomizeChunk(seed));
                    //chunks[x, z].blocks2 = RandomizeChunk(seed);
                }
            }
           // Debug.Log(chunks[5,5].blocks2.Length);
        }
    }
    
}
