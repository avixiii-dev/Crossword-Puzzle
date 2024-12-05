using UnityEngine;
using System;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdmobController : MonoBehaviour
{

    public static AdmobController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!CUtils.IsAdsRemoved())
        {
            
        }
    }
    

    public void HideBanner()
    {
        Advertisements.Instance.HideBanner();
    }

    public void ShowInterstitial()
    {
        if (!CUtils.IsAdsRemoved())
        {
            Advertisements.Instance.ShowInterstitial();
        }
    }

    public void ShowRewardBasedVideo()
    {
       
    }



    
}
