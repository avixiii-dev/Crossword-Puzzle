using UnityEngine;
using System;

[System.Serializable]
public class GameConfig
{
    public Admob admob;

    [Header("")]
    public int adPeriod;
    public int rewardedVideoPeriod;
    public int rewardedVideoAmount;
    public string androidPackageID;
    public string iosAppID;
    public string macAppID;
    public string facebookPageID;

    [Header("")]
    public int showLetterCost;
    public int maxQuestionsInPuzzle;
}

[System.Serializable]
public class Admob
{
    //
}
