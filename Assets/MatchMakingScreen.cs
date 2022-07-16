using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMakingScreen : UIScreen
{
    [SerializeField]
    GameObject gameModePanel;
    [SerializeField]
    GameObject matchMakigPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Initialize()
    {
        gameModePanel.SetActive(true);
        matchMakigPanel.SetActive(false);
    }
    public void PlayRandomMatch()
    {
        UIController.Instance._NetworkHandle.JoinRandomRoom();
    }
    public void ShowMatchMakingScreen()
    {
        gameModePanel.SetActive(false);
        matchMakigPanel.SetActive(true);
    }

    public void OnOpponentJoined()
    {

    }
    // Update is called once per frame
}
