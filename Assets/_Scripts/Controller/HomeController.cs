using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HomeController : BaseController {
    private const int PLAY = 0;
    private const int FACEBOOK = 2;

    protected override void Start()
    {
        base.Start();
        Music.instance.ChangeMusic();
    }

    public void OnClick(int index)
    {
        switch (index)
        {
            case PLAY:
                CUtils.LoadScene(1, true);
                break;
            case FACEBOOK:
                CUtils.LikeFacebookPage(ConfigController.Config.facebookPageID);
                break;
        }
        Sound.instance.PlayButton();
    }
}
