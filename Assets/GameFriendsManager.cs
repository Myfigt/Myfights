using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Windows.Markup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UIController;

public class GameFriendsManager : UIScreen
{

    List<FriendRequest> FriendrequestList;
    List<Friends> FriendsList;
    [SerializeField]
    GameObject SearcheUserTemplate;
    [SerializeField]
    GameObject FriendRequestTemplate;
    [SerializeField]
    GameObject FriendTemplate;
    [SerializeField]
    TMP_Text ErrorMessageField;
    [SerializeField]
    GameObject[] Tabs;
    [SerializeField]
    string WSURL;

    // Start is called before the first frame update
    public void ToggleTabButtonClick(int tabIndex)
    {
        for (int i = 0; i < Tabs.Length; i++)
        {
            if (i == tabIndex)
                Tabs[i].SetActive(true);
            else
                Tabs[i].SetActive(false);
        }
    }
    public override void Initialize(params object[] _params)
    {
        base.Initialize(_params);
        
    }
    private void OnEnable()
    {
        WebServicesManager.FetchFriendRequestListComplete += WebServicesManager_FetchFriendRequestListComplete;
        WebServicesManager.FetchFriendRequestListFailed += WebServicesManager_FetchFriendRequestListFailed;
        WebServicesManager.GetFriendListComplete += WebServicesManager_GetFriendListComplete;
        WebServicesManager.GetFriendListFailed += WebServicesManager_GetFriendListFailed;
        WebServicesManager.SearchFriendComplete += WebServicesManager_SearchFriendComplete;
        WebServicesManager.SearchFriendFailed += WebServicesManager_SearchFriendFailed;
        WebServicesManager.SendFriendRequestComplete += WebServicesManager_SendFriendRequestComplete;
        WebServicesManager.SendFriendRequestFailed += WebServicesManager_SendFriendRequestFailed;
        ClearData(FriendRequestTemplate.transform.parent);
        ClearData(FriendTemplate.transform.parent);
        WebServicesManager.Instance.GetFriendList();
        WebServicesManager.Instance.FetchFriendRequestList();
       
    }
    private void OnDisable()
    {
        WebServicesManager.FetchFriendRequestListComplete -= WebServicesManager_FetchFriendRequestListComplete;
        WebServicesManager.FetchFriendRequestListFailed -= WebServicesManager_FetchFriendRequestListFailed;
        WebServicesManager.GetFriendListComplete -= WebServicesManager_GetFriendListComplete;
        WebServicesManager.GetFriendListFailed -= WebServicesManager_GetFriendListFailed;
        WebServicesManager.SearchFriendComplete -= WebServicesManager_SearchFriendComplete;
        WebServicesManager.SearchFriendFailed -= WebServicesManager_SearchFriendFailed;
        WebServicesManager.SendFriendRequestComplete -= WebServicesManager_SendFriendRequestComplete;
        WebServicesManager.SendFriendRequestFailed -= WebServicesManager_SendFriendRequestFailed;
        
    }
    #region Callback Events
    private void WebServicesManager_SendFriendRequestFailed(string responce)
    {
        throw new System.NotImplementedException();
    }

    private void WebServicesManager_SendFriendRequestComplete(string responce)
    {
        
    }

    private void WebServicesManager_SearchFriendFailed(string error)
    {
        ShowStatus("search friend failed  no result found");
    }

    private void WebServicesManager_SearchFriendComplete(string responce)
    {
    
        try
        {
            List<SearchedUsers> users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchedUsers>>(responce);
            if (users != null)
            {
                GameObject searchedUserObject = GameObject.Instantiate(SearcheUserTemplate, SearcheUserTemplate.transform.parent);
                searchedUserObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = users[0].Name;
                searchedUserObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = users[0].Email;
                searchedUserObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = users[0].id.ToString();
                searchedUserObject.SetActive(true);
            }
        }
        catch
        {
            ShowStatus("unable to parse user");
        }
    }

    private void WebServicesManager_GetFriendListFailed(string responce)
    {
     
    }

