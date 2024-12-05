using UnityEngine;
using System.Collections.Generic;
using System;

namespace Superpow
{
    public class Utils
    {
        public static int GetLeaderboardScore()
        {
            return Prefs.UnlockedGroup;
        }

        public static Puzzle LoadPuzzle(int group, int puzzle, bool ignoreLimit = false)
        {
            var textAsset = Resources.Load("Group " + group + "/Puzzle " + puzzle) as TextAsset;
            if (textAsset == null) return null;

            var lines = textAsset.text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            List<AWord> words = new List<AWord>();
            int length = Mathf.Min(ignoreLimit ? 1000 : ConfigController.Config.maxQuestionsInPuzzle, lines.Length / 2);

            for(int i = 0; i < length; i++)
            {
                AWord word = new AWord() { definition = lines[2 * i], word = lines[2 * i + 1] };
                words.Add(word);
            }

            return new Puzzle() { words = words };
        }

        public static bool IsValidAnswer(string answer, List<string> allKeys)
        {
            for(int i = 0; i < answer.Length; i++)
            {
                if (!allKeys.Contains(answer[i].ToString())) return false;
            }

            return true;
        }
    }
}