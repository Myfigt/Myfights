using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    [SerializeField]
    public VideoPlayer _player;
    [SerializeField]
    GameObject loading;
    
    // Start is called before the first frame update
    void Start()
    {
        //_player.prepareCompleted += OnVideoPrepared;
        //_player.errorReceived += OnVideoError;
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.Log("message : " + message);
    }

    public void Initialize(int fighterid,int videoid)
    {
        loading.SetActive(false);
        VideosContainer.Instance.PlayVideo(fighterid, videoid, _player, null);
    }

    public void Initialize(string _path)
    {
        loading.SetActive(true);
        _player.url = _path;
        _player.Prepare();
    }
    void OnVideoPrepared(VideoPlayer vPlayer)
    {
        loading.SetActive(false);
        _player.Play();
    }
}
