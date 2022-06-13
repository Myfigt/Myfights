using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategiesTitleItem : MonoBehaviour
{
    public TMPro.TMP_Text titleText;
    public void Initialize(string _title)
    {
        titleText.text = _title;
    }
}