    private void WebServicesManager_GetFriendListComplete(string responce)
    {
        FriendsList = new List<Friends>();
        FriendsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Friends>>(responce);
        foreach (Friends request in FriendsList)
        {
            GameObject searchedUserObject = GameObject.Instantiate(FriendTemplate, FriendTemplate.transform.parent);
            searchedUserObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = request.Friend_id.ToString();
            searchedUserObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = request.Friends_email.ToString();
            searchedUserObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = request.Friends_is_online.ToString();
            searchedUserObject.SetActive(true);
        }
    }

    private void WebServicesManager_FetchFriendRequestListFailed(string responce)
    {
        
    }

    private void WebServicesManager_FetchFriendRequestListComplete(string responce)
    {
        FriendrequestList = new List<FriendRequest>();
        FriendrequestList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FriendRequest>>(responce);
        foreach (FriendRequest request in FriendrequestList)
        {
            GameObject searchedUserObject = GameObject.Instantiate(FriendRequestTemplate, FriendRequestTemplate.transform.parent);
            searchedUserObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = request.Friend_id.ToString();
            searchedUserObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = request.Friends_name.ToString();
            searchedUserObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = request.Friends_email.ToString();
            searchedUserObject.SetActive(true);
        }
    }
    #endregion
    void Start()
    {
        ToggleTabButtonClick(0);
    }

    public void OnSearchButtonClick(TMPro.TMP_InputField _searchIDText)
    {

        WebServicesManager.Instance.SearchFriend(_searchIDText.text);
        ClearData(SearcheUserTemplate.transform.parent);
    }
    public void OnAddFriendButtonClick(TMPro.TMP_Text _UserIDText)
    {
        FriendRequestMessage message = new FriendRequestMessage() { user_id = UIController.Instance._myprofile.id, friend_id = int.Parse(_UserIDText.text)};
        Debug.Log(JsonUtility.ToJson(message)); 
        UIController.Instance.WSConnector.SendRequest(JsonUtility.ToJson(message), WS_ActionType.add_friend,WebServicesManager.Instance.authorization );
        WebServicesManager.Instance.SendFriendRequest(UIController.Instance._myprofile.id, int.Parse(_UserIDText.text));
    }
    public void ChallangeFriendButtobClick(TMPro.TMP_InputField _searchIDText)
    {
        UIController.Instance.SetupScreen(Screens.LetsFightScreen);
    }
    public void OnAcceptFriendButtonClick(TMPro.TMP_Text _ReqText)
    {
        WebServicesManager.Instance.AcceptFriendRequest( (FriendrequestList.Find(item => item.Friend_id == int.Parse(_ReqText.text)).Friends_request_id),true);
    }
    public IEnumerator ShowStatus( string _message)
    {
        ErrorMessageField.text = _message;
        ErrorMessageField.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        ErrorMessageField.gameObject.SetActive(false);
    }
    public void ClearData(Transform _parent)
    {
        for (int i = 0; i < _parent.childCount; i++)
        {
            if (_parent.GetChild(i).gameObject.activeSelf)
            {
                DestroyImmediate(_parent.GetChild(i).gameObject);
                i--;
            }
        }
    }

}
[Serializable]
public class FriendRequestMessage
{
    public int user_id;
    public int friend_id;
}
public class SearchedUsers
{
    public int id { get; set; }
    public int is_friend { get; set; }
    public DateTime created_at { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string Photo { get; set; }
    public string Email { get; set; }
    public string details { get; set; }

}
public class FriendRequest
{
    public int Friends_request_id { get; set; } 
    public int Friend_id { get; set; }
    public DateTime created_at { get; set; }
    public string Friends_name { get; set; }
    public string Friends_email { get; set; }
    public string Friends_is_online { get; set; }
}
public class Friends
{
    public int Friends_request_id { get; set; }
    public int Friend_id { get; set; }
    public DateTime created_at { get; set; }
    public string Friends_name { get; set; }
    public string Friends_email { get; set; }
    public string Friends_is_online { get; set; }
}