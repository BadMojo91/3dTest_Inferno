using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inferno {
    public class LightDetector : MonoBehaviour {
        public Texture2D currentLightTarget;
        public float currentLightRange;
        public float sensitivity = 1.5f;
        public float frameTime = 2;
        float tick;

        public bool grab;
        public Camera camera;


        private void OnPostRender() {
            if(grab) {
                tick -= Time.deltaTime;
                if(tick < 0) {
                    GetScreen();
                    LightDetectorUpdate();
                    tick = frameTime;
                }
            }
        }

        private void Update() {

        }

        public void GetScreen() {
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            camera.targetTexture = rt;
            currentLightTarget = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            currentLightTarget.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            currentLightTarget.Resize(8, 8);
            camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
        }

        public void LightDetectorUpdate() {
            Texture2D t = currentLightTarget;
            t.ReadPixels(new Rect(0, 0, 8, 8), 0, 0);
            t.Apply();
            List<float> bright = new List<float>();
            for(int x = 0; x < 8; x++) {
                for(int y = 0; y < 8; y++) {
                    Color c = t.GetPixel(x, y);
                    bright.Add(Mathf.Clamp01(Brightness(c)));
                    //Debug.Log(bright);
                }
            }
            float sum = 0;
            foreach(float i in bright) {
                sum += i;
            }
            currentLightRange = (sum / 64) * sensitivity;
        }

        private float Brightness(Color c) {
            return Mathf.Sqrt(
               c.r * c.r * .241f +
               c.g * c.g * .691f +
               c.b * c.b * .068f);
        }
    }
}