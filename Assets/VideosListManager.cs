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
    List<Fighter_Videos> MyFighters = null;

    // Start is called before the first frame update
    void Start()
    {

    }
    public void Initialize(List<Fighter_Videos> _allFighters)
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

        MyFighters = _allFighters;
    }
    // Update is called once per frame
    
    public void OnFighterVideoSelected(TMPro.TMP_Text index)
    {
        foreach (var item in MyFighters)
        {
            if (item.FileName == index.text)
            {
                GameObject.FindObjectOfType<UIController>().PlayVideo(item.Path);
                //WebServicesManager.Instance.FetchVideos(item.id, 1);
            }
        }
    }
}
