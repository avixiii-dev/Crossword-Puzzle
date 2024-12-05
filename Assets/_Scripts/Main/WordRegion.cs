using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Superpow;

public class WordRegion : MonoBehaviour {
    public Transform lineWordsTr;
    public RectTransform rootCanvas;
    public Text definitionText;
    public MagicStar magicStar;

    private List<LineWord> lines = new List<LineWord>();
    private float limitX, limitY;
    private int currentLineIndex;

    private readonly float cellSize = 80f;
    private Puzzle levelData;

    [HideInInspector]
    public bool interactable = false;

    public static WordRegion instance;

    private void Awake()
    {
        instance = this;
    }

    public void Load(Puzzle levelData)
    {
        this.levelData = levelData;
        for(int i = 0; i < levelData.words.Count; i ++)
        {
            levelData.words[i].word = levelData.words[i].word.ToUpper();
            levelData.words[i].word = levelData.words[i].word.Trim();
            levelData.words[i].word = Regex.Replace(levelData.words[i].word, @"\s+", "");
        }

        var allKeys = Keyboard.instance.GetAllKeys();

        for(int i = levelData.words.Count - 1; i >= 0; i--)
        {
            if (!Utils.IsValidAnswer(levelData.words[i].word, allKeys))
            {
                levelData.words.RemoveAt(i);
            }
        }

        var wordList = levelData.words.Select(x => x.word).ToList();

        CrosswordGenerator generator = new CrosswordGenerator();
        generator.SetWords(wordList);
        var crossedWords = generator.Generate();
        if (crossedWords.Count < 2)
        {
            Toast.instance.ShowMessage("This puzzle is not valid");
            DoWhenGameComplete();
            return;
        }
        wordList = crossedWords.Select(x => x.word).ToList();

        string[] puzzleData = Prefs.PuzzleData;
        bool useProgress = false;

        if (puzzleData.Length != 0)
        {
            useProgress = CheckValidPuzzleData(puzzleData, wordList);
            if (!useProgress)
            {
                PlayerPrefs.DeleteKey(Prefs.PuzzleDataKey);
                Prefs.SetPuzzleProgress(Prefs.CurrentGroup, Prefs.CurrentPuzzle, 0);
            }
        }

        int lineIndex = 0;
        foreach (var word in wordList)
        {
            LineWord line = Instantiate(MonoUtils.instance.lineWord);
            line.name = "LineWord (" + word + ")";
            line.answer = word;
            line.cellSize = cellSize;
            line.crossedWord = crossedWords[lineIndex];
            line.Build();

            if (useProgress)
            {
                line.SetProgress(puzzleData[lineIndex]);
            }

            line.transform.SetParent(lineWordsTr);
            line.transform.localScale = Vector3.one;
            line.transform.localPosition = Vector3.zero;

            lines.Add(line);
            lineIndex++;
        }

        SetLinesPosition();
        StartCoroutine(ShowLines());
    }

    private void SetLinesPosition()
    {
        var distance = cellSize * (1 + Const.CELL_GAP_COEF);
        foreach(var line in lines)
        {
            Position pos = line.crossedWord.position;
            line.transform.localPosition = new Vector2(pos.x, pos.y) * distance;
        }

        float minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

        foreach(var line in lines)
        {
            foreach(var cell in line.cells)
            {
                minX = Mathf.Min(minX, cell.transform.position.x);
                minY = Mathf.Min(minY, cell.transform.position.y);
                maxX = Mathf.Max(maxX, cell.transform.position.x);
                maxY = Mathf.Max(maxY, cell.transform.position.y);
            }
        }

        minX -= cellSize / 200f;
        minY -= cellSize / 200f;
        maxX += cellSize / 200f;
        maxY += cellSize / 200f;

        Vector3 center = new Vector3((maxX + minX) / 2f, (maxY + minY) / 2f, 0);
        Vector3 moveDelta = lineWordsTr.position - center;
        moveDelta.z = 0;

        foreach (Transform child in lineWordsTr)
        {
            child.position += moveDelta;
        }

        float width = (maxX - minX) * 100f;
        float height = (maxY - minY) * 100f;

        lineWordsTr.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        float ratio = width / height;

        var rt = GetComponent<RectTransform>();
        float boardWidth = rt.rect.width;
        float boardHeight = rt.rect.height;

        float fitScale = ratio < boardWidth / boardHeight ? Mathf.Min(boardHeight / height, 1) : Mathf.Min(boardWidth / width, 1);
        lineWordsTr.localScale = Vector3.one * fitScale;

        limitX = (width * Const.ZOOM_IN_SCALE - boardWidth ) / 200f;
        limitY = (height * Const.ZOOM_IN_SCALE - boardHeight) / 200f;

        if (limitX < 0) limitX = 0;
        if (limitY < 0) limitY = 0;
    }

