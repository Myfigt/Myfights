using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using System;

public class CreateStrategyPanel : UIScreen
{
    [SerializeField]
    Color toggleSelectedColor;
    [SerializeField]
    Color toggleUnSelectedColor;
    [SerializeField]
    Button[] toggleButtons;
    [SerializeField]
    GameObject[] combinations;

    int selectedcombination = 0;
    List<ActionCard> MyActionCards = null;
    FightStrategy _mystrategy = null;
    public Transform allActionCardContent;
    public Transform[] combinationContent;
    public GameObject ActionCardTamplete;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    //public override void Initialize(params object[] _params)
    //{
    //    Initializeer((List<ActionCard>)_params[0], (FightStrategy)_params[1]);
    //}
    public void Initialize(List<ActionCard> _allActionCards, FightStrategy  _strategy)
    {
        MyActionCards = _allActionCards;
        _mystrategy = _strategy;
        // Setting Action Card UI 
        for (int i = 0; i < allActionCardContent.childCount; i++)
        {
            if (allActionCardContent.GetChild(i).gameObject.activeSelf)
            {
                DestroyImmediate(allActionCardContent.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < MyActionCards.Count; i++)
        {
            GameObject Card = GameObject.Instantiate(ActionCardTamplete, allActionCardContent);
            var alltexts = Card.GetComponentsInChildren<TMPro.TMP_Text>();
            alltexts[0].text = MyActionCards[i].FileName;
            alltexts[1].text = MyActionCards[i].Belt;
            alltexts[2].text = MyActionCards[i].Type;
            Card.transform.GetChild(5).gameObject.SetActive(false);
            Card.SetActive(true);
        }

        // setting combination UI
        for (int i = 0; i < _strategy._Combinations.Length; i++)
        {
            if (_strategy._Combinations[i] == null)
                combinationContent[i].GetChild(0).gameObject.SetActive(false);
            else
            combinationContent[i].GetChild(0).gameObject.SetActive(true);

            combinationContent[i].GetChild(0).GetChild(4).gameObject.SetActive(false);
        }
        OnToggleButtonClick(0);
    }
    public void OnToggleButtonClick(int index)
    {
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            if (i == index)
            {
                if (toggleButtons[i] != null)
                {
                    toggleButtons[i].GetComponent<Image>().color = toggleSelectedColor;
                    toggleButtons[i].GetComponentInChildren<TMPro.TMP_Text>().color = toggleUnSelectedColor;
                    combinations[i].SetActive(true);
                    selectedcombination = i;
                }
               
            }
            else
            {
                if (toggleButtons[i] != null)
                {
                    toggleButtons[i].GetComponent<Image>().color = toggleUnSelectedColor;
                    toggleButtons[i].GetComponentInChildren<TMPro.TMP_Text>().color = toggleSelectedColor;
                    combinations[i].SetActive(false);
                }
            }
        }
    }
    public void OnActionCardTap(GameObject rb)
    {
        rb.SetActive(!rb.activeSelf);
    }
    public void RemoveActionCardFromCombination(int index)
    {
        _mystrategy._Combinations[index] = null;
        combinationContent[index].GetChild(0).gameObject.SetActive(false);
    }
    public void OnActionCardSelected(GameObject ub)
    {
        if (CheckAvailableSlot() != -1)
        {
            ub.SetActive(!ub.activeSelf);
        }
    }
    public void AddToCombination(TMPro.TMP_Text index)
    {

        int i = CheckAvailableSlot();
        foreach (var item in MyActionCards)
        {
            if (item.FileName == index.text)
            {
                _mystrategy._Combinations[i] = new FightCombination();
                _mystrategy._Combinations[i].id = item.id;
                _mystrategy._Combinations[i].player = UIController.Instance._myprofile.id;
                _mystrategy._Combinations[i].combination = selectedcombination;
                _mystrategy._Combinations[i].player_video_url = item.Path;
                _mystrategy._Combinations[i].status = item.sub_type;
                _mystrategy._Combinations[i].created_by = UIController.Instance._myprofile.id;
                _mystrategy._Combinations[i].created_at = DateTime.Now;
                _mystrategy._Combinations[i].updated_by = UIController.Instance._myprofile.id;
                _mystrategy._Combinations[i].updated_at = DateTime.Now;
            }
        }

        combinationContent[i].GetChild(0).gameObject.SetActive(true);
        combinationContent[i].GetChild(0).GetChild(4).gameObject.SetActive(false);
    }

    int CheckAvailableSlot()
    {
        for (int i = selectedcombination * 3; i < selectedcombination * 3 + 3; i++)
        {
            if (_mystrategy._Combinations[i] == null)
            {
                return i;
            }
        }
       
        return -1;
    }
}
[Serializable]
public class FightStrategy
{
    public string title;
    public int id;
    public DateTime created_at;
    public FightCombination[] _Combinations;
}
[Serializable]
public class FightCombination
{
    public int id;
    public int player;
    public int combination;
    public string player_video_url;
    public string status;
    public int created_by;
    public DateTime created_at;
    public int updated_by;
    public DateTime updated_at;
}