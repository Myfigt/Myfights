using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelDataTypes
{
    tCards,
    iCards,
    ArmElbow,
    LegKnee,
    Defense
}

public class LevelData
{
    public string LevelName;
    public int Level;
    public int ActionCards;
    public int tCard;
    public int iCard;
    public int ArmElbow;
    public int LegKnee;
    public int Defense;

    public LevelData()
    {
        LevelName = "NA";
        Level = 0;
        ActionCards = 0;
        tCard = 0;
        iCard = 0;
        ArmElbow = 0;
        LegKnee = 0;
        Defense = 0;
    }

    public LevelData(string _LevelName, int _Level, int _ActionCards,int _tCard,int _iCard,int _armElbow,int _legKnee,int _defense)
    {
        LevelName = _LevelName;
        Level = _Level;
        ActionCards = _ActionCards;
        tCard = _tCard;
        iCard = _iCard;
        ArmElbow = _armElbow;
        LegKnee = _legKnee;
        Defense = _defense;
    }
    public void AddData(LevelDataTypes type,int count)
    {
        switch(type)
        {
            case LevelDataTypes.tCards:
                tCard += count;
                break;
            case LevelDataTypes.iCards:
                iCard += count;
                break;
            case LevelDataTypes.ArmElbow:
                ArmElbow += count;
                ActionCards += count;
                break;
            case LevelDataTypes.LegKnee:
                LegKnee += count;
                ActionCards += count;
                break;
            case LevelDataTypes.Defense:
                Defense += count;
                ActionCards += count;
                break;
        }
    }
    public bool isCompleted
    {
        get
        {
            return !(tCard > 0 || iCard > 0 || ArmElbow > 0 || LegKnee > 0 || Defense > 0);
        }
    }
    public override string ToString()
    {
        return $"Belt Name : {LevelName} : Level : {Level} " +
            $": Action Cards : {ActionCards} : tCards : {tCard}" +
            $": iCard : {iCard} : ArmElbow : {ArmElbow} " +
            $": LegKnee : {LegKnee} : Defense : {Defense}";
    }
}
public class PlayerEconomicMatrix
{
    List<LevelData> levelDatas = new List<LevelData>();

    public void AddLevelData(string _LevelName, int _Level, int _ActionCards, int _tCard, int _iCard, int _armElbow, int _legKnee, int _defense)
    {
        levelDatas.Add(new LevelData(_LevelName, _Level, _ActionCards, _tCard, _iCard, _armElbow, _legKnee, _defense));
    }

    public LevelData GetData(string belt)
    {
        return levelDatas.Find(item => item.LevelName.ToLower().Equals(belt.ToLower()));
    }

    public LevelData GetData(int level)
    {
        return levelDatas.Find(item => item.Level == level);
    }
}
