using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
    public class MeshBuilder : MonoBehaviour {
        //public Block[,] blocks = new Block[Global.maxChunkSize,Global.maxChunkSize];
        public Chunk currentChunk;
        private SubMeshData[] subMeshes;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;
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
        //========================================================
        /// <summary>
        /// Find chunks surrounding this chunk
        /// </summary>
        public void FindSurroundingChunks() {
            surroundingChunks = new GameObject[4];
            if(GameObject.Find("Chunk_" + (chunkPosX - 1) + "," + chunkPosZ))
            surroundingChunks[0] = GameObject.Find("Chunk_" + (chunkPosX - 1) + "," + chunkPosZ);
            if(GameObject.Find("Chunk_" + (chunkPosX + 1) + "," + chunkPosZ))
                surroundingChunks[1] = GameObject.Find("Chunk_" + (chunkPosX + 1) + "," + chunkPosZ);
            if(GameObject.Find("Chunk_" + chunkPosX + "," + (chunkPosZ - 1)))
                surroundingChunks[2] = GameObject.Find("Chunk_" + chunkPosX + "," + (chunkPosZ - 1));
            if(GameObject.Find("Chunk_" + chunkPosX + "," + (chunkPosZ + 1)))
                surroundingChunks[3] = GameObject.Find("Chunk_" + chunkPosX + "," + (chunkPosZ + 1));
        }

        /// <summary>
        /// Loads chunk in path string name
        /// </summary>
        /// <param name="name"></param>
        public void LoadChunk(string name) {
            Block[,] b;
            b = IOChunks.LoadChunk(name);
            currentChunk.blocks2 = b;
            //Debug.Log(blocks[0, 0].isFloor);
        }

        /// <summary>
        /// Converts chunk to binary friendly and saves to folder of string name
        /// </summary>
        /// <param name="name"></param>
        public void SaveChunk(string name) {
            //currentChunk = ConvertToChunk(currentChunk.blocks2);
            IOChunks.SaveChunk(currentChunk, name);
        }

        /// <summary>
        /// Initializes blocks array, and submesh.
        /// </summary>
        public void InitChunk() {
            //blocks = new Block[Global.maxChunkSize, Global.maxChunkSize]; //A new beginning
            //StartCoroutine(Global.GatherMaterials()); //gathers materials for mesh renderer
            //init submeshes
            //====================================================
            currentChunk = Global.GetChunk(chunkPosX, chunkPosZ);
        }
        
        /// <summary>
        /// Random number between 0 and maxChunkSize
        /// </summary>
        /// <returns></returns>
        int Rand() {
            int r = Random.Range(0, Global.maxChunkSize);
            return r;
        }

        /// <summary>
        /// Random path maker, single file, rejoins back to first node
        /// </summary>
        /// <param name="pathCount"></param>
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
        
        /// <summary>
        /// Draws a line in the chunk
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="isFloor"></param>
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

        /// <summary>
        /// Remove block at
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public void RemoveAt(int x, int z) {
            if(x < Global.maxChunkSize && x >= 0 && z < Global.maxChunkSize && z >= 0) 
            currentChunk.blocks2[x, z].isFloor = true;
   
            BuildMesh();
            UpdateMesh();
        }

        /// <summary>
        /// Returns the submesh array for the 4 walls of a cube
        /// </summary>
        /// <param name="up"></param>
        /// <param name="down"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="front"></param>
        /// <param name="back"></param>
        /// <returns></returns>
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
            currentChunk = Global.GetChunk(chunkPosX, chunkPosZ);
            currentChunk.blocks2[x, z].isFloor = v;
            currentChunk.blocks2 = Global.SecondPass(seed, currentChunk.blocks2);
            //BuildMesh();
            //UpdateMesh();

        }

        private void Cleanup() {
            count = 0;
            foreach(SubMeshData i in subMeshes) {
                i.triangles.Clear();
            }
            vertices.Clear();
            triangles.Clear();
            colVertices.Clear();
            colTriangles.Clear();
            uvs.Clear();
        }
        int GetTex(int x, int z, string texture) {
            Chunk c = currentChunk;
            int tex = 0;
            switch(texture) {
                case "north":
                    tex = c.blocks2[x, z].north;
                    break;
                case "south":
                    tex = c.blocks2[x, z].south;
                    break;
                case "east":
                    tex = c.blocks2[x, z].east;
                    break;
                case "west":
                    tex = c.blocks2[x, z].west;
                    break;
                case "floor":
                    tex = c.blocks2[x, z].floor;
                    break;
                case "ceiling":
                    tex = c.blocks2[x, z].ceiling;
                    break;
                default:
                    tex = 0;
                    break;
            }
            return tex;
        }

        /// <summary>
        /// Builds mesh from blocks array
        /// </summary>
        public void BuildMesh() {
            subMeshes = new SubMeshData[Global.materials.Length];
            for(int i = 0; i < subMeshes.Length; i++) {
                subMeshes[i].triangles = new List<int>();
            }
            //Debug.Log(chunkPosX + " " + chunkPosZ);
            //currentChunk = Global.GetChunk(chunkPosX, chunkPosZ);
            for(int z = 0; z < Global.maxChunkSize; z++) {
                for(int x = 0; x < Global.maxChunkSize; x++) {
                    //Debug.Log(currentChunk.blocks2[x,z].subMesh.Length);
                    if(currentChunk.blocks2[x, z].isFloor)
                        CreateFloor(x, 0, z, GetTex(x, z, "floor"), GetTex(x,z,"ceiling"));
                    else
                        CreateCube(x, 0, z, GetTex(x,z,"north"), GetTex(x, z, "south"), GetTex(x, z, "east"), GetTex(x, z, "west"));
                }
            }
        }
        /// <summary>
        /// Updates mesh, run after BuildMesh.
        /// </summary>
        public void UpdateMesh() {
            if(vertices == null || colVertices == null)
                return;
            Mesh mesh = new Mesh();
            Mesh colMesh = new Mesh();

            mesh.Clear();
            mesh.subMeshCount = subMeshes.Length;
            mesh.vertices = vertices.ToArray();
            for(int i = 0; i < subMeshes.Length; i++) {
                mesh.SetTriangles(subMeshes[i].triangles.ToArray(), i);
            }
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            meshFilter.sharedMesh = mesh;

            colMesh.Clear();
            colMesh.vertices = colVertices.ToArray();
            colMesh.triangles = colTriangles.ToArray();
            meshCollider.sharedMesh = colMesh;


            Cleanup();
            
        }

        public void CreateFloor(int x, int y, int z, int floorTex, int ceilingTex) {
            AddVerts(x, y, z, ceilingTex, CubeFace.up);
            AddVerts(x, y, z, floorTex, CubeFace.down);
        }
        private void CreateCube(int x, int y, int z, int north, int south, int east, int west) {
            AddVerts(x, y, z, west, CubeFace.left);
            AddVerts(x, y, z, east, CubeFace.right);
            AddVerts(x, y, z, north, CubeFace.front);
            AddVerts(x, y, z, south, CubeFace.back);
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
            Chunk newChunk = new Chunk(c);
            newChunk.blocks = _chunk;
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
            if(currentChunk.blocks2[posX, posZ].damage <= 0)
                return;
           // GameObject c = null;
            x -= 0.5f; //sets the offsets here, better that way.
            z -= 0.5f;
            y -= 0.5f;

            switch(face) {
                case CubeFace.up:
                    AddVertsUp(x, y, z, s);
                    break;
                case CubeFace.down:
                    AddVertsDown(x, y, z, s);
                    break;
                case CubeFace.left:
                    AddVertsLeft(x, y, z, s);
                    break;
                case CubeFace.right:
                    AddVertsRight(x, y, z, s);
                    break;
                case CubeFace.front:
                    AddVertsFront(x, y, z, s);
                    break;
                case CubeFace.back:
                    AddVertsBack(x, y, z, s);
                    break;
            }

            //if(currentChunk.blocks2[posX, posZ].isFloor) {
            //    if(face == CubeFace.up) {
            //        AddVertsUp(x, y, z, s);
            //    }
            //    else if(face == CubeFace.down) {
            //        AddVertsDown(x, y, z, s);
            //    }
            //    else if(face == CubeFace.left) {
            //        if(posX - 1 >= 0 && currentChunk.blocks2[posX - 1, posZ].isFloor == false)
            //            AddVertsLeft(x, y, z, s);
            //        else if (posX - 1 < 0 && surroundingChunks[0] == null)
            //            AddVertsLeft(x, y, z, s);
            //        else if (posX - 1 < 0 && surroundingChunks[0] != null && Global.GetChunk(chunkPosX - 1, chunkPosZ).blocks2[Global.maxChunkSize - 1,posZ].isFloor == false)
            //            AddVertsLeft(x, y, z, s); 
            //    }
            //    else if(face == CubeFace.right) {
            //        if(posX + 1 < Global.maxChunkSize && currentChunk.blocks2[posX + 1, posZ].isFloor == false)
            //            AddVertsRight(x, y, z, s);
            //        else if (posX + 1 == Global.maxChunkSize && surroundingChunks[1] == null)
            //            AddVertsRight(x, y, z, s);
            //        else if(surroundingChunks[1] != null && posX + 1 == Global.maxChunkSize && Global.GetChunk(chunkPosX + 1, chunkPosZ).blocks2[0, posZ].isFloor == false)
            //            AddVertsRight(x, y, z, s);
                    
            //    }
            //    else if(face == CubeFace.front) {
            //        if(posZ - 1 >= 0 && currentChunk.blocks2[posX, posZ - 1].isFloor == false)
            //            AddVertsFront(x, y, z, s);
            //        else if (posZ - 1 < 0 && surroundingChunks[2] == null)
            //            AddVertsFront(x, y, z, s);
            //        else if (surroundingChunks[2] != null && posZ - 1 < 0 && Global.GetChunk(chunkPosX, chunkPosZ - 1).blocks2[posX, Global.maxChunkSize - 1].isFloor == false)
            //            AddVertsFront(x, y, z, s);
            //    }
            //    else if(face == CubeFace.back) {
            //        if(posZ + 1 < Global.maxChunkSize && currentChunk.blocks2[posX, posZ + 1].isFloor == false)
            //            AddVertsBack(x, y, z, s);
            //        else if(posZ + 1 == Global.maxChunkSize && surroundingChunks[3] == null)
            //            AddVertsBack(x, y, z, s);
            //        else if((posZ + 1 == Global.maxChunkSize && surroundingChunks[3] != null && Global.GetChunk(chunkPosX, chunkPosZ + 1).blocks2[posX, 0].isFloor == false))
            //            AddVertsBack(x, y, z, s);
            //    }
            //}
        }

        private void AddVertsLeft(float x, float y, float z, int s) {
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x, y, z));

            colVertices.Add(new Vector3(x, y, z + 1));
            colVertices.Add(new Vector3(x, y + 1, z + 1));
            colVertices.Add(new Vector3(x, y + 1, z));
            colVertices.Add(new Vector3(x, y, z));
            AddTriangles(s);
            }
        private void AddVertsRight(float x, float y, float z, int s) {

            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));

            colVertices.Add(new Vector3(x + 1, y, z));
            colVertices.Add(new Vector3(x + 1, y + 1, z));
            colVertices.Add(new Vector3(x + 1, y + 1, z + 1));
            colVertices.Add(new Vector3(x + 1, y, z + 1));
           
            AddTriangles(s);
        }
        private void AddVertsFront(float x, float y, float z, int s) {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            vertices.Add(new Vector3(x + 1, y, z));

            colVertices.Add(new Vector3(x, y, z));
            colVertices.Add(new Vector3(x, y + 1, z));
            colVertices.Add(new Vector3(x + 1, y + 1, z));
            colVertices.Add(new Vector3(x + 1, y, z));

            AddTriangles(s);
        }
        private void AddVertsBack(float x, float y, float z, int s) {
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x, y, z + 1));

            colVertices.Add(new Vector3(x + 1, y, z + 1));
            colVertices.Add(new Vector3(x + 1, y + 1, z + 1));
            colVertices.Add(new Vector3(x, y + 1, z + 1));
            colVertices.Add(new Vector3(x, y, z + 1));
            
            AddTriangles(s);
        }
        private void AddVertsUp(float x, float y, float z, int s) {

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
        private void AddVertsDown(float x, float y, float z, int s) {

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
        //=====end of class
    }
}
  