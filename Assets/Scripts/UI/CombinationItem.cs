using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombinationItem : MonoBehaviour
{
    public Image titleImage;
    public int fighterID;
    public FightCombination[] combination;
    public static System.Action<CombinationItem> OnCombinationClicked;
    public void Initialize(Sprite _sprite, int _fighterID, FightCombination[] _combinations)
    {
        //titleImage.sprite = _sprite;
        combination = _combinations;
        fighterID = _fighterID;
    }
    public void OnClick_CombinationItem(CombinationItem item)
    {
        OnCombinationClicked?.Invoke(this);
    }
}
