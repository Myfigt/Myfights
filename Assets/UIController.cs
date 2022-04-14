using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts;
using System;
using UnityEngine.Video;

public class UIController : MonoBehaviour
{
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
                SetupScreen(Screens.LoginScreen);
                
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
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
        List<Fighter_Videos> fighters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fighter_Videos>>(data);
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
}
