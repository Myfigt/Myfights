﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using TMPro;
using Assets.Scripts;
using System;
using UnityEngine.Video;
using Facebook.Unity;
//using System.Windows.Forms;
//using System.Diagnostics;
//using System.Threading.Tasks;
using static VideosContainer;

public class UIController : MonoBehaviour
{
    enum Scope
    {
        PublicProfile = 0b_0000_0001,
        UserFriends = 0b_0000_0010,
        UserBirthday = 0b_0000_0100,
        UserAgeRange = 0b_0000_1000,
        PublishActions = 0b_0001_0000,
        UserLocation = 0b_0010_0000,
        UserHometown = 0b_0100_0000,
        UserGender = 0b_1000_0000,
    };
    [Serializable]
    public enum Screens
    {
        CurrentScreen = -1,
        SplashScreen = 0,
        SignUpScreen = 1,
        LoginScreen = 2,
        LoadingScreen = 3,
        ColorSelectionScreen = 4,
        HomeScreen = 5,
        FighterSelection = 6,
        Figher_VideoSelection = 7,
        VideoPlayerScreen = 8,
        ActionCardRecording = 9,
        ActionCardReview = 10,
        CreateFightStrategy = 11,
        LibraryScreen = 12,
        LetsFightScreen = 13,
        AllTribes = 14,
        MatchMakingScreen = 15,
        DebugScreen = 16,
        FightModeSelectionScreen = 17,
        FriendsManagerScreen = 18,
        ProfileScreen = 19,

    }
    
    public Screens _CurrentScreen = Screens.HomeScreen;
    //::TODO Technical debt .... change myscreens varialble type to UI screen ... for code cleaning;
    public GameObject[] MyScreens;
    public GameObject LoginPanel, SignUpPanel;
    public TMP_InputField signUpEmail, signUpUserName, sighUpPWD;
    public TMP_InputField logInUserName, logInPWD;
    public TMP_Text loginStatusText, signUpStatusText;
    public float Splashtime = 3;
    float timer = 3;
    [SerializeField]
    FighterListManager _fighterSelection;
    [SerializeField]
    public VideosListManager _VideoSelection;
    [SerializeField]
    public VideoPlayerManager _VideoPlayer;
    [SerializeField]
    CreateStrategyPanel _StrategyCreation;
    [SerializeField]
    LibraryScreen _librarycontroller;
    [SerializeField]
    public LetsFightScreen _letsFightScreen;
    [SerializeField]
    TribesManager _tribesManager;
    [SerializeField]
    MatchMakingScreen _matchMakingScreen;
    [SerializeField]
    UIScreen _popUpMessage;

    [SerializeField]
    VideoPlayer _actionCardPreview;
    [SerializeField]
    VideoPlayer _masterCardPreview;
    [SerializeField]
    GameObject _RecordingCotroll;
    [SerializeField]
    public ServerCommunication WSConnector;


    [SerializeField]
    NetworkConnectionManager _NetworkHandle;
    [SerializeField]
    private TMP_Text homeScreenProfileText;
    public UserProfile _myprofile = null;


    [Header("Debug Settings")]
    public bool _deletePlayerPrefs;
    public bool _UseDefaultCridentials;
    public const string Defaultusername = "bilalkhan@mailinator.comm";
    public const string DefaultPwd = "Password@123";
   
