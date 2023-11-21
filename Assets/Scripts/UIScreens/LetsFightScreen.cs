using Assets.Scripts;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LetsFightScreen : UIScreen
{
    public GameObject LoadingText;
    public Transform VideoItemParent;
    public VideoItem VideoItemTemplete;
    public VideoPlayer selfVideoPlayer;
    public RawImage selfVideoItemRawImage;
    public VideoPlayer opponentVideoPlayer;
    public RawImage opponentVideoItemRawImage;
    public Transform strategiesTitleItemParent;
    public Transform combinationItemParent;
    public Transform strategiesVideoItemParent;
    public StrategiesTitleItem strategiesTitleItem;
    public CombinationItem combinationItem;
    public StrategiesVideoItem strategiesVideoItem;
    public GameObject strategiesItemLayout;
    List<Fighter> currentFighters;
    FightStrategy selfStrategy;
    FightStrategy opponentStrategy;
    List<GameObject> layouts = new List<GameObject>();

    private const byte ActionCardDeployedEventCode = 1;

    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text oppoName;
    [SerializeField] Transform actionCardListContent;
    [SerializeField] GameObject actionCardTemplete;

    private MultiPlayerMatchInstance match;

    private void OnEnable()
    {
        VideoItem.OnItemClicked += Handle_OnItemClicked;
        StrategiesTitleItem.OnClicked += Handle_OnStrategyItemClicked;
        CombinationItem.OnCombinationClicked += Handle_OnCombinationClicked;
        StrategiesVideoItem.OnVideoClicked += Handle_OnVideoClicked;
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        VideoItem.OnItemClicked -= Handle_OnItemClicked;
        StrategiesTitleItem.OnClicked -= Handle_OnStrategyItemClicked;
        CombinationItem.OnCombinationClicked -= Handle_OnCombinationClicked;
        StrategiesVideoItem.OnVideoClicked -= Handle_OnVideoClicked;
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        selfVideoPlayer.targetTexture.Release();
        opponentVideoPlayer.targetTexture.Release();
    }

    private void OnEvent(EventData _event)
    {
        
        byte eventCode = _event.Code;
        if (eventCode == ActionCardDeployedEventCode)
        {
            object[] data = (object[])_event.CustomData;
            int videoId = (int)data[0];
            int senderId = (int)data[1];
            if (senderId  == UIController.Instance._myprofile.id)
            {
                selfVideoPlayer.url = VideosContainer.Instance.opponentVidoData[0].VideoData.Find(item => item.actionCard.fighter_video_id == videoId).localPath;
                selfVideoPlayer.Play();
            }
            else
            {
                opponentVideoPlayer.url = VideosContainer.Instance.opponentVidoData[1].VideoData.Find(item => item.actionCard.fighter_video_id == videoId).localPath;
                opponentVideoPlayer.Play();
            }
           
        }
    }

    private void Handle_OnCombinationClicked(CombinationItem obj)
    {
        foreach (Transform child in strategiesVideoItemParent)
        {
            Destroy(child.gameObject);
        }

        foreach (FightCombination fightCombination in obj.combination)
        {
            StrategiesVideoItem videoItem = GameObject.Instantiate(strategiesVideoItem, strategiesVideoItemParent);
            videoItem.Initialize(null, selfStrategy.id, fightCombination);
        }
        strategiesVideoItemParent.gameObject.SetActive(true);
    }

    private void Handle_OnVideoClicked(StrategiesVideoItem obj)
    {
        //VideosContainer.Instance.PlayVideo(obj.fighterID, obj.combination.id, selfVideoPlayer, null);

    }

    private void Handle_OnStrategyItemClicked(int index)
    {
        //layouts.ForEach(item => item.SetActive(false));
        //layouts[index].SetActive(true);
    }

    private void Handle_OnItemClicked(VideoItem obj)
    {
        selfVideoPlayer.gameObject.SetActive(true);
        match.RaiseActionCardEvent(obj.VideoID);
    }

    public void Initialize(List<Fighter> _allFighters)
    {
        LoadingText.SetActive(true);

        selfVideoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 1);
        selfVideoItemRawImage.texture = selfVideoPlayer.targetTexture;

        opponentVideoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 1);
        opponentVideoItemRawImage.texture = opponentVideoPlayer.targetTexture;

        currentFighters = _allFighters;
        VideosContainer.Instance.LoadAllFighterVideos(_allFighters, Handle_VideosLoaded);
    }

    public void Initialize(FightStrategy _strategy, FightStrategy _opponentStrategy, NetworkConnectionManager _handle = null)
    {
        LoadingText.SetActive(true);
        playerName.text = _strategy.player_name;
        oppoName.text = _opponentStrategy.player_name;
        match = MultiPlayerMatchInstance.CreateMatchInstance(this);
        selfStrategy = _strategy;
        opponentStrategy = _opponentStrategy;
        SetupActionCardUI();
        VideosContainer.Instance.LoadAllFighterVideos(new List<FightStrategy>(){
            _strategy,
            _opponentStrategy
        }, Handle_VideosLoaded);
        
    }

    public void SetupActionCardUI()
    {
        if (actionCardListContent.childCount > 1)
        {
            for (int i = 0; i < actionCardListContent.childCount; i++)
            {
                if (actionCardListContent.GetChild(i).gameObject.activeSelf)
                {
                    DestroyImmediate(actionCardListContent.GetChild(i));
                    i--;
                }
            }
        }

    }
    public void Initialize(List<ActionCard> _myCards, List<ActionCard> _opponentCards)
    {
        LoadingText.SetActive(true);
        strategiesVideoItemParent.gameObject.SetActive(false);

        selfVideoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 1);
        selfVideoItemRawImage.texture = selfVideoPlayer.targetTexture;

        opponentVideoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 1);
        opponentVideoItemRawImage.texture = opponentVideoPlayer.targetTexture;

        
        //selfStrategy = _strategy;
        // opponentStrategy = _opponentStrategy;
        //VideosContainer.Instance.LoadAllFighterVideos(new List<FightStrategy>(){
        //    _strategy,
        //    _opponentStrategy
        //}, Handle_VideosLoaded);
    }
    private void Handle_VideosLoaded()
    {
        Debug.Log("Handle_fight combination loaded" + VideosContainer.Instance.opponentVidoData.Count);
        layouts.Clear();
        List<ActionCard> cards = VideosContainer.Instance.GetActionCards(selfStrategy.id);
        int counter = 0;

        foreach (var item in VideosContainer.Instance.opponentVidoData[0].VideoData)
        {
            if (item != null)
            {
                var go = GameObject.Instantiate(actionCardTemplete, actionCardListContent);
                go.transform.GetChild(0).GetComponent<TMP_Text>().text = item.actionCard.id.ToString();
                if (go.TryGetComponent<VideoItem>(out var videoItem))
                {
                    videoItem.Intialize(item.actionCard.player_id, item.actionCard.fighter_video_id, item.actionCard.FileName);
                }
                go.SetActive(true);
            }

        }
        LoadingText.SetActive(false);
    }

}
