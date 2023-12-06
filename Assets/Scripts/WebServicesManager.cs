using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class WebServicesManager : MonoBehaviour
{
    #region Constants
    public const string baseURL = "http://54.66.142.35:8000/api/";
    public string authorization = "Token b106701ab5fb114ced11845e6ee25469739d12a765cff141cc1a53a1ff7256d0";
    public const string contentType = "application/json";
    public string deviceID;
    public string platform;
    #endregion

    #region Initializers
    private static WebServicesManager instance = null;
    public static WebServicesManager Instance
    {
        get
        {
            if (instance == null)
            {
                StartJPWebServicesManager();
                return instance;
            }
            else
            {
                return instance;
            }
        }
    }

    private static void StartJPWebServicesManager()
    {
        GameObject WebServices = new GameObject("WebServicesManager");
        DontDestroyOnLoad(WebServices);
        instance = WebServices.AddComponent<WebServicesManager>();
        instance.deviceID = SystemInfo.deviceUniqueIdentifier;
        if (PlayerPrefs.HasKey("access_token"))
        {
            instance.authorization = PlayerPrefs.GetString("access_token");
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            instance.platform = "iOS";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            instance.platform = "Android";
        }
        else
        {
            instance.platform = "Unity";
        }
    }
    #endregion

    #region Register User API
    public delegate void OnRegisterUserComplete(string responce);
    public delegate void OnRegisterUserFailed(string error);

    public static event OnRegisterUserComplete registerUserComplete;
    public static event OnRegisterUserFailed registerUserFailed;

    public void ResgisterUser(string _email, string _userName, string _password)
    {
        StartCoroutine(_ResgisterUser(_email, _userName, _password));
    }

    IEnumerator _ResgisterUser(string _email, string _userName, string _password)
    {
        string url = baseURL + "signup";
        Debug.LogError(_userName + " email ----" + _email);

        WWWForm data = new WWWForm();
        data.AddField("name", _userName);
        //data.AddField("last_name", "zia");
        data.AddField("email", _email);
        data.AddField("password", _password);
        data.AddField("device", platform);
        data.AddField("device_id", deviceID);
        data.AddField("firebase_token", "");

        WWW www = new WWW(url, data);
        yield return www;
        Debug.Log("ResgisterUser RESPONCE = " + www.text);
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "error")
            {
                if (bool.TryParse(item.Value.ToString(), out bool _error))
                {
                    if (_error)
                    {
                        foreach (DictionaryEntry result in responceData)
                        {
                            if (result.Key.ToString() == "message")
                            {

                                if (registerUserComplete != null)
                                {
                                    registerUserFailed(result.Value.ToString());
                                }
                                break;
                            }

                        }
                    }
                    else
                    {
                        Debug.Log("ResgisterUser success = ");
                        foreach (DictionaryEntry result in responceData)
                        {
                            if (result.Key.ToString() == "message")
                            {

                                if (registerUserComplete != null)
                                {
                                    PlayerPrefs.SetString("access_token", "Token " + responceData["access_token"].ToString());
                                    authorization = "Token " + responceData["access_token"].ToString();
                                    registerUserComplete(result.Value.ToString());
                                }
                                break;
                            }

                        }

                    }
                }
            }
        }

        yield return null;
    }
    #endregion

    #region Login User API
    public delegate void OnLoginUserComplete(string responce);
    public delegate void OnLoginUserFailed(string error);
    public static event OnLoginUserComplete loginUserComplete;
    public static event OnLoginUserFailed loginUserFailed;

    public void LoginUser(string _email, string _password, bool _override = false)
    {
        StartCoroutine(_LoginUser(_email, _password, _override));
    }

    IEnumerator _LoginUser(string _email, string _password, bool _override)
    {
        string url = baseURL + "login";

        WWWForm data = new WWWForm();
        data.AddField("username", _email);
        data.AddField("password", _password);
        data.AddField("platform", platform);
        data.AddField("deviceId", deviceID);
        data.AddField("firebase_token", "234987039485");
        //override flag for multi device login
        _override = true;

        if (_override)
        {
            data.AddField("override", _override.ToString());
        }

        //using (UnityWebRequest webRequest = UnityWebRequest.Post(url,data))
        //{
        //    // Request and wait for the desired page.
        //    yield return webRequest.SendWebRequest();


        //    switch (webRequest.result)
        //    {
        //        case UnityWebRequest.Result.ConnectionError:
        //        case UnityWebRequest.Result.DataProcessingError:
        //            break;
        //        case UnityWebRequest.Result.ProtocolError:
        //            Debug.LogError( ": HTTP Error: " + webRequest.error);
        //            break;
        //        case UnityWebRequest.Result.Success:
        //            Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
        //            break;
        //    }
        //}
        WWW www = new WWW(url, data);
        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "error")
            {
                if (bool.TryParse(item.Value.ToString(), out bool _error))
                {
                    if (!_error)
                    {
                        foreach (DictionaryEntry result in responceData)
                        {
                            if (result.Key.ToString() == "data")
                            {
                                Hashtable da = (Hashtable)result.Value;
                                foreach (DictionaryEntry entry in da)
                                {
                                    Debug.Log(entry.Key);
                                }
                                Debug.Log(result.Value.ToString());
                                if (loginUserComplete != null)
                                {
                                    PlayerPrefs.SetString("access_token", "Token " + responceData["access_token"].ToString());
                                    loginUserComplete(easy.JSON.JsonEncode(result.Value));
                                }
                                break;
                            }
                            if (result.Key.ToString() == "access_token")
                            {
                                authorization = "Token " + result.Value.ToString();
                            }
                        }

                    }
                    else
                    {
                        foreach (DictionaryEntry result in responceData)
                        {
                            if (result.Key.ToString() == "message")
                            {

                                if (loginUserFailed != null)
                                    loginUserFailed(result.Value.ToString());
                                break;
                            }

                        }

                    }

                }

            }
        }
        yield return null;
    }
    #endregion

    #region LogOut User API
    public delegate void OnLogOutComplete(string responce);
    public delegate void OnLogOutFailed(string error);
    public static event OnLoginUserComplete logOutComplete;
    public static event OnLoginUserFailed logOutFailed;

    public void LogOutUser()
    {
        StartCoroutine(_LogOutUser());
    }

    IEnumerator _LogOutUser()
    {
        string url = baseURL + "logout";

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", contentType);
        headers.Add("Authorization", authorization);
        headers.Add("Accept", contentType);

        WWWForm data = new WWWForm();
        data.AddField("title", "");
        var json = JsonUtility.ToJson(data);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
        WWW www = new WWW(url, postData, headers);
        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "error")
            {
                if (bool.TryParse(item.Value.ToString(), out bool _error))
                {
                    if (!_error)
                    {
                        logOutComplete(www.text.ToString());
                    }
                    else
                    {
                        foreach (DictionaryEntry result in responceData)
                        {
                            if (result.Key.ToString() == "message")
                            {

                                if (loginUserFailed != null)
                                    logOutFailed(result.Value.ToString());
                                break;
                            }

                        }

                    }

                }

            }
            else if (item.Value.ToString().Contains("Success"))
            {
                logOutComplete(www.text.ToString());
                yield return null;
            }
        }
        logOutFailed("unable to log out");
        yield return null;
    }
    #endregion

    #region GetFighters
    public delegate void OnFetchFightersComplete(string responce);
    public delegate void OnFetchFightersFailed(string error);
    public static event OnFetchFightersComplete FetchFightersComplete;
    public static event OnFetchFightersFailed FetchFightersFailed;

    public void FetchFighter()
    {
        StartCoroutine(_FetchFighters());
    }

    IEnumerator _FetchFighters()
    {
        string url = baseURL + "get_fighters";

        //Dictionary<string, string> headers = new Dictionary<string, string>();
        //headers.Add("Content-Type", contentType);
        //headers.Add("Authorization", authorization);
        //headers.Add("Accept", contentType);
        //Hashtable data = new Hashtable();
        //data.Add("email", "test@majid.com");
        //string JSONStr = easy.JSON.JsonEncode(data);
        //byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

        WWW www = new WWW(url);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {
                FetchFightersComplete(easy.JSON.JsonEncode(item.Value));
                isScuccess = true;
            }
        }
        if (!isScuccess)
        {
            FetchFightersFailed("Faild to find fighters");
        }
        yield return null;
    }
    #endregion

    #region GetVideos
    public delegate void OnFetchVideosComplete(string responce);
    public delegate void OnFetchVideosFailed(string error);
    public static event OnFetchFightersComplete FetchVideosComplete;
    public static event OnFetchFightersFailed FetchVideosFailed;

    public void FetchVideos(int _fighterID, string _belt, Action<bool, string> OnComplete = null)
    {
        StartCoroutine(_FetchVideos(_fighterID, _belt, OnComplete));
    }

    IEnumerator _FetchVideos(int _fighterID, string _belt, Action<bool, string> OnComplete)
    {

        string url = baseURL + "fighter_videos/" + _fighterID + "/" + _belt;
        //Dictionary<string, string> headers = new Dictionary<string, string>();
        //headers.Add("Content-Type", contentType);
        //headers.Add("Authorization", authorization);
        //headers.Add("Accept", contentType);
        //Hashtable data = new Hashtable();
        //data.Add("email", "test@majid.com");
        //string JSONStr = easy.JSON.JsonEncode(data);
        //byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

        WWW www = new WWW(url);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {
                FetchVideosComplete(easy.JSON.JsonEncode(item.Value));
                isScuccess = true;
                OnComplete?.Invoke(isScuccess, easy.JSON.JsonEncode(item.Value));
            }
        }
        if (!isScuccess)
        {
            FetchVideosFailed("Faild to find fighters");
            OnComplete?.Invoke(isScuccess, "Faild to find fighters");
        }
        yield return null;
    }
    #endregion

    #region CreateActionCard
    public delegate void OnUploadVideoComplete(string responce);
    public delegate void OnUploadVideoFailed(string error);
    public static event OnUploadVideoComplete UploadVideosComplete;
    public static event OnUploadVideoFailed UploadVideosFailed;

    public void UploadVideos(string _title, string _path, int _figherVideoID, int _fighterID)
    {
        StartCoroutine(_UploadVideos(_title, _path, _figherVideoID, _fighterID));
    }

    IEnumerator _UploadVideos(string _title, string _path, int _figherVideoID, int _fighterID)
    {

        string url = baseURL + "upload_video";
        WWWForm data = new WWWForm();
        data.AddField("title", _title);
        data.AddBinaryData("file_name", File.ReadAllBytes(_path));
        data.AddField("fighter_video", _figherVideoID);
        data.AddField("player", _fighterID);

        WWW www = new WWW(url, data);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);

        bool isScuccess = false;
       
        if (www.responseHeaders["STATUS"].Contains("201"))
        {
            isScuccess = true;
            UploadVideosComplete(www.text);
        }
        if (!isScuccess)
        {
            UploadVideosFailed(www.text);
        }
        yield return null;
    }
    #endregion

    #region GetFightStrategy
    public delegate void OnFetchStrategyComplete(string responce);
    public delegate void OnFetchStrategyFailed(string error);
    public static event OnFetchStrategyComplete FetchStrategyComplete;
    public static event OnFetchStrategyFailed FetchStrategyFailed;

    public void FetchStrategies(int _fighterID, Action<bool, string> OnComplete = null)
    {
        StartCoroutine(_FetchStrategies(_fighterID, OnComplete));
    }

    IEnumerator _FetchStrategies(int _fighterID, Action<bool, string> OnComplete = null)
    {

        string url = baseURL + "list_fight_strategy/" + _fighterID;
        //Dictionary<string, string> headers = new Dictionary<string, string>();
        //headers.Add("Content-Type", contentType);
        //headers.Add("Authorization", authorization);
        //headers.Add("Accept", contentType);
        //Hashtable data = new Hashtable();
        //data.Add("email", "test@majid.com");
        //string JSONStr = easy.JSON.JsonEncode(data);
        //byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

        WWW www = new WWW(url);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {
                if ((item.Value as ArrayList).Count == 0)
                {
                    FetchStrategyComplete(easy.JSON.JsonEncode(string.Empty));
                }
                else
                {
                    foreach (var res in item.Value as ArrayList)
                    {
                        isScuccess = true;
                        if (OnComplete != null)
                            OnComplete?.Invoke(isScuccess, easy.JSON.JsonEncode(res));
                        else
                        FetchStrategyComplete(easy.JSON.JsonEncode(res));
                        
                       
                        break;
                    }
                }
                
            }
        }
        if (!isScuccess)
        {
            FetchStrategyFailed("Faild to find fighters");
        }
        yield return null;
    }
    #endregion

    #region CreateFightStrategy
    public delegate void OnCreateStrategyComplete(string responce);
    public delegate void OnCreateStrategyFailed(string error);
    public static event OnCreateStrategyComplete CreateStrategyComplete;
    public static event OnCreateStrategyFailed CreateStrategyFailed;

    public void CreateStrategy(FightStrategy _strategy)
    {
        StartCoroutine(_CreateFightStrategy( _strategy));
    }

    IEnumerator _CreateFightStrategy( FightStrategy _strategy)
    {

        string url = baseURL + "create_fight_strategy";
        WWWForm data = new WWWForm();
        string s = JsonUtility.ToJson(_strategy);
        data.AddField("fight_strategy",s );
       
        WWW www = new WWW(url, data);
        yield return www;
        FightStrategy responce = (FightStrategy)JsonUtility.FromJson<FightStrategy>(www.text);
        if (responce == null)
        {
            CreateStrategyFailed("Failed to upload strategy");
        }
        else
        {
            CreateStrategyComplete(www.text);
        }
        yield return null;
    }
    #endregion

    #region GetUserProfile
    public delegate void OnFetchUserComplete(string responce);
    public delegate void OnFetchUserFailed(string error);
    public static event OnFetchUserComplete FetchUserComplete;
    public static event OnFetchUserFailed FetchUserFailed;

    public void FetchUser()
    {
        StartCoroutine(_FetchUser());
    }

    IEnumerator _FetchUser()
    {

        string url = baseURL + "user_profile";
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", contentType);
        headers.Add("Authorization", authorization);
        headers.Add("Accept", contentType);
        //Hashtable data = new Hashtable();
        //data.Add("email", "test@majid.com");
        Debug.Log(authorization);

        WWW www = new WWW(url, null, headers);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "data")
            {
                FetchUserComplete(easy.JSON.JsonEncode(item.Value));
                isScuccess = true;
            }
        }
        if (!isScuccess)
        {
            FetchUserFailed("Faild to get user profile");
        }
        yield return null;
    }
    #endregion

    #region GetAllActionCards
    public delegate void OnGetActionCardsComplete(string responce);
    public delegate void OnGetActionCardsFailed(string error);
    public static event OnGetActionCardsComplete GetActionCardsComplete;
    public static event OnGetActionCardsFailed GetActionCardsFailed;

    public void FetchActionCard(int _playerID, Action<bool, string> OnComplete = null)
    {
        StartCoroutine(_FetchVideos(_playerID));
    }

    IEnumerator _FetchVideos(int _playerID, Action<bool, string> OnComplete = null)
    {

        string url = baseURL + "player_videos/" + _playerID;
        

        WWW www = new WWW(url);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {
                GetActionCardsComplete(easy.JSON.JsonEncode(item.Value));
                isScuccess = true;
                OnComplete?.Invoke(isScuccess , easy.JSON.JsonEncode(item.Value));
            }
        }
        if (!isScuccess)
        {
            GetActionCardsFailed("Faild to find fighters");
        }
        yield return null;
    }
    #endregion

    #region CreateTribe
    public delegate void OnCreateTribeComplete(string responce);
    public delegate void OnCreateTribeFailed(string error);
    public static event OnCreateTribeComplete CreateTribeComplete;
    public static event OnCreateTribeFailed CreateTribeFailed;

    public void CreateTribe(string _tribeName, string _playerID)
    {
        StartCoroutine(_CreateTribe(_tribeName, _playerID));
    }

    IEnumerator _CreateTribe(string _tribeName, string _playerID)
    {

        string url = baseURL + "add_tribe";
        WWWForm data = new WWWForm();
        data.AddField("name", _tribeName);
        //data.AddField("last_name", "zia");
        data.AddField("user_id", _playerID);
        data.AddField("platform", platform);
        data.AddField("deviceId", deviceID);

        WWW www = new WWW(url, data);

        yield return www;
        if (www.error == null)
        {
            Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
            bool isScuccess = false;
            foreach (DictionaryEntry item in responceData)
            {
                if (item.Key.ToString() == "status")
                {
                    if (bool.TryParse(item.Value.ToString(), out isScuccess) && !isScuccess)
                    {
                        CreateTribeComplete(easy.JSON.JsonEncode(responceData));
                    }
                }
            }
            if (isScuccess)
            {
                CreateTribeFailed("Faild to find fighters");
            }
        }

        yield return null;
    }
    #endregion

    #region GetAllTribes
    public delegate void OnGetTribesComplete(string responce);
    public delegate void OnGetTribesFailed(string error);
    public static event OnGetTribesComplete GetTribesComplete;
    public static event OnGetTribesFailed GetTribesFailed;

    public void FetchTribes()
    {
        StartCoroutine(_FetchTribes());
    }

    IEnumerator _FetchTribes()
    {

        string url = baseURL + "list_tribes";
        WWW www = new WWW(url);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {
                GetTribesComplete(easy.JSON.JsonEncode(item.Value));
                isScuccess = true;
            }
        }
        if (!isScuccess)
        {
            GetTribesFailed("Faild to Fetch Tribes");
        }
        yield return null;
    }
    #endregion

    #region Join/Leave Tribe
    public delegate void OnJoinTribesComplete(string responce);
    public delegate void OnjoinTribesFailed(string error);
    public static event OnJoinTribesComplete JoinTribesComplete;
    public static event OnJoinTribesComplete JoinTribesFailed;

    public delegate void OnLeaveTribesComplete(string responce);
    public delegate void OnLeaveTribesFailed(string error);
    public static event OnLeaveTribesComplete LeaveTribesComplete;
    public static event OnLeaveTribesComplete LeaveTribesFailed;

    public void JoinOrLeaveTribe(int _tribeID)
    {
        StartCoroutine(_JoinOrLeaveTribe(_tribeID));
    }

    IEnumerator _JoinOrLeaveTribe(int _tribeID)
    {

        string url = baseURL + "join_leave_tribe/" + _tribeID;
        WWW www = new WWW(url);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {
                if (_tribeID == 0)
                {

                }
                else
                {

                }
                isScuccess = true;
            }
        }
        if (!isScuccess)
        {
            if (_tribeID == 0)
            {

            }
            else
            {

            }
        }
        yield return null;
    }
    #endregion

    #region Online/Offline Player
    public delegate void OnPlayerStatusChangeComplete(string responce);
    public delegate void OnPlayerStatusChangedFailed(string error);
    public static event OnPlayerStatusChangeComplete StatusUpdateComplete;
    public static event OnPlayerStatusChangedFailed StatusUpdateFailed;



    public void StatusUpdate(int Status)
    {
        StartCoroutine(_StatusUpdate(Status));
    }

    IEnumerator _StatusUpdate(int Status)
    {

        string url = baseURL + "make_user_online_offline/";
        WWWForm data = new WWWForm();
        data.AddField("is_online", Status);
        data.AddField("platform", platform);
        data.AddField("deviceId", deviceID);
        data.AddField("firebase_token", "234987039485");
        //override flag for multi device login


        WWW www = new WWW(url, data);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {

                isScuccess = true;
            }
        }
        if (!isScuccess)
        {

        }
        yield return null;
    }
    #endregion

    #region SearchFriends
    public delegate void OnSearchFriendComplete(string responce);
    public delegate void OnSearchFriendFailed(string error);
    public static event OnSearchFriendComplete SearchFriendComplete;
    public static event OnSearchFriendFailed SearchFriendFailed;

    public void SearchFriend(string UserID, Action<bool, string> OnComplete = null)
    {
        StartCoroutine(_SearchFriend(UserID, OnComplete));
    }

    IEnumerator _SearchFriend(string _fighterID, Action<bool, string> OnComplete)
    {

        string url = baseURL + "search_player?player_id=" + int.Parse(_fighterID);
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", contentType);
        headers.Add("Authorization", authorization);
        headers.Add("Accept", contentType);
        //Hashtable data = new Hashtable();
        //data.Add("email", "test@majid.com");


        WWW www = new WWW(url, null, headers);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {
                SearchFriendComplete(easy.JSON.JsonEncode(item.Value));
                isScuccess = true;
                OnComplete?.Invoke(isScuccess, easy.JSON.JsonEncode(item.Value));
            }
        }
        if (!isScuccess)
        {
            SearchFriendFailed("Faild to find fighters");
            OnComplete?.Invoke(isScuccess, "Faild to find fighters");
        }
        yield return null;
    }
    #endregion

    #region Accept Friend Request 
    public delegate void OnAcceptFriendRequestComplete(string responce);
    public delegate void OnAcceptFriendRequestFailed(string error);
    public static event OnAcceptFriendRequestComplete AcceptFriendRequestComplete;
    public static event OnAcceptFriendRequestComplete AcceptFriendRequestFailed;



    public void AcceptFriendRequest(int RequestID, bool Acceptance)
    {
        StartCoroutine(_AcceptFriendRequest(RequestID, Convert.ToInt32(Acceptance)));
    }

    IEnumerator _AcceptFriendRequest(int _requestID, int _acceptance)
    {

        string url = baseURL + "accept_friend_request";
        WWWForm data = new WWWForm();
        data.AddField("friend_request_id", _requestID);
        data.AddField("accept", (int)_acceptance);
        data.AddField("platform", platform);
        data.AddField("deviceId", deviceID);
        data.AddField("firebase_token", "234987039485");
        //override flag for multi device login
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", contentType);
        headers.Add("Authorization", authorization);
        headers.Add("Accept", contentType);

        WWW www = new WWW(url, data);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {

                isScuccess = true;
            }
        }
        if (!isScuccess)
        {

        }
        yield return null;
    }
    #endregion

    #region Send Friend Request 
    public delegate void OnSendFriendRequestComplete(string responce);
    public delegate void OnSendFriendRequestFailed(string error);
    public static event OnSendFriendRequestComplete SendFriendRequestComplete;
    public static event OnSendFriendRequestComplete SendFriendRequestFailed;



    public void SendFriendRequest(int UserID, int FriendID)
    {
        StartCoroutine(_SendFriendRequest(UserID, FriendID));
    }

    IEnumerator _SendFriendRequest(int _user, int _friend)
    {

        string url = baseURL + "add_friend";
        WWWForm data = new WWWForm();
        data.AddField("user", _user);
        data.AddField("friend", _friend);
        data.AddField("platform", platform);
        data.AddField("deviceId", deviceID);

        //override flag for multi device login


        WWW www = new WWW(url, data);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "success")
            {
                if (bool.TryParse(item.Value.ToString(), out isScuccess))
                {
                    if (isScuccess)
                    {
                        SendFriendRequestComplete("friend request complete");
                    }
                    else { SendFriendRequestFailed("unable to send friend request"); }

                }
            }
        }
        yield return null;
    }
    #endregion

    #region Fetch Friends Request List 
    public delegate void OnFetchFriendRequestListComplete(string responce);
    public delegate void OnFetchFriendRequestListFailed(string error);
    public static event OnFetchFriendRequestListComplete FetchFriendRequestListComplete;
    public static event OnFetchFriendRequestListComplete FetchFriendRequestListFailed;



    public void FetchFriendRequestList()
    {
        StartCoroutine(_FetchFriendRequestList());
    }

    IEnumerator _FetchFriendRequestList()
    {

        string url = baseURL + "list_friend_requests";
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", contentType);
        headers.Add("Authorization", authorization);
        headers.Add("Accept", contentType);
        WWW www = new WWW(url, null, headers);

        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {
                FetchFriendRequestListComplete(easy.JSON.JsonEncode(item.Value));
                isScuccess = true;
            }
        }
        if (!isScuccess)
        {
            FetchFriendRequestListFailed(www.text);
        }
        yield return null;
    }
    #endregion

    #region Fetch Friends  List 
    public delegate void OnGetFriendListComplete(string responce);
    public delegate void OnGetFriendListFailed(string error);
    public static event OnGetFriendListComplete GetFriendListComplete;
    public static event OnGetFriendListComplete GetFriendListFailed;



    public void GetFriendList()
    {
        StartCoroutine(_GetFriendList());
    }

    IEnumerator _GetFriendList()
    {

        string url = baseURL + "list_friends";
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", contentType);
        headers.Add("Authorization", authorization);
        headers.Add("Accept", contentType);
        WWW www = new WWW(url, null, headers);
        yield return www;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(www.text);
        bool isScuccess = false;
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "results")
            {
                GetFriendListComplete(easy.JSON.JsonEncode(item.Value));
                isScuccess = true;
            }
        }
        if (!isScuccess)
        {
            GetFriendListFailed(www.error);

        }
        yield return null;
    }
    #endregion

}
