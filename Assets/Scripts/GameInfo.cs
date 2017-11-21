using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inferno {
    [ExecuteInEditMode]
    public class GameInfo : MonoBehaviour {
        [Tooltip("Possible game types:\n 0 = Grid based platform no gravity(heartlight)\n 1 = Platform with gravity(commander keen)\n 2 = Grid based 2.5d restricted(hypercycles)\n 3 = 2.5d (doom, duke3d)\n 4 = full 3d (quake)\n 5 = flight(decent)\n")]
        public byte gameType;
        public GameObject inventoryView;
        public GameObject pickupPrefab;
        private void Update() {
            if(Global.GameType != gameType) {
                Global.GameType = gameType;
                gameType = Global.GameType;
            }
            if(Global.inventoryViewPrefab != inventoryView)
                Global.inventoryViewPrefab = inventoryView;

            if(Global.pickupPrefab != pickupPrefab)
                Global.pickupPrefab = pickupPrefab;
        }
    }

}