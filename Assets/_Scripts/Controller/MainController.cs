using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Superpow;

public class MainController : BaseController {
    public Text definitionText;

    [HideInInspector]
    public bool isGameComplete;
    private Puzzle puzzleData;

    public static MainController instance;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        GameState.mainCells = new Dictionary<Position, Cell>();

        definitionText.text = "";

        puzzleData = Utils.LoadPuzzle(Prefs.CurrentGroup, Prefs.CurrentPuzzle);
        WordRegion.instance.Load(puzzleData);
    }

    public void OnComplete(bool canReplay = false)
    {
        if (isGameComplete) return;
        isGameComplete = true;

        Music.instance.Stop();

        Timer.Schedule(this, canReplay ? 0f : 0.5f, () =>
        {
            var dialog = (WinDialog)DialogController.instance.GetDialog(DialogType.Win);
            dialog.canReplay = canReplay;
            DialogController.instance.ShowDialog(dialog);

            Sound.instance.Play(Sound.Others.Win);
            CUtils.ShowInterstitialAd();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !DialogController.instance.IsDialogShowing())
        {
            DialogController.instance.ShowDialog(DialogType.Pause);
        }
    }
}
