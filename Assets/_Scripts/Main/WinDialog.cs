using UnityEngine.UI;
using UnityEngine;

public class WinDialog : Dialog {
    public Text title;
    public GameObject rateObj, replayObj;

    [HideInInspector]
    public bool canReplay;

    protected override void Start()
    {
        base.Start();

        title.text = string.Format(title.text, Prefs.CurrentGroup);

        if (Prefs.IsLastPuzzle())
        {
            Prefs.UnlockedPuzzle++;
            if (Prefs.UnlockedPuzzle > Const.PUZZLE_IN_GROUP)
            {
                Prefs.UnlockedPuzzle = 1;
                Prefs.UnlockedGroup++;
            }
        }

        replayObj.SetActive(canReplay);
        rateObj.SetActive(!canReplay);
    }

    public void NextClick()
    {
        Close();
        Sound.instance.PlayButton();
        Prefs.CurrentPuzzle++;
        if (Prefs.CurrentPuzzle > Const.PUZZLE_IN_GROUP)
        {
            Prefs.CurrentPuzzle = 1;
            Prefs.CurrentGroup++;
        }

        CUtils.LoadScene(Prefs.CurrentPuzzle == 1 ? 1 : 2, true);
    }

    public void HomeClick()
    {
        Close();
        Sound.instance.PlayButton();
        CUtils.LoadScene(1, true);
    }

    public void RateClick()
    {
        Sound.instance.PlayButton();
        CUtils.RateGame();
    }

    public void ReplayClick()
    {
        Close();
        Prefs.ClearPuzzleData();
        Prefs.SetPuzzleProgress(Prefs.CurrentGroup, Prefs.CurrentPuzzle, 0);
        CUtils.ReloadScene(true);
    }
}
