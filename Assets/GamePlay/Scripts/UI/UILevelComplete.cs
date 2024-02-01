using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UILevelComplete : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }

    private bool animing = true;

    private int newThemeId;
    bool addPropUndo;

    public override void OnInit()
    {
        base.OnInit();

        addPropUndo = UnityEngine.Random.value > 0.5f;

        this.btnNextLevel.onClick.AddListener(() =>
        {
            if (!animing)
            {
                if (NormalDataHandler.Instance.CurrIsNormalLevel && !NormalDataHandler.Instance.CurrNormalLevelIsHard && NormalDataHandler.Instance.CurrNormalLevelId - 1 == NormalDataHandler.Instance.NextSpecialLevelOpenId)
                {
                    UIMgr.Instance.CloseUI<UILevelComplete>(true);
                    UIMgr.Instance.OpenUI<UISpecialLevelTip>();
                }
                else
                {
                    if (NormalDataHandler.Instance.CurrIsNormalLevel && !NormalDataHandler.Instance.CurrNormalLevelIsHard
                        && LevelPlayMgr.Instance.levelData.TypeLevel == ConfigPB.LevelType.Mask
                    )
                    {
                        NormalDataHandler.Instance.NextSpecialLevelOpenId = NormalDataHandler.Instance.CurrNormalLevelId + 1;
                    }

                    UIMgr.Instance.CloseUI<UILevelComplete>(true);
                    LevelPlayMgr.Instance.LoadNextNormalLevel();
                    UIMgr.Instance.OpenUI<UIMain>();
                    if(NormalDataHandler.Instance.CurrNormalLevelId > 2)
                    {
                        BaseAdsManager.INSTANCE.ShowNormalInterstitial();
                    }
                }
            }
        });

        this.btnClaimChest.onClick.AddListener(() => {
            OnClickClaimChest();
        });
        this.btnGetAllChest.onClick.AddListener(() =>
        {
            if (BaseAdsManager.INSTANCE.RewardedAdsIsReady())
            {
                BaseAdsManager.INSTANCE.ShowRewardAds(BaseAdsManager.RewardType.LevelWinProp, () =>
                {
                    OnClickClaimChest();
                });
            }
        });

        this.btnLaterTheme.onClick.AddListener(OnClickLaterTheme);
        this.btnClaimTheme.onClick.AddListener(OnClickUseTheme);

        this.btnWatchAds.onClick.AddListener(() =>
        {
            BaseAdsManager.INSTANCE.ShowRewardAds(BaseAdsManager.RewardType.LevelWinGem, () =>
            {
                NormalDataHandler.Instance.GoldCount += 40;
                topDiamondUI.PlayDiamondFly();
                this.btnWatchAds.interactable = false;
                this.btnWatchAds.transform.localPosition = new Vector3(-1000, 0, 0);
            });
        });

        PlayAni();
    }

    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        this.tfLevelChest.gameObject.SetActive(false);
        this.tfThemeProcess.gameObject.SetActive(false);
    }

    private async void PlayAni()
    {
        animing = true;
        NormalDataHandler.Instance.GoldCount += 10;
        topDiamondUI.PlayDiamondFly();
        themeIcon.sprite = iconTheme.sprite = resLoader.LoadAsset<Sprite>(DTTheme.Instance.GetThemeByID(NormalDataHandler.Instance.CurrThemeFragId).IconResPath);
        await UniTask.Delay(500);
        //play silder;
        this.tfLevelChest.gameObject.SetActive(true);
        this.tfThemeProcess.gameObject.SetActive(NormalDataHandler.Instance.CurrNormalLevelId >= 5);
        this.tfLevelChest.transform.localScale = Vector3.zero;
        this.tfThemeProcess.transform.localScale = Vector3.zero;
        this.tfLevelChest.transform.DOScale(1, 0.25f);
        if(NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            this.tfThemeProcess.transform.DOScale(1, 0.25f);

        this.fillChest.fillAmount = NormalDataHandler.Instance.CurrChestFragCount * 1.0f / 4;
        if (NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            this.fillTheme.fillAmount = NormalDataHandler.Instance.CurrThemeFragCount * 1.0f / 5;

        this.tChest.text = $"{NormalDataHandler.Instance.CurrChestFragCount}/4";
        if (NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            this.tTheme.text = $"{NormalDataHandler.Instance.CurrThemeFragCount}/5";

        await UniTask.Delay(400);

        NormalDataHandler.Instance.CurrChestFragCount++;
        if (NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            NormalDataHandler.Instance.CurrThemeFragCount++;

        this.fillChest.DOFillAmount(NormalDataHandler.Instance.CurrChestFragCount * 1.0f / 4, 0.6f);
        if (NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            this.fillTheme.DOFillAmount(NormalDataHandler.Instance.CurrThemeFragCount * 1.0f / 5, 0.6f);

        await UniTask.Delay(600);

        this.tChest.text = $"{NormalDataHandler.Instance.CurrChestFragCount}/4";
        if (NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            this.tTheme.text = $"{NormalDataHandler.Instance.CurrThemeFragCount}/5";

        await UniTask.Delay(1000);

        if (NormalDataHandler.Instance.CurrChestFragCount >= 4)
        {
            PopupWin.gameObject.SetActive(false);
            PopupRewardChest.gameObject.SetActive(true);
            NormalDataHandler.Instance.CurrChestFragCount -= 4;

            tfGem.gameObject.SetActive(false);
            tfUndo.gameObject.SetActive(false);
            tfAddRow.gameObject.SetActive(false);

            var gemPos = tfGem.anchoredPosition;
            tfGem.anchoredPosition = Vector3.zero;

            var undoPos = tfUndo.anchoredPosition;
            tfUndo.anchoredPosition = Vector3.zero;

            var addRowPos = tfAddRow.anchoredPosition;
            tfAddRow.anchoredPosition = Vector3.zero;

            await UniTask.Delay(834);

            Par_F.Play();

            await UniTask.Delay(1000);
            idleOpenChest.DOPlay();

            tfGem.gameObject.SetActive(true);
            tfUndo.gameObject.SetActive(addPropUndo);
            tfAddRow.gameObject.SetActive(!addPropUndo);

            tfGem.DOAnchorPos(gemPos, 0.4f);
            if (addPropUndo)
            {
                tfUndo.DOAnchorPos(undoPos, 0.4f);
            }
            else
            {
                tfAddRow.DOAnchorPos(addRowPos, 0.4f);
            }
            await UniTask.Delay(400);
            fxGem.Play();
            if (addPropUndo)
            {
                fxUndo.Play();
            }
            else
            {
                fxAddRow.Play();
            }

            while (PopupRewardChest.gameObject.activeSelf)
            {
                await UniTask.Delay(200);
            }
            await UniTask.Delay(200);
        }

        if (NormalDataHandler.Instance.CurrThemeFragCount >= 5)
        {
            newThemeId = NormalDataHandler.Instance.CurrThemeFragId;
            PopupWin.gameObject.SetActive(false);
            PopupRewardTheme.gameObject.SetActive(true);
            NormalDataHandler.Instance.CurrThemeFragCount -= 5;
            NormalDataHandler.Instance.AddUnlockedBackGroundId(newThemeId);

            while (PopupRewardTheme.gameObject.activeSelf)
            {
                await UniTask.Delay(200);
            }
            await UniTask.Delay(200);
        }

        animing = false;
    }

    private bool bGetedChest = false;
    private async void OnClickClaimChest()
    {
        if (bGetedChest)
        {
            return;
        }
        bGetedChest = true;
        tfGem.DOAnchorPosX(0, 0.2f);
        if (tfUndo.gameObject.activeSelf)
        {
            tfUndo.DOAnchorPosX(0, 0.2f);
        }

        if (tfAddRow.gameObject.activeSelf)
        {
            tfAddRow.DOAnchorPosX(0, 0.2f);
        }
        

        await UniTask.Delay(250);

        NormalDataHandler.Instance.GoldCount += 20;

        topDiamondUI.PlayDiamondFly();
        await UniTask.Delay(500);
        PopupRewardChest.gameObject.SetActive(false);
        PopupWin.GetComponent<Animator>().enabled = false;
        PopupWin.gameObject.SetActive(true);
    }

    private async void GetAll()
    {
        if (bGetedChest)
        {
            return;
        }
        bGetedChest = true;
        tfGem.DOAnchorPosX(0, 0.2f);
        if (tfUndo.gameObject.activeSelf)
        {
            tfUndo.DOAnchorPosX(0, 0.2f);
        }

        if (tfAddRow.gameObject.activeSelf)
        {
            tfAddRow.DOAnchorPosX(0, 0.2f);
        }
        

        await UniTask.Delay(250);

        NormalDataHandler.Instance.GoldCount += 20;
        if(addPropUndo)
        {
            NormalDataHandler.Instance.PropUndoCount += 1;
        }
        else
        {
            NormalDataHandler.Instance.PropAddRowCount += 1;
        }
        

        topDiamondUI.PlayDiamondFly();
        await UniTask.Delay(500);
        PopupRewardChest.gameObject.SetActive(false);
        PopupWin.GetComponent<Animator>().enabled = false;
        PopupWin.gameObject.SetActive(true);
    }

    private void OnClickUseTheme()
    {
        NormalDataHandler.Instance.CurrSelectBackGroundId = newThemeId;
        LevelPlayMgr.Instance.RefreshBGSkin();
        PopupRewardTheme.gameObject.SetActive(false);
        PopupWin.gameObject.SetActive(true);
    }
    private void OnClickLaterTheme()
    {
        PopupRewardTheme.gameObject.SetActive(false);
        PopupWin.gameObject.SetActive(true);
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
