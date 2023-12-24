using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NatSuite.Examples.Components;

public class ActionCardRecordingHandle : UIScreen
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField]
    CameraPreview _cameraView;
    private MP4Recorder recorder;
    private Coroutine recordVideoCoroutine;


    public async void startRecording()
    {
        // Create a recorder
        recorder = new MP4Recorder(_cameraView.cameraTexture.width, _cameraView.cameraTexture.height, 30, bitrate: 6_000_000,   // bits per second
        keyframeInterval: 3);
        //webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name, 720, 1280, 30);
        //webcamTexture.Play();
        //Start recording
        //webcampreview.texture = webcamTexture;
        recordVideoCoroutine = StartCoroutine(recording());
    }

    WebCamTexture webcamTexture;
    private IEnumerator recording()
    {
        // Create a clock for generating recording timestamps
        var clock = new RealtimeClock();
        for (int i = 0; ; i++)
        {
            // Commit the frame to NatCorder for encoding
            recorder.CommitFrame(_cameraView.cameraTexture.GetPixels32(), clock.timestamp);
            // Wait till end of frame
            yield return new WaitForEndOfFrame();
        }
    }

    public async void stopRecording()
    {
        //Stop Coroutine
        StopCoroutine(recordVideoCoroutine);
        // Finish writing
        var recordingPath = await recorder.FinishWriting();
        Debug.Log("filse stored ");
        UIController.Instance.GoToActionCardReview(recordingPath);
    }
   
}
