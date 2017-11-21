using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inferno {
    public class InventoryView : MonoBehaviour {
        /// <summary>
        /// InventoryView child transform "Content"
        /// </summary>
        public Transform content;
        /// <summary>
        /// Item button prefab
        /// </summary>
        public GameObject itemObj;
        /// <summary>
        /// UI buttons for each item
        /// </summary>
        public List<GameObject> items = new List<GameObject>();
        public GameObject currentSelectedItem;
        public GameObject CurrentSelectedItem {
            get { return currentSelectedItem; }
            set { currentSelectedItem = value; }
        }
        public void AddInventoryToInventoryView(Inventory inv) {
            foreach(Item i in inv.items) {
                GameObject newItem = Instantiate(itemObj, content);
                newItem.GetComponent<UIItem>().item = i;
                newItem.GetComponent<UIItem>().iv = this;
            }
            items.Sort();
        }

        public void AddItemToInventoryView(Item i) {
            GameObject newItem = Instantiate(itemObj, content);
            newItem.GetComponent<UIItem>().item = i;
            newItem.GetComponent<UIItem>().iv = this;
            items.Add(newItem);
            items.Sort();
        }
        private void OnDestroy() {
            Global.uiActive = false;
        }
    }

 
}