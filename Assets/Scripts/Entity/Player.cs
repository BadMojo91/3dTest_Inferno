using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno {

    public struct BlockReturn {
        public int x, y, z;
        public int chunkIndex;
    }

    [System.Serializable]
    public class PlayerInfo {
        public LightDetector lightDetector;
        public bool stealthEnabled;
        public float lightRange;
        public float health = 100;
        public float armor = 0;
    }
    public class Player : MonoBehaviour {
        public static BlockReturn blockReturn;
        public PlayerInfo playerInfo;
        private Global.Compass currentDirection;
        private Inventory inventory;
        public Global.Compass CurrentDirection {
            get { return currentDirection; }
        }
        public float interactDelay;
        private float iDelayTime;
        public bool lockMouse;

        private float moveSpeed;
        public float MoveSpeed {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }
        public float maxMoveSpeed = 5;
        public float accelleration = 5;
        public float turnSpeed = 10;

        private Vector3 targetRotation;
        private Vector3 targetPosition;
        public Vector3 currentPosition;
        public Vector2 currentChunk;
        Vector3 tempPos;

        private void OnGUI() {
            //Show light meter
            GUI.BeginGroup(new Rect(Screen.width / 2 - (64 / 2), Screen.height - 64, 64, 64));
            GUI.Box(new Rect(0.5f, 0, 64, 64), playerInfo.lightDetector.lightMeter);
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(Screen.width - 200, 0, 200, 600));
            GUI.Box(new Rect(0, 0, 200, 100), "Chunk X: " + currentChunk.x + " Z: " + currentChunk.y);
            GUI.Box(new Rect(0, 10, 200, 100), "Position X: " + currentPosition.x + " Y: " + currentPosition.y + " Z: " + currentPosition.z);
            GUI.EndGroup();
        }

        private void Awake() {
            inventory = GetComponent<Inventory>();
        }
        private void Start() {
            targetPosition = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z)); //Start position rounded to grid
        }
        private void Update() {
            //enable light meter
            if(playerInfo.stealthEnabled)
                playerInfo.lightRange = playerInfo.lightDetector.currentLightRange;

            //Interact delay
            if(iDelayTime < interactDelay)
                iDelayTime += Time.deltaTime;

            //Player main update
            PlayerUpdate();

            //Show inventory menu
            if(Input.GetKeyDown(KeyCode.Tab)) {
                if(Global.activeUi) {
                    Destroy(Global.activeUi);
                    return;
                }
                else {
                    Global.uiActive = true;
                    GameObject invView = Instantiate(Global.inventoryViewPrefab, GameObject.Find("Canvas").transform);
                    invView.GetComponentInChildren<InventoryView>().AddInventoryToInventoryView(inventory);
                    Global.activeUi = invView;
                }
            }
        }
        private void FixedUpdate() {
            PlayerFixedUpdate();
        }
        private void PlayerFixedUpdate() {
            //Platform NO/G
            if(Global.GameType == 0) { }
            //Platform WITH/G
            if(Global.GameType == 1) { }
            //2.5d HYPER
            if(Global.GameType == 2) {
            }
            //2.5d WOLF
            if(Global.GameType == 3) { }
            //3d
            if(Global.GameType == 4) { }
            //flight
            if(Global.GameType == 5) { }
        }

        private void PlayerUpdate() {


            //Set current posistion on grid
            currentPosition = Global.RoundVector3(transform.position);
            //Set current chunk
            int px = (int)currentPosition.x, pz = (int)currentPosition.z;
            int cx = 0, cz = 0;
            while(px > Global.maxChunkSize) {
                px -= Global.maxChunkSize;
                cx++;
            }
            while(px < -Global.maxChunkSize) {
                px += Global.maxChunkSize;
                cx--;
            }
            while(pz > Global.maxChunkSize) {
                pz -= Global.maxChunkSize;
                cz++;
            }
            while(pz < -Global.maxChunkSize) {
                pz += Global.maxChunkSize;
                cz--;
            }
            currentChunk = new Vector2(cx, cz);

            //Mouse cursor state
            if(lockMouse) {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
                Cursor.lockState = CursorLockMode.None;

            if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.G)) {
                if(lockMouse)
                    lockMouse = false;
                else
                    lockMouse = true;
            }

            //Game mode dependant stuff
            //==========================
            //Platform NO/G
            if(Global.GameType == 0) { }
            //Platform WITH/G
            if(Global.GameType == 1) { }
            //2.5d HYPER
            if(Global.GameType == 2) {
                int tDir = 0;// temp turn direction

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

                currentPosition = Global.RoundVector3(transform.position);
                if(currentPosition == tempPos && moveSpeed > 0) {
                    moveSpeed = 0;
                }

                if(Input.touchCount > 0) {
                    Touch touch1 = Input.touches[0];
                    int width = Screen.width;
                    int height = Screen.height;
                    Vector2 tPos = new Vector2(Mathf.InverseLerp(width, 0, touch1.position.x), Mathf.InverseLerp(height, 0, touch1.position.y));
                    //Debug.Log("tPos: " + tPos);
                    if(tPos.y > 0.8f && tPos.x > 0.2f && tPos.x < 0.8f)
                        // moveSpeed += Time.deltaTime * accelleration;
                        moveSpeed = accelleration;
                    else if(tPos.y < 0.2f && tPos.x > 0.2f && tPos.x < 0.8f)
                        // moveSpeed -= Time.deltaTime * accelleration;
                        moveSpeed = accelleration;
                    switch(touch1.phase) {
                            case TouchPhase.Began:
                                if(tPos.x > 0.8f)
                                    tDir = -90;
                                if(tPos.x < 0.2f)
                                    tDir = 90;
                                if(tPos.y < 0.8f && tPos.y > 0.2f && tPos.x > 0.2f && tPos.x < 0.8f)
                                    Interact(true);

                                    if(tPos.y > 0.8f && tPos.x > 0.2f && tPos.x < 0.8f)
                                        tempPos = Global.RoundVector3(currentPosition + transform.TransformDirection(Vector3.forward).normalized);
                                    if(tPos.y < 0.2f && tPos.x > 0.2f && tPos.x < 0.8f)
                                        tempPos = Global.RoundVector3(currentPosition + transform.TransformDirection(Vector3.forward).normalized);
                              
                                break;
                        }
                }
                transform.position = Vector3.MoveTowards(transform.position,tempPos, Time.deltaTime * accelleration);
#elif UNITY_STANDALONE || UNITY_WEBPLAYER
                if(Input.GetButtonDown("Fire1"))
                    Interact();
                moveSpeed += Input.GetAxis("Vertical") * Time.deltaTime * accelleration;

                if(Input.GetButtonDown("Left"))
                    tDir = -90;
                if(Input.GetButtonDown("Right"))
                    tDir = 90;

                //Movement
                targetPosition = Global.RoundVector3(transform.position) + transform.TransformDirection(Vector3.forward);
                if(moveSpeed > maxMoveSpeed)
                    moveSpeed = maxMoveSpeed;
                GB2DMovement.MoveUpdate(transform, moveSpeed, targetPosition, true);
#endif



                targetRotation = GB2DMovement.TurnUpdate(targetRotation, transform, tDir, turnSpeed);

            }
            //2.5d WOLF
            if(Global.GameType == 3) {
                if(Input.GetButton("Fire1") && iDelayTime >= interactDelay) {
                    Interact();
                    iDelayTime = 0f;
                }
                if(lockMouse)
                    GB3DMovement.MoveUpdate(transform, maxMoveSpeed, turnSpeed);
            }
            //3d
            if(Global.GameType == 4) { }
            //flight
            if(Global.GameType == 5) { }
        }

        public void DestroyBlock() {
            if(GetBlock().collider != null) {
            MeshBuilder meshBuilder = GetBlock().collider.gameObject.GetComponent<MeshBuilder>();
            //if(meshBuilder != null) {
                Global.chunks[blockReturn.chunkIndex].blocks[blockReturn.x, blockReturn.z].isFloor = true;

                meshBuilder.BuildMesh();
                meshBuilder.UpdateMesh();
                //WorldGen.UpdateAllChunks();
                Debug.Log(Global.chunks[blockReturn.chunkIndex].blocks[blockReturn.x, blockReturn.z].isFloor);
            }
            // }
        }

        public RaycastHit GetBlock() {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit)) {
                if(hit.collider.tag == "World") {

                    Vector3 hitPoint = hit.point + hit.normal * -0.5f;
                    hitPoint.y = 0;
                    //Debug.Log(hitPoint);
                    int cPosX = 0;
                    int cPosZ = 0;
                    int x = Mathf.FloorToInt(hitPoint.x + 0.5f);
                    int z = Mathf.FloorToInt(hitPoint.z + 0.5f);
                    //Debug.DrawLine(currentPosition, new Vector3(x, 0, z), Color.red, 5);
                    while(x >= Global.maxChunkSize) {
                        x -= Global.maxChunkSize;
                        cPosX += 1;
                    }
                    while(x <= -Global.maxChunkSize) {
                        x += Global.maxChunkSize;
                        cPosX -= 1;
                    }
                    while(z >= Global.maxChunkSize) {
                        z -= Global.maxChunkSize;
                        cPosZ += 1;
                    }
                    while(z <= -Global.maxChunkSize) {
                        z += Global.maxChunkSize;
                        cPosZ -= 1;
                    }
                    //Debug.Log(hit.collider.gameObject.name + x + "," + z);

                    if(x >= 0 && x < Global.maxChunkSize && z >= 0 && z < Global.maxChunkSize) {
                        BlockReturn r = new BlockReturn();
                        r.chunkIndex = Global.GetChunkIndex(cPosX, cPosZ);
                        r.x = x;
                        r.z = z;
                        r.y = 0;
                        blockReturn = r;
                        
                    }



                    //WorldGen.ChunkUpdate(hit.collider.GetComponent<MeshBuilder>());
                    //for(int i = 0; i < 4; i++) {
                    //    if(hit.collider.GetComponent<MeshBuilder>().surroundingChunks[i] != null)
                    //        WorldGen.ChunkUpdate(hit.collider.GetComponent<MeshBuilder>().surroundingChunks[i].GetComponent<MeshBuilder>());
                    //}

                }
            }

            return hit;

        }
        public void Interact() {
            if(!Global.uiActive) {
                DestroyBlock();
            }
        }
        public void Look() {
            int tDir = 0;
            if(Input.GetAxis("Horizontal") < 0)
                tDir = -90;
            if(Input.GetAxis("Horizontal") > 0)
                tDir = 90;

            Vector3 lookPos = targetRotation - targetPosition;
            lookPos.y = 0;
            Quaternion rotation = transform.rotation;
            if(lookPos != Vector3.zero) {
                rotation = Quaternion.LookRotation(lookPos);
            }
            rotation = Quaternion.Euler(0, tDir, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        }

    }





}