    // Start is called before the first frame update
    #region Initializers
    private static UIController instance = null;
    public static UIController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject Manager = new GameObject();
                DontDestroyOnLoad(Manager);
                instance = Manager.AddComponent<UIController>();
                return instance;
            }
            else
            {
                return instance;
            }
        }
    }


    #endregion
    private void Start()
    {
        instance = this;
        SetupScreen(Screens.SplashScreen);
        //Task.Run(() => ServerCommunication .Instance.Initialize());   
        FB.Init(this.OnInitComplete, this.OnHideUnity);
        if (_deletePlayerPrefs)
        {
            PlayerPrefs.DeleteKey("access_token");
        }
        UnityEngine.Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    void OnEnable()
    {
        WebServicesManager.loginUserComplete += OnLoginSuccess;
        WebServicesManager.loginUserFailed += OnLoginFailed;

        WebServicesManager.logOutComplete += OnLogOutSuccess;
        WebServicesManager.logOutFailed += OnLogOutFailed;

        WebServicesManager.registerUserComplete += OnSignUpSuccess;
        WebServicesManager.registerUserFailed += OnSignUpFailed;

        WebServicesManager.FetchFightersComplete += OnFetchAllFighterComplete;
        WebServicesManager.FetchFightersFailed += OnFetchAllFighterFailed;

        WebServicesManager.FetchVideosComplete += OnFetchVideosComplete;
        WebServicesManager.FetchVideosFailed += OnFetchVideosFailed;

        WebServicesManager.UploadVideosComplete += OnVideoUploadComplete;
        WebServicesManager.UploadVideosFailed += OnVideoUplaodFailed;

        WebServicesManager.FetchUserComplete += WebServicesManager_FetchUserComplete;
        WebServicesManager.FetchUserFailed += WebServicesManager_FetchUserFailed;

        WebServicesManager.FetchStrategyComplete += WebServicesManager_FetchStrategyComplete;
        WebServicesManager.FetchStrategyFailed += WebServicesManager_FetchStrategyFailed;

        WebServicesManager.GetActionCardsComplete += WebServicesManager_GetActionCardsComplete;
        WebServicesManager.GetActionCardsFailed += WebServicesManager_GetActionCardsFailed;

        WebServicesManager.GetTribesComplete += WebServicesManager_GetTribesComplete;
        WebServicesManager.GetTribesFailed += WebServicesManager_GetTribesFailed;

        ServerCommunication.NewMessageRecieved += ServerCommunication_NewMessageRecieved;

        WebServicesManager.GetFriendListComplete += WebServicesManager_GetFriendListComplete;
        WebServicesManager.GetFriendListFailed += WebServicesManager_GetFriendListFailed;

    }
    private void WebServicesManager_GetFriendListFailed(string responce)
    {
        throw new NotImplementedException();
    }

    private void WebServicesManager_GetFriendListComplete(string responce)
    {
        _myprofile._allFriends = new List<Friends>();
        _myprofile._allFriends = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Friends>>(responce);
    }
    private void ServerCommunication_NewMessageRecieved(string responce, WS_ActionType _action)
    {
        _popUpMessage.gameObject.SetActive(true);
        _popUpMessage.Initialize(responce ,_action);

    }

    #region Fetch user profile CallBacks
    private void WebServicesManager_FetchUserFailed(string error)
    {
      UnityEngine.Debug.Log(error);
    }
    private void WebServicesManager_FetchUserComplete(string responce)
    {
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(responce);
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "user")
            {
                string obj = easy.JSON.JsonEncode(item.Value);
                _myprofile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(obj);
                homeScreenProfileText.text = _myprofile.name;
                WebServicesManager.Instance.FetchActionCard(_myprofile.id);// _myprofile.id);
                WebServicesManager.Instance.FetchStrategies(_myprofile.id);
                WebServicesManager.Instance.GetFriendList();
                WSConnector.ConnectToServer(_myprofile.id);
            }
        }

        UnityEngine.Debug.Log(responce);
    }
    #endregion
    internal void PlayVideo(int fighterid,int videoid)
    {
        SetupScreen(Screens.VideoPlayerScreen);
        _VideoPlayer.Initialize(fighterid,videoid);
    }
    internal void PlayVideo(string videopath)
    {
        SetupScreen(Screens.VideoPlayerScreen);
        _VideoPlayer.Initialize(videopath);
    }
    internal void ViewActionCard(ActionCard card)
    {
        SetupScreen(Screens.VideoPlayerScreen);
        _VideoPlayer.Initialize(card);
    }
    void OnDisable()
    {
        WebServicesManager.loginUserComplete -= OnLoginSuccess;
        WebServicesManager.loginUserFailed -= OnLoginFailed;
        WebServicesManager.registerUserComplete -= OnSignUpSuccess;
        WebServicesManager.registerUserFailed -= OnSignUpFailed;
        WebServicesManager.FetchFightersComplete -= OnFetchAllFighterComplete;
        WebServicesManager.FetchFightersFailed -= OnFetchAllFighterFailed;
        WebServicesManager.FetchVideosComplete -= OnFetchVideosComplete;
        WebServicesManager.FetchVideosFailed -= OnFetchVideosFailed;
        WebServicesManager.UploadVideosComplete -= OnVideoUploadComplete;
        WebServicesManager.UploadVideosFailed -= OnVideoUplaodFailed;
        WebServicesManager.FetchUserComplete -= WebServicesManager_FetchUserComplete;
        WebServicesManager.FetchUserFailed -= WebServicesManager_FetchUserFailed;
        WebServicesManager.GetTribesComplete -= WebServicesManager_GetTribesComplete;
        WebServicesManager.GetTribesFailed -= WebServicesManager_GetTribesFailed;
        ServerCommunication.NewMessageRecieved -= ServerCommunication_NewMessageRecieved;
        WebServicesManager.GetFriendListComplete -= WebServicesManager_GetFriendListComplete;
        WebServicesManager.GetFriendListFailed -= WebServicesManager_GetFriendListFailed;
        WebServicesManager.FetchStrategyComplete -= WebServicesManager_FetchStrategyComplete;
        WebServicesManager.FetchStrategyFailed -= WebServicesManager_FetchStrategyFailed;
    }
    public void SetupScreen(Screens _screenIndex = Screens.CurrentScreen)
    {
        UnityEngine.Debug.Log("Setting Up Screen " + _screenIndex.ToString());

        if (_screenIndex == Screens.CurrentScreen)
        {
            _screenIndex = _CurrentScreen;
        }
        if (_screenIndex == Screens.ActionCardRecording)
        {
            Screen.orientation = ScreenOrientation.Portrait;
            _masterCardPreview.url = _VideoPlayer._player.url;
            _masterCardPreview.Play();
            _RecordingCotroll.SetActive(true);


        }
        else
        {
           Screen.orientation = ScreenOrientation.LandscapeRight;
            _RecordingCotroll.SetActive(false);
        }
        for (int i = 0; i < MyScreens.Length; i++)
        {
            if (i == (int)_screenIndex)
            {
                MyScreens[i].SetActive(true);
                if (_screenIndex == Screens.LibraryScreen)
                {
                    MyScreens[i].GetComponent<UIScreen>()?.Initialize(_myprofile._allActionCards);
                }
                else
                MyScreens[i].GetComponent<UIScreen>()?.Initialize();
            }
            else
            {
                MyScreens[i].SetActive(false);
            }

        }
        _CurrentScreen = _screenIndex;
        if (_CurrentScreen == Screens.SplashScreen)
        {
            timer = Splashtime;
        }
        if (_CurrentScreen == Screens.LoginScreen)
        {
            SignUpPanel.SetActive(false);
            LoginPanel.SetActive(true);
        }
        if (_CurrentScreen == Screens.SignUpScreen)
        {
            SignUpPanel.SetActive(true);
            LoginPanel.SetActive(false);
        }
        if (_CurrentScreen == Screens.HomeScreen)
        {
            _NetworkHandle.gameObject.SetActive(true);
        }

       // yield return null;
    }
    public void Update()
    {
        if (_CurrentScreen == Screens.SplashScreen)
        {
            if (timer <= 0)
            {
                if (PlayerPrefs.HasKey("access_token"))
                {
                    SetupScreen(Screens.HomeScreen);
                    WebServicesManager.Instance.FetchUser();
                }
                else
                    SetupScreen(Screens.LoginScreen);

            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }
    #region Facebook Integration 
    private void OnInitComplete()
    {

    }
    private void OnHideUnity(bool isGameShown)
    {

    }
    public void LoginWithFB()
    {
        this.CallFBLogin(LoginTracking.LIMITED, Scope.PublicProfile | Scope.UserFriends);
    }
    private void CallFBLogin(LoginTracking mode, Scope scopemask)
    {
        List<string> scopes = new List<string>();

        if ((scopemask & Scope.PublicProfile) > 0)
        {
            scopes.Add("public_profile");
        }
        if ((scopemask & Scope.UserFriends) > 0)
        {
            scopes.Add("user_friends");
        }
        if ((scopemask & Scope.UserBirthday) > 0)
        {
            scopes.Add("user_birthday");
        }
        if ((scopemask & Scope.UserAgeRange) > 0)
        {
            scopes.Add("user_age_range");
        }

        if ((scopemask & Scope.UserLocation) > 0)
        {
            scopes.Add("user_location");
        }

        if ((scopemask & Scope.UserHometown) > 0)
        {
            scopes.Add("user_hometown");
        }

        if ((scopemask & Scope.UserGender) > 0)
        {
            scopes.Add("user_gender");
        }


        if (mode == LoginTracking.ENABLED)
        {
            FB.Mobile.LoginWithTrackingPreference(LoginTracking.ENABLED, scopes, "classic_nonce123", this.HandleResult);
        }
        else // mode == loginTracking.LIMITED
        {
            FB.Mobile.LoginWithTrackingPreference(LoginTracking.LIMITED, scopes, "limited_nonce123", this.HandleLimitedLoginResult);
        }

    }
    protected void HandleResult(IResult result)
    {
        if (result == null)
        {
            // this.LastResponse = "Null Response\n";
            //LogView.AddLog(this.LastResponse);
            return;
        }

        // this.LastResponseTexture = null;

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            // this.Status = "Error - Check log for details";
            // this.LastResponse = "Error Response:\n" + result.Error;
        }
        else if (result.Cancelled)
        {
            //this.Status = "Cancelled - Check log for details";
            //this.LastResponse = "Cancelled Response:\n" + result.RawResult;
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            // this.Status = "Success - Check log for details";
            // this.LastResponse = "Success Response:\n" + result.RawResult;
        }
        else
        {
            //this.LastResponse = "Empty Response\n";
        }

        // LogView.AddLog(result.ToString());
    }
    protected void HandleLimitedLoginResult(IResult result)
    {
        if (result == null)
        {
            //  this.LastResponse = "Null Response\n";
            //  LogView.AddLog(this.LastResponse);
            return;
        }

        //this.LastResponseTexture = null;

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            // this.Status = "Error - Check log for details";
            // this.LastResponse = "Error Response:\n" + result.Error;
        }
        else if (result.Cancelled)
        {
            //  this.Status = "Cancelled - Check log for details";
            //  this.LastResponse = "Cancelled Response:\n" + result.RawResult;
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            //  this.Status = "Success - Check log for details";
            //  this.LastResponse = "Success Response:\n" + result.RawResult;
        }
        else
        {
            //this.LastResponse = "Empty Response\n";
        }
        String resultSummary = "Limited login results\n\n";
        var profile = FB.Mobile.CurrentProfile();
        resultSummary += "name: " + profile.Name + "\n";
        resultSummary += "id: " + profile.UserID + "\n";
        resultSummary += "email: " + profile.Email + "\n";
        resultSummary += "pic URL: " + profile.ImageURL + "\n";
        resultSummary += "birthday: " + profile.Birthday + "\n";
        resultSummary += "age range: " + profile.AgeRange + "\n";
        resultSummary += "first name: " + profile.FirstName + "\n";
        resultSummary += "middle name: " + profile.MiddleName + "\n";
        resultSummary += "last name: " + profile.LastName + "\n";
        resultSummary += "friends: " + String.Join(",", profile.FriendIDs) + "\n";
        resultSummary += "hometown: " + profile.Hometown?.Name + "\n";
        resultSummary += "location: " + profile.Location?.Name + "\n";
        resultSummary += "gender: " + profile.Gender + "\n";


    }
    #endregion

    #region UserSignup/Signin
    public void Login()
    {
        if (_UseDefaultCridentials)
        {
            WebServicesManager.Instance.LoginUser(Defaultusername, DefaultPwd);
        }
        else
            WebServicesManager.Instance.LoginUser(logInUserName.text, logInPWD.text);
    }
    void OnLoginSuccess(string data)
    {
        //Debug.Log("UIController --" + data);
        StartCoroutine(ShowStatus(loginStatusText, "logged in successfully", Screens.HomeScreen));
        WebServicesManager.Instance.FetchUser();
    }
    void OnLoginFailed(string data)
    {
        UnityEngine.Debug.Log("UIController --" + data);
        StartCoroutine(ShowStatus(loginStatusText, data));
    }

    public void LogOut()
    {
            WebServicesManager.Instance.LogOutUser();
    }
    void OnLogOutSuccess(string data)
    {
        PlayerPrefs.DeleteKey("access_token");
        SetupScreen(Screens.SplashScreen);
    }
    void OnLogOutFailed(string data)
    {
        UnityEngine.Debug.Log("Unable to log out due to error " + data);
 
    }

    public void ShowSignInPWD(bool value)
    {
        if (value)
        {
            logInPWD.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            logInPWD.contentType = TMP_InputField.ContentType.Password;
        }
        logInPWD.textComponent.SetAllDirty();
    }
    public void ShowSignUpPWD(bool value)
    {
        if (value)
        {
            sighUpPWD.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            sighUpPWD.contentType = TMP_InputField.ContentType.Password;
        }
        logInPWD.textComponent.SetAllDirty();
    }
    public void SignUp()
    {
        WebServicesManager.Instance.ResgisterUser(signUpEmail.text, signUpUserName.text, sighUpPWD.text);
    }
    void OnSignUpSuccess(string data)
    {
        StartCoroutine(ShowStatus(signUpStatusText, data,Screens.HomeScreen));
        WebServicesManager.Instance.FetchUser();
    }
    void OnSignUpFailed(string data)
    {
        StartCoroutine(ShowStatus(signUpStatusText, data));
    }
    #endregion
    public void FetchAllFighters()
    {
        WebServicesManager.Instance.FetchFighter();
    }
    void OnFetchAllFighterComplete(string data)
    {
        List<Fighter> fighters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fighter>>(data);
        SetupScreen(Screens.FighterSelection);
        _fighterSelection.Initialize(fighters);

    }
    void OnFetchAllFighterFailed(string data)
    {
        UnityEngine.Debug.Log(data);
    }

    void OnFetchVideosComplete(string data)
    {
        //List<ActionCard> fighters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ActionCard>>(data);
        //_VideoSelection.Initialize(fighters,_fighterSelection.SelectefFighter);
        //SetupScreen(Screens.Figher_VideoSelection);
    }
    void OnFetchVideosFailed(string data)
    {

    }
    public void BackToVideoSelection()
    {
        SetupScreen(Screens.Figher_VideoSelection);
    }

    public IEnumerator ShowStatus(TMP_Text _element, string _message, Screens _switchTo = Screens.CurrentScreen)
    {
        _element.text = _message;
        _element.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        _element.gameObject.SetActive(false);
        if (_switchTo != Screens.CurrentScreen)
        {
            SetupScreen(_switchTo);
        }
    }

    public void SelectColor()
    {
        SetupScreen(Screens.HomeScreen);
    }

    public void RecordActionCard()
    {
       SetupScreen(Screens.ActionCardRecording);
    }
    public void GoToActionCardReview(string videoPath)
    {
        SetupScreen(Screens.ActionCardReview);
        _actionCardPreview.url = videoPath;
        _actionCardPreview.Play();
    }

    [SerializeField]
    TMP_Text UploadingVideoText;
    public void OnUploadVideoButtonClick()
    {
        UploadingVideoText.gameObject.SetActive(true);
        WebServicesManager.Instance.UploadVideos(_VideoSelection.SelectedVideo.sub_type, _actionCardPreview.url, _fighterSelection.SelectedFighter.id, _myprofile.id);
    }
    public void OnVideoUploadComplete(string data)
    {
        UnityEngine.Debug.LogError("Video Uploaded successfully " + data);
        ActionCard card = Newtonsoft.Json.JsonConvert.DeserializeObject <ActionCard>(data);
        if (card!= null)
            _myprofile._allActionCards.Insert(0,card);
        ViewActionCard(card);
        UploadingVideoText.gameObject.SetActive(false);
    }
    public void OnVideoUplaodFailed(string data)
    {
        UnityEngine.Debug.LogError(data);
        _popUpMessage.gameObject.SetActive(true);
        _popUpMessage.Initialize(data);
        UploadingVideoText.gameObject.SetActive(false);

    }

    public void OnCreateFightStrategyButtonClick()
    {
        _StrategyCreation.Initialize(_myprofile._allActionCards, _myprofile._myStrategy);
        SetupScreen(Screens.CreateFightStrategy);
        //WebServicesManager.Instance.FetchStrategies(_myprofile.id);
    }
    private void WebServicesManager_FetchStrategyFailed(string error)
    {
        throw new NotImplementedException();
    }
    private void WebServicesManager_FetchStrategyComplete(string responce)
    {
        if (_CurrentScreen != Screens.HomeScreen)
        {
            return;
        }
        FightStrategy strategy = null;
        try
        {
        strategy = Newtonsoft.Json.JsonConvert.DeserializeObject<FightStrategy>(responce);
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(responce);
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "combinations")
            {
                strategy.combinations = new FightCombination[9];
                int i = 0;
                foreach (var res in item.Value as ArrayList)
                {

                    string combos = easy.JSON.JsonEncode(res);
                    strategy.combinations[i] = Newtonsoft.Json.JsonConvert.DeserializeObject<FightCombination>(combos);
                    i++;
                    if (i >= 9)
                    {
                        break;
                    }
                }

            }
        }
        }
        catch (Exception)
        {
            UnityEngine.Debug.Log("unable to parse fight strategy");
        }
        if (strategy == null)
        {
            strategy = new FightStrategy() { title = _myprofile.name+"_Strategy" +_myprofile.id, combinations = new FightCombination[9] , playerID = _myprofile.id, player_name = _myprofile.name };
        }
        _myprofile._myStrategy = strategy;
        
    }

    private void WebServicesManager_GetActionCardsFailed(string error)
    {

    }
    private void WebServicesManager_GetActionCardsComplete(string responce)
    {
        UnityEngine.Debug.Log(responce);
        List<ActionCard> fighters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ActionCard>>(responce);
        _myprofile._allActionCards = fighters;
    }

    public void OnlibraryButtonClick()
    {
        _librarycontroller.Initialize(_myprofile._allActionCards);
        SetupScreen(Screens.LibraryScreen);
    }

    public void OnAllTribesButtonClick()
    {
        WebServicesManager.Instance.FetchTribes();
    }
    private void WebServicesManager_GetTribesFailed(string error)
    {
        UnityEngine.Debug.LogError("Error Fetching tribes");
    }
    private void WebServicesManager_GetTribesComplete(string responce)
    {
        List<Tribe> _allTribes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Tribe>>(responce);
        _tribesManager.Initialize(_allTribes);
        SetupScreen(Screens.AllTribes);

    }

    //public void QuickMatch() => _NetworkHandle.JoinRandomRoom();
    public void QuickMatch()
    {
        SetupScreen(UIController.Screens.FightModeSelectionScreen);
        //::TODO this is to be done when fight mode is selected and opponent has already joined the room.
        //SetupScreen(UIController.Screens.LetsFightScreen);
        //_letsFightScreen.Initialize(_myprofile._myStrategy, _myprofile._myStrategy);
    }

    public void PlayWithFriendsButtonClick(TMP_Text status)
    {
        status.gameObject.SetActive(true);
        _NetworkHandle.CreateNewRoom(_myprofile.id.ToString());
    }
    public void FriendsManagerButtonClick()
    {
        SetupScreen(UIController.Screens.FriendsManagerScreen);
    }
    public void PlayRandom(TMP_Text status)
    {
        status.gameObject.SetActive(true);
        _NetworkHandle.JoinRandomRoom();
        //SetupScreen(UIController.Screens.LetsFightScreen);
    }
    public void GoToMatchMakingScreen( string roomID)
    {
        _matchMakingScreen.Initialize(roomID);
        SetupScreen(UIController.Screens.MatchMakingScreen);
    }
    public void GoToMatchScreen( FightStrategy _opponentStrategy)
    {
        SetupScreen(UIController.Screens.LetsFightScreen);
        _letsFightScreen.Initialize(_myprofile._myStrategy, _opponentStrategy);
    }
    public void OnShowProfileButtonClick()
    {
        SetupScreen(UIController.Screens.ProfileScreen);
    }
    public void OnDebugScene()
    {
        SetupScreen(UIController.Screens.DebugScreen);
    }

    public void OnFriendChallangeAccepted(string roomID)
    {
        _NetworkHandle.JoinRoom(roomID) ;
    }

    public void OnplayerLeavelRoom(string message)
    {
        _popUpMessage.gameObject.SetActive(true);
        _popUpMessage.Initialize(message);
    }

}
