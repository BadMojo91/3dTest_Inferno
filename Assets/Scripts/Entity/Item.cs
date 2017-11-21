using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Item : IComparable<Item>{
    public int id;
    public string name;
    public enum Type {Consumable, Weapon, Equipable, Quest, Misc};
    public Type type;
    public Sprite icon;
    public Item(string n, Type t) {
        id = -1;
        name = n;
        type = t;
    }

    public int CompareTo(Item other) {
        if(other == null)
            return 1;

        return id - other.id;
    }


}
