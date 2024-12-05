using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineWord : MonoBehaviour {
    [HideInInspector]
    public string answer;
    [HideInInspector]
    public float cellSize;
    [HideInInspector]
    public List<Cell> cells = new List<Cell>();
    [HideInInspector]
    public int numLetters;
    [HideInInspector]
    public float lineWidth, lineHeight;
    public CrossedWord crossedWord;

    public Sprite cellBack0, cellBack1, cellBack2;
    public enum Status { Normal, Selected };

    private int selectedCell;

    public void Build()
    {
        numLetters = answer.Length;
        float cellGap = cellSize * Const.CELL_GAP_COEF;

        for(int i = 0; i < numLetters; i++)
        {
            int xDelta = crossedWord.horizontal ? i : 0;
            int yDelta = crossedWord.horizontal ? 0 : -i;
            Position position = new Position() { x = crossedWord.position.x + xDelta, y = crossedWord.position.y + yDelta };

            Cell cell = Instantiate(MonoUtils.instance.cell);
            cell.letter = answer[i].ToString();
            cell.letterText.transform.localScale = Vector3.one * (cellSize / 100f);
            RectTransform cellTransform = cell.GetComponent<RectTransform>();
            cellTransform.SetParent(transform);
            cellTransform.sizeDelta = new Vector2(cellSize, cellSize);
            cellTransform.localScale = Vector3.one;
            cell.position = position;
            cell.gameObject.SetActive(false);

            float x, y;

            if (crossedWord.horizontal)
            {
                x = i * (cellSize + cellGap);
                y = 0;
            }
            else
            {
                x = 0;
                y = -i * (cellSize + cellGap);
            }

            cellTransform.localPosition = new Vector3(x, y);
            cells.Add(cell);
        }

        foreach(var cell in cells)
        {
            if (!GameState.mainCells.ContainsKey(cell.position))
            {
                GameState.mainCells.Add(cell.position, cell);
            }
        }
    }

    public IEnumerator ShowLine()
    {
        foreach (var cell in cells)
        {
            cell.Main.gameObject.SetActive(true);
            yield return new WaitForSeconds(Const.TIME_SHOW_CELL);
        }
    }

    public void UpdateUI(Status status)
    {
        foreach (var cell in cells)
        {
            cell.Main.GetComponent<Image>().sprite = status == Status.Normal ? cellBack0 : cellBack1;
        }
    }

    public void SelectCell(int index)
    {
        selectedCell = index;
        UpdateUI(Status.Selected);
        cells[index].Main.GetComponent<Image>().sprite = cellBack2;
    }

    public void SelectAvailableCell()
    {
        int i = 0;
        foreach (var cell in cells)
        {
            if (!cell.Main.IsShown)
            {
                SelectCell(i);
                break;
            }
            i++;
        }
    }

    public void FillSelectedCell(string value)
    {
        var cell = cells[selectedCell];
        cell.Main.FillText(value);
    }

    public void RemoveSelectedCellText()
    {
        var cell = cells[selectedCell];
        cell.Main.RemoveText();
    }

    public Cell GetSelectedCell()
    {
        return cells[selectedCell].Main;
    }

    public int GetNextSelectedCell()
    {
        int next = selectedCell;
        while (true)
        {
            next = (next + 1) % cells.Count;
            if (!cells[next].Main.IsShown) break;
        }
        return next;
    }

    public int GetBackSelectedCell()
    {
        int back = selectedCell;
        bool available = false;

        while (back > 0)
        {
            back = Mathf.Max(0, back - 1);
            if (!cells[back].Main.IsShown)
            {
                available = true;
                break;
            }
        }
        return available ? back : selectedCell;
    }

    public void SetLineWidth()
    {
        int numLetters = answer.Length;
        var rt = GetComponent<RectTransform>();

        var length = numLetters * cellSize + (numLetters - 1) * cellSize * Const.CELL_GAP_COEF;
        if (crossedWord.horizontal)
        {
            lineWidth = length;
            lineHeight = cellSize;
        }
        else
        {
            lineWidth = cellSize;
            lineHeight = length;
        }
        rt.sizeDelta = new Vector2(lineWidth, lineHeight);
    }

    public void SetProgress(string progress)
    {
        int i = 0;
        foreach(var cell in cells)
        {
            if (progress[i] == '1')
            {
                cell.IsShown = true;
                cell.ShowAnswerText();
            }
            else if (progress[i] != '0')
            {
                cell.FillText(progress[i].ToString());
            }
            i++;
        }
    }

    public void ShowAnswer()
    {
        foreach (var cell in cells)
        {
            cell.Main.ShowAnswer();
        }
    }

    public void ShowHint()
    {
        for(int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            if (!cell.Main.IsShown)
            {
                cell.Main.ShowAnswer();
                return;
            }
        }
    }

    public bool IsFilled()
    {
        return cells.Find(x => !x.Main.IsFilled()) == null;
    }

    public bool IsShown()
    {
        return cells.Find(x => !x.Main.IsShown) == null;
    }

    public void UnFill()
    {
        foreach (var cell in cells)
        {
            if (!cell.Main.IsShown)
            {
                cell.Main.RemoveText();
            }
        }
    }

    public void SetIncorrectColor()
    {
        foreach(var cell in cells)
        {
            cell.Main.SetIncorrectColor();
        }
    }

    public void ResetColor()
    {
        foreach(var cell in cells)
        {
            cell.Main.ResetColor();
        }
    }

    public void Shake()
    {
        Vector3 amount = 0.15f * (crossedWord.horizontal ? Vector3.right : Vector3.down);
        foreach(var cell in cells)
        {
            iTween.ShakePosition(cell.Main.gameObject, amount, 0.1f);
        }
    }

    public bool IsCorrectAnswer()
    {
        string filledText = "";
        foreach(var cell in cells)
        {
            filledText += cell.Main.letterText.text;
        }
        return filledText == answer;
    }

    public bool ContainsCell(Cell cell)
    {
        return cells.Find(x => x == cell || x.Main == cell) != null;
    }

    public Cell GetCell(Position position)
    {
        return cells.Find(x => x.position.IsEqual(position));
    }

    public int GetCellIndex(Position position)
    {
        return cells.FindIndex(x => x.position.IsEqual(position));
    }
}
