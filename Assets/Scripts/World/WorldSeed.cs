using UnityEngine;
using System.Collections.Generic;
using System;
[Serializable]
public class WorldSeed{
    public float frequency;
    public float amplitude;
    public float octave;
    public List<Reigon> reigons = new List<Reigon>();
    public List<Item> randomLoot = new List<Item>();
}
[Serializable]
public class Reigon {
    public int[] subMeshes = new int[6];
    public float level;
}