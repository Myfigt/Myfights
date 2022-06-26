using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategiesTitleItem : MonoBehaviour
{
    public TMPro.TMP_Text titleText;
    public int index = -1;
    public static System.Action<int> OnClicked;
    public void Initialize(string _title,int _index)
    {
        titleText.text = _title;
        index = _index;
    }

    public void OnClick_Button()
    {
        OnClicked?.Invoke(index);
    }
}
