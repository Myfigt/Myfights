using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrategiesVideoItem : MonoBehaviour
{
    public Image titleImage;
    public int fighterID;
    public FightStrategy combination;
    public static System.Action<StrategiesVideoItem> OnVideoClicked;
    public void Initialize(Sprite _sprite, int _fighterID ,FightStrategy _combination)
    {
        //titleImage.sprite = _sprite;
        combination = _combination;
        fighterID = _fighterID;
    }
    public void OnClick_VideoItem()
    {
        OnVideoClicked?.Invoke(this);
    }
}
