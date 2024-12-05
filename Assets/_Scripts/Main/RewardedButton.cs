using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardedButton : MonoBehaviour
{
    public GameObject content;
    public GameObject adAvailableTextHolder;
    public TimerText timerText;

    private const string ACTION_NAME = "rewarded_video";
    private bool isEventAttached;

    private void Start()
    {
        if (timerText != null) timerText.onCountDownComplete += OnCountDownComplete;

#if UNITY_ANDROID || UNITY_IOS
        
        if (!IsAvailableToShow())
        {
            content.SetActive(false);
            if (IsAdAvailable() && !IsActionAvailable())
            {
                int remainTime = (int)(ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
                ShowTimerText(remainTime);
            }
        }

        InvokeRepeating("IUpdate", 1, 1);
#else
        content.SetActive(false);
#endif
    }

    private void IUpdate()
    {
        content.SetActive(IsAvailableToShow());
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
            content.SetActive(false);
            ShowTimerText(ConfigController.Config.rewardedVideoPeriod);
        }
    }

    private void ShowTimerText(int time)
    {
        if (adAvailableTextHolder != null)
        {
            adAvailableTextHolder.SetActive(true);
            timerText.SetTime(time);
            timerText.Run();
        }
    }

    private void OnCountDownComplete()
    {
        adAvailableTextHolder.SetActive(false);
        if (IsAdAvailable())
        {
            content.SetActive(true);
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

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (adAvailableTextHolder.activeSelf)
            {
                int remainTime = (int)(ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
                ShowTimerText(remainTime);
            }
        }
    }
}
