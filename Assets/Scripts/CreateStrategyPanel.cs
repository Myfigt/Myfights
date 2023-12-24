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
    public GameObject WaitingPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        WebServicesManager.CreateStrategyComplete += WebServicesManager_CreateStrategyComplete; ;
        WebServicesManager.CreateStrategyFailed += WebServicesManager_CreateStrategyFailed;
        WaitingPanel.SetActive(false);

    }
    private void OnDisable()
    {
        WebServicesManager.CreateStrategyComplete -= WebServicesManager_CreateStrategyComplete; ;
        WebServicesManager.CreateStrategyFailed -= WebServicesManager_CreateStrategyFailed; 
    }

    private void WebServicesManager_CreateStrategyFailed(string error)
    {
        WaitingPanel.SetActive(false);
    }

    private void WebServicesManager_CreateStrategyComplete(string responce)
    {
        FightStrategy strategy = null;
        try
        {
            strategy = Newtonsoft.Json.JsonConvert.DeserializeObject<FightStrategy>(responce);
            Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(responce);
            foreach (DictionaryEntry item in responceData)
            {
                if (item.Key.ToString() == "combinations")
                {
                    strategy.combinations = new FightCombination[9];
                    int i = 0;
                    foreach (var res in item.Value as ArrayList)
                    {

                        string combos = easy.JSON.JsonEncode(res);
                        strategy.combinations[i] = Newtonsoft.Json.JsonConvert.DeserializeObject<FightCombination>(combos);
                        i++;
                        if (i >= 9)
                        {
                            break;
                        }
                    }

                }
            }
        }
        catch (Exception)
        {
            UnityEngine.Debug.Log("unable to parse fight strategy");
        }
        if (strategy != null)
        {
            UIController.Instance._myprofile._myStrategy = strategy;
        }
     
        Initialize(UIController.Instance._myprofile._allActionCards, UIController.Instance._myprofile._myStrategy);
        WaitingPanel.SetActive(false);
    }

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
        for (int i = 0; i < _strategy.combinations.Length; i++)
        {
            if (_strategy.combinations[i] == null)
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
        _mystrategy.combinations[index] = null;
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
                _mystrategy.combinations[i] = new FightCombination() { ActionCardID = item.id ,combination = selectedcombination , player_video_url = item.Path , status = item.sub_type , created_by = _mystrategy.playerID };
            }
        }

        combinationContent[i].GetChild(0).gameObject.SetActive(true);
        combinationContent[i].GetChild(0).GetChild(4).gameObject.SetActive(false);
    }
    public override void Goback()
    {
        base.Goback();
        UIController.Instance._myprofile._myStrategy = _mystrategy;
    }
    int CheckAvailableSlot()
    {
        for (int i = selectedcombination * 3; i < selectedcombination * 3 + 3; i++)
        {
            if (_mystrategy.combinations[i] == null)
            {
                return i;
            }
        }
       
        return -1;
    }

    public void SaveStrategy()
    {
        WaitingPanel.SetActive(true);
        WebServicesManager.Instance.CreateStrategy( _mystrategy);
    }
}
[Serializable]
public class FightStrategy
{
    public string title;
    public int id;
    public int playerID;
    public string player_name;
    public DateTime created_at;
    public FightCombination[] combinations;
}
[Serializable]
public class FightCombination
{
    public int ActionCardID;
    public int combination;
    public string player_video_url;
    public string status;
    public int created_by;
    public DateTime created_at;
    public int updated_by;
    public DateTime updated_at;
}