    public IEnumerator ShowLines()
    {
        foreach(var line in lines)
        {
            StartCoroutine(line.ShowLine());
            yield return new WaitForSeconds(line.cells.Count * Const.TIME_SHOW_CELL + 0.1f);
        }

        if (IsGameComplete())
        {
            MainController.instance.isGameComplete = true;
            interactable = true;
        }
        else OnDoneShowLines();
    }

    public void OnDoneShowLines()
    {
        iTween.ScaleTo(lineWordsTr.gameObject, Const.ZOOM_IN_SCALE * Vector3.one, 0.2f);
        Timer.Schedule(this, 0.3f, OnDoneZooming);
        Music.instance.ChangeMusic();
    }

    public void OnDoneZooming()
    {
        currentLineIndex = -1;
        NextWord(1);
        interactable = true;
        magicStar.enabled = true;
    }

    public void NextWord(int delta)
    {
        if (lines.Count == 0) return;

        currentLineIndex = (currentLineIndex + delta + lines.Count) % lines.Count;

        if (lines[currentLineIndex].IsShown() && !MainController.instance.isGameComplete)
        {
            NextWord(delta);
        }
        else
        {
            CentralizeLine(currentLineIndex);

            var line = lines[currentLineIndex];
            line.UpdateUI(LineWord.Status.Selected);
            line.SelectAvailableCell();
        }
    }

    public void CentralizeLine(int index)
    {
        var line = lines[index];
        definitionText.text = levelData.words.Find(x => x.word == line.answer).definition;

        foreach (var l in lines)
        {
            l.UpdateUI(LineWord.Status.Normal);
        }

        if (MainController.instance.isGameComplete) return;

        Vector3 centralPosition = (line.cells[line.cells.Count - 1].transform.position + line.cells[0].transform.position) / 2f;
        Vector3 moveDelta = transform.position - centralPosition;
        moveDelta.z = 0;

        float newX = Mathf.Clamp(lineWordsTr.position.x + moveDelta.x, transform.position.x - limitX, transform.position.x + limitX);
        float newY = Mathf.Clamp(lineWordsTr.position.y + moveDelta.y, transform.position.y - limitY, transform.position.y + limitY);
        float newZ = lineWordsTr.position.z;

        iTween.MoveTo(lineWordsTr.gameObject, new Vector3(newX, newY, newZ), 0.3f);
    }

    public void OnCellClick(Cell cell)
    {
        if (!interactable) return;

        var line = GetLineThatContainsCell(cell);
        if (line == null) return;

        currentLineIndex = lines.IndexOf(line);
        CentralizeLine(currentLineIndex);

        line.UpdateUI(LineWord.Status.Selected);

        if (!cell.Main.IsShown)
            line.SelectCell(line.GetCellIndex(cell.position));
        else
            line.SelectAvailableCell();
    }

    private LineWord GetLineThatContainsCell(Cell cell)
    {
        var currentLine = lines[currentLineIndex];
        if (currentLine.ContainsCell(cell))
        {
            return currentLine;
        }
        return lines.Find(x => x.ContainsCell(cell));
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.inputString.Length >= 1)
            {
                var c = Input.inputString[0];
                if (c >= 'a' && c <= 'z')
                {
                    OnKeyPressed(c.ToString().ToUpper());
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            OnKeyPressed("DEL");
        }
    }

