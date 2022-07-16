using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TribesManager : UIScreen
{

    [SerializeField]
    Tribe _tribeTamplate;
    [SerializeField]
    GameObject createTribePanel;
    [SerializeField]
    Transform _content;
    [SerializeField]
    TMP_InputField _newTribeName;


    List<Tribe> AllTribes = null;

    public void Initialize(List<Tribe> _allTribes = null)
    {
        string[] tribes = DataManager.Instance.LoadTribesData();
        for (int i = 0; i < _content.childCount; i++)
        {
            if (_content.GetChild(i).gameObject.activeSelf)
            {
                DestroyImmediate(_content.GetChild(i).gameObject);
            }
        }
        for (int i = 1; i < tribes.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(tribes[i]))
            {
                string[] values = tribes[i].Split(new char[] { ',' });

                Tribe _tribe = GameObject.Instantiate(_tribeTamplate, _content);
                TMPro.TMP_Text[] _alltexts =  _tribe.transform.GetComponentsInChildren<TMPro.TMP_Text>();
                _alltexts[0].text = values[3];
                _alltexts[1].text = values[4];
                if (ColorUtility.TryParseHtmlString("#"+values[1], out var newCol))
                {
                    if (_tribe.TryGetComponent<Image>(out var _image))
                    {
                        _image.color = newCol;
                    }
                }
                else
                    Debug.Log("can not convert color " + values[1]);
                
               
                _tribe.gameObject.SetActive(true);
            }
        }

        AllTribes = _allTribes;
        createTribePanel.SetActive(false);
    }
    private void OnEnable()
    {
        WebServicesManager.CreateTribeComplete += WebServicesManager_CreateTribeComplete;
        WebServicesManager.CreateTribeFailed += WebServicesManager_CreateTribeFailed;
    }

    private void WebServicesManager_CreateTribeFailed(string error)
    {
        throw new System.NotImplementedException();
    }

    private void WebServicesManager_CreateTribeComplete(string responce)
    {
        
        Tribe _newTribes = Newtonsoft.Json.JsonConvert.DeserializeObject<Tribe>(responce);
       
            Tribe _tribe = GameObject.Instantiate(_tribeTamplate, _content);
            _tribe.transform.GetComponentInChildren<TMPro.TMP_Text>().text = _newTribes.name;
            _tribe.gameObject.SetActive(true);
        CloseCreateTribePanel();
        Debug.Log(responce);
    }

    private void OnDisable()
    {
        WebServicesManager.CreateTribeComplete -= WebServicesManager_CreateTribeComplete;
        WebServicesManager.CreateTribeFailed -= WebServicesManager_CreateTribeFailed;
    }
    public void OnCreateTribeButtonClick()
    {
        createTribePanel.SetActive(true);
    }
    public void CloseCreateTribePanel()
    {
        createTribePanel.SetActive(false);
    }

    public void CreateTribe()
    {
        WebServicesManager.Instance.CreateTribe(_newTribeName.text, UIController.Instance._myprofile.id.ToString());
    }
}
