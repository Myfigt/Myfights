using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using System;

public class VideosListManager : UIScreen
{
    [SerializeField]
    GameObject FighterTemplateObject;
    [SerializeField]
    Transform _content;
    List<ActionCard> MyVideos = null;
    [SerializeField]
    TMPro.TMP_Text FighterNameFeild;
    Fighter selectedFighter;
    // Start is called before the first frame update
    public ActionCard SelectedVideo = null;
    public TMPro.TMP_Text loading;
    void Start()
    {

    }
    public override void Goback()
    {
        for (int i = 0; i < _content.childCount; i++)
        {
            if (_content.GetChild(i).gameObject.activeSelf)
            {
                Destroy(_content.GetChild(i).gameObject);
            }
        }
        base.Goback();
    }
    public void Initialize(Fighter _selectedFighter)
    {
        selectedFighter = _selectedFighter;
        for (int i = 0; i < _content.childCount; i++)
        {
            if (_content.GetChild(i).gameObject.activeSelf)
            {
                Destroy(_content.GetChild(i).gameObject);
            }
        }
        loading?.gameObject.SetActive(true);
        VideosContainer.Instance.LoadFighterVideos(_selectedFighter, Handle_VideosLoaded);
    }

    private void Handle_VideosLoaded()
    {
        List<ActionCard> _allFighters = VideosContainer.Instance.GetActionCards(selectedFighter.id);

        for (int i = 0; i < _allFighters.Count; i++)
        {
            GameObject fighter = GameObject.Instantiate(FighterTemplateObject, _content);
            var alltexts = fighter.GetComponentsInChildren<TMPro.TMP_Text>();
            alltexts[0].text = _allFighters[i].FileName;
            alltexts[1].text = _allFighters[i].Belt;
            alltexts[2].text = _allFighters[i].Type;
            fighter.SetActive(true);
        }
        FighterNameFeild.text = selectedFighter.Name;
        MyVideos = _allFighters;
        loading?.gameObject.SetActive(false);
    }

    public void Initialize(List<ActionCard> _allFighters , Fighter _selectedFighter = null)
    {
        for (int i = 0; i < _content.childCount; i++)
        {
            if (_content.GetChild(i).gameObject.activeSelf)
            {
                DestroyImmediate(_content.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < _allFighters.Count; i++)
        {
            GameObject fighter = GameObject.Instantiate(FighterTemplateObject, _content);
            var alltexts = fighter.GetComponentsInChildren<TMPro.TMP_Text>();
            alltexts[0].text = _allFighters[i].FileName;
            alltexts[1].text = _allFighters[i].Belt;
            alltexts[2].text = _allFighters[i].Type;
            fighter.SetActive(true);
        }
        FighterNameFeild.text = _selectedFighter.Name;
        MyVideos = _allFighters;
    }
    // Update is called once per frame
    
    public void OnFighterVideoSelected(TMPro.TMP_Text index)
    {
        foreach (var item in MyVideos)
        {
            if (item.FileName == index.text)
            {
                SelectedVideo = item;
                GameObject.FindObjectOfType<UIController>().PlayVideo(selectedFighter.id,item.id);
            }
        }
    }
}
