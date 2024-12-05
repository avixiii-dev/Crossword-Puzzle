using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CrosswordGenerator {

    public List<string> words;
    public List<string> _words;
    private List<string> boardWords;
    public List<CrossedWord> crossedWords;
    private Dictionary<Position, char> boardLetters;

    private int getIndex;

    public void SetWords(List<string> _words)
    {
        this._words = _words;
    }

    public List<CrossedWord> Generate()
    {
        var bestCrossedWords = new List<CrossedWord>();

        for(int i = 0; i < _words.Count; i++)
        {
            string firstWord = _words[i];
            words = new List<string>();
            words.AddRange(_words);
            words.RemoveAt(i);
            words.Insert(0, firstWord);


            boardWords = new List<string>();
            crossedWords = new List<CrossedWord>();
            boardLetters = new Dictionary<Position, char>();

            string newWord = null;
            getIndex = 0;

            while ((newWord = GetNewWord()) != null)
            {
                bool result = InsertWordToBoard(newWord);
                if (result) getIndex = 0;
                else
                {
                    getIndex = words.IndexOf(newWord);
                    getIndex++;
                }
            }

            if (crossedWords.Count > bestCrossedWords.Count)
            {
                bestCrossedWords = crossedWords;
                if (bestCrossedWords.Count == _words.Count)
                {
                    return bestCrossedWords;
                }
            }
        }

        return bestCrossedWords;
    }

    private string GetNewWord()
    {
        for(int i = getIndex; i < words.Count; i++)
        {
            if (!boardWords.Contains(words[i]))
            {
                return words[i];
            }
        }
        return null;
    }

    private List<PairInt> GetIntersections(string newWord, string boardWord)
    {
        List<PairInt> result = new List<PairInt>();
        for(int i = 0; i < newWord.Length; i++)
        {
            for(int j = 0; j < boardWord.Length; j++)
            {
                if (newWord[i] == boardWord[j])
                {
                    result.Add(new PairInt() { Value1 = j, Value2 = i });
                }
            }
        }
        return result;
    }

    private bool CheckValidPosition(string word, bool horizontal, Position position)
    {
        for(int i = 0; i < word.Length; i++)
        {
            var letterPosition = horizontal ? new Position() { x = position.x + i, y = position.y } : 
                                              new Position() { x = position.x, y = position.y - i };

            if (boardLetters.ContainsKey(letterPosition) && boardLetters[letterPosition] != word[i])
            {
                return false;
            }

            var left = new Position() { x = letterPosition.x - 1, y = letterPosition.y };
            var right = new Position() { x = letterPosition.x + 1, y = letterPosition.y };
            var up = new Position() { x = letterPosition.x, y = letterPosition.y + 1 };
            var down = new Position() { x = letterPosition.x, y = letterPosition.y - 1 };

            if (horizontal)
            {
                if (i == 0 && boardLetters.ContainsKey(left)) return false;
                if (i == word.Length - 1 && boardLetters.ContainsKey(right)) return false;
                if (!boardLetters.ContainsKey(letterPosition))
                {
                    if (boardLetters.ContainsKey(up) || boardLetters.ContainsKey(down)) return false;
                }
            }
            else
            {
                if (i == 0 && boardLetters.ContainsKey(up)) return false;
                if (i == word.Length - 1 && boardLetters.ContainsKey(down)) return false;
                if (!boardLetters.ContainsKey(letterPosition))
                {
                    if (boardLetters.ContainsKey(left) || boardLetters.ContainsKey(right)) return false;
                }
            }
        }

        return true;
    }

    private int GetScore(string word, bool horizontal, Position position)
    {
        int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
        if (horizontal)
        {
            minX = position.x;
            maxX = position.x + word.Length;
        }
        else
        {
            maxY = position.y;
            minY = position.y - word.Length;
        }

        foreach(var crossedWord in crossedWords)
        {
            var pos = crossedWord.position;

            if (crossedWord.horizontal)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.x + crossedWord.word.Length > maxX) maxX = pos.x + crossedWord.word.Length;
            }
            else
            {
                if (pos.y > maxY) maxY = pos.y;
                if (pos.y - crossedWord.word.Length < minY) minY = pos.y - crossedWord.word.Length;
            }
        }

        int width = maxX - minX;
        int height = maxY - minY;

        int score = width + height + Mathf.Abs(width - height);
        return score;
    }

    private bool InsertWordToBoard(string newWord)
    {
        Position newPos = new Position();
        bool horizontal = true;
        bool canInsert = false;

        if (boardWords.Count == 0)
        {
            newPos.Set(0, 0);
            canInsert = true;
        }
        else
        {
            int bestScore = int.MaxValue;
            foreach(var crossedWord in crossedWords)
            {
                var pos = crossedWord.position;
                var boardWord = crossedWord.word;
                var tempHorizontal = !crossedWord.horizontal;
                var intersections = GetIntersections(newWord, boardWord);

                if (intersections.Count > 0)
                {
                    foreach (var bestIntersection in intersections)
                    {

                        var intersectionPos = crossedWord.horizontal ? new Position() { x = pos.x + bestIntersection.Value1, y = pos.y } :
                                                                       new Position() { x = pos.x, y = pos.y - bestIntersection.Value1 };

                        var tempPos = tempHorizontal ? new Position() { x = intersectionPos.x - bestIntersection.Value2, y = intersectionPos.y } :
                                                   new Position() { x = intersectionPos.x, y = intersectionPos.y + bestIntersection.Value2 };

                        if (CheckValidPosition(newWord, tempHorizontal, tempPos))
                        {
                            int score = GetScore(newWord, tempHorizontal, tempPos);
                            if (score < bestScore)
                            {
                                bestScore = score;
                                horizontal = tempHorizontal;
                                newPos = tempPos;
                                canInsert = true;
                            }
                        }
                    }
                }
            }
        }

        if (!canInsert) return false;

        boardWords.Add(newWord);
        CrossedWord newCrossedWord = new CrossedWord()
        {
            horizontal = horizontal,
            position = newPos,
            word = newWord
        };

        crossedWords.Add(newCrossedWord);

        for(int i = 0; i < newWord.Length; i++)
        {
            Position letterPos = horizontal ? new Position() { x = newPos.x + i, y = newPos.y } : new Position() { x = newPos.x, y = newPos.y - i };
            if (!boardLetters.ContainsKey(letterPos))
            {
                boardLetters.Add(letterPos, newWord[i]);
            }
        }

        return true;
    }
}

public class CrosswordBoard
{
    public List<CrossedWord> crossedWords;
}

public class CrossedWord
{
    public Position position;
    public string word;
    public bool horizontal;
}

public struct Position
{
    public int x;
    public int y;

    public void Set(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", x, y);
    }

    public bool IsEqual(Position b)
    {
        return x == b.x && y == b.y;
    }
}
