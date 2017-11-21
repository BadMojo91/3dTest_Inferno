using UnityEngine;
using System;
namespace Inferno {
    [Serializable]
    public class Chunk{
        public int x,z;
        public Block[] blocks;
        public Block[,] blocks2;
        public Chunk(Block[] _blocks) {
            blocks = _blocks;
        }
        public Chunk(Block[,] _blocks) {
            blocks2 = _blocks;
        }
    }
}