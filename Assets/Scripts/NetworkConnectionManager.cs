using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;

namespace Assets.Scripts
{
    public class NetworkConnectionManager : MonoBehaviourPunCallbacks
    {
        [Header("Match Settings")]
        [SerializeField]
        byte _MaxPlayers = 2;

        public Button BtnConnectRoom;

        protected bool TriesToConnectToMaster;
        protected bool TriesToConnectToRoom;

        private void Awake()
        {
            PhotonNetwork.OfflineMode = false;
        }
        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(this);
            TriesToConnectToMaster = false;
            TriesToConnectToRoom = false;
            OnClickConnectToMaster();
        }


        // Update is called once per frame
        void Update()
        {

            if (BtnConnectRoom != null)
                BtnConnectRoom.gameObject.GetComponent<Button>().interactable = PhotonNetwork.IsConnected && !TriesToConnectToMaster && !TriesToConnectToRoom;

        }

        public void OnClickConnectToMaster()
        {
            TriesToConnectToMaster = true;

            //Settings (all optional and only for tutorial purpose)
            PhotonNetwork.OfflineMode = false;           //true would "fake" an online connection
            PhotonNetwork.NickName = UIController.Instance._myprofile.name;       //to set a player name
            PhotonNetwork.AutomaticallySyncScene = true; //to call PhotonNetwork.LoadLevel()
            PhotonNetwork.GameVersion = "v1";            //only people with the same game version can play together

            //PhotonNetwork.ConnectToMaster(ip,port,appid); //manual connection
            if (!PhotonNetwork.OfflineMode)
                PhotonNetwork.ConnectUsingSettings();           //automatic connection based on the config file in Photon/PhotonUnityNetworking/Resources/PhotonServerSettings.asset


        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            TriesToConnectToMaster = false;
            Debug.Log("Connected to Master!");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            TriesToConnectToMaster = false;
            TriesToConnectToRoom = false;
            Debug.Log(cause);
        }

        public void OnClickConnectToRoom()
        {
            if (!PhotonNetwork.IsConnected)
                return;
            if (UIController.Instance._myprofile._myStrategy != null)
            {
                return;
            }
            ExitGames.Client.Photon.Hashtable strategy = new ExitGames.Client.Photon.Hashtable();
            strategy.Add("_strategy", UIController.Instance._myprofile._myStrategy);
            PhotonNetwork.LocalPlayer.SetCustomProperties(strategy);
            TriesToConnectToRoom = true;
            //PhotonNetwork.CreateRoom("Peter's Game 1"); //Create a specific Room - Error: OnCreateRoomFailed
            //PhotonNetwork.JoinRoom("Peter's Game 1");   //Join a specific Room   - Error: OnJoinRoomFailed  
            PhotonNetwork.JoinRandomRoom();               //Join a random Room     - Error: OnJoinRandomRoomFailed  
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            //no room available
            //create a room (null as a name means "does not matter")
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = _MaxPlayers });
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log(message);
            base.OnCreateRoomFailed(returnCode, message);
            TriesToConnectToRoom = false;
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            TriesToConnectToRoom = false;
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == _MaxPlayers)
            {
                SetupMatch();
            }

            Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name + " Region: " + PhotonNetwork.CloudRegion);

            //if (PhotonNetwork.IsMasterClient && SceneManager.GetActiveScene().name != "Main")
            //    PhotonNetwork.LoadLevel("Main");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == _MaxPlayers)
            {
                SetupMatch();
            }
            base.OnPlayerEnteredRoom(newPlayer);
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
        }

        internal void JoinRandomRoom()
        {
            OnClickConnectToRoom();
        }

        public IEnumerator SetupMatch() {
            FightStrategy myStrategy = null, opponentStategy= null;
            foreach (var item in PhotonNetwork.CurrentRoom.Players)
            {
                ExitGames.Client.Photon.Hashtable customprops = (item.Value as Player).CustomProperties;

                foreach (var props in customprops)
                {
                    if (props.Key.ToString() == "_strategy")
                    {
                        if ((item.Value as Player).IsLocal)
                        {
                            myStrategy = (FightStrategy)props.Value;
                        }
                        else
                        {
                            opponentStategy = (FightStrategy)props.Value;
                        }

                    }
                }
            }
            yield return new WaitForSeconds(5);
            UIController.Instance._letsFightScreen.Initialize(myStrategy, opponentStategy);
        }
    }
}
