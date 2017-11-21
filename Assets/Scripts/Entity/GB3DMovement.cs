using UnityEngine;
using System.Collections;

public class GB3DMovement {

    public static void MoveUpdate(Transform self, float speed, float turnSpeed) {
        float mouseInput = Input.GetAxis("Mouse X") * turnSpeed;
        Quaternion turn = Quaternion.Euler(new Vector3(0, mouseInput, 0));
        self.rotation *= turn;
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        dir = self.TransformDirection(dir);
        RaycastHit hit;
        if(!Physics.Raycast(self.position, dir.normalized, out hit, 0.5f) || hit.collider.tag == "Pickup")
            self.position += dir * Time.deltaTime * speed;
    }
}
