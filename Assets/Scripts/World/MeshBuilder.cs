using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
    public class MeshBuilder : MonoBehaviour {
        public Block[,] blocks = new Block[Global.maxChunkSize,Global.maxChunkSize];
        public Chunk currentChunk;
        private SubMeshData[] subMeshes;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        public int chunkPosX,chunkPosZ;
        static int[] defaultSubmeshes = { 0, 9, 2, 3, 4, 5 };
        [SerializeField]public WorldSeed seed;
        public GameObject[] surroundingChunks;
       
        //========================================================
        [System.Serializable]
        public struct SubMeshData {
            public List<int> triangles;
            public SubMeshData(List<int> t) {
                triangles = t;
            }
        }

        //========================================================
        //Main functions

        private void Awake() {
           // InitChunk();
        }
        private void Start() {
            //InitChunk();
            surroundingChunks = new GameObject[4];
            surroundingChunks[0] = GameObject.Find("Chunk_" + (chunkPosX - 1) + "," + chunkPosZ);
            surroundingChunks[1] = GameObject.Find("Chunk_" + (chunkPosX + 1) + "," + chunkPosZ);
            surroundingChunks[2] = GameObject.Find("Chunk_" + chunkPosX + "," + (chunkPosZ + 1));
            surroundingChunks[3] = GameObject.Find("Chunk_" + chunkPosX + "," + (chunkPosZ - 1));

            
            //RandomizeChunk();
            //BuildMesh();
            //UpdateMesh();
            //PathMaker(20);
            //DrawLine(new Vector3(2, 0, 2), new Vector3(30, 0, 30), true);
            //IOChunks.SaveChunk(ConvertToChunk(blocks), "Test2");
        }
        public void Update() {
            if(surroundingChunks[0] == null)
                surroundingChunks[0] = GameObject.Find("Chunk_" + (chunkPosX - 1) + "," + chunkPosZ);
            if(surroundingChunks[1] == null)
                surroundingChunks[1] = GameObject.Find("Chunk_" + (chunkPosX + 1) + "," + chunkPosZ);
            if(surroundingChunks[2] == null)
                surroundingChunks[2] = GameObject.Find("Chunk_" + chunkPosX + "," + (chunkPosZ + 1));
            if(surroundingChunks[3] == null)
                surroundingChunks[3] = GameObject.Find("Chunk_" + chunkPosX + "," + (chunkPosZ - 1));
            //UpdateMesh();
        }
        public void LoadChunk(string name) {
            Block[,] b;
            b = IOChunks.LoadChunk(name);
            blocks = b;
            Debug.Log(blocks[0, 0].isFloor);
        }

        public void SaveChunk(string name) {
            IOChunks.SaveChunk(ConvertToChunk(blocks), name);
        }
        /// <summary>
        /// Initializes blocks array, and submesh.
        /// </summary>
        public void InitChunk() {
            blocks = new Block[Global.maxChunkSize, Global.maxChunkSize];
            StartCoroutine(Global.GatherMaterials());
            //init submeshes
            subMeshes = new SubMeshData[Global.materials.Length];
            for(int i = 0; i < subMeshes.Length; i++) {
                subMeshes[i].triangles = new List<int>();
            }
            //Debug.Log(subMeshes.Length);

            //default chunk
            for(int z = 0; z < Global.maxChunkSize; z++) {
                for(int x = 0; x < Global.maxChunkSize; x++) {
                    blocks[x, z] = new Block(x, 0, z, defaultSubmeshes);
                    //blocks[x, z].visible = Random.Range(0, 100) > 50 ? true : false;
                    blocks[x, z].isFloor = true;
                }
            }
           
        }
        //public void RandomizeChunk() {
        //    //First pass
        //    for(int z = 0; z < Global.maxChunkSize; z++) {
        //        for(int x = 0; x < Global.maxChunkSize; x++) {
        //            float perlin = Mathf.PerlinNoise(x + (chunkPosX * 32) * seed.frequency, z + (chunkPosZ * 32) * seed.frequency) * seed.amplitude + seed.octave;
        //            blocks[x, z].isFloor = perlin > seed.reigons[0].level ? true : false;
        //            blocks[x, z].subMesh = defaultSubmeshes;
                   
        //        }
        //    }
        //    SecondPass();
        //}

        //public void SecondPass() {
        //    //Second pass
        //    for(int z = 0; z < Global.maxChunkSize; z++) {
        //        for(int x = 0; x < Global.maxChunkSize; x++) {
        //            if(!blocks[x, z].isFloor) {
        //                int[] s = seed.reigons[0].subMeshes;
        //                SetSurroundingBlocks(x, z, s[2], s[3], s[4], s[5]);

                        
        //            }
        //        }
        //    }
        //}
        int Rand() {
            int r = Random.Range(0, Global.maxChunkSize);
            return r;
        }
        public void PathMaker(int pathCount) {
            List<Vector3> paths = new List<Vector3>();
            for(int i = 0; i < pathCount; i++) {
                paths.Add(new Vector3(Rand(),0,Rand()));
            }
            for(int i = 0; i < pathCount; i++) {
                int a, b;
                a = i;
                b = i + 1;
                if(b >= pathCount)
                    b = 0;

                DrawLine(paths[a], paths[b], true);
            }
        }
        
        public void DrawLine(Vector3 pos1, Vector3 pos2, bool isFloor) {
            pos1 = Global.GridVector3(pos1);
            pos2 = Global.GridVector3(pos2);
            float dist = Vector3.Distance(pos1, pos2);

            for(float i = 0; i < 1; i += 0.01f) {
                //float d = Mathf.Clamp(i, 0, dist);
                Vector3 pos = Vector3.Lerp(pos1, pos2, i);
                pos = Global.RoundVector3(pos);
                Debug.Log(pos);
                RemoveAt(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
            }

            BuildMesh();
            UpdateMesh();
        }
        public void RemoveAt(int x, int z) {
            if(x < Global.maxChunkSize && x >= 0 && z < Global.maxChunkSize && z >= 0)
                blocks[x, z].isFloor = true;
        }
        /// <summary>
        /// Sets each face's submesh surrounding non-visible block
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="s"></param>
        //public void SetSurroundingBlocks(int x, int z, int s) {
        //    SetSurroundingBlocks(x, z, s, s, s, s);
        //}
        //public void SetSurroundingBlocks(int x, int z, int left, int right, int front, int back) {
        //    if(x + 1 < Global.maxChunkSize && blocks[x + 1, z].isFloor) {
        //        int[] sub = blocks[x + 1, z].subMesh;
        //        blocks[x + 1, z].subMesh = CubeMaterial(sub[0], sub[1], left, sub[3], sub[4], sub[5]);
        //    }
        //    if(x - 1 >= 0 && blocks[x - 1, z].isFloor) {
        //        int[] sub = blocks[x - 1, z].subMesh;
        //        blocks[x - 1, z].subMesh = CubeMaterial(sub[0], sub[1], sub[2], right, sub[4], sub[5]);
        //    }
        //    if(z + 1 < Global.maxChunkSize && blocks[x, z + 1].isFloor) {
        //        int[] sub = blocks[x, z + 1].subMesh;
        //        blocks[x, z + 1].subMesh = CubeMaterial(sub[0], sub[1], sub[2], sub[3], front, sub[5]);
        //    }
        //    if(z - 1 >= 0 && blocks[x, z - 1].isFloor) {
        //        int[] sub = blocks[x, z - 1].subMesh;
        //        blocks[x, z - 1].subMesh = CubeMaterial(sub[0], sub[1], sub[2], sub[3], sub[4], back);
        //    }
        //}
        public int[] CubeMaterial(int up, int down, int left, int right, int front, int back) {
            int[] cMat = new int[6];
            cMat[0] = up;
            cMat[1] = down;
            cMat[2] = left;
            cMat[3] = right;
            cMat[4] = front;
            cMat[5] = back;

            return cMat;
        }
        /// <summary>
        /// Sets block submesh in blocks array
        /// </summary>
        /// <param name="x">position x</param>
        /// <param name="z">position z</param>
        /// <param name="s">Submesh for each face (up, down, left, right, front, back)</param> 
        public void SetBlock(int x, int z, bool v) {
            if(blocks == null)
                return;

            blocks[x, z].isFloor = v;
            //SetSurroundingBlocks(x, z, 6);
            blocks = Global.SecondPass(seed, blocks);
            BuildMesh();
            UpdateMesh();

        }

        /// <summary>
        /// Builds mesh from blocks array
        /// </summary>
        public void BuildMesh() {
            count = 0;
            foreach(SubMeshData i in subMeshes) {
                i.triangles.Clear();
            }
            vertices.Clear();
            triangles.Clear();
            colVertices.Clear();
            colTriangles.Clear();
            uvs.Clear();
           // Debug.Log(chunkPosX + " " + chunkPosZ);
            blocks = Global.chunks[chunkPosX, chunkPosZ].blocks2;
            for(int z = 0; z < Global.maxChunkSize; z++) {
                for(int x = 0; x < Global.maxChunkSize; x++) {
                    CreateCube(x, 0, z, blocks[x, z].subMesh);
                    
                }
            }
            //UpdateMesh();
        }
        /// <summary>
        /// Updates mesh, run after BuildMesh.
        /// </summary>
        public void UpdateMesh() {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.subMeshCount = subMeshes.Length;
            mesh.vertices = vertices.ToArray();
            for(int i = 0; i < subMeshes.Length; i++) {
                mesh.SetTriangles(subMeshes[i].triangles.ToArray(), i);
            }
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            meshRenderer.sharedMaterials = Global.materials;
            meshFilter.sharedMesh = mesh;

            Mesh colMesh = new Mesh();
            colMesh.Clear();
            colMesh.vertices = colVertices.ToArray();
            colMesh.triangles = colTriangles.ToArray();
            meshCollider.sharedMesh = colMesh;

            count = 0;
            foreach(SubMeshData i in subMeshes) {
                i.triangles.Clear();
            }
            vertices.Clear();
            triangles.Clear();
            colVertices.Clear();
            colTriangles.Clear();
            uvs.Clear();

            currentChunk = ConvertToChunk(blocks);
  
        }
        private void CreateCube(int x, int y, int z, int[] s) {
            AddVerts(x, y, z, s[0], CubeFace.up);
            AddVerts(x, y, z, s[1], CubeFace.down);
            AddVerts(x, y, z, s[2], CubeFace.left);
            AddVerts(x, y, z, s[3], CubeFace.right);
            AddVerts(x, y, z, s[4], CubeFace.front);
            AddVerts(x, y, z, s[5], CubeFace.back);
        }

        /// <summary>
        /// Converts a Block 2D multi array into a single array and returns as chunk.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Chunk ConvertToChunk(Block[,] c) {
            Block[] _chunk = new Block[Global.maxChunkSize * Global.maxChunkSize];
            int i = 0;
            for(int z = 0; z < c.GetLength(1); z++) {
                for(int x = 0; x < c.GetLength(0); x++, i++) {
                    _chunk[i] = c[x, z];
                }
            }
            Chunk newChunk = new Chunk(_chunk);
            return newChunk;
        }
        //========================================================
        //Mesh building
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<Vector3> colVertices = new List<Vector3>();
        private List<int> colTriangles = new List<int>();
        private List<Vector2> uvs = new List<Vector2>();
        private int count = 0;
        enum CubeFace {up, down, left, right, front, back};

        private void AddTriangles(int s) {
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));

            List<int> t = new List<int>();
            t.Add(count * 4);
            t.Add((count * 4)+1);
            t.Add((count * 4)+2);
            t.Add(count * 4);
            t.Add((count * 4)+2);
            t.Add((count * 4)+3);
            triangles.AddRange(t);
            subMeshes[s].triangles.AddRange(t);

            colTriangles.Add(count * 4);
            colTriangles.Add((count * 4) + 1);
            colTriangles.Add((count * 4) + 2);
            colTriangles.Add(count * 4);
            colTriangles.Add((count * 4) + 2);
            colTriangles.Add((count * 4) + 3);

            count++;
        }
        private void AddVerts(float x, float y, float z, int s, CubeFace face) {
            int posX = (int)x;
            int posZ = (int)z;
           // GameObject c = null;
            x -= 0.5f; //sets the offsets here, better that way.
            z -= 0.5f;
            y -= 0.5f;
            if(blocks[posX, posZ].isFloor) {
                if(face == CubeFace.up) {
                    vertices.Add(new Vector3(x, y + 1, z));
                    vertices.Add(new Vector3(x + 1, y + 1, z));
                    vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                    vertices.Add(new Vector3(x, y + 1, z + 1));

                    colVertices.Add(new Vector3(x, y + 1, z));
                    colVertices.Add(new Vector3(x + 1, y + 1, z));
                    colVertices.Add(new Vector3(x + 1, y + 1, z + 1));
                    colVertices.Add(new Vector3(x, y + 1, z + 1));
                    AddTriangles(s);
                }
                else if(face == CubeFace.down) {
                    vertices.Add(new Vector3(x, y, z));
                    vertices.Add(new Vector3(x, y, z + 1));
                    vertices.Add(new Vector3(x + 1, y, z + 1));
                    vertices.Add(new Vector3(x + 1, y, z));

                    colVertices.Add(new Vector3(x, y, z));
                    colVertices.Add(new Vector3(x, y, z + 1));
                    colVertices.Add(new Vector3(x + 1, y, z + 1));
                    colVertices.Add(new Vector3(x + 1, y, z));
                    AddTriangles(s);
                }
                else if(face == CubeFace.left) {
                    if(posX - 1 < 0 || !blocks[posX - 1, posZ].isFloor) {
                        vertices.Add(new Vector3(x, y, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x, y + 1, z + 1));
                        vertices.Add(new Vector3(x, y, z + 1));

                        colVertices.Add(new Vector3(x, y, z));
                        colVertices.Add(new Vector3(x, y + 1, z));
                        colVertices.Add(new Vector3(x, y + 1, z + 1));
                        colVertices.Add(new Vector3(x, y, z + 1));
                        AddTriangles(s);
                    }
                }
                else if(face == CubeFace.right) {
                    if(posX + 1 > Global.maxChunkSize - 1 || !blocks[posX + 1, posZ].isFloor) {
                        vertices.Add(new Vector3(x + 1, y, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                        vertices.Add(new Vector3(x + 1, y, z));

                        colVertices.Add(new Vector3(x + 1, y, z + 1));
                        colVertices.Add(new Vector3(x + 1, y + 1, z + 1));
                        colVertices.Add(new Vector3(x + 1, y + 1, z));
                        colVertices.Add(new Vector3(x + 1, y, z));
                        AddTriangles(s);
                    }
                    
                }
                else if(face == CubeFace.front) {
                    if(posZ - 1 < 0 || !blocks[posX, posZ - 1].isFloor) {
                        vertices.Add(new Vector3(x + 1, y, z));
                        vertices.Add(new Vector3(x + 1, y + 1, z));
                        vertices.Add(new Vector3(x, y + 1, z));
                        vertices.Add(new Vector3(x, y, z));

                        colVertices.Add(new Vector3(x + 1, y, z));
                        colVertices.Add(new Vector3(x + 1, y + 1, z));
                        colVertices.Add(new Vector3(x, y + 1, z));
                        colVertices.Add(new Vector3(x, y, z));
                        AddTriangles(s);
                    }
                }
                else if(face == CubeFace.back) {
                    if(posZ + 1 > Global.maxChunkSize - 1 || !blocks[posX, posZ + 1].isFloor) {
                        vertices.Add(new Vector3(x, y, z + 1));
                        vertices.Add(new Vector3(x, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y, z + 1));

                        colVertices.Add(new Vector3(x, y, z + 1));
                        colVertices.Add(new Vector3(x, y + 1, z + 1));
                        colVertices.Add(new Vector3(x + 1, y + 1, z + 1));
                        colVertices.Add(new Vector3(x + 1, y, z + 1));
                        AddTriangles(s);
                    }
                }
            }
        }
        //========================================================
    }
    /*
//Right
                    if(x + 1 < Global.maxChunkSize && !blocks[x + 1, z].visible) {
                        if(x + 2 < Global.maxChunkSize && blocks[x + 2, z].visible) {
                            blocks[x, z].subMesh = CubeMaterial(13,1,2,6,4,5);
                            blocks[x + 2, z].subMesh = CubeMaterial(13, 1, 6, 3, 4, 5);
                            if(z+1 < Global.maxChunkSize)
                                blocks[x + 1, z + 1].subMesh = CubeMaterial(13, 1, 2, 3, 6, 5);
                            if(z - 1 > 0)
                                blocks[x + 1, z - 1].subMesh = CubeMaterial(13, 1, 2, 3, 4, 6);
                        }
                    }
                    //Left
                    if(x - 1 > 0 && !blocks[x - 1, z].visible) {
                        if(x - 2 < 0 && blocks[x - 2, z].visible) {
                            blocks[x, z].subMesh = CubeMaterial(13, 1, 6, 3, 4, 5);
                            blocks[x - 2, z].subMesh = CubeMaterial(13, 1, 2, 6, 4, 5);
                            if(z + 1 < Global.maxChunkSize)
                                blocks[x - 1, z + 1].subMesh = CubeMaterial(13, 1, 2, 3, 4, 6);
                            if(z - 1 > 0)
                                blocks[x - 1, z - 1].subMesh = CubeMaterial(13, 1, 2, 3, 6, 5);
                        }
                    }
                    //Forward
                    if(z + 1 < Global.maxChunkSize && !blocks[x, z + 1].visible) {
                        if(z + 2 < Global.maxChunkSize && blocks[x, z + 2].visible) {
                            blocks[x, z].subMesh = CubeMaterial(13, 1, 2, 3, 4, 6);
                            blocks[x, z + 2].subMesh = CubeMaterial(13, 1, 2, 3, 6, 5);
                            if(x + 1 < Global.maxChunkSize)
                                blocks[x + 1, z + 1].subMesh = CubeMaterial(13, 1, 2, 3, 6, 5);
                            if(x - 1 > 0)
                                blocks[x - 1, z + 1].subMesh = CubeMaterial(13, 1, 2, 3, 4, 6);
                        }
                    }
                    //Back
                    if(z - 1 > 0 && !blocks[x, z -1].visible) {
                        if(z - 2 < 0 && blocks[x, z-2].visible) {
                            blocks[x, z].subMesh = CubeMaterial(13, 1, 2, 3, 6, 5);
                            blocks[x, z - 1].subMesh = CubeMaterial(13, 1, 3, 3, 4, 6);
                            if(x + 1 < Global.maxChunkSize)
                                blocks[x + 1, z - 1].subMesh = CubeMaterial(13, 1, 2, 3, 6, 5);
                            if(x - 1 > 0)
                                blocks[x - 1, z - 1].subMesh = CubeMaterial(13, 1, 2, 3, 4, 6);
                        }
                    }

 */

}


