using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Video;
using System.IO;
using UnityEngine.SceneManagement;
using AWSSDK;
using Amazon;
public class Manager : MonoBehaviour

{
    // public GameObject timer;
    public static Manager instance;
    public VideoPlayer video, video2, recordedplayer;
    public TextMeshProUGUI timerText;
    string recordedpath = "";

    public BlazePoseSample BPS;
    // Start is called before the first frame update
    private void Awake()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        instance = this;
    }
    void Start()
    {/*
      video.url= Path.Combine(Application.streamingAssetsPath, "L Cross Front.mp4");
        video2.url = Path.Combine(Application.streamingAssetsPath, "L Cross Front.mp4");
        video.Play();
        video2.Play();
        */
        VideoClip clip = Resources.Load<VideoClip>("pexels-alena-darmel-7320034") as VideoClip;
        video.clip = clip;
        //video2.clip = clip;
        video.Play();
        //video2.Play();
    }
    public void enablePose(bool check)
    {
        this.GetComponent<BlazePoseSample>().enabled = check;
    }
    // Update is called once per frame
    void Update()
    {
        timerText.text = DateTime.UtcNow.ToString("HH:mm:ss");
    }
    public void setPoseNumber(int number)
    {
        SendSceneManager.POSENUMBER=number;
    }
    public void setFolderName(string name)
    {
        SendSceneManager.FolderName=name;
    }
    public void assignFrontVid(string name)
    {
        SendSceneManager.FrontVid = name;
    }
    public void assignLeftVid(string name)
    {
        SendSceneManager.LeftVid = name;
    }
    public void assignRighttVid(string name)
    {
        SendSceneManager.RightVid = name;
    }
    public void changeVideo(string filename)
    {
        video.Stop();
        //video2.Stop();
        /*video.url = Path.Combine(Application.streamingAssetsPath, filename);
        video2.url= Path.Combine(Application.streamingAssetsPath, filename);
        video.Play();
        video2.Play();*/
        VideoClip clip = Resources.Load<VideoClip>(filename.Replace(".mp4", "")) as VideoClip;
        //VideoClip clip2 = Resources.Load<VideoClip>(filename.Replace(".mp4", "")) as VideoClip;
        video.clip = clip;
        //video2.clip = clip2;
        video.Play();
        //video2.Play();
    }
    public void playvideo(string path)
    {
        recordedplayer.url = path;
        recordedplayer.Play();
    }
    public void back()
    {
        video.Stop();
        //video2.Stop();
        recordedplayer.Stop();
        recordedplayer.gameObject.SetActive(false);
        //WebCam.instance.enablerecordbutton();
   }
    public void stopvideo(VideoPlayer player)
    {
        player.enabled = false;
    }
    public void loadscene(string name)
    {
        SceneManager.LoadScene(name);
    }

}
