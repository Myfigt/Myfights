using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetsFightScreen : UIScreen
{
    public GameObject LoadingText;
    List<Fighter> currentFighters;
    public void Initialize(List<Fighter> _allFighters)
    {
        LoadingText.SetActive(true);
        currentFighters = _allFighters;
        VideosContainer.Instance.LoadAllFighterVideos(_allFighters, Handle_VideosLoaded);
    }

    private void Handle_VideosLoaded()
    {
        LoadingText.SetActive(false);
    }
}
