using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public enum Screens
    {
        SplashScreen = 1,
        SignUpScreen = 2,
        LoginScreen =3,
        LoadingScreen = 4, 

       
    }
    public GameObject SplashScreen, LoginScreen, LoadingScreen , Mainscreen;
    public GameObject LoginPanel, SignUpPanel;
    public TMP_InputField signUpEmail, signUpUserName, sighUpPWD;
    public TMP_InputField logInUserName, logInPWD ;
    public TMP_Text loginStatusText , signUpStatusText;

    // Start is called before the first frame update
    void OnEnable()
    {
        WebServicesManager.loginUserComplete += OnLoginSuccess;
        WebServicesManager.loginUserFailed += OnLoginFailed;
        WebServicesManager.registerUserComplete += OnSignUpSuccess;
        WebServicesManager.registerUserFailed += OnSignUpFailed;
    }
    void OnDisable()
    {
        WebServicesManager.loginUserComplete -= OnLoginSuccess;
        WebServicesManager.loginUserFailed -= OnLoginFailed;
        WebServicesManager.registerUserComplete -= OnSignUpSuccess;
        WebServicesManager.registerUserFailed -= OnSignUpFailed;
    }
    public void SetupScreen(Screens _screenIndex)
    {
        switch (_screenIndex)
        {
            case Screens.SplashScreen:
                break;
            case Screens.SignUpScreen:
                break;
            case Screens.LoginScreen:
                break;
            case Screens.LoadingScreen:
                break;
            default:
                break;
        }
    }

    public void Login()
    {
        WebServicesManager.Instance.LoginUser(logInUserName.text, logInPWD.text);
    }

    void OnLoginSuccess(string data)
    {
        Debug.Log("UIController --" + data);

        StartCoroutine(ShowStatus(loginStatusText, "logged in successfully"));
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


    public IEnumerator ShowStatus(TMP_Text _element, string _message)
    {
        _element.text = _message;
        _element.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        _element.gameObject.SetActive(false);
    }

}
