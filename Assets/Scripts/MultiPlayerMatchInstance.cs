using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
//using System.Collections;
//using System.Collections.Generic;
//using System.Management.Instrumentation;
using UnityEngine;

public class MultiPlayerMatchInstance : MonoBehaviour
{
    private LetsFightScreen screen;

    private const byte ActionCardDeployedEventCode = 1;
    private const byte StartMatchTimerEventCode = 2;
    private const byte StopMatchTimerEventCode = 3;

    private void Awake()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static MultiPlayerMatchInstance CreateMatchInstance(LetsFightScreen matchScreen)
    {
        GameObject Match = new GameObject("MatchInstance");
        var matchInstance  = Match.AddComponent<MultiPlayerMatchInstance>();
        matchInstance.screen = matchScreen;
        return matchInstance;
    }
    public void RaiseActionCardEvent(int deployedActionCardID)
    {
        object[] content = new object[] { deployedActionCardID, UIController.Instance._myprofile.id };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ActionCardDeployedEventCode, content, raiseEventOptions, SendOptions.SendReliable);

    }

    public void RaiseAStartMatchEvent()
    {
        object[] content = new object[] {};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(StartMatchTimerEventCode, content, raiseEventOptions, SendOptions.SendReliable);

    }
    private void OnEvent(EventData _event)
    {

        byte eventCode = _event.Code;
        if (eventCode == ActionCardDeployedEventCode)
        {
            object[] data = (object[])_event.CustomData;
            int videoId = (int)data[0];
            int senderId = (int)data[1];
            if (senderId == UIController.Instance._myprofile.id)
            {
                screen.selfVideoPlayer.url = VideosContainer.Instance.opponentVidoData[0].VideoData.Find(item => item.actionCard.fighter_video_id == videoId).localPath;
                screen.selfVideoPlayer.Play();
            }
            else
            {
                screen.opponentVideoPlayer.url = VideosContainer.Instance.opponentVidoData[1].VideoData.Find(item => item.actionCard.fighter_video_id == videoId).localPath;
                screen.opponentVideoPlayer.Play();
            }

        }
        else if (eventCode == StartMatchTimerEventCode)
        {
            //start timer 
        }
        else if (eventCode == StopMatchTimerEventCode)
        {

        }
    }
}
