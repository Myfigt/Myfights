using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class FighterListManager : MonoBehaviour
{
    [SerializeField]
    GameObject FighterTemplateObject;
    [SerializeField]
    Transform _content;
    List<Fighter> MyFighters = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Initialize(List<Fighter> _allFighters)
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
            fighter.GetComponentInChildren<TMPro.TMP_Text>().text = _allFighters[i].Name;

            //fighter.GetComponent<Button>().onClick.AddListener(() => OnFighterSelected(i));
            fighter.SetActive(true);
        }
       
        MyFighters = _allFighters;
    }
    // Update is called once per frame
    public void OnFighterSelectionChanged(Vector2 vector)
    {

    }
    public void OnFighterSelected(TMPro.TMP_Text index)
    {
        foreach (var item in MyFighters)
        {
            if (item.Name == index.text)
            {
                WebServicesManager.Instance.FetchVideos(item.id, 1);
            }
        }
    }
}
