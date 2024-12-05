using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardedVideoButton : MonoBehaviour
{
    private const string ACTION_NAME = "rewarded_video";

    private void Start()
    {
        Timer.Schedule(this, 0.1f, AddEvents);
    }

    private void AddEvents()
    {
#if UNITY_ANDROID || UNITY_IOS
        
#endif
    }

    public void OnClick()
    {
        Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
        Sound.instance.PlayButton();
    }

    private void CompleteMethod(bool completed, string advertise)
    {
        Debug.Log("Closed rewarded from: " + advertise + " -> Completed " + completed);
        if (completed == true)
        {
            Timer.Schedule(this, 0.3f, () =>
            {
                var dialog = (RewardedVideoDialog)DialogController.instance.GetDialog(DialogType.RewardedVideo);
                dialog.SetAmount(ConfigController.Config.rewardedVideoAmount);
                DialogController.instance.ShowDialog(dialog);
            });

            CUtils.SetActionTime(ACTION_NAME);
        }
    }


    public bool IsAvailableToShow()
    {
        return IsActionAvailable() && IsAdAvailable();
    }

    private bool IsActionAvailable()
    {
        return CUtils.IsActionAvailable(ACTION_NAME, ConfigController.Config.rewardedVideoPeriod);
    }

    private bool IsAdAvailable()
    {
        bool isLoaded = Advertisements.Instance.IsRewardVideoAvailable();
        return isLoaded;
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        
#endif
    }
}
