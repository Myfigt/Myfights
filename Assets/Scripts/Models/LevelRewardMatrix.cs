using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardData
{
    public int ActionCards;
    public int Level;
    public int tCard;
    public int iCard;
    public int TurnValue;
    public int RewardTurn;
    public int Box;
    public int WinBattle;
    public int Milestone;
    public int TRM;
    public int LossBattle;
    public float Price;

    public RewardData(int _ActionCards, int _Level, int _tCard, int _iCard, int _TurnValue, int _RewardTurn, int _Box, int _WinBattle, int _Milestone, int _TRM, int _LossBattle, float _Price)
    {
        ActionCards = _ActionCards;
        Level = _Level;
        tCard = _tCard;
        iCard = _iCard;
        TurnValue = _TurnValue;
        RewardTurn = _RewardTurn;
        Box = _Box;
        WinBattle = _WinBattle;
        Milestone = _Milestone;
        TRM = _TRM;
        LossBattle = _LossBattle;
        Price = _Price;
    }

    public override string ToString()
    {
        return $"Action Cards : {ActionCards} : Level : {Level} : " +
            $"tCard : {tCard} : iCard : {iCard} : Turn Value : {TurnValue} : " +
            $"Reward Turn : {RewardTurn} : Box : {Box} :" +
            $"Win Battle : {WinBattle} : Milestone : {Milestone} :" +
            $"TRM : {TRM} : Loss Battle : {LossBattle} : Price : {Price}";
    }
}
public class LevelRewardMatrix
{
    List<RewardData> levelRewardData = new List<RewardData>();

    public void AddRewardData(int _ActionCards, int _Level, int _tCard, int _iCard, int _TurnValue, int _RewardTurn, int _Box, int _WinBattle, int _Milestone, int _TRM, int _LossBattle, float _Price)
    {
        levelRewardData.Add(new RewardData(_ActionCards, _Level, _tCard, _iCard, _TurnValue, _RewardTurn, _Box, _WinBattle, _Milestone, _TRM, _LossBattle, _Price));
    }

    public RewardData GetReward(int level)
    {
        return levelRewardData.Find(item => item.Level == level);
    }
}
