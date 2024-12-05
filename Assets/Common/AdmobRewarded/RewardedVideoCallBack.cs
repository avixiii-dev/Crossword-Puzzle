using GoogleMobileAds.Api;
using UnityEngine;

public class RewardedVideoCallBack : MonoBehaviour {

    private void Start()
    {
        Timer.Schedule(this, 0.1f, AddEvents);
    }

    private void AddEvents()
    {
#if UNITY_ANDROID || UNITY_IOS
        Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
#endif
    }

    private void CompleteMethod(bool completed, string advertise)
    {
        Debug.Log("Closed rewarded from: " + advertise + " -> Completed " + completed);
        if (completed == true)
        {
            int amount = ConfigController.Config.rewardedVideoAmount;
            CurrencyController.CreditBalance(amount);
            Toast.instance.ShowMessage("You've received " + amount + " rubies", 3);
            CUtils.SetActionTime(ACTION_NAME);
        }
    }

    private const string ACTION_NAME = "rewarded_video";
    

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        
#endif
    }
}
