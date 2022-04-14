using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;


public class VideosListManager : MonoBehaviour
{
    [SerializeField]
    GameObject FighterTemplateObject;
    [SerializeField]
    Transform _content;
    List<Fighter_Videos> MyVideos = null;
    [SerializeField]
    TMPro.TMP_Text FighterNameFeild;
    // Start is called before the first frame update
    public Fighter_Videos SelectedVideo = null;
    void Start()
    {

    }
    public void Initialize(List<Fighter_Videos> _allFighters , Fighter _selectedFighter = null)
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
                GameObject.FindObjectOfType<UIController>().PlayVideo(item.Path);
            }
        }
    }
}
