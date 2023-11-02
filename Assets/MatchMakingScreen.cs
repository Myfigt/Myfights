using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MatchMakingScreen : UIScreen
{
    [SerializeField]
    GameObject FriendTemplate;
    [SerializeField]
    TMP_Text roomName;
    List<Friends> FriendsList;
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
        FriendsList = new List<Friends>();
        FriendsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Friends>>(responce);
        foreach (Friends request in FriendsList)
        {
            GameObject searchedUserObject = GameObject.Instantiate(FriendTemplate, FriendTemplate.transform.parent);
            searchedUserObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = request.Friends_name.ToString();
            //searchedUserObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = request.Friends_is_online.ToString();
            searchedUserObject.SetActive(true);
        }
    }

    public override void Initialize(params object[] _params)// 1st variable room name 
    {
        if (_params.Count() != 0)
        {
            roomName.text = _params[0] as string;
            if (UIController.Instance._myprofile._allFriends != null)
            {
                foreach (Friends request in UIController.Instance._myprofile._allFriends)
                {
                    GameObject searchedUserObject = GameObject.Instantiate(FriendTemplate, FriendTemplate.transform.parent);
                    searchedUserObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = request.Friends_name.ToString();
                    searchedUserObject.SetActive(true);
                }
            }
            else
            {
                WebServicesManager.Instance.GetFriendList();
            }
        }
       

    }
    void Start()
    {
       
    }

    public void ChallangeAFriend(int index)
    {
        if (UIController.Instance._myprofile._allFriends.Count != 0)
        {
            GameChallangeMessage payLoad = new GameChallangeMessage()
            {
                data = Newtonsoft.Json.JsonConvert.SerializeObject(UIController.Instance._myprofile),
                user_id = UIController.Instance._myprofile.id,
                room_id = roomName.text,
                friend_id = UIController.Instance._myprofile._allFriends[index].Friend_id
            };
            UIController.Instance.WSConnector.SendRequest(Newtonsoft.Json.JsonConvert.SerializeObject(payLoad), WS_ActionType.join_room, WebServicesManager.Instance.authorization);
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

