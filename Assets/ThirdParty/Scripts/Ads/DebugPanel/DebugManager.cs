using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{

    public static DebugManager instance;

    public GameObject debugPanel,fps,button;

    public float AfkAB,FAB , InterAB;

    public float AfkRemote, FRemote, InterRemote;

    public float AfkDefault,FDefault, InterDefault;

    public Text Afk_time, F_time, Inter_time, Banner_time,Banner_hist;

    public Text afkab, fab, interab;

    public Text afkremote, fremote, interremote;

    public Text afkdefault, fdefault, interdefault;

    public Text AdsProvider,reward;

    private float interTime,afkTime,bannertime, forceTime;

    private Queue<float> timeQueue;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else
        {

            Destroy(gameObject);

        }
    }
    


    private void Start()
    {
        if (Debug.isDebugBuild)
        {
            button.SetActive(true);
        }
        timeQueue = new Queue<float>();
    }


    private void Update()
    {
        updateInterTimeInterval();
        updateBannerTimeInterval();
    }


    public void adProvider()
    {
        if (GameObject.Find("ToponAdsManager")!=null)
        {
            AdsProvider.text = "Topon";
        }
        if (GameObject.Find("MAXAdsManager") != null)
        {
            AdsProvider.text = "MAX";
        }
    }


    public void initData(float[]num)
    {
        AfkDefault = num[0];
        FDefault = num[1];
        InterDefault = num[2];
    }
    public void initRemoteData(float[] num)
    {
        AfkRemote = num[0];
        FRemote = num[1];
        InterRemote = num[2];
    }


    void updateTimeInfo()
    {
        if (BaseAdsManager.AFK_ADS != AfkDefault && BaseAdsManager.AFK_ADS != AfkRemote)
        {
            AfkAB = BaseAdsManager.AFK_ADS;
            afkab.text = AfkAB.ToString();
            afkremote.text = "-";
            afkdefault.text = "-";
        }
        else if(BaseAdsManager.AFK_ADS == AfkRemote)
        {
            afkab.text = "-";
            afkremote.text = AfkRemote.ToString();
            afkdefault.text = "-";
        }
        else
        {
            afkab.text = "-";
            afkremote.text = "-";
            afkdefault.text = AfkDefault.ToString();
        }

        if (BaseAdsManager.FORCE_ADS != FRemote && BaseAdsManager.FORCE_ADS != FDefault)
        {
            FAB= BaseAdsManager.FORCE_ADS;
            fab.text = FAB.ToString();
            fremote.text = "-";
            fdefault.text = "-";
        }
        else if(BaseAdsManager.FORCE_ADS == FRemote)
        {
            fab.text = "-";
            fremote.text = FRemote.ToString();
            fdefault.text = "-";
        }
        else 
        {
            fab.text = "-";
            fremote.text = "-";
            fdefault.text = FDefault.ToString();
        }

        if (BaseAdsManager.INTER_ADS !=InterRemote && BaseAdsManager.INTER_ADS != InterDefault)
        {
            InterAB = BaseAdsManager.INTER_ADS;
            interab.text = InterAB.ToString();
            interremote.text = "-";
            interdefault.text = "-";
        }
        else if(BaseAdsManager.INTER_ADS == InterRemote)
        {
            interab.text = "-";
            interremote.text = InterRemote.ToString();
            interdefault.text = "-";
        }
        else
        {
            interab.text = "-";
            interremote.text = "-";
            interdefault.text = InterDefault.ToString();
        }
    }

    void updateInterTimeInterval()
    {
        interTime += Time.deltaTime;
        Inter_time.text = interTime.ToString("f1");

    }

    public void setInterTime()
    {
        interTime = 0f;
        Debug.Log("InterTime");
    }

    public void updateAFKTimeInterval(float timeafk)
    {
        afkTime = timeafk;
        Afk_time.text = afkTime.ToString("f1");
    }

    public void updateForceTimeInterval(float time)
    {
        forceTime = time;
        F_time.text = forceTime.ToString("f1");
    }

    public void resetForceTime()
    {
        forceTime = 0f;
        F_time.text = forceTime.ToString("f1");
        
    }

    bool trigger=false;
    void updateBannerTimeInterval()
    {
        if (trigger == true)
        {
            trigger = false;
            updateBannerTime();

        }
        bannertime += Time.deltaTime;
        Banner_time.text = bannertime.ToString("f1");

    }
    
    public void setBannerTime()
    {
        timeQueue.Enqueue(bannertime);
        bannertime = 0f;
        trigger = true;
        if (timeQueue.Count == 4)
        {
            timeQueue.Dequeue();
        }
    }

    public void updateBannerTime()
    {
        string temp ="";

        foreach(float num in timeQueue)
        {
            temp += num.ToString("f1")+"__";
        }
        Banner_hist.text = temp;
    }

    public void showReward()
    {
        //BaseAdsManager.INSTANCE.ShowRewardAds(BaseAdsManager.RewardType.Test, () =>
        //{
        //    reward.color = Color.yellow;
        //    StartCoroutine(DelaySetColorBack());
        //});


    }

    IEnumerator DelaySetColorBack()
    {
        yield return new WaitForSeconds(2f);
        reward.color = Color.white;
    }


    #region panel control
    public void onClickDebugPanel()
    {
        
        if (debugPanel.activeSelf == true)
        {
            debugPanel.SetActive(false);
        }
        else
        {
            updateTimeInfo();
            adProvider();
            debugPanel.SetActive(true);

        }
    }
    public void onClickFps()
    {
        if (fps.activeSelf == true)
        {
            fps.SetActive(false);
        }
        else
        {
            fps.SetActive(true);
        }
    }
    #endregion

    public void test()
    {
        BaseAdsManager.INSTANCE.showForceInterstitial();    
    }



    #region Custom

    public void addGold()
    {

    }

    public void skipLevel()
    {

    }

    #endregion

}
