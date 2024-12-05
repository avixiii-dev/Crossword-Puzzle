using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public GameObject letterTextObj;
    public Text letterText;
    public Color answerColor, filledColor, incorrectColor;

    [HideInInspector]
    public string letter;
    [HideInInspector]
    public Position position;
    [HideInInspector]
    public bool IsShown;

    public Cell Main
    {
        get { return GameState.mainCells[position]; }
    }

    public void ShowAnswer()
    {
        IsShown = true;
        ShowAnswerText();
        letterTextObj.transform.localScale = Vector3.zero;
        letterTextObj.GetComponent<Animator>().SetTrigger("show");
    }

    public void ShowAnswerText()
    {
        letterTextObj.SetActive(true);
        letterText.text = letter;
        letterText.color = answerColor;
    }

    public void FillText(string value)
    {
        letterTextObj.SetActive(true);
        letterText.text = value;
        letterText.color = filledColor;
    }

    public void RemoveText()
    {
        letterTextObj.SetActive(false);
        letterText.text = "";
    }

    public void SetIncorrectColor()
    {
        letterText.color = incorrectColor;
    }

    public void ResetColor()
    {
        letterText.color = IsShown ? answerColor : Color.black;
    }

    public void OnCellClick()
    {
        WordRegion.instance.OnCellClick(this);
    }

    public bool IsFilled()
    {
        return !string.IsNullOrEmpty(letterText.text);
    }
}
