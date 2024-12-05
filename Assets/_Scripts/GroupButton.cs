using UnityEngine;
using UnityEngine.UI;

public class GroupButton : MonoBehaviour
{
    public Sprite current, locked, passed;
    public Image image;

    private int groupNumber;

    void Start()
    {
        groupNumber = transform.GetSiblingIndex() + 1;
        GetComponentInChildren<Text>().text = groupNumber.ToString();

        if (groupNumber == Prefs.UnlockedGroup)
        {
            image.sprite = current;
        }
        else if (groupNumber > Prefs.UnlockedGroup)
        {
            image.sprite = locked;
            GetComponent<Button>().interactable = false;
        }
        else
        {
            image.sprite = passed;
        }
    }

    public void OnButtonClick()
    {
        Prefs.CurrentGroup = groupNumber;
        var dialog = (PuzzleSelectDialog)DialogController.instance.GetDialog(DialogType.PuzzleSelect);
        dialog.groupNumber = groupNumber;
        DialogController.instance.ShowDialog(dialog);
        Sound.instance.PlayButton();
    }
}
