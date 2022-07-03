using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class DebugScreen : UIScreen
{
    public TMP_Text PlayerCurrentValue;
    public TMP_Text NextLevel;
    public TMP_Text RequiredForNextLevel;
    public TMP_Text NextLevelRewards;
    public TMP_InputField iCardInput;
    public TMP_InputField tCardInput;
    public TMP_InputField armInput;
    public TMP_InputField legInput;
    public TMP_InputField defenseInput;

    public override void Initialize(params object[] _params)
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        DataManager.Instance.CalculateRequiredThingsForLevel();
        PlayerCurrentValue.text = DataManager.Instance._activePlayerData.ToString();
        NextLevel.text = DataManager.Instance.GetLevelDetail(DataManager.Instance._activePlayerData.Level + 1).ToString();
        NextLevelRewards.text = DataManager.Instance.GetRewardDetail(DataManager.Instance._activePlayerData.Level + 1).ToString();
        RequiredForNextLevel.text = DataManager.Instance._requiredPlayerData.ToString();
    }

    public void OnClick_AddCardPoints()
    {
        DataManager.Instance.AddData(LevelDataTypes.tCards, int.Parse(tCardInput.text));
        DataManager.Instance.AddData(LevelDataTypes.iCards, int.Parse(iCardInput.text));
        UpdateUI();
    }
    public void OnClick_AddCards()
    {
        DataManager.Instance.AddData(LevelDataTypes.ArmElbow, int.Parse(armInput.text));
        DataManager.Instance.AddData(LevelDataTypes.LegKnee, int.Parse(legInput.text));
        DataManager.Instance.AddData(LevelDataTypes.Defense, int.Parse(defenseInput.text));
        UpdateUI();
    }
}