using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MatchMakingScreen : UIScreen
{
    [SerializeField]
    GameObject friendTemplate;
    [SerializeField]
    TMP_Text roomName;
    List<Friends> friendsList;
    [SerializeField]
    GameObject friendsPanel;
    // Start is
    // called before the first frame update
    private void OnEnable()
    {
        WebServicesManager.GetFriendListComplete += WebServicesManager_GetFriendListComplete;
        WebServicesManager.GetFriendListFailed += WebServicesManager_GetFriendListFailed;
        
        //WebServicesManager.Instance.GetFriendList();
    }
    private void OnDisable()
    {
        WebServicesManager.GetFriendListComplete -= WebServicesManager_GetFriendListComplete;
        WebServicesManager.GetFriendListFailed -= WebServicesManager_GetFriendListFailed;
    }
    private void WebServicesManager_GetFriendListFailed(string responce)
    {
        throw new NotImplementedException();
    }

    private void WebServicesManager_GetFriendListComplete(string responce)
    {
        friendsList = new List<Friends>();
        friendsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Friends>>(responce);
        foreach (Friends request in friendsList)
        {
            GameObject searchedUserObject = GameObject.Instantiate(friendTemplate, friendTemplate.transform.parent);
            searchedUserObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = request.Friends_name.ToString();
            searchedUserObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = request.Friend_id.ToString();
            searchedUserObject.SetActive(true);
        }
    }

    public override void Initialize(params object[] _params)// 1st variable room name 
    {
        if (_params.Count() != 0)
        {
            
            bool isPlayingRandom = bool.Parse( _params[1] as string);
            if (isPlayingRandom)
            {
                roomName.text = "Waiting for Player to join";
                friendsPanel.SetActive(false);
            }
            else
            {
                roomName.text = _params[0] as string;
                ClearUIList();
                if (UIController.Instance._myprofile._allFriends != null)
                {
                    foreach (Friends request in UIController.Instance._myprofile._allFriends)
                    {
                        GameObject searchedUserObject = GameObject.Instantiate(friendTemplate, friendTemplate.transform.parent);
                        searchedUserObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = request.Friends_name.ToString();
                        searchedUserObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = request.Friend_id.ToString();
                        searchedUserObject.SetActive(true);
                    }
                    friendsPanel.SetActive(true);
                }
                else
                {
                    WebServicesManager.Instance.GetFriendList();
                }
            }
            
        }
       

    }
    public override void Goback()
    {
        UIController.Instance.DisconnectConnectedRoom();
        base.Goback();
    }
    public void ChallangeAFriend(Transform index)
    {
        if (UIController.Instance._myprofile._allFriends.Count != 0)
        {
            GameChallangeMessage payLoad = new GameChallangeMessage()
            {
                data = string.Empty,// Newtonsoft.Json.JsonConvert.SerializeObject(UIController.Instance._myprofile),
                user_id = UIController.Instance._myprofile.id,
                room_id = roomName.text,
                friend_id = UIController.Instance._myprofile._allFriends[index.GetSiblingIndex()-1].Friend_id
            };
            UIController.Instance.WSConnector.SendRequest(Newtonsoft.Json.JsonConvert.SerializeObject(payLoad), WS_ActionType.join_room, WebServicesManager.Instance.authorization);
        }
    }
    void ClearUIList()
    {
        for (int i = 0; i < friendTemplate.transform.parent.childCount; i++)
        {
            if (friendTemplate.transform.parent.GetChild(i).gameObject.activeSelf)
            {
                DestroyImmediate(friendTemplate.transform.parent.GetChild(i).gameObject);
                i--;
            }
        }
        
    }
}

public class GameChallangeMessage : WSSocketMessageBase
{
    public String room_id;
    public int user_id;
    public int friend_id;
    public String data;
}
public class WSSocketMessageBase
{
    
}

