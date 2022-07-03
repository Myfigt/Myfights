using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance = null;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataManager>();
            }
            return instance;
        }
    }

    public TextAsset PlayerMatrixText;
    public TextAsset LevelRewardMatrixText;

    public PlayerEconomicMatrix PlayerMatrix;
    public LevelRewardMatrix LevelRewardMatrix;

    public LevelData _activePlayerData;
    public LevelData _requiredPlayerData;
    public const string currentPlayerPref = "CurrentPlayerData";

    void Awake()
    {
        LoadPlayerEconomicMatrix();
        LoadLevelRewardMatrix();
        GetActiveData();
    }

    public LevelData GetActiveData()
    {
        if (PlayerPrefs.HasKey(currentPlayerPref))
        {
            LoadActivePlayerData(PlayerPrefs.GetString(currentPlayerPref));
        }
        else
            _activePlayerData = new LevelData();
        return _activePlayerData;
    }

    public void LoadActivePlayerData(string json)
    {
        _activePlayerData = JsonUtility.FromJson<LevelData>(json);
    }

    public string PackActivePlayerData()
    {
        return JsonUtility.ToJson(_activePlayerData);
    }

    public LevelData GetLevelDetail(int level)
    {
        return PlayerMatrix.GetData(level);
    }

    public RewardData GetRewardDetail(int level)
    {
        return LevelRewardMatrix.GetReward(level);
    }

    public void CalculateRequiredThingsForLevel()
    {
        LevelData nextLevelData = PlayerMatrix.GetData(_activePlayerData.Level + 1);
        _requiredPlayerData = new LevelData();
        _requiredPlayerData.Level = nextLevelData.Level;
        _requiredPlayerData.LevelName = nextLevelData.LevelName;
        _requiredPlayerData.tCard = nextLevelData.tCard - _activePlayerData.tCard;
        _requiredPlayerData.iCard = nextLevelData.iCard - _activePlayerData.iCard;
        _requiredPlayerData.ArmElbow = nextLevelData.ArmElbow - _activePlayerData.ArmElbow;
        _requiredPlayerData.LegKnee = nextLevelData.LegKnee - _activePlayerData.LegKnee;
        _requiredPlayerData.Defense = nextLevelData.Defense - _activePlayerData.Defense;
    }

    public void AddData(LevelDataTypes type, int count)
    {
        _activePlayerData.AddData(type, count);
        CalculateRequiredThingsForLevel();
    }

    [ContextMenu("LoadPlayerEconomicMatrix")]
    public void LoadPlayerEconomicMatrix()
    {
        PlayerMatrix = new PlayerEconomicMatrix();
        string[] Lines = PlayerMatrixText.text.Split(new char[] { '\n','\r' });
        foreach(string line in Lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] values = line.Split(new char[] { ',' });
                    PlayerMatrix.AddLevelData((string)values[0],
                        int.Parse(values[1]),
                        int.Parse(values[2]),
                        int.Parse(values[3]),
                        int.Parse(values[4]),
                        int.Parse(values[5]),
                        int.Parse(values[6]),
                        int.Parse(values[7]));
            }
        }
    }

    [ContextMenu("LoadRewardMatrix")]
    public void LoadLevelRewardMatrix()
    {
        LevelRewardMatrix = new LevelRewardMatrix();
        string[] Lines = LevelRewardMatrixText.text.Split(new char[] { '\n', '\r' });
        foreach (string line in Lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] values = line.Split(new char[] { ',' });
                LevelRewardMatrix.AddRewardData(
                    int.Parse(values[0]),
                    int.Parse(values[1]),
                    int.Parse(values[2]),
                    int.Parse(values[3]),
                    int.Parse(values[4]),
                    int.Parse(values[5]),
                    int.Parse(values[6]),
                    int.Parse(values[7]),
                    int.Parse(values[8]),
                    int.Parse(values[9]),
                    int.Parse(values[10]),
                    float.Parse(values[11]));
            }
        }
    }
}
