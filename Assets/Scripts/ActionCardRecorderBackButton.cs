using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCardRecorderBackButton : MonoBehaviour
{
    [SerializeField] GameObject videoPlayerScreen;
    [SerializeField] GameObject actionCardRecoderScreen;
    [SerializeField] GameObject recordActionCard;


    public void GoBack()
    {
        videoPlayerScreen.SetActive(true);
        actionCardRecoderScreen.SetActive(false);
        recordActionCard.SetActive(false);
    }
}
