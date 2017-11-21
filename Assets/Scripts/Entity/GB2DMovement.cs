using UnityEngine;
using System.Collections;
namespace Inferno {
    public class GB2DMovement {
        private static bool isObstructed;
        
        public static void MoveUpdate(Transform self, float speed, Vector3 dir, bool raycast) {

            dir.x = Mathf.Round(dir.x);
            dir.z = Mathf.Round(dir.z);
            dir.y = Mathf.Round(0);

            //Raycast
            if(raycast) {
                RaycastHit hit;
                Debug.DrawRay(self.position, self.TransformDirection(Vector3.forward)* 0.6f, Color.blue + Color.red);
                if(Physics.Raycast(self.position, self.TransformDirection(Vector3.forward), out hit, 0.6f)) {
                    if(hit.collider.gameObject.tag != "Player" && hit.collider.gameObject.tag != "Pickup") {
                        isObstructed = true;
                        Vector3 newPoint = Global.RoundVector3(hit.point);
                        //Debug.Log(hit.collider.gameObject);
                        self.GetComponent<Player>().MoveSpeed = 0;
                        self.transform.position = Global.RoundVector3(self.transform.position);
                        
                    }
                }
                else {
                    isObstructed = false;
                }
            }

            //Movement
            if(!isObstructed && self.position != dir) {
                self.transform.position = Vector3.MoveTowards(self.position, dir, Time.deltaTime * speed);
            }
        }

        public static Vector3 TurnUpdate(Vector3 targetRot, Transform self, int degrees, float speed) {
            Vector3 targetPos;
            if(degrees > 0 || degrees < 0) {
                self.position = Global.RoundVector3(self.position);
                targetPos = Global.RoundVector3(self.position);
            }

            targetRot += new Vector3(0, degrees, 0);
            if(targetRot.y > 360 || targetRot.y < -360)
                targetRot.y = 0;

            Quaternion rotation = self.rotation;
            rotation = Quaternion.Euler(0, targetRot.y, 0);

            if(self.rotation != rotation)
                self.rotation = Quaternion.Slerp(self.rotation, rotation, Time.deltaTime * speed);

            return targetRot;
        }
//===================
    }
}