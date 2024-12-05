using UnityEngine;
using UnityEditor;

public class SuperpowWindowEditor
{
    [MenuItem("Superpow/Clear all playerprefs")]
    static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("Superpow/Unlock all levels")]
    static void UnlockAllLevel()
    {
        Prefs.UnlockedGroup = Const.TOTAL_GROUP;
        Prefs.UnlockedPuzzle = Const.PUZZLE_IN_GROUP;
    }

    [MenuItem("Superpow/Check valid puzzles")]
    static void CheckValidPuzzles()
    {
        CUtils.ClearConsole();

        for(int i = 1; i <= Const.TOTAL_GROUP; i++)
        {
            Debug.Log("Group " + i);
            for (int j = 1; j <= Const.PUZZLE_IN_GROUP; j++)
            {
                Puzzle puzzle = Superpow.Utils.LoadPuzzle(i, j, true);
                if (puzzle == null)
                {
                    Debug.LogError("Puzzle " + j + " text file doesn't exists");
                    return;
                }

                foreach(var aWord in puzzle.words)
                {
                    if (string.IsNullOrEmpty(aWord.word.Trim()))
                    {
                        Debug.LogError("Puzzle " + j + ": One of the answers is empty");
                        return;
                    }

                    if (string.IsNullOrEmpty(aWord.definition.Trim()))
                    {
                        Debug.LogError("Puzzle " + j + ": One of the clues is empty");
                        return;
                    }

                    if (aWord.word.Trim().Length > 20)
                    {
                        Debug.LogWarning("Puzzle " + j + ": The answer is quite long: " + aWord.word.Trim() + ". Make sure it is correct");
                    }
                }
            }
        }
    }

    [MenuItem("Superpow/Credit balance (ruby, coin..)")]
    static void AddRuby()
    {
        CurrencyController.CreditBalance(1000);
    }

    [MenuItem("Superpow/Set balance to 0")]
    static void SetBalanceZero()
    {
        CurrencyController.SetBalance(0);
    }
}