using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
    public float speed;
    public Vector3 dir;
	void Update () {
        transform.Rotate(dir * Time.deltaTime * speed);
	}
}
