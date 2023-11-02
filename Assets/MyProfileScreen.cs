using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MyProfileScreen : UIScreen
{
    [SerializeField]
    TMPro.TMP_Text[] info;
    public override void Initialize(params object[] _params)
    {
        base.Initialize(_params);
        info[0].text = UIController.Instance._myprofile.id.ToString();
        info[1].text = UIController.Instance._myprofile.name.ToString();
        info[2].text = UIController.Instance._myprofile.email.ToString();
        info[3].text = UIController.Instance._myprofile.belt_type.ToString();
        info[4].text = UIController.Instance._myprofile.created_at.ToString();
    }
}
