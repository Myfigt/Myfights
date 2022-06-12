using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public void Initialize(List<Tribe> _allTribes)
    {
        for (int i = 0; i < _content.childCount; i++)
        {
            if (_content.GetChild(i).gameObject.activeSelf)
            {
                DestroyImmediate(_content.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < _allTribes.Count; i++)
        {
            Tribe _tribe = GameObject.Instantiate(_tribeTamplate, _content);
            _tribe.transform.GetComponentInChildren<TMPro.TMP_Text>().text = _allTribes[i].name;
            _tribe.gameObject.SetActive(true);
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
