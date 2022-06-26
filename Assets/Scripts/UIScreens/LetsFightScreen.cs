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
    public Transform strategiesTitleItemParent;
    public Transform strategiesVideoItemParent;
    public StrategiesTitleItem strategiesTitleItem;
    public StrategiesVideoItem strategiesVideoItem;
    public GameObject strategiesItemLayout;
    List<Fighter> currentFighters;
    FightStrategy selfStrategy;
    FightStrategy opponentStrategy;
    List<GameObject> layouts = new List<GameObject>();

    private void OnEnable()
    {
        VideoItem.OnItemClicked += Handle_OnItemClicked;
        StrategiesTitleItem.OnClicked += Handle_OnStrategyItemClicked;
        StrategiesVideoItem.OnVideoClicked += Handle_OnVideoClicked;
    }

    private void OnDisable()
    {
        VideoItem.OnItemClicked -= Handle_OnItemClicked;
        StrategiesTitleItem.OnClicked -= Handle_OnStrategyItemClicked;
        StrategiesVideoItem.OnVideoClicked -= Handle_OnVideoClicked;
    }

    private void Handle_OnVideoClicked(StrategiesVideoItem obj)
    {
        VideosContainer.Instance.PlayVideo(obj.fighterID, obj.combination.id, videoPlayer, null);
    }

    private void Handle_OnStrategyItemClicked(int index)
    {
        layouts.ForEach(item => item.SetActive(false));
        layouts[index].SetActive(true);
    }

    private void Handle_OnItemClicked(VideoItem obj)
    {
        VideosContainer.Instance.PlayVideo(obj.FighterID, obj.VideoID, videoPlayer,null);
    }

    public void Initialize(List<Fighter> _allFighters)
    {
        LoadingText.SetActive(true);

        videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 1);
        VideoItemRawImage.texture = videoPlayer.targetTexture;

        currentFighters = _allFighters;
        VideosContainer.Instance.LoadAllFighterVideos(_allFighters, Handle_VideosLoaded);
    }

    public void Initialize(FightStrategy _strategy , FightStrategy _opponentStrategy)
    {
        LoadingText.SetActive(true);

        videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 1);
        VideoItemRawImage.texture = videoPlayer.targetTexture;
        selfStrategy = _strategy;
        opponentStrategy = _opponentStrategy;
        VideosContainer.Instance.LoadAllFighterVideos(new List<FightStrategy>(){
            _strategy,
            _opponentStrategy
        }, Handle_VideosLoaded);
    }

    private void Handle_VideosLoaded()
    {
        layouts.Clear();
        List<ActionCard> cards = VideosContainer.Instance.GetActionCards(selfStrategy.id);
        int counter = 0;
        for(int i = 0; i < selfStrategy._Combinations.Length; i+=3)
        {
            StrategiesTitleItem title = GameObject.Instantiate(strategiesTitleItem, strategiesTitleItemParent);
            title.Initialize("Strategy " + counter, counter);
            GameObject itemsLayout = GameObject.Instantiate(strategiesItemLayout, strategiesVideoItemParent);
            itemsLayout.SetActive(false);
            layouts.Add(itemsLayout);
            for (int j = 0; j < 3; j++)
            {
                StrategiesVideoItem videoItem = GameObject.Instantiate(strategiesVideoItem, itemsLayout.transform);
                videoItem.Initialize(null, selfStrategy.id, selfStrategy._Combinations[i]);
            }
            counter++;
        }
        Handle_OnStrategyItemClicked(0);
        LoadingText.SetActive(false);
    }
}
