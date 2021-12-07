/* 
*   NatCorder
*   Copyright (c) 2020 Yusuf Olokoba
*/

namespace NatSuite.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using Recorders;
    using Recorders.Clocks;
    using System.Linq;
    using UnityEngine.Video;
    using DG.Tweening;
    using System.IO;
    using UnityEngine.UI;

    public class WebCam : MonoBehaviour {
        public static WebCam instance;
        public static int move;
        [Header(@"UI")]
        public RawImage rawImage;
        public static string clippath;
        public AspectRatioFitter aspectFitter;
        public Button createcard, save, delete, close;
        public GameObject videopanel;
        public GameObject camerapanel, recordbutton;
        public GameObject replayvideos, gameplay;
        public GameObject cardscroll, manager;
      
        WebCamDevice frontCameraDevice;
        WebCamDevice backCameraDevice;
        WebCamDevice activeCameraDevice;
        string myCamera = "BACK";
        WebCamTexture frontCameraTexture;
        WebCamTexture backCameraTexture;
        WebCamTexture activeCameraTexture;
        private MP4Recorder recorder;
        private IClock clock;
        private bool recording;
        private Color32[] pixelBuffer;
        Vector3 rotationVector = new Vector3(0f, 0f, 0f);
        Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
        Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

        // Image Parent's scale
        Vector3 defaultScale = new Vector3(1f, 1f, 1f);
        Vector3 fixedScale = new Vector3(-1f, 1f, 1f);
        #region --Recording State--
        private void Awake()
        {
            instance = this;
        }
        public void StartRecording () {
            // Start recording
            clock = new RealtimeClock();
            recorder = new MP4Recorder(activeCameraTexture.width, activeCameraTexture.height, 30);
            pixelBuffer = activeCameraTexture.GetPixels32();
            recording = true;
            deactivescroll();
        }
        public void enablerecordbutton()
        {
            recordbutton.SetActive(true);
        }
        public void activescroll()
        {
            cardscroll.SetActive(true);
            //cardscroll.GetComponent<RingCardDeck>().enabled = true;
        }
        public void deactivescroll()
        {
            //cardscroll.GetComponent<RingCardDeck>().enabled = false;
            cardscroll.SetActive(false);
        }
        public void secondscroll()
        {
            //cardscroll2.SetActive(true);
            //manager.GetComponent<SliderMenu>().enabled = true;
           // cardscroll2.transform.SetAsLastSibling();
        }
        public async void StopRecording () {
            // Stop recording
            recording = false;
            var path = await recorder.FinishWriting();
            Debug.Log("Name Of File: " +path);
          //  SSTools.ShowMessage(path, SSTools.Position.bottom, SSTools.Time.threeSecond);
            // Playback recording
            Debug.Log($"Saved recording to: {path}");
            clippath = path;
            recordbutton.SetActive(false);
            Manager.instance.recordedplayer.gameObject.SetActive(true);
            Manager.instance.playvideo(path);

         
          //  Handheld.PlayFullScreenMovie($"file://{path}",Color.black,FullScreenMovieControlMode.Full);
        }
        #endregion

        #region --Operations--

//        IEnumerator Start () {
//            // Request camera permission
//            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
//            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
//                yield break;
//            if (WebCamTexture.devices.Length == 0)
//            {
//                Debug.Log("No devices cameras found");
//                yield break;
//            }
//            frontCameraDevice = WebCamTexture.devices.Last();
//            backCameraDevice = WebCamTexture.devices.First();
//            frontCameraTexture = new WebCamTexture(frontCameraDevice.name);
//            backCameraTexture = new WebCamTexture(backCameraDevice.name);

//            frontCameraTexture.filterMode = FilterMode.Trilinear;
//            backCameraTexture.filterMode = FilterMode.Trilinear;
//            backCameraTexture = new WebCamTexture((int)rawImage.gameObject.GetComponent<RectTransform>().sizeDelta.x,
//                (int)rawImage.gameObject.GetComponent<RectTransform>().sizeDelta.y, 30)
//;            if (myCamera.Equals("FRONT"))
//                SetActiveCamera(frontCameraTexture);
//            else if (myCamera.Equals("BACK"))
//                SetActiveCamera(backCameraTexture);
//            else 
//                SetActiveCamera(backCameraTexture);

//            yield return new WaitUntil(() => activeCameraTexture.width != 16 && activeCameraTexture.height != 16); 
                                                                                                                  
                                                                                                                  
//            if (activeCameraTexture.width < 100)
//            {
//                Debug.Log("Still waiting another frame for correct info...");
//                yield break;
//            }

      
//            rotationVector.z = -activeCameraTexture.videoRotationAngle;

//            rawImage.rectTransform.localEulerAngles = rotationVector;

    
//            SSTools.ShowMessage("" + activeCameraTexture.width + "/" + activeCameraTexture.height, SSTools.Position.bottom, SSTools.Time.threeSecond);
//            float videoRatio =
//                (float)activeCameraTexture.width / (float)activeCameraTexture.height;
//        }

        void Update () {
           // Record frames from the webcam
            //if (recording && activeCameraTexture.didUpdateThisFrame)
            //{
            //    activeCameraTexture.GetPixels32(pixelBuffer);
            //    recorder.CommitFrame(pixelBuffer, clock.timestamp);
            //}

        }
        public void SetActiveCamera(WebCamTexture cameraToUse)
        {
            if (activeCameraTexture != null)
            {
                activeCameraTexture.Stop();
            }

            activeCameraTexture = cameraToUse;
            activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device =>
                device.name == cameraToUse.deviceName);

            rawImage.texture = activeCameraTexture;
            rawImage.material.mainTexture = activeCameraTexture;
          

            activeCameraTexture.Play();
        }
        #endregion
        public void MovePanel(GameObject panel)
        {
            panel.SetActive(true);
            panel.transform.SetAsLastSibling();
            panel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.2f);
        }
        public void hidepanel(GameObject panel)
        {
            panel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1300, 0), 0.2f);
        }
        public void MoveSelected(int value)
        {
            move = value;
        }
        public void SaveClip()
        {
            Debug.Log(move);
            PlayerPrefs.SetString(move + "", clippath);
            hidepanel(videopanel);
          //  player.Stop();
            gameplay.SetActive
                (false);
            replayvideos.SetActive(true);
            
        }
        public void deleteclip()
        {
            File.Delete(clippath);
            hidepanel(videopanel);
          //  player.Stop();
            gameplay.SetActive(true);
            replayvideos.SetActive(false);
        }
        public void Loadclip(string value)
        {
            if (PlayerPrefs.HasKey(value)){
                var loadpath = PlayerPrefs.GetString(value);
                MovePanel(videopanel);
              //  player.url = loadpath;
             //   player.Play();
                save.interactable = false;
                StartCoroutine(delay());
            }
        }
        public void quit()
        {
            Application.Quit();
        }
        IEnumerator delay()
        {
            yield return new WaitForSeconds(4f);
            hidepanel(videopanel);
        }

    }
}