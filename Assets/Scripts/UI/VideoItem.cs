using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoItem : MonoBehaviour
{
    public int FighterID { get; private set; }
    public int VideoID { get; private set; }
    public string VideoName { get; private set; }
    public static System.Action<VideoItem> OnItemClicked;
    public TMPro.TMP_Text Title;
    public int score = 0;
    public void Intialize(int _fighterID,int _videoID,string _videoName, int itemScore)
    {
        FighterID = _fighterID;
        VideoID = _videoID;
        VideoName = _videoName;
        score = itemScore;
        //Title.text = VideoName;
    }

    public void OnClick_VideoItem()
    {
        OnItemClicked?.Invoke(this);
    }
}
