using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inferno {
    [ExecuteInEditMode]
    public class CameraMain : MonoBehaviour {
        private GameObject player;
        private void Update() {
            if(player == null || player.tag != "Player") {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            else {
                UpdateCamera();

            }

        }

        private void UpdateCamera() {
            //Platform NO/G
            if(Global.GameType == 0) { } 
            //Platform WITH/G
            if(Global.GameType == 1) { }
            //2.5d HYPER
            if(Global.GameType == 2 || Global.GameType == 3) {
                transform.position = player.transform.position;
                transform.rotation = player.transform.rotation;
            }
            //3d
            if(Global.GameType == 4) { }
            //flight
            if(Global.GameType == 5) { }
        }
    }

}