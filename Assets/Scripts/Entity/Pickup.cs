using UnityEngine;
using System.Collections;
namespace Inferno {
    public class Pickup : MonoBehaviour {

        public Item item;
        public Sprite sprite;
        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.tag == "Player") {
                other.GetComponent<Inventory>().items.Add(item);
                Destroy(gameObject);
            }
        }
    }
}