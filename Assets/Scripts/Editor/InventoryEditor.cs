using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public enum ITEMTYPE {
    Consumable = 0,
    Weapon = 1,
    Equipable = 2,
    Quest = 3,
    Misc = 4
}

namespace Inferno {
    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : Editor {
        public string itemName;
        public ITEMTYPE itemType;
        public int itemId;
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            Inventory inv = (Inventory)target;

            itemName = EditorGUILayout.TextField(itemName);
            itemType = (ITEMTYPE)EditorGUILayout.EnumPopup("Item type: ", itemType);
            if(GUILayout.Button("Clear Inventory")) {
                inv.ClearItems();
                inv.itemCount = inv.items.Count;
            }

            if(GUILayout.Button("Add Item")) {
                switch(itemType) {
                    case ITEMTYPE.Consumable:
                        inv.AddItem(new Item(itemName, Item.Type.Consumable));
                        break;
                    case ITEMTYPE.Weapon:
                        inv.AddItem(new Item(itemName, Item.Type.Weapon));
                        break;
                    case ITEMTYPE.Equipable:
                        inv.AddItem(new Item(itemName, Item.Type.Equipable));
                        break;
                    case ITEMTYPE.Quest:
                        inv.AddItem(new Item(itemName, Item.Type.Quest));
                        break;
                    case ITEMTYPE.Misc:
                        inv.AddItem(new Item(itemName, Item.Type.Misc));
                        break;
                    default:
                        Debug.LogError("Unrecognized Item type");
                        break;
                }
               
            }
            itemId = EditorGUILayout.IntField("Item id: ", itemId);
            if(GUILayout.Button("Remove Item")) {
                inv.RemoveItem(itemId);
            }
        }
    }
}