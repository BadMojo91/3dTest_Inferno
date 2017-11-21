using System.Collections;
using System;
using UnityEngine;

namespace Inferno {
    [Serializable]
    public class Block{
        public int[] subMesh = new int[6];
        public int posX;
        public int posZ;
        public int posY;
        public bool isFloor;
        public float damage;
        /// <summary>
        /// Block(x, y, z, submesh)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="s"></param>
        public Block(int x, int y, int z, int[] s) {
            posX = x;
            posY = y;
            posZ = z;
            subMesh = s;
            damage = 100;
        }

        public Block(int x, int y, int z, int[] s, float d) {
            posX = x;
            posY = y;
            posZ = z;
            subMesh = s;
            damage = d;
        }
    }
}
