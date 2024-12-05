using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleSelectDialog : Dialog {

    public Text title;
    public Transform puzzleButtons;

    [HideInInspector]
    public int groupNumber;

    protected override void Start()
    {
        base.Start();
        title.text = string.Format(title.text, groupNumber);

        foreach(Transform child in puzzleButtons)
        {
            child.GetComponent<PuzzleButton>().UpdateUI(groupNumber);
        }
    }
}
