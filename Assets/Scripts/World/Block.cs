using System.Collections;
using System;
using UnityEngine;

namespace Inferno {
    [Serializable]
    public class Block{
        public int posX;
        public int posZ;
        public int posY;
        public bool isFloor;
        public float damage;
        public int north,south,east,west, floor, ceiling;
        /// <summary>
        /// Block(x, y, z, submesh)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="s"></param>
        public Block(int x, int y, int z, int c, int f, int w, int e, int n, int s) {
            posX = x;
            posY = y;
            posZ = z;
            north = n;
            south = s;
            east = e;
            west = w;
            floor = f;
            ceiling = c;
            damage = 100;
        }

        public Block(int x, int y, int z, int c, int f, int w, int e, int n, int s, float d) {
            posX = x;
            posY = y;
            posZ = z;
            north = n;
            south = s;
            east = e;
            west = w;
            floor = f;
            ceiling = c;
            damage = d;
        }
    }
}
