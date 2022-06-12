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
    public void Goback()
    {
        UIController.Instance.SetupScreen(this.BackScreen);
    }
}
