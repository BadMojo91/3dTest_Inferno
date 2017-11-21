using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inferno {
    public class UiActivator : MonoBehaviour {
        private void Update() {
            Global.uiActive = true;
            if(Input.GetKeyDown(KeyCode.Tab)) {
                Destroy(this.gameObject);
            }
        }
        private void OnDestroy() {
            Global.uiActive = false;
        }

    }
}