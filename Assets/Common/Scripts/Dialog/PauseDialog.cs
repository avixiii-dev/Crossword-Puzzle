using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseDialog : Dialog
{

    protected override void Start()
    {
        base.Start();

        if (WordRegion.instance != null)
        {
            WordRegion.instance.SavePuzzleData();
        }
    }

    public void OnContinueClick()
    {
        Sound.instance.PlayButton();
        Close();
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnMenuClick()
    {
        CUtils.LoadScene(1, true);
        Sound.instance.PlayButton();
        Close();
    }

    public void ReplayClick()
    {
        Close();
        Prefs.ClearPuzzleData();
        Prefs.SetPuzzleProgress(Prefs.CurrentGroup, Prefs.CurrentPuzzle, 0);
        CUtils.ReloadScene(true);
    }
}
