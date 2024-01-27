using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UILevelPlaying : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }

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
            LevelPlayMgr.Instance.CacelMove();
        });
        this.btnAddStick.onClick.AddListener(() => {
            LevelPlayMgr.Instance.AddStick();
        });
        this.btnRestart.onClick.AddListener(() => {
            LevelPlayMgr.Instance.RefreshLevel();
        });

        if(NormalDataHandler.Instance.CurrIsNormalLevel)
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
}