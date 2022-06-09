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
        public VideoPlayer player;
        public string localPath;
        public Action OnLoaded;
        public Action OnPlayComplete;

        public VideoData(ActionCard _actionCard, VideoPlayer _playerPrefab,Transform _playerParent)
        {
            actionCard = _actionCard;
            player = GameObject.Instantiate(_playerPrefab, _playerParent);
            player.gameObject.SetActive(false);
            status = VideoStatus.idle;
        }
        public void LoadVideo(int fighterId,string fighterName)
        {
            if (status != VideoStatus.idle)
                return;

            status = VideoStatus.loading;

            string FileName = Path.GetFileName(actionCard.Path);

            localPath = Path.Combine(Application.persistentDataPath, fighterId.ToString() + "-" + fighterName);
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            localPath = Path.Combine(localPath, FileName);

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
                        status = VideoStatus.Failed;
                    }
                    OnLoaded?.Invoke();
                });
            }

            player.seekCompleted += (tempPlayer) =>
            {
                player.gameObject.SetActive(false);
                OnPlayComplete?.Invoke();
            };

            //player.prepareCompleted += (tempPlayer) =>
            //{
            //    status = VideoStatus.loaded;
            //    OnLoaded?.Invoke();
            //};
            //player.Prepare();
        }
        public void Play(Action OnPlayCompleted)
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

        public void AddVideos(List<ActionCard> _actionCards, VideoPlayer _playerPrefab, Transform _playerParent, Action VideosLoaded)
        {
            foreach(ActionCard action in _actionCards)
            {
                if (VideoData.Find(item => item.actionCard.Path == action.Path) == null)
                {
                    VideoData newData = new VideoData(action, _playerPrefab, _playerParent);
                    VideoData.Add(newData);
                }
            }
            foreach(VideoData video in VideoData)
            {
                video.OnLoaded = () =>
                {
                    if (VideoData.Find(item => item.status != VideoStatus.loaded) == null)
                    {
                        VideosLoaded?.Invoke();
                    }
                };
                video.LoadVideo(fighter.id,fighter.Name);
            }
        }
    }

    public Transform videoParent;
    public VideoPlayer videoPlayerTemplete;
    List<FighterData> fightersData = new List<FighterData>();

    public void PlayVideo(int fighterID,int videoID,Action OnPlayCompleted)
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
            OnPlayCompleted();
            return;
        }
        VideoData vidData = fightersData[fighterIndex].VideoData[videoIndex];
        if (vidData.status != VideoStatus.loaded)
        {
            Debug.LogError($"Can't Play Because Video status is {vidData.status}");
            OnPlayCompleted();
            return;
        }
        vidData.Play(OnPlayCompleted);
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
                        newFighterData.AddVideos(fightersVideos, videoPlayerTemplete, videoParent, () => {
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
}
