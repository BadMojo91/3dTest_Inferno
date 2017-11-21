using UnityEngine;
using System.Collections;

public class FaceToward : MonoBehaviour {

    GameObject player;
    void Update() {
        if(player == null) {
            player = GameObject.Find("Player");

        }
        else {
            transform.LookAt(player.transform);
        }
    }
}
