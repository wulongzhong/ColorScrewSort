using Cysharp.Threading.Tasks;
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

        if(NormalDataHandler.Instance.CurrNormalLevelId <= 4)
        {
            btnSkip.gameObject.SetActive(false);
            btnRollBack.gameObject.SetActive(false);
            btnAddStick.gameObject.SetActive(false);
            btnRestart.gameObject.SetActive(false);

            if(NormalDataHandler.Instance.CurrNormalLevelId <= 2)
            {
                UIMgr.Instance.OpenUI<UIGuide>();
            }
        }
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

    public void RefreshLeveInfo()
    {
        if (NormalDataHandler.Instance.CurrIsNormalLevel)
        {
            tLevel.enabled = true;
            step.gameObject.SetActive(true);

            tLevel.text = NormalDataHandler.Instance.CurrNormalLevelId.ToString();
            step.GetChild(0).gameObject.SetActive(!NormalDataHandler.Instance.CurrNormalLevelIsHard);
            step.GetChild(1).gameObject.SetActive(NormalDataHandler.Instance.CurrNormalLevelIsHard);
        }
        else
        {
            tLevel.enabled = false;
            step.gameObject.SetActive(false);
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
        UIMgr.Instance.CloseUI<UILevelPlaying>(true);
        
        if (bFinishLevel)
        {
            UIMgr.Instance.OpenUI<UILevelComplete>(null);
        }
        else
        {
            LevelPlayMgr.Instance.LoadNextLevel();
            UIMgr.Instance.OpenUI<UIMain>(null);
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