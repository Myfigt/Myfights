using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiPlayerMatchInstance : MonoBehaviour
{
    private LetsFightScreen screen;

    private const byte ActionCardDeployedEventCode = 1;
    private const byte StartMatchTimerEventCode = 2;
    private const byte StopMatchTimerEventCode = 3;

    private float timer = 0;
    private float timeout = 240;//4 minutes
    bool isMatchStart = false;

    public int PlayerScore { get; private set; } = 0; 

    public int OppnentScore { get; private set; } = 0;


    void Update()
    {
        if (isMatchStart)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = timeout;
                RaiseEndMatchEvent();
            }
            else
                screen.UdpateTimerText((int)timer);
        }
       
    }
    private void Awake()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    private void OnDestroy()
    {
       
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void StartMatchTimer()
    {
        timer = timeout;
        isMatchStart = true;
        screen.UpdateScoreText(PlayerScore, OppnentScore);
    }
    void StopMatchTimer()
    {
        timer = 0;
        isMatchStart = false;
    }
    public static MultiPlayerMatchInstance CreateMatchInstance(LetsFightScreen matchScreen)
    {
        GameObject Match = new GameObject("MatchInstance");
        var matchInstance  = Match.AddComponent<MultiPlayerMatchInstance>();
        matchInstance.screen = matchScreen;
        return matchInstance;
    }
    public void RaiseActionCardEvent(int deployedActionCardID,int actionCardScore)
    {
            object[] content = new object[] { deployedActionCardID, UIController.Instance._myprofile.id,actionCardScore };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            PhotonNetwork.RaiseEvent(ActionCardDeployedEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
 
    public void RaiseStartMatchEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            object[] content = new object[] { };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            PhotonNetwork.RaiseEvent(StartMatchTimerEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        }
    }
    public void RaiseEndMatchEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            object[] content = new object[] { };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            PhotonNetwork.RaiseEvent(StopMatchTimerEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        }
    }
    private void OnEvent(EventData _event)
    {

        byte eventCode = _event.Code;
        if (eventCode == ActionCardDeployedEventCode)
        {
            if (!isMatchStart)
                return;
            object[] data = (object[])_event.CustomData;
            int videoId = (int)data[0];
            int senderId = (int)data[1];
            int actionCardScore = (int)data[2];
            if (senderId == UIController.Instance._myprofile.id)
            {
                screen.selfVideoPlayer.url = VideosContainer.Instance.opponentVidoData[0].VideoData.Find(item => item.actionCard.fighter_video_id == videoId).localPath;
                screen.selfVideoPlayer.Play();
                PlayerScore += actionCardScore;
            }
            else
            {
                screen.opponentVideoPlayer.url = VideosContainer.Instance.opponentVidoData[1].VideoData.Find(item => item.actionCard.fighter_video_id == videoId).localPath;
                screen.opponentVideoPlayer.Play();
                OppnentScore += actionCardScore;
            }
            screen.UpdateScoreText(PlayerScore, OppnentScore);
        }
        else if (eventCode == StartMatchTimerEventCode)
        {
            //start timer 
            StartMatchTimer();
        }
        else if (eventCode == StopMatchTimerEventCode)
        {
            StopMatchTimer();
            screen.ShowEndMatchScreen();
        }
    }
}
