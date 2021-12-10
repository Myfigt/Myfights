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
    public TMP_InputField logInUserName, logInPWD;

    // Start is called before the first frame update
    public void Awake()
    {
    }
    void Start()
    {
        
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
    public void SignUp()
    {
        WebServicesManager.Instance.ResgisterUser(signUpUserName.text, signUpEmail.text, sighUpPWD.text);
    }
}
