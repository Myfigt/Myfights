using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using System;

public class CreateStrategyPanel : MonoBehaviour
{
    [SerializeField]
    Color toggleSelectedColor;
    [SerializeField]
    Color toggleUnSelectedColor;
    [SerializeField]
    Button[] toggleButtons;
    [SerializeField]
    GameObject[] combinations;

    FightStrategy _myStrategy;
    int selectedcombination = 0;
    List<ActionCard> MyActionCards = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Initialize(List<ActionCard> _allActionCards)
    {
        MyActionCards = _allActionCards;


    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnToggleButtonClick(int index)
    {
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            if (i == index)
            {
                toggleButtons[i].GetComponent<Image>().color = toggleSelectedColor;
                toggleButtons[i].GetComponentInChildren<TMPro.TMP_Text>().color = toggleUnSelectedColor;
                combinations[i].SetActive(true);
                selectedcombination = i;
            }
            else
            {
                toggleButtons[i].GetComponent<Image>().color = toggleUnSelectedColor;
                toggleButtons[i].GetComponentInChildren<TMPro.TMP_Text>().color = toggleSelectedColor;
                combinations[i].SetActive(false);
            }
        }
    }
}
[Serializable]
public class FightStrategy
{
    public FightCombination[] _Combinations = new FightCombination[3];
}
[Serializable]
public class FightCombination
{
   public  ActionCard[] _ActionCards = new ActionCard[3];

}