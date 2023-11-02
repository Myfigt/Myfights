using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour 
{
    [SerializeField]
    UIController.Screens BackScreen;
    public virtual void Initialize(params object[] _params)
    {
        
    }
    public virtual void Goback()
    {
        UIController.Instance.SetupScreen(this.BackScreen);
    }

    public virtual void SetGobackScreen(UIController.Screens _screen)
    {
        BackScreen = _screen;
    }
    public virtual UIController.Screens GetGobackScreen()
    {
         return BackScreen;
    }
}
