using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProfileViewer : UIScreen
{
    [SerializeField]
    TMP_Text _usernameText;
    [SerializeField]
    TMP_Text _beltText;
    [SerializeField]
    TMP_Text _matchesWonText;
    [SerializeField]
    TMP_Text _matchesPlayedText;
    [SerializeField]
    TMP_Text _matcheslostText;

    // Start is called before the first frame update
    public void SetProfileData(UserProfile _profile)
    {
        _usernameText.text = _profile.name;
        _beltText.text = _profile.belt_type.ToString();
    }
}