    public void OnKeyPressed(string keyValue)
    {
        if (!interactable) return;
        var line = lines[currentLineIndex];

        if (line.IsShown()) return;

        var selectedCell = line.GetSelectedCell();
        if (selectedCell.IsShown) return;

        keyValue = keyValue.ToUpper();

        if (keyValue != "DEL")
        {
            line.FillSelectedCell(keyValue);
            var linesHaveCell = lines.FindAll(x => !x.IsShown() && x.ContainsCell(selectedCell));

            foreach(var l in linesHaveCell)
            {
                if (l.IsFilled()) DoWhenLineWordIsFull(l);
            }

            if (!line.IsFilled())
            {
                int nextCell = line.GetNextSelectedCell();
                line.SelectCell(nextCell);
            }
        }
        else
        {
            line.RemoveSelectedCellText();

            int backCell = line.GetBackSelectedCell();
            if (backCell != -1)
            {
                line.SelectCell(backCell);
            }
        }
    }

    public void DoWhenLineWordIsFull(LineWord line)
    {
        if (line.IsCorrectAnswer())
        {
            StartCoroutine(DoWhenAnswerCorrect(line));
        }
        else
        {
            StartCoroutine(DoWhenAnswerNotCorrect(line));
        }
    }

    public IEnumerator DoWhenAnswerCorrect(LineWord line)
    {
        interactable = false;

        yield return new WaitForSeconds(0.5f);
        line.ShowAnswer();
        line.UpdateUI(LineWord.Status.Normal);
        Sound.instance.Play(Sound.Others.Match);

        int numFilled = lines.FindAll(x => x.IsFilled()).Count();
        Prefs.SetPuzzleProgress(Prefs.CurrentGroup, Prefs.CurrentPuzzle, (float)numFilled / lines.Count);

        yield return new WaitForSeconds(0.5f);

        CUtils.ShowInterstitialAd();

        if ((IsGameComplete()))
        {
            DoWhenGameComplete();
            yield break;
        }

        if (line == lines[currentLineIndex]) NextWord(1);
        interactable = true;
    }

    public IEnumerator DoWhenAnswerNotCorrect(LineWord line)
    {
        interactable = false;
        yield return new WaitForSeconds(0.5f);
        line.SetIncorrectColor();
        Sound.instance.Play(Sound.Others.Incorrect);
        line.Shake();
        GetComponent<ScrollRect>().enabled = false;

        yield return new WaitForSeconds(0.7f);
        line.UnFill();
        line.ResetColor();

        CUtils.ShowInterstitialAd();
        
        if (line == lines[currentLineIndex]) line.SelectAvailableCell();
        interactable = true;
        GetComponent<ScrollRect>().enabled = true;
    }

    private bool IsGameComplete()
    {
        return lines.FindAll(x => x.IsShown()).Count == lines.Count;
    }

    private void DoWhenGameComplete(bool canReplay = false)
    {
        SavePuzzleData();
        MainController.instance.OnComplete(canReplay);
    }

    public void SavePuzzleData()
    {
        if (lines.Count == 0) return;

        List<string> results = new List<string>();
        foreach(var line in lines)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var cell in line.cells)
            {
                sb.Append(cell.IsShown ? "1" : cell.IsFilled() ? cell.letterText.text : "0");
            }
            results.Add(sb.ToString());
        }

        Prefs.PuzzleData = results.ToArray();
    }

    public bool CheckValidPuzzleData(string[] puzzleData, List<string> wordList)
    {
        if (puzzleData.Length != wordList.Count) return false;

        for (int i = 0; i < wordList.Count; i++)
        {
            if (puzzleData[i].Length != wordList[i].Length) return false;
        }
        return true;
    }

    public Cell GetNearestCell(Vector3 position, float radius)
    {
        float minDistance = int.MaxValue;
        Cell result = null;

        foreach(var line in lines)
        {
            foreach(var cell in line.cells)
            {
                if (!cell.Main.IsShown)
                {
                    float distance = Vector3.Distance(cell.gameObject.transform.position, position);
                    if (distance <= radius && distance < minDistance)
                    {
                        minDistance = distance;
                        result = cell.Main;
                    }
                }
            }
        }

        return result;
    }

    public void OnDoneMagicStar(Cell cell)
    {
        var linesHaveCell = lines.FindAll(x => x.ContainsCell(cell));
        foreach (var l in linesHaveCell)
        {
            if (l.IsFilled()) DoWhenLineWordIsFull(l);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SavePuzzleData();
    }

    private void OnApplicationQuit()
    {
        SavePuzzleData();
    }
}
