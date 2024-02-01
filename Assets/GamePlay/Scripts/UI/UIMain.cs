using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public partial class UIMain : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }
    public override void OnInit()
    {
        base.OnInit();

        btnRemoveADs.onClick.AddListener(() =>
        {

        });

        btnFreeGem.onClick.AddListener(() =>
        {
            if (BaseAdsManager.INSTANCE.RewardedAdsIsReady())
            {
                BaseAdsManager.INSTANCE.ShowRewardAds(BaseAdsManager.RewardType.LevelWinProp, () =>
                {
                    var key = (ConfigPB.GlobalCfg.KeyType)((int)ConfigPB.GlobalCfg.KeyType.StoreAd1GoldCount + NormalDataHandler.Instance.StoreAdWatchCount);
                    var addCount = DTGlobalCfg.Instance.GetIntByKey(key);
                    NormalDataHandler.Instance.GoldCount += addCount;
                    NormalDataHandler.Instance.StoreAdWatchCount++;
                    RefreshFreeGem();
                });
            }
        });


        btnCustomize.onClick.AddListener(() =>
        {
            UIMgr.Instance.CloseUI<UIMain>(true);
            UIMgr.Instance.OpenUI<UIShop>();
        });

        btnTapToStart.onClick.AddListener(() =>
        {
            LevelPlayMgr.Instance.bWaitPlay = false;
            UIMgr.Instance.CloseUI<UIMain>(true);
            UIMgr.Instance.OpenUI<UILevelPlaying>();
        });

        RefreshFreeGem();
    }
    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void OnClose(bool bRecycle = true)
    {
        base.OnClose(bRecycle);
        if (EventHandlerIndex != 0)
        {
            this.ClearUpEventHandle();
        }
    }

    private void RefreshFreeGem()
    {
        if(NormalDataHandler.Instance.StoreAdWatchCount >= 4)
        {
            btnFreeGem.gameObject.SetActive(false);
            return;
        }
        var key = (ConfigPB.GlobalCfg.KeyType)((int)ConfigPB.GlobalCfg.KeyType.StoreAd1GoldCount + NormalDataHandler.Instance.StoreAdWatchCount);
        var addCount = DTGlobalCfg.Instance.GetIntByKey(key);
        tFreeGem.text = addCount.ToString();
    }
}