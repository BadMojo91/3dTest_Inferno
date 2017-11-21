using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inferno {
    public class Inventory : MonoBehaviour {
        public GameObject inventoryMenu;
        public List<Item> items = new List<Item>();
        public int itemCount;
        public void ClearItems() {
            items.Clear();
        }
        public void AddItem(Item i) {
            items.Add(i);
            i.id = itemCount;
            Debug.Log("Item added with id: " + i.id);
            itemCount = items.Count;
            items.Sort();

        }
        public void RemoveItem(int id) {
            bool removedItem = false;
            for(int i = 0; i < items.Count; i++) {
                if(items[i].id == id) {
                    Debug.Log("Removing item: " + items[i].id + "_" + items[i].name + "_" + items[i].type);
                    items.RemoveAt(i);
                    removedItem = true;
                    break;
                }
            }
            if(removedItem) {
                itemCount = items.Count;
                items.Sort();
            }
            else {
                Debug.LogError("Item id out of range");
            }
        }

        public void OpenInventoryMenu(Inventory inv) {
            if(!Global.uiActive)
                inventoryMenu = Instantiate(Resources.Load("InventoryView") as GameObject);


        }
    }
}