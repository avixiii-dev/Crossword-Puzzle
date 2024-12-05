using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefs {

    public static int CurrentGroup
    {
        get { return PlayerPrefs.GetInt("current_group", 1); }
        set { PlayerPrefs.SetInt("current_group", value); }
    }

    public static int UnlockedGroup
    {
        get { return PlayerPrefs.GetInt("unlocked_group", 1); }
        set { PlayerPrefs.SetInt("unlocked_group", value); }
    }

    public static int CurrentPuzzle
    {
        get { return PlayerPrefs.GetInt("current_puzzle", 1); }
        set { PlayerPrefs.SetInt("current_puzzle", value); }
    }

    public static int UnlockedPuzzle
    {
        get { return PlayerPrefs.GetInt("unlocked_puzzle", 1); }
        set { PlayerPrefs.SetInt("unlocked_puzzle", value); }
    }

    public static bool IsLastPuzzle()
    {
        return CurrentGroup == UnlockedGroup && CurrentPuzzle == UnlockedPuzzle;
    }

    public static string PuzzleDataKey
    {
        get { return "puzzle_data_" + CurrentGroup + "_" + CurrentPuzzle; }
    }

    public static string[] PuzzleData
    {
        get { return CryptoPlayerPrefsX.GetStringArray(PuzzleDataKey); }
        set { CryptoPlayerPrefsX.SetStringArray(PuzzleDataKey, value); }
    }

    public static void ClearPuzzleData()
    {
        PlayerPrefs.DeleteKey(PuzzleDataKey);
    }

    public static void AddToNumHint(int level)
    {
        int numHint = GetNumHint(level);
        PlayerPrefs.SetInt("numhint_used_" + level, numHint + 1);
    }

    public static int GetNumHint(int level)
    {
        return PlayerPrefs.GetInt("numhint_used_" + level);
    }

    public static void SetPuzzleProgress(int group, int puzzleNumber, float progress)
    {
        PlayerPrefs.SetFloat("progress_" + group + "_" + puzzleNumber, progress);
    }

    public static float GetPuzzleProgress(int group, int puzzleNumber)
    {
        return PlayerPrefs.GetFloat("progress_" + group + "_" + puzzleNumber, 0);
    }

    public static void SetUnlockedPuzzle(int value)
    {
        PlayerPrefs.SetInt("unlocked_puzzle", value);
    }

    public static int GetUnlockedPuzzle()
    {
        return PlayerPrefs.GetInt("unlocked_puzzle", 1);
    }
}
