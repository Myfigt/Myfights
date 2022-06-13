using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrategiesVideoItem : MonoBehaviour
{
    public Image titleImage;
    public void Initialize(Sprite _sprite)
    {
        titleImage.sprite = _sprite;
    }
}
