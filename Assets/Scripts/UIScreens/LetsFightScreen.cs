using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LetsFightScreen : UIScreen
{
    public GameObject LoadingText;
    public Transform VideoItemParent;
    public VideoItem VideoItemTemplete;
    public VideoPlayer videoPlayer;
    public RawImage VideoItemRawImage;
    List<Fighter> currentFighters;

    private void OnEnable()
    {
        VideoItem.OnItemClicked += Handle_OnItemClicked;
    }

    private void OnDisable()
    {
        VideoItem.OnItemClicked -= Handle_OnItemClicked;
    }

    private void Handle_OnItemClicked(VideoItem obj)
    {
        VideosContainer.Instance.PlayVideo(obj.FighterID, obj.VideoID, videoPlayer,null);
    }

    public void Initialize(List<Fighter> _allFighters)
    {
        LoadingText.SetActive(true);
        //foreach(Transform child in VideoItemParent)
        //{
        //    if (child.GetInstanceID() != VideoItemTemplete.GetInstanceID())
        //        Destroy(child.gameObject);
        //}

        videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 1);
        VideoItemRawImage.texture = videoPlayer.targetTexture;

        currentFighters = _allFighters;
        VideosContainer.Instance.LoadAllFighterVideos(_allFighters, Handle_VideosLoaded);
    }

    public void Initialize(FightStrategy _strategy , FightStrategy _opponentStrategy) { }


    private void Handle_VideosLoaded()
    {
        List<ActionCard> cards = VideosContainer.Instance.GetActionCards(currentFighters[0].id);
        foreach(ActionCard card in cards)
        {
            VideoItem tempItem = GameObject.Instantiate(VideoItemTemplete, VideoItemParent);
            tempItem.Intialize(currentFighters[0].id, card.id, card.FileName);
            tempItem.gameObject.SetActive(true);
        }
        LoadingText.SetActive(false);
    }
}
