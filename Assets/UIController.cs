using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    public void Awake()
    {
    }
    void Start()
    {
        
    }
    public void SetupScreen(int _screenIndex)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
