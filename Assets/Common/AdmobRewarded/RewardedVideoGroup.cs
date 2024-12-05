using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardedVideoGroup : MonoBehaviour
{
    public GameObject buttonGroup;
    public GameObject textGroup;
    public TimerText timerText;

    private const string ACTION_NAME = "rewarded_video";

    private void Start()
    {
        if (timerText != null) timerText.onCountDownComplete += OnCountDownComplete;

#if UNITY_ANDROID || UNITY_IOS
        Timer.Schedule(this, 0.1f, AddEvents);

        if (!IsAvailableToShow())
        {
            buttonGroup.SetActive(false);
            if (IsAdAvailable() && !IsActionAvailable())
            {
                int remainTime = (int)(ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
                ShowTimerText(remainTime);
            }
        }

        InvokeRepeating("IUpdate", 1, 1);
#else
        buttonGroup.SetActive(false);
#endif
    }

    private void AddEvents()
    {
        
    }

    private void IUpdate()
    {
        buttonGroup.SetActive(IsAvailableToShow());
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
            buttonGroup.SetActive(false);
            ShowTimerText(ConfigController.Config.rewardedVideoPeriod);
        }
    }

    private void ShowTimerText(int time)
    {
        if (textGroup != null)
        {
            textGroup.SetActive(true);
            timerText.SetTime(time);
            timerText.Run();
        }
    }


    private void OnCountDownComplete()
    {
        textGroup.SetActive(false);
        if (IsAdAvailable())
        {
            buttonGroup.SetActive(true);
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
            if (textGroup != null && textGroup.activeSelf)
            {
                int remainTime = (int)(ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
                ShowTimerText(remainTime);
            }
        }
    }
}
