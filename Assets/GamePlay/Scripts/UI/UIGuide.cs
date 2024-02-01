using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIGuide : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }

    public override void OnInit()
    {
        base.OnInit();

        if(NormalDataHandler.Instance.CurrNormalLevelId == 1)
        {
            PlayGuide1();
        }
        else if(NormalDataHandler.Instance.CurrNormalLevelId == 2)
        {
            PlayGuide2();
        }
    }

    private async void PlayGuide1()
    {
        this.guide1.gameObject.SetActive(true);
        bool bWaitPop = true;
        GpEventMgr.Instance.Register<LevelPlayMgr.LevelPopNutEvent>(this, (evtarg) => { bWaitPop = false; });
        while (bWaitPop)
        {
            await UniTask.Delay(50);
        }
        bWaitPop = true;
        GpEventMgr.Instance.Register<LevelPlayMgr.LevelMoveNutEvent>(this, (evtarg) => { bWaitPop = false; });
        guide1.Find("GuidePromptText").gameObject.SetActive(false);
        guide1.anchoredPosition = new Vector2(150, -172);
        while (bWaitPop)
        {
            await UniTask.Delay(50);
        }
        UIMgr.Instance.CloseUI<UIGuide>(false);
    }

    private async void PlayGuide2()
    {
        this.guide2.gameObject.SetActive(true);
        guide2.Find("NormalHand").GetComponent<Image>().enabled = false;
        guide2.Find("TouchlHand").GetComponent<Image>().enabled = false;
        bool bWaitWin = true;
        GpEventMgr.Instance.Register<LevelPlayMgr.LevelWinEvent>(this, (evtarg) => { bWaitWin = false; });
        while (bWaitWin)
        {
            await UniTask.Delay(50);
        }
        UIMgr.Instance.CloseUI<UIGuide>(false);
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