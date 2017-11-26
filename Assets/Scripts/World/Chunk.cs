using UnityEngine;
using System;
namespace Inferno {
    [Serializable]
    public class Chunk {
        public int x,z;
        public Block[,] blocks;

        public Chunk(Block[,] _blocks) {
            blocks = _blocks;
        }
        public Chunk(int _x, int _z, Block[,] _blocks) {
            blocks = _blocks;
            x = _x;
            z = _z;
        }
    }
    [Serializable]
    public class SavedChunk {
        public int x,z;
        public Block[] blocks;

        public SavedChunk(int _x, int _z, Block[] _blocks) {
            blocks = _blocks;
            x = _x;
            z = _z;
        }
    }
}