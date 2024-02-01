using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using GameAnalyticsSDK;

public class BaseAdsManager : MonoBehaviour
{
    public static BaseAdsManager INSTANCE = null;


    //根据项目更改rewardtype
    public enum RewardType
    {
        Null,
        SkipLevel = 1,//跳过关卡
        RefreshLevel,//刷新关卡
        BattleAdGem,//战斗界面宝石
        BattleAdPropUndoMove,//战斗界面道具撤回
        BattleAdPropAddStick,//战斗界面道具柱子
        StoreGem,//商店宝石
        LevelWinGem,//关卡结算宝石
        LevelWinProp,//关卡结算道具
    }

    
    private float originTimeScale;
    
    //AB 配置为Default数据
    public static float INTER_ADS = 60f;
    public static int START_ADS_LV = 2;
    public static float AFK_ADS = 30f;
    public static float FORCE_ADS = 180f;

    public static int START_GARDEN_LV = 999;
    public static int ABTEST_SWITCH = 0;

    //RemoteDefault 手动配置
    public static float INTER_ADS_R = 60f;
    public static float AFK_ADS_R = 15f;
    public static float FORCE_ADS_R = 60f;

    //Default 手动配置
    public static float INTER_ADS_D = 60f;
    public static float AFK_ADS_D = 15f;
    public static float FORCE_ADS_D = 60f;

    public static float[] defaultData = new float[] { AFK_ADS_D, FORCE_ADS_D, INTER_ADS_D };
    public static float[] remoteData = new float[] { AFK_ADS_R, FORCE_ADS_R, INTER_ADS_R };
    public static RewardType rewardType = RewardType.Null;
    protected Action rewardCallback;

    [HideInInspector]
    public UnityEvent onCloseInterstitial = new UnityEvent();

    public virtual string GaSdkName ()
    {
        return "admob";
    }

    public virtual void Awake()
    {
        if(INSTANCE != null && INSTANCE != this)
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
            return;
        }

        if (INSTANCE == null)
        {
            INSTANCE = this;
            Application.targetFrameRate = 60;

            GameAnalytics.Initialize();
            GameAnalytics.OnRemoteConfigsUpdatedEvent += MyOnRemoteConfigsUpdateFunction;
            MyOnRemoteConfigsUpdateFunction();

            InitSDK();
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("Base Ads Manager Init Success!");
        }
    }

    protected virtual void Start()
    {
        //DebugManager.instance.initData(defaultData);
        //DebugManager.instance.initRemoteData(remoteData);
        originTimeScale = Time.timeScale;
    }

    protected virtual void Update()
    {

    }

    public virtual void InitSDK()
    {

    }

    protected virtual void ShowInterstitial() {

    }


    public virtual int ReturnADStartLvl() {
        return START_ADS_LV;
    }


    //根据项目更改rewardtype
    public string GetGAPlacementByRewardType()
    {
        string ret_str;

        if(rewardType == RewardType.Null)
        {
            ret_str = string.Empty;
        }
        else
        {
            ret_str = rewardType.ToString();
        }
        return ret_str;
    }

    public virtual void PlayerRewardAds() {}

    public virtual bool RewardedAdsIsReady() { return false; }

    public virtual void NotShowOpenAd() { }


    //显示插屏广告用这个 BaseAdsManager.INSTANCE.ShowNormalInterstitial();
    public virtual void ShowNormalInterstitial()
    {

    }

    //RewardAD 用这个
    //BaseAdsManager.INSTANCE.ShowRewardAds(BaseAdsManager.RewardType.Null, () =>
    //{

    //});

    public void ShowRewardAds(RewardType rType, Action p)
    {
        //Debug.Log("Enter Show Reward: " + rType.ToString());
        rewardType = rType;
        rewardCallback = p;
        PlayerRewardAds();
    }


    public void StopTime()
    {
        OnApplicationPauseOnAds(true);
    }


    public void ResetTime()
    {
        OnApplicationPauseOnAds(false);
    }

    public void OnApplicationPauseOnAds(bool isPaused)
    {
        Time.timeScale = isPaused ? 0 : originTimeScale;
        AudioListener.pause = isPaused;
    }


    private static void MyOnRemoteConfigsUpdateFunction()
    {

        //需要手动填写 Remote数据 Example("INTER_ADS", "30"),"30"
        INTER_ADS = float.Parse(GameAnalytics.GetRemoteConfigsValueAsString("INTER_ADS", "60"));
        START_ADS_LV = int.Parse(GameAnalytics.GetRemoteConfigsValueAsString("START_ADS_LV", "2"));
        START_GARDEN_LV = int.Parse(GameAnalytics.GetRemoteConfigsValueAsString("START_GARDEN_LV", "999"));
        ABTEST_SWITCH = int.Parse(GameAnalytics.GetRemoteConfigsValueAsString("ABTEST_SWITCH", "0"));
        AFK_ADS = int.Parse(GameAnalytics.GetRemoteConfigsValueAsString("AFK_ADS", "20"));
        FORCE_ADS = int.Parse(GameAnalytics.GetRemoteConfigsValueAsString("FORCE_ADS", "15"));
        Debug.Log(INTER_ADS + "_" + START_ADS_LV);
        Debug.Log(AFK_ADS + "_" + FORCE_ADS);
    }

    bool crunning = false;

    IEnumerator ForceInter;
    public void showForceInterstitial()
    {
        if (crunning == false)
        {
            ForceInter = ForceInterTimer();
            StartCoroutine(ForceInter);
        }
        
    }

    public void stopForceInterstitial()
    {
        crunning = false;
        StopCoroutine(ForceInter);
        
    }


    IEnumerator ForceInterTimer()
    {
        crunning = true;
        float t = 0;
        bool showCount = false;
        bool showAds = false;
        GameObject countDownparent = GameObject.Find("CountDown");
        GameObject countDown = countDownparent.transform.Find("CountDown").gameObject;

        while (t < FORCE_ADS)
        {
            t += Time.deltaTime;

            if(showAds == false)
            {
                //DebugManager.instance.updateForceTimeInterval(t);
            }
            
            if (showCount == false && t > (FORCE_ADS - 5))
            {
                showCount = true;
                countDown.SetActive(true);
                

            }
            if(t > (FORCE_ADS - 5)&&showAds==false)
            {
                countDown.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = Mathf.Ceil(FORCE_ADS - t).ToString();
                countDown.transform.GetChild(0).GetComponent<Image>().fillAmount = (1 - (FORCE_ADS - t) / 5f);
            }

            if (showAds == false && t > (FORCE_ADS - 0.1f))
            {
                showAds = true;
                //DebugManager.instance.resetForceTime();
                countDown.SetActive(false);
                crunning = false;
                ShowInterstitial();

                showForceInterstitial();


                yield return null;
            }
            yield return null;
        }

        yield return null;



    }
    
    public virtual void HideBanner()
    {
        
    }



}



