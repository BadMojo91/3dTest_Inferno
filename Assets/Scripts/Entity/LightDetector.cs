using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Inferno {
    public class LightDetector : MonoBehaviour {
        public Texture2D currentLightTarget;
        public Texture2D lightMeter;
        public float currentLightRange;
        public float sensitivity = 1f;
        public int resolution;
        public int size = 32;
        public float frameTime = 0.5f;
        float tick;

        public bool grab;
        public Camera camera;

        private void Start() {

        }
        private void OnPostRender() {
            if(grab) {
                tick -= Time.deltaTime;
                if(tick < 0) {
                    GetScreen();
                    lightMeter = ResizeTexture(currentLightTarget);
                    tick = frameTime;
                }
            }

           
           

        }

        public Texture2D ResizeTexture(Texture2D tex) {
            Texture2D t = tex;
            TextureScale.Point(t, 64, 64);
            t.Apply();
            return t;
        }

        private void Update() {

        }

        public void GetScreen() {

            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            camera.targetTexture = rt;
            currentLightTarget = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
           // camera.Render();
            RenderTexture.active = rt;
            currentLightTarget.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
           currentLightTarget.Resize(resolution, resolution);
            
            LightDetectorUpdate();
            currentLightTarget.Apply();
        }

        public void LightDetectorUpdate() {
            Texture2D t = currentLightTarget;
            t.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            t.Apply();
            List<float> bright = new List<float>();
            for(int x = 0; x < resolution; x++) {
                for(int y = 0; y < resolution; y++) {
                    Color c = t.GetPixel(x, y);
                    bright.Add(Mathf.Clamp01(Brightness(c)));
                    //Debug.Log(bright);
                }
            }
            float sum = 0;
            foreach(float i in bright) {
                sum += i;
            }
            currentLightRange = Mathf.Clamp((sum /(resolution*resolution)) * sensitivity, 0, 1);
        }

        private float Brightness(Color c) {
            return Mathf.Sqrt(
               c.r * c.r * .241f +
               c.g * c.g * .691f +
               c.b * c.b * .068f);
        }
    }
}