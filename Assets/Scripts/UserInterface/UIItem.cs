using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Inferno {
    public class UIItem : MonoBehaviour {
        public Item item;
        public InventoryView iv;
        public bool pressed;
        public void Start() {
            GetComponentInChildren<Text>().text = item.name;
        }
        public void SelectItem() {
            iv.currentSelectedItem = gameObject;
            pressed = true;
        }

        private void OnGUI() {
            pressed = GUILayout.Toggle(pressed, item.name, "Button");
        }
    }
}