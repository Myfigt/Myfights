using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Video;

public class VideosContainer : MonoBehaviour
{
    private static VideosContainer instance = null;
    public static VideosContainer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<VideosContainer>();
            }
            return instance;
        }
    }

    public enum VideoStatus
    {
        idle = 0,
        loading = 1,
        loaded,
        Failed
    }
    public class VideoData
    {
        public ActionCard actionCard;
        public VideoStatus status { get; private set; }
        public string localPath;
        public Action OnLoaded;
        public Action OnPlayComplete;

        public VideoData(ActionCard _actionCard)
        {
            actionCard = _actionCard;
            status = VideoStatus.idle;
        }
        public void LoadVideo(int fighterId,string fighterName)
        {
            if (status != VideoStatus.idle)
            {
                OnLoaded?.Invoke();
                return;
            }
            status = VideoStatus.loading;

            string FileName = Path.GetFileName(actionCard.Path);

            localPath = Path.Combine(Application.persistentDataPath, fighterId.ToString() + "-" + fighterName);
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            localPath = Path.Combine(localPath, FileName);

            localPath = Path.ChangeExtension(localPath,"mp4");

            if (File.Exists(localPath))
            {
                status = VideoStatus.loaded;
                OnLoaded?.Invoke();
            }
            else
            {
                FileUtility.Instance.DownloadFile(actionCard.Path, (success, handler) =>
                {
                    if (success)
                    {
                        FileUtility.Instance.WriteBytes(localPath, handler.data);
                        status = VideoStatus.loaded;
                    }
                    else
                    {
                        if (!File.Exists(localPath))
                        File.Copy(Path.Combine(Application.dataPath, "SampleVideo.mp4"), localPath);
                        localPath = $"file:///{localPath}";
                        status = VideoStatus.Failed;
                    }
                    OnLoaded?.Invoke();
                });
            }
        }
        public void Play(Action OnPlayCompleted,VideoPlayer player)
        {
            player.gameObject.SetActive(true);
            OnPlayComplete = OnPlayCompleted;
            player.url = localPath;
            player.Play();
        }
    }
    public class FighterData
    {
        public Fighter fighter;
        public List<VideoData> VideoData = new List<VideoData>();

        public FighterData(Fighter _fighter)
        {
            fighter = _fighter;
            VideoData = new List<VideoData>();
        }

        public void AddVideos(List<ActionCard> _actionCards, Action VideosLoaded , bool allowDuplicate = false)
        {
            int counter = 0;
            foreach(ActionCard action in _actionCards)
            {
                if (VideoData.Find(item => item.actionCard.id == action.id) == null || allowDuplicate)
                {
                    VideoData newData = new VideoData(action);
                    VideoData.Add(newData);
                }
            }
            foreach(VideoData video in VideoData)
            {
                video.OnLoaded = () =>
                {
                    counter++;
                    if (counter >= VideoData.Count)
                        VideosLoaded?.Invoke();
                };
                video.LoadVideo(fighter.id,fighter.Name);
            }
        }
    }

    List<FighterData> fightersData = new List<FighterData>();

    public List<FighterData> opponentVidoData = new List<FighterData>();

    public FighterData PlayerActionCards;

    public void PlayVideo(int fighterID,int videoID,VideoPlayer player,Action OnPlayCompleted)
    {
        int fighterIndex = fightersData.FindIndex(item => item.fighter.id == fighterID);
        if (fighterIndex == -1)
        {
            Debug.LogError("Fighter Not Found");
            OnPlayCompleted();
            return;
        }

        int videoIndex = fightersData[fighterIndex].VideoData.FindIndex(item => item.actionCard.id == videoID);
        if (videoIndex == -1)
        {
            Debug.LogError("Video Not Found");
            OnPlayCompleted?.Invoke();
            return;
        }
        VideoData vidData = fightersData[fighterIndex].VideoData[videoIndex];
        //if (vidData.status != VideoStatus.loaded)
        //{
        //    Debug.LogError($"Can't Play Because Video status is {vidData.status}");
        //    OnPlayCompleted?.Invoke();
        //    return;
        //}
        vidData.Play(OnPlayCompleted,player);
    }

    public List<ActionCard> GetActionCards(int fighterID)
    {
        int fighterIndex = fightersData.FindIndex(item => item.fighter.id == fighterID);
        if (fighterIndex == -1)
        {
            Debug.LogError("Fighter Not Found");
            return null;
        }
        return fightersData[fighterIndex].VideoData.Select(item => item.actionCard).ToList();
    }   

    public void LoadFighterVideos(Fighter fighter, Action OnLoaded)
    {
        LoadAllFighterVideos(new List<Fighter> { fighter }, OnLoaded);
    }
    public void LoadAllFighterVideos(List<Fighter> fighters, Action OnLoaded)
    {
        int counter = 0;
        foreach(Fighter fighter in fighters)
        {
            if (fightersData.Find(item => item.fighter.id == fighter.id) == null)
            {
                FighterData newFighterData = new FighterData(fighter);
                WebServicesManager.Instance.FetchVideos(fighter.id, Belts.blackbelt.ToString(), (success, response) =>
                {
                    if (success)
                    {
                        List<ActionCard> fightersVideos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ActionCard>>(response);
                        newFighterData.AddVideos(fightersVideos, () => {
                            counter++;
                            if (counter == fighters.Count)
                                OnLoaded?.Invoke();
                        });
                    }
                    else
                    {
                        Debug.LogError($"Unable To Fetch Videos of Fighter : id : {fighter.id} : Name : {fighter.Name} ");
                        counter++;
                        if (counter == fighters.Count)
                            OnLoaded?.Invoke();
                    }
                });
                fightersData.Add(newFighterData);
            }
            else
            {
                counter++;
                if (counter == fighters.Count)
                    OnLoaded?.Invoke();
            }
        }
    }

    public void LoadAllFighterVideos(List<FightCombo> fightStrategies, Action OnLoaded)
    {
        opponentVidoData = new List<FighterData>();
        int counter = 0;
        foreach (FightCombo strategy in fightStrategies)
        {
            FighterData newFighterData = new FighterData(new Fighter()
            {
                id = strategy.id,
                created_at = "",
                Name = strategy.player_name,
                Photo = "",
                Status = -1
            });
            List<ActionCard> cards = new List<ActionCard>();
            foreach (FightStrategy fightCombination in strategy.strategies)
            {
                if (fightCombination != null)
                {
                    for (int i = 0; i < fightCombination.actionCards.Length; i++)
                    {
                        if (fightCombination.actionCards[i]!= null)
                        {
                            ActionCard newCard = new ActionCard();
                            newCard.FileName = Path.GetFileName(fightCombination.actionCards[0].Path);//::TODO
                            newCard.Path = fightCombination.actionCards[0].Path;//::TODO
                            newCard.id = fightCombination.actionCards[0].id;
                            cards.Add(newCard);
                        }
                    }
                    
                }
              
            }
            newFighterData.AddVideos(cards, () => {
                counter++;
                if (counter >= fightStrategies.Count)
                    OnLoaded?.Invoke();
            }, true);
            opponentVidoData.Add(newFighterData);
        }
    }

    public void LoadAllAllPlayerActionCards(List<ActionCard> cards, Action OnLoaded)
    {
        if (PlayerActionCards == null)
        {
            PlayerActionCards = new FighterData(new Fighter()
            {
                id = UIController.Instance._myprofile.id,
                Name = UIController.Instance._myprofile.name,
                created_at = "",
                Photo = "",
                Status = -1
            });
        }

        PlayerActionCards.AddVideos(cards, () =>
        {
                OnLoaded?.Invoke();
        });
    }
}
