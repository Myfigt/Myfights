/* 
*   NatCorder
*   Copyright (c) 2020 Yusuf Olokoba
*/

namespace NatSuite.Examples.Components {

    using UnityEngine;
    using UnityEngine.Android;
    using UnityEngine.UI;
    using System.Collections;
    using TMPro;
	[RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
    public class CameraPreview : MonoBehaviour {

        public WebCamTexture cameraTexture { get; private set; }
		public RawImage rawImage;
		private AspectRatioFitter aspectFitter;
        public TMP_Text temp;
        public Quaternion baseRotation;
        private void OnEnable()
        {
            StartCoroutine(InitializeCamera());
        }
        IEnumerator InitializeCamera() {
			rawImage = GetComponent<RawImage>();
			aspectFitter = GetComponent<AspectRatioFitter>();
            yield return new WaitForSeconds(1);
            // Request camera permission
            if (Application.platform == RuntimePlatform.Android) {
                if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) {
                    Permission.RequestUserPermission(Permission.Camera);
                    yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
                }
            } else {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
                if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
                    yield break;
            }
            // Start the WebCamTexture
            if (WebCamTexture.devices.Length >=2)
            {
                cameraTexture = new WebCamTexture(WebCamTexture.devices[1].name, Screen.width, Screen.height, 30);//requesting front camera

            }
            else
                cameraTexture = new WebCamTexture(WebCamTexture.devices[0].name, 720, 1280, 30);
            baseRotation = transform.rotation;
            cameraTexture.Play();
            yield return new WaitUntil(() => cameraTexture.width != 16 && cameraTexture.height != 16); // Workaround for weird bug on macOS
            // Setup preview shader with correct orientation
            rawImage.texture = cameraTexture;
            rawImage.material.SetFloat("_Rotation", cameraTexture.videoRotationAngle * Mathf.PI / 180f);
            rawImage.material.SetFloat("_Scale", cameraTexture.videoVerticallyMirrored ? -1 : 1);
            // Scale the preview panel
            if (cameraTexture.videoRotationAngle == 90 || cameraTexture.videoRotationAngle == 270)
                aspectFitter.aspectRatio = (float)cameraTexture.height / cameraTexture.width;
            else
                aspectFitter.aspectRatio = (float)cameraTexture.width / cameraTexture.height;
        }

        private void OnDisable()
        {
            cameraTexture.Stop();
        }
        private void Update()
        {
            if (cameraTexture !=  null)
            {
                temp.text = cameraTexture.videoRotationAngle.ToString();
               // transform.rotation = baseRotation * Quaternion.AngleAxis(cameraTexture.videoRotationAngle, Vector3.up);
            }
        }
    }
}