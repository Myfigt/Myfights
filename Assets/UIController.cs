using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts;
using System;

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
    // Start is called before the first frame update
    private void Start()
    {
        //SetupScreen(Screens.HomeScreen);
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
    }

    internal void PlayVideo(string path)
    {
        _VideoPlayer.Initialize(path);
        SetupScreen(Screens.VideoPlayerScreen);
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
    }
    public void SetupScreen(Screens _screenIndex = Screens.CurrentScreen)
    {
        
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
        List<Fighter> fighters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fighter>>(data);
        _fighterSelection.Initialize(fighters);
        SetupScreen(Screens.FighterSelection);
    }
    void OnFetchAllFighterFailed(string data)
    {

    }

    void OnFetchVideosComplete(string data)
    {
        List<Fighter_Videos> fighters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Fighter_Videos>>(data);
        _VideoSelection.Initialize(fighters);
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

}
