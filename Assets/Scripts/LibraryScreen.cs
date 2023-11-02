using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryScreen : UIScreen
{

    public Transform allActionCardContent;
    List<ActionCard> _myActionCards;
    public GameObject ActionCardTamplete;
    public void Initialize(List<ActionCard> _allActionCards)
    {
        _myActionCards = new List<ActionCard>();
        _myActionCards = _allActionCards;
        for (int i = 0; i < allActionCardContent.childCount; i++)
        {
            if (allActionCardContent.GetChild(i).gameObject.activeSelf)
            {
                DestroyImmediate(allActionCardContent.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < _allActionCards.Count; i++)
        {
            GameObject Card = GameObject.Instantiate(ActionCardTamplete, allActionCardContent);
            var alltexts = Card.GetComponentsInChildren<TMPro.TMP_Text>();
            alltexts[0].text = _allActionCards[i].FileName;
            alltexts[1].text = _allActionCards[i].Belt;
            alltexts[2].text = _allActionCards[i].Type;
            Card.transform.GetChild(4).gameObject.SetActive(false);
            Card.SetActive(true);
        }
    }
    public void OnActionCardSelected(TMPro.TMP_Text index)
    {
        foreach (var item in _myActionCards)
        {
            if (item.FileName == index.text)
            {
                UIController.Instance.ViewActionCard(item);
               // GameObject.FindObjectOfType<UIController>().PlayVideo(item.Path);
            }
        }
    }
}
