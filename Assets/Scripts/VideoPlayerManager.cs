using Assets.Scripts;
using System;
//using System.Collections;
//using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

public class VideoPlayerManager : UIScreen
{
    [SerializeField] GameObject mimicButton;
    [SerializeField] GameObject cardDetailsPanel;
    [SerializeField] public VideoPlayer _player;
    [SerializeField] GameObject loading;

    [SerializeField] TMP_Text textBox_Head;
    [SerializeField] TMP_Text textBox_LShoulder;
    [SerializeField] TMP_Text textBox_RShoulder; 
    [SerializeField] TMP_Text textBox_LElbow;
    [SerializeField] TMP_Text textBox_RElbow;
    [SerializeField] TMP_Text textBox_LWrist;
    [SerializeField] TMP_Text textBox_RWrist;
    [SerializeField] TMP_Text textBox_LHip;
    [SerializeField] TMP_Text textBox_RHip;
    [SerializeField] TMP_Text textBox_LKnee;
    [SerializeField] TMP_Text textBox_RKnee;
    [SerializeField] TMP_Text textBox_LAnkle;
    [SerializeField] TMP_Text textBox_RAnkle;



    private void OnEnable()
    {
        _player.prepareCompleted += OnVideoPrepared;
        _player.errorReceived += OnVideoError;
    }
    private void OnDisable()
    {
        _player.prepareCompleted -= OnVideoPrepared;
        _player.errorReceived -= OnVideoError;
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.Log("message : " + message);
    }

    public void Initialize(int fighterid,int videoid)
    {

        loading.SetActive(false);
        mimicButton.SetActive(true);
        cardDetailsPanel.SetActive(false);
        VideosContainer.Instance.PlayVideo(fighterid, videoid, _player, null);
        this.SetGobackScreen(UIController.Screens.Figher_VideoSelection);
    }

    public void Initialize(string _path)
    {
        this.SetGobackScreen(UIController.Screens.LibraryScreen);
        mimicButton.SetActive(false);
        cardDetailsPanel.SetActive(true);
        loading.SetActive(true);
        _player.url = _path;
        _player.Prepare();
       
    }
    public void Initialize(ActionCard _card)
    {
        this.SetGobackScreen(UIController.Screens.LibraryScreen);
        loading.SetActive(true);
        mimicButton.SetActive(false);

        textBox_Head.text = _card.comparison_results.Head.ToString();
        textBox_LAnkle.text = _card.comparison_results.LAnkle.ToString();
        textBox_LElbow.text = _card.comparison_results.LElbow.ToString();
        textBox_LHip.text = _card.comparison_results.LHip.ToString();
        textBox_LKnee.text = _card.comparison_results.LKnee.ToString();
        textBox_LShoulder.text = _card.comparison_results.LShoulder.ToString();
        textBox_LWrist.text = _card.comparison_results.LWrist.ToString();
        textBox_RAnkle.text = _card.comparison_results.RAnkle.ToString();
        textBox_RElbow.text = _card.comparison_results.RElbow.ToString();
        textBox_RHip.text = _card.comparison_results.RHip.ToString();
        textBox_RKnee.text = _card.comparison_results.RKnee.ToString();
        textBox_RShoulder.text = _card.comparison_results.RShoulder.ToString();
        textBox_RWrist.text = _card.comparison_results.RWrist.ToString();

        cardDetailsPanel.SetActive(true);
        Uri u = new Uri(_card.Path);
        _player.url = u.AbsoluteUri;
        _player.Prepare();

    }

    public override void Goback()
    {
       
        UIController.Instance.SetupScreen(GetGobackScreen());
    }
    void OnVideoPrepared(VideoPlayer vPlayer)
    {
        loading.SetActive(false);
        _player.Play();
    }
}
