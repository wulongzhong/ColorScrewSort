using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using GameAnalyticsSDK;
using System;
using UnityEngine.Networking;

public class MAXAdsManager : BaseAdsManager
{
    public static bool IsDebug = false;

    public GameObject noInternetPanel = null;

    public string SDK_KEY_STR = "7HAGxU2S75w0HktmIhAOmohfnDWwR9RgVLLsKp5DFwuuAOAI4NquAODP_mrfgfvrWIdoy85llx3ZGJT67TJNZD";

    public string openScreenAppId = "";
    public string interstitialAdsAppId = "";
    public string bannerAppid = "";
    public string rewardedAppId = "";
    

    bool canOpenAd = true;
    bool appopened = false;
    public bool startLoadInterAd;



    private bool _startLoopInterstitialAds = false;
    public float LoopInterstitialCD = 40f;
    private float _loopInterstitialTimer = 0;


    private bool _startTouchInterstitialAds = false;
    public float TouchInterstitialCD = 10f;
    private float _touchInterstitialTimer = 0;


    public override string GaSdkName()
    {
        return "applovin";
    }

    public override void InitSDK()
    {
        MaxSdk.SetIsAgeRestrictedUser(false);
        MaxSdk.SetHasUserConsent(true);
        MaxSdk.SetDoNotSell(false);
        
        
        base.InitSDK();

        if (!MaxSdk.IsInitialized())
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {

                Debug.Log(GaSdkName() + " SDK Init Success!");
                InitialOpenAppAds();
                InitializeInterstitialAds();
                StartLoopInterstitial();
                StartTouchInterstitial();

                InitializeRewardedAds();

                //设置banner
                InitializeBannerAds();
            };
            MaxSdk.SetSdkKey(SDK_KEY_STR);
            MaxSdk.InitializeSdk();
        }

    }

    //#if MAX_ENABLE

    #region AppOpen SDK

    public void InitialOpenAppAds()
    {
        MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenLoadedEvent;
        MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenLoadFailedEvent;

        MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
        MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenDisplayedEvent;

        MaxSdk.LoadAppOpenAd(openScreenAppId);
        

    }

    public void ShowAdIfReady()
    {
        Debug.Log("App Open Show");
        if (canOpenAd && MaxSdk.IsAppOpenAdReady(openScreenAppId))
        {
            
            MaxSdk.ShowAppOpenAd(openScreenAppId);
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, GaSdkName(), "OpenApp");
        }
        NotShowOpenAd();
        
    }

    private void OnAppOpenLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
    {
        Debug.Log($"加载启动广告失败:{arg1};{arg2.Code}:{arg2.Message}");
        ProcessLoadPage();
        GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, GaSdkName(), "OpenApp");
    }

    private void OnAppOpenLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        Debug.Log("加载启动广告成功");
        ShowAdIfReady();
    }

    private void OnAppOpenDismissedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {

        ResetTime();
        MaxSdk.LoadAppOpenAd(openScreenAppId);
        ProcessLoadPage();
    }


    private void OnAppOpenDisplayedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        StopTime();
    }


    public void ProcessLoadPage()
    {
        if (Splash.Instance != null)
        {
            //var init = CommonTools.FindSceneRoot("GameInit").GetComponent<GameInit>();
            Splash.Instance.SkipOpenAd();
            ResetLoopInterstitial();
            ResetTouchInterstitial();
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus&& appopened==true)
        {
            Debug.Log("App Open Show1111");
            canOpenAd = true;
            ShowAdIfReady();
            Debug.Log(pauseStatus);
        }
    }
    

    public override void NotShowOpenAd()
    {
        canOpenAd = false;
        appopened = true;
    }
    #endregion

    #region Banner SDK
    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerAppid, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerAppid, new Color(0, 0, 0, 1));

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;

        //MaxSdk.HideBanner(bannerAppid);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            //DelayProcessor.Instance.DelayProcessFunc(5f, () =>
            //{
            //    MaxSdk.ShowBanner(bannerAppid);
            //    //DebugManager.instance.setBannerTime();
            //    Debug.Log("Show Banner Success!");
            //});
        };
        Debug.Log("InitializeBannerAds Success!");

        //DelayProcessor.DelayProcess(5f, () =>
        //{
        //    MaxSdk.ShowBanner(bannerAppid);
        //    Debug.Log("Show Banner Success!");
        //});
    }


    private void OnBannerAdCollapsedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
    }

    private void OnBannerAdExpandedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
    }

    private void OnBannerAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        //DebugManager.instance.setBannerTime();
    }

    private void OnBannerAdClickedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Banner, GaSdkName(), "Banner");
    }

    private void OnBannerAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
    {
        GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Banner, GaSdkName(), "Banner");

        Debug.Log("Load Banner Failed! " + arg1);
    }

    private void OnBannerAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Banner, GaSdkName(), "Banner");
    }
    #endregion

    #region Rewarded SDK


    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardedAppId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        StopTime();
        GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, GaSdkName(), GetGAPlacementByRewardType());
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, GaSdkName(), GetGAPlacementByRewardType());
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.RewardedVideo, GaSdkName(), GetGAPlacementByRewardType());
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        ResetTime();
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        ProcessGetReward(rewardType);

    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }

    public override void PlayerRewardAds()
    {
        if (RewardedAdsIsReady())
        {
            MaxSdk.ShowRewardedAd(rewardedAppId);
        }

        Debug.Log("Reward is Loaded? :" + RewardedAdsIsReady() + " " + rewardType.ToString());
        Debug.Log("Show reward ads success!" + rewardType.ToString());
    }

    private void ProcessGetReward(RewardType reward)
    {
        if (RewardType.Null != reward)
        {
            //rewardCallback?.Invoke();
            ThreadedCallbackManager.AddCallback(rewardCallback);
        }
            
        //InterstitialCD.ReStart();
        rewardType = RewardType.Null;
        //showForceInterstitial();

        ResetLoopInterstitial();
    }

    public override bool RewardedAdsIsReady()
    {
        Debug.Log("Reward PlacementId:" + rewardedAppId);
        return MaxSdk.IsRewardedAdReady(rewardedAppId);
    }


    private string sendFBMessageStr;

    #endregion

    #region Interstitial SDK
    int retryAttempt = 0;

    public void InitializeInterstitialAds()
    {

        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;


        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialAdsAppId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("开始加载插页广告");
        retryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)
        Debug.Log("加载插页广告失败" + errorInfo.Message);
        retryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttempt));
        GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, GaSdkName(), "Interstitial");
        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        StopTime();
        GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, GaSdkName(), "Interstitial");
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Interstitial, GaSdkName(), "Interstitial");
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("播放插页广告失败" + errorInfo.Code);
        // Interstitial ad failed to display. We recommend loading the next ad
        GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, GaSdkName(), "Interstitial");
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("关闭插页广告");
        // Interstitial ad is hidden. Pre-load the next ad
        ResetTime();
        LoadInterstitial();

        onCloseInterstitial.Invoke();
    }


    /// <summary>
    /// 显示插页广告
    /// </summary>
    protected override void ShowInterstitial()
    {

        MaxSdk.ShowInterstitial(interstitialAdsAppId);
        //if (MaxSdk.IsInterstitialReady(interstitialAdsAppId) && isOpenInterstitial && InterstitialCD.IsFinish)
        //{
        //    Debug.Log("插屏");
        //    MaxSdk.ShowInterstitial(interstitialAdsAppId);
        //    InterstitialCD.ReStart();
        //}
    }

    public void StartLoopInterstitial()
    {
        _startLoopInterstitialAds = true;
        _loopInterstitialTimer = 0;
    }

    public void ResetLoopInterstitial()
    {
        _loopInterstitialTimer = 0;
    }

    public void ProcessLoopInterstitial()
    {
        if (!_startLoopInterstitialAds) return;

        //检测到CD被改动，则重新开始
        if (LoopInterstitialCD != INTER_ADS)
        {
            LoopInterstitialCD = INTER_ADS;
            StartLoopInterstitial();
        }

        _loopInterstitialTimer += Time.deltaTime;
        //if (_loopInterstitialTimer > LoopInterstitialCD)
        //{
        //    _loopInterstitialTimer = 0;
        //    ShowInterstitial();
        //}
    }


    public override void ShowNormalInterstitial()
    {
        if (_loopInterstitialTimer > LoopInterstitialCD)
        {
            _loopInterstitialTimer = 0;
            ShowInterstitial();
            //DebugManager.instance.setInterTime();
        }
    }


    public void StartTouchInterstitial()
    {
        _startTouchInterstitialAds = true;
        _touchInterstitialTimer = 0;
    }

    public void ResetTouchInterstitial()
    {
        _touchInterstitialTimer = 0;
    }

    public void ProcessTouchInterstitial()
    {
        if (!_startTouchInterstitialAds) return;

        _touchInterstitialTimer += Time.deltaTime;
        //DebugManager.instance.updateAFKTimeInterval(_touchInterstitialTimer);

        if (Input.GetMouseButton(0))
        {
            _touchInterstitialTimer = 0;
        }
        if (Input.GetMouseButtonDown(0))
        {
            _touchInterstitialTimer = 0;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _touchInterstitialTimer = 0;
        }

        if (_touchInterstitialTimer > AFK_ADS)
        {
            _touchInterstitialTimer = 0;
            ResetLoopInterstitial();
            //ShowInterstitial();
            //DebugManager.instance.setInterTime();
        }

    }
    #endregion



    //#endif

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GameAnalyticsILRD.SubscribeMaxImpressions();
    }

    public IEnumerator CheckInternetConnection(Action<bool> syncResult)
    {
        const string echoServer = "http://bing.com";

        bool result;
        using (var request = UnityWebRequest.Head(echoServer))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();
            result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
        }
        syncResult(result);
    }


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        

        //ProcessTouchInterstitial();
        ProcessLoopInterstitial();

        

        /*if (nointernetCD.IsFinish)
        { 
            StartCoroutine(CheckInternetConnection((isConnected) => {
                if (isConnected)
                {
                    noInternetPanel.SetActive(false);
                }
                else
                {
                    noInternetPanel.SetActive(true);
                }
            }));
        }*/
    }
    
    public override void HideBanner()
    {
        //MaxSdk.HideBanner(bannerAppid);
    }

}
