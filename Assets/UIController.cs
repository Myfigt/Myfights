using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts;
using System;
using UnityEngine.Video;
using Facebook.Unity;

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
    public enum Screens
    {
        CurrentScreen = -1,
        SplashScreen = 0,
        SignUpScreen = 1,
        LoginScreen =2,
        LoadingScreen = 3,
        ColorSelectionScreen = 4,
        HomeScreen = 5,
        FighterSelection = 6,
        Figher_VideoSelection = 7,
        VideoPlayerScreen = 8,
        ActionCardRecording = 9,
        ActionCardReview = 10,
        CreateFightStrategy = 11,
    }
    public Screens _CurrentScreen = Screens.HomeScreen;
    public GameObject[] MyScreens;
    public GameObject LoginPanel, SignUpPanel;
    public TMP_InputField signUpEmail, signUpUserName, sighUpPWD;
    public TMP_InputField logInUserName, logInPWD ;
    public TMP_Text loginStatusText , signUpStatusText;
    public float Splashtime = 3;
    float timer = 3;
    [SerializeField]
    FighterListManager _fighterSelection;
    [SerializeField]
    VideosListManager _VideoSelection;
    [SerializeField]
    VideoPlayerManager _VideoPlayer;
    [SerializeField]
    BlazePoseSample _BPS;
    [SerializeField]
    VideoPlayer _actionCardPreview;
    [SerializeField]
    VideoPlayer _masterCardPreview;
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
        //SetupScreen(Screens.HomeScreen);
        instance = this;
        SetupScreen(Screens.SplashScreen);
        FB.Init(this.OnInitComplete, this.OnHideUnity);
    }
    void OnEnable()
    {
        WebServicesManager.loginUserComplete += OnLoginSuccess;
        WebServicesManager.loginUserFailed += OnLoginFailed;
        WebServicesManager.registerUserComplete += OnSignUpSuccess;
        WebServicesManager.registerUserFailed += OnSignUpFailed;
        WebServicesManager.FetchFightersComplete += OnFetchAllFighterComplete;
        WebServicesManager.FetchFightersFailed += OnFetchAllFighterFailed;
        WebServicesManager.FetchVideosComplete += OnFetchVideosComplete;
        WebServicesManager.FetchVideosFailed += OnFetchVideosFailed;
        WebServicesManager.UploadVideosComplete += OnVideoUploadComplete;
        WebServicesManager.UploadVideosFailed += OnVideoUplaodFailed;
    }

    internal void PlayVideo(string path)
    {
        SetupScreen(Screens.VideoPlayerScreen);
        _VideoPlayer.Initialize(path);
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
    }
    public void SetupScreen(Screens _screenIndex = Screens.CurrentScreen)
    {
        Debug.Log("Setting Up Screen " + _screenIndex.ToString());
        
        if (_screenIndex == Screens.CurrentScreen)
        {
            _screenIndex = _CurrentScreen;
        }
        for (int i = 0; i < MyScreens.Length; i++)
        {
            if (i == (int)_screenIndex)
            {
                MyScreens[i].SetActive(true);
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
            WebServicesManager.Instance.FetchUser();
        }
        if (_CurrentScreen == Screens.ActionCardRecording)
        {
            //_BPS.gameObject.SetActive(true);
            _masterCardPreview.url = _VideoPlayer._player.url;
            _masterCardPreview.Play();
            _masterCardPreview.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            _BPS.gameObject.SetActive(false);
            _masterCardPreview.transform.parent.gameObject.SetActive(false);
        }
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
    public void Login()
    {
        WebServicesManager.Instance.LoginUser(logInUserName.text, logInPWD.text);
    }
    void OnLoginSuccess(string data)
    {
        Debug.Log("UIController --" + data);

        StartCoroutine(ShowStatus(loginStatusText, "logged in successfully", Screens.ColorSelectionScreen));
    }
    void OnLoginFailed(string data)
    {
        Debug.Log("UIController --" +data);
        StartCoroutine(ShowStatus(loginStatusText, data));
    }
    public void SignUp()
    {
        WebServicesManager.Instance.ResgisterUser(signUpEmail.text ,signUpUserName.text,  sighUpPWD.text);
    }
    void OnSignUpSuccess(string data)
    {
        StartCoroutine(ShowStatus(signUpStatusText, data));
    }
    void OnSignUpFailed(string data)
    {
        StartCoroutine(ShowStatus(signUpStatusText, data));
    }
    public void FetchAllFighters()
    {
        WebServicesManager.Instance.FetchFighter();
    }
    void OnFetchAllFighterComplete(string data)
    {
        Debug.Log("Recieved data " + data);
        List<Fighter> fighters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fighter>>(data);
        Debug.Log("Deserializing done");
        _fighterSelection.Initialize(fighters);
        SetupScreen(Screens.FighterSelection);
    }
    void OnFetchAllFighterFailed(string data)
    {
        Debug.Log(data);
    }

    void OnFetchVideosComplete(string data)
    {
        List<ActionCard> fighters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ActionCard>>(data);
        _VideoSelection.Initialize(fighters,_fighterSelection.SelectefFighter);
        SetupScreen(Screens.Figher_VideoSelection);
    }
    void OnFetchVideosFailed(string data)
    {

    }
    public void BackToVideoSelection()
    {
        SetupScreen(Screens.Figher_VideoSelection);
    }

    public IEnumerator ShowStatus(TMP_Text _element, string _message , Screens _switchTo =Screens.CurrentScreen)
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
    public void OnUploadVideoButtonClick()
    {
        WebServicesManager.Instance.UploadVideos(_VideoSelection.SelectedVideo.sub_type, _actionCardPreview.url, _fighterSelection.SelectefFighter.id, 1);
    }
    public void OnVideoUploadComplete(string data)
    {
        Debug.LogError("Video Uploaded successfully");
    }
    public void OnVideoUplaodFailed(string data)
    {
        Debug.LogError(data);
    }

    public void OnCreateFightStrategyButtonClick()
    {
        SetupScreen(Screens.CreateFightStrategy);
    }
}
