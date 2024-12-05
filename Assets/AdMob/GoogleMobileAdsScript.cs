using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Advertisements;
// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class GoogleMobileAdsScript : MonoBehaviour
{


    private static string outputMessage = string.Empty;


    int npaValue = -1;
    //"npa"=Non Personalized Ads

    public static GoogleMobileAdsScript instance;
    

    private UnityAction rewardVideoAction;

    public static string OutputMessage
    {
        set { outputMessage = value; }
    }
    //public void RequestVideo()
    //{
    //    this.RequestInterstitial();
    //    this.RequestRewardBasedVideo();
    //}
    public void Start()
    {


        Advertisements.Instance.Initialize();


        Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM, BannerType.Banner);

        npaValue = PlayerPrefs.GetInt("npa", 0);
        Debug.Log("npa = " + npaValue.ToString());
    }



}
