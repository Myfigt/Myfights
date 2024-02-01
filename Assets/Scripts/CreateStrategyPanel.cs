using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using System;
using DG.Tweening;

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
    [SerializeField]
    TMPro.TMP_Text selectedStrategyText;

    int selectedcombination = 0;
    List<ActionCard> MyActionCards = null;
    FightCombo _myCombo = null;
    public Transform allActionCardContent;
    public Transform[] strategiesContent;
    public GameObject ActionCardTamplete;
    public GameObject WaitingPanel;

    int selectedTabIndex;

    ActionCard selectedActionCard = null;
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
        FightCombo newCombo = null;
        try
        {
            newCombo = Newtonsoft.Json.JsonConvert.DeserializeObject<FightCombo>(responce);
            Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(responce);
            foreach (DictionaryEntry item in responceData)
            {
                if (item.Key.ToString() == "strategies")
                {
                    newCombo.strategies = new List<FightStrategy>();
                    int i = 0;
                    foreach (var res in item.Value as ArrayList)
                    {

                        string combos = easy.JSON.JsonEncode(res);
                        newCombo.strategies.Add( Newtonsoft.Json.JsonConvert.DeserializeObject<FightStrategy>(combos));
                        i++;
                    }

                }
            }
        }
        catch (Exception)
        {
            UnityEngine.Debug.Log("unable to parse fight strategy");
        }
        if (newCombo != null)
        {
            UIController.Instance._myprofile._myCombo = newCombo;
        }
     
        Initialize(UIController.Instance._myprofile._allActionCards, UIController.Instance._myprofile._myCombo);
        WaitingPanel.SetActive(false);
    }

    public void Initialize(List<ActionCard> _allActionCards, FightCombo  _combo)
    {
        MyActionCards = _allActionCards;
        _myCombo = _combo;
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
        UpdateStrategiesTab();
        UpdateStrategyData(true);
        OnToggleButtonClick(0);
    }
    public void OnToggleButtonClick(int index)
    {
        if (index >= _myCombo.strategies.Count)
            return;
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
        selectedTabIndex = index;
        selectedStrategyText.text = "Strategy " + (index + 1);
    }
    public void OnActionCardTap(GameObject rb)
    {
        if (selectedActionCard != null)
        {
            // relace action card
            _myCombo.strategies[selectedTabIndex].actionCards[rb.transform.parent.parent.GetSiblingIndex()] = selectedActionCard;
            UpdateStrategyData();
            selectedActionCard = null;
           // StopAllCoroutines();
        }
        else
        rb.SetActive(!rb.activeSelf);
    }
    public void RemoveActionCardFromCombination(int index)
    {
        _myCombo.strategies[selectedTabIndex].actionCards[index] = null;
        strategiesContent[selectedTabIndex*3+index].GetChild(0).gameObject.SetActive(false);
    }
    public void OnActionCardSelected(GameObject ub)
    {
        //if (CheckAvailableSlot() != -1)
        //{
            ub.SetActive(!ub.activeSelf);
       // }
    }
    public void OnUseButtonClick(TMPro.TMP_Text index)
    {

        foreach (var item in MyActionCards)
        {
            if (item.FileName == index.text)
            {
                selectedActionCard = item;
            }
        }

        StartCoroutine(KeepShaking(combinations[selectedTabIndex].transform.GetChild(0)));
        StartCoroutine(KeepShaking(combinations[selectedTabIndex].transform.GetChild(1)));
        StartCoroutine(KeepShaking(combinations[selectedTabIndex].transform.GetChild(2)));
        index.transform.parent.GetChild(5).gameObject.SetActive(false);
        // strategiesContent[i].GetChild(0).gameObject.SetActive(true);
        // strategiesContent[i].GetChild(0).GetChild(4).gameObject.SetActive(false);
    }
    IEnumerator KeepShaking(Transform T)
    {
        float puchStrength = .01f;
        while (selectedActionCard != null)
        {
            T.DOPunchScale(new Vector3(puchStrength, puchStrength, puchStrength),2,1);
            yield return new WaitForSeconds(1);
        }
    }
    public override void Goback()
    {
        base.Goback();
        UIController.Instance._myprofile._myCombo = _myCombo;
    }
    int CheckAvailableSlot()
    {
        for (int i = selectedcombination * 3; i < selectedcombination * 3 + 3; i++)
        {
            if (_myCombo.strategies[i] == null)
            {
                return i;
            }
        }
       
        return -1;
    }
    void UpdateStrategiesTab()
    {


        for (int i = 0; i < combinations.Length; i++)
        {
            if (i < _myCombo.strategies.Count)
            {
                //combinations[i].gameObject.SetActive(true);
                toggleButtons[i].gameObject.SetActive(true);
            }
            else
            {
                combinations[i].gameObject.SetActive(false);
                toggleButtons[i].gameObject.SetActive(false);
            }
                

        }
    }
    void UpdateStrategyData(bool isINitializing = false)
    {
        for (int i = 0; i < _myCombo.strategies.Count; i++)
        {
            for (int j = 0; j < _myCombo.strategies[i].actionCards.Length; j++)
            {
                if (_myCombo.strategies[i].actionCards[j] == null || _myCombo.strategies[i].actionCards[j].id ==0)
                    combinations[i].transform.GetChild(j).GetChild(0).gameObject.SetActive(false);
                else
                {
                    combinations[i].transform.GetChild(j).GetChild(0).GetChild(1).gameObject.GetComponent<TMPro.TMP_Text>().text = _myCombo.strategies[i].actionCards[j].fighter_video_id.ToString();

                    combinations[i].transform.GetChild(j).GetChild(0).GetChild(2).GetComponent<TMPro.TMP_Text>().text = _myCombo.strategies[i].actionCards[j].result.ToString();

                    combinations[i].transform.GetChild(j).GetChild(0).GetChild(3).GetComponent<TMPro.TMP_Text>().text = _myCombo.strategies[i].actionCards[j].id.ToString();

                    combinations[i].transform.GetChild(j).GetChild(0).gameObject.SetActive(true);
                }
               

                if (isINitializing)
                {
                    combinations[i].transform.GetChild(j).GetChild(0).GetChild(4).gameObject.SetActive(false);//disabling remove button on initialize
                }
            }


             
        }
    }
    public void AddNewStrategy()
    {
        if (_myCombo.strategies.Count<5)
        {
            _myCombo.strategies.Add(new FightStrategy() { actionCards = new ActionCard[3], combinationIndex = _myCombo.strategies.Count });
            UpdateStrategiesTab();
            UpdateStrategyData();
            OnToggleButtonClick(_myCombo.strategies.Count-1);
        }
        else
        {
            UIController.Instance.ShowPopUp("strategies Limit reached");
        }
       
    }
    public void SaveStrategy()
    {
        WaitingPanel.SetActive(true);
        //for (int i = 0; i < 5; i++)
        //{
        //    ActionCard[] temp = new ActionCard[3];
        //    for (int j = 0; j < temp.Length; j++)
        //    {
        //        temp[j] = MyActionCards[j];
        //    }
        //    _myCombo.strategies.Add(new FightStrategy() { actionCards = temp, combinationIndex = i });
        //}
        WebServicesManager.Instance.CreateStrategy( _myCombo);
    }
}
[Serializable]
public class FightCombo
{
    public string title;
    public int id;
    public int playerID;
    public string player_name;
    public DateTime created_at;
    public List <FightStrategy> strategies;
}
[Serializable]
public class FightStrategy
{
    public int combinationIndex;
    public ActionCard[] actionCards;

}