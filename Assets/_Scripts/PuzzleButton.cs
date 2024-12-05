using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleButton : MonoBehaviour {
    public Text puzzleName;
    public Image progressImage;
    public Image playImage;
    public Sprite playActive, playInactive, done;

    private int puzzleNumber;
    private bool isUnlocked;

    private void Start()
    {
        puzzleNumber = transform.GetSiblingIndex() + 1;
        puzzleName.text = string.Format(puzzleName.text, puzzleNumber);
    }

    public void UpdateUI(int groupNumber)
    {
        puzzleNumber = transform.GetSiblingIndex() + 1;
        float progress = Prefs.GetPuzzleProgress(groupNumber, puzzleNumber);
        progressImage.fillAmount = progress;

        int unlockedPuzzle = groupNumber == Prefs.UnlockedGroup ? Prefs.GetUnlockedPuzzle() : Const.PUZZLE_IN_GROUP;
        isUnlocked = puzzleNumber <= unlockedPuzzle;

        playImage.sprite = isUnlocked ? (puzzleNumber < unlockedPuzzle && progress  == 1 ? done : playActive) : playInactive;
        GetComponent<Button>().interactable = isUnlocked;
    }

    public void OnClick()
    {
        Prefs.CurrentPuzzle = puzzleNumber;
        DialogController.instance.CloseCurrentDialog();
        CUtils.LoadScene(2, true);
        Sound.instance.PlayButton();
    }
}
