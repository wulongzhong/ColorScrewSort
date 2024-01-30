using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UILevelPlaying : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }

    private bool bCurrGetPropUndo = false;

    public override void OnInit()
    {
        base.OnInit();

        GpEventMgr.Instance.Register<LevelPlayMgr.LevelWinEvent>(this, (evtArg) => {
            LevelPlayMgr.LevelWinEvent evt = (LevelPlayMgr.LevelWinEvent)evtArg;
            PlayWin(evt.bSkip, evt.bFinishLevel);
        });

        GpEventMgr.Instance.Register<StickBev.StickEndEvent>(this, (evtArg) => {
            fxNormal.Play();
        });

        GpEventMgr.Instance.Register<LevelPlayMgr.LevelNotContinueEvent>(this, (evtArg) => {
            if (!this.tfNoMorePossibleMove.gameObject.activeSelf)
            {
                this.tfNoMorePossibleMove.gameObject.SetActive(true);
                this.tfNoMorePossibleMove.anchoredPosition = Vector2.zero;
                this.tfNoMorePossibleMove.DOAnchorPosY(100, 1.0f).OnComplete(() =>
                {
                    this.tfNoMorePossibleMove.gameObject.SetActive(false);
                });
            }
            if (PlayerLocalCacheMgr.instance.IsPlayAddStickTip)
            {
                PlayerLocalCacheMgr.instance.IsPlayAddStickTip = false;
                tfAddStickTip.gameObject.SetActive(true);
            }
        });

        GpEventMgr.Instance.Register<LevelPlayMgr.LevelMoveNutEvent>(this, (evtArg) => {

        });

        this.btnSkip.onClick.AddListener(() => {
            LevelPlayMgr.Instance.SkipLevel();
        });
        this.btnRollBack.onClick.AddListener(() => {
            if(NormalDataHandler.Instance.PropUndoCount > 0)
            {
                LevelPlayMgr.Instance.CacelMove();
                RefreshPropCount();
            }
            else
            {
                bCurrGetPropUndo = true;
                GetMoveProp();
            }
        });
        this.btnAddStick.onClick.AddListener(() => {
            if (NormalDataHandler.Instance.PropAddRowCount > 0)
            {
                LevelPlayMgr.Instance.AddStick();
                RefreshPropCount();
            }
            else
            {
                bCurrGetPropUndo = false;
                GetMoveProp();
            }
            tfAddStickTip.gameObject.SetActive(false);
        });
        this.btnRestart.onClick.AddListener(() => {
            LevelPlayMgr.Instance.RefreshLevel();
        });

        this.btnPause.onClick.AddListener(() =>
        {
            UIMgr.Instance.OpenUI<UIPause>();
        });

        this.PopupAddItem.onClick.AddListener(CloseGetMoveProp);
        this.btnCloseMoreItem.onClick.AddListener(CloseGetMoveProp);
        this.btnBuyItem.onClick.AddListener(OnClickBuyItem);
        this.btnWatchAdsItem.onClick.AddListener(OnClickWatchAdsItem);

        RefreshLeveInfo();
        RefreshPropCount();
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

    private async void PlayHardTip()
    {
        tfPopupStartLevel.gameObject.SetActive(true);
        await UniTask.Delay(2375);
        tfPopupStartLevel.gameObject.SetActive(false);
    }

    public void RefreshLeveInfo()
    {
        if (NormalDataHandler.Instance.CurrIsNormalLevel)
        {
            tLevel.enabled = true;
            step.gameObject.SetActive(NormalDataHandler.Instance.CurrNormalLevelId > 4);

            tLevel.text = $"LEVEL {NormalDataHandler.Instance.CurrNormalLevelId}";
            step.GetChild(0).gameObject.SetActive(!NormalDataHandler.Instance.CurrNormalLevelIsHard);
            step.GetChild(1).gameObject.SetActive(NormalDataHandler.Instance.CurrNormalLevelIsHard);

            step.GetChild(2).gameObject.SetActive(!NormalDataHandler.Instance.CurrNormalLevelIsHard);
            step.GetChild(3).gameObject.SetActive(NormalDataHandler.Instance.CurrNormalLevelIsHard);
        }
        else
        {
            tLevel.enabled = false;
            step.gameObject.SetActive(false);
        }

        if (NormalDataHandler.Instance.CurrNormalLevelId <= 4)
        {
            btnSkip.gameObject.SetActive(false);
            btnRollBack.gameObject.SetActive(false);
            btnAddStick.gameObject.SetActive(false);
            btnRestart.gameObject.SetActive(false);

            if (NormalDataHandler.Instance.CurrNormalLevelId <= 2)
            {
                UIMgr.Instance.OpenUI<UIGuide>();
            }
        }

        if (LevelPlayMgr.Instance.levelData.Data.Count >= 8)
        {
            PlayHardTip();
        }
    }

    public void RefreshPropCount()
    {
        this.tRollBack.text = NormalDataHandler.Instance.PropUndoCount.ToString();
        this.tAddStick.text = NormalDataHandler.Instance.PropAddRowCount.ToString();
        this.tfRollAdd.gameObject.SetActive(NormalDataHandler.Instance.PropUndoCount <= 0);
        this.tfStickAdd.gameObject.SetActive(NormalDataHandler.Instance.PropAddRowCount <= 0);
    }

    private async void PlayWin(bool bSkip, bool bFinishLevel)
    {
        if (!bSkip)
        {
            await UniTask.Delay(1000);
        }
        
        fxHard.Play();
        await UniTask.Delay(1200);
        panelVictoryStep.gameObject.SetActive(true);
        await UniTask.Delay(1000);
        winAnimator.Play("VictoryStepback");
        await UniTask.Delay(800);
        
        if (bFinishLevel)
        {
            UIMgr.Instance.CloseUI<UILevelPlaying>(true);
            UIMgr.Instance.OpenUI<UILevelComplete>(null);
        }
        else
        {
            LevelPlayMgr.Instance.LoadNextNormalLevel();
            RefreshLeveInfo();
            RefreshPropCount();
            LevelPlayMgr.Instance.bWaitPlay = false;
        }
    }

    private void GetMoveProp()
    {
        LevelPlayMgr.Instance.bPause = true;
        this.PopupAddItem.gameObject.SetActive(true);

        this.imageItemRollBack.gameObject.SetActive(bCurrGetPropUndo);
        this.imageItemRollBack2.gameObject.SetActive(bCurrGetPropUndo);
        this.imageItemAddStick.gameObject.SetActive(!bCurrGetPropUndo);
        this.imageItemAddStick2.gameObject.SetActive(!bCurrGetPropUndo);

        topDiamondUI.gameObject.SetActive(true);
        topDiamondUI.RefreshDiamondCount();
    }

    private void OnClickWatchAdsItem()
    {

    }

    private void OnClickBuyItem()
    {
        if(NormalDataHandler.Instance.GoldCount < 150)
        {
            return;
        }
        NormalDataHandler.Instance.GoldCount -= 150;

        if (bCurrGetPropUndo)
        {
            NormalDataHandler.Instance.PropUndoCount += 7;
        }
        else
        {
            NormalDataHandler.Instance.PropAddRowCount += 7;
        }
        
        RefreshPropCount();
    }

    public void CloseGetMoveProp()
    {
        LevelPlayMgr.Instance.bPause = false;
        this.PopupAddItem.gameObject.SetActive(false);
        topDiamondUI.gameObject.SetActive(false);
    }
}