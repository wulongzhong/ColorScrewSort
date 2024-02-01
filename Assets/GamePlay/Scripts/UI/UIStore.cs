using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIStore : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }
    public override void OnInit()
    {
        base.OnInit();
        this.btnClose.onClick.AddListener(()=>UIMgr.Instance.CloseUI<UIStore>(false));
        this.btnClose2.onClick.AddListener(()=>UIMgr.Instance.CloseUI<UIStore>(false));
        this.btnWatch.onClick.AddListener(() => {
            if (BaseAdsManager.INSTANCE.RewardedAdsIsReady())
            {
                BaseAdsManager.INSTANCE.ShowRewardAds(BaseAdsManager.RewardType.LevelWinProp, () =>
                {
                    var key = (ConfigPB.GlobalCfg.KeyType)((int)ConfigPB.GlobalCfg.KeyType.StoreAd1GoldCount + NormalDataHandler.Instance.StoreAdWatchCount);
                    var addCount = DTGlobalCfg.Instance.GetIntByKey(key);
                    NormalDataHandler.Instance.GoldCount += addCount;
                    NormalDataHandler.Instance.StoreAdWatchCount++;
                    RefreshState();
                    topDiamondUI.PlayDiamondFly();
                });
            }
        });
        RefreshState();
        topDiamondUI.RefreshDiamondCount();
    }

    private void RefreshState()
    {
        int watchCount = NormalDataHandler.Instance.StoreAdWatchCount;
        for(int i = 0; i < tfContent.childCount; ++i)
        {
            var tf = tfContent.GetChild(i);
            tf.Find("ImageSelected").gameObject.SetActive(i == watchCount);
            tf.Find("ImageClaimed").gameObject.SetActive(i < watchCount);
            var key = (ConfigPB.GlobalCfg.KeyType)((int)ConfigPB.GlobalCfg.KeyType.StoreAd1GoldCount + i);
            tf.Find("ImageGem").Find("Value").GetComponent<TextMeshProUGUI>().text = DTGlobalCfg.Instance.GetIntByKey(key).ToString();
        }
        btnWatch.interactable = watchCount < tfContent.childCount;
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
}