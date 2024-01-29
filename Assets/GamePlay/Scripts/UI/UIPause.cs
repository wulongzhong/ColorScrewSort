using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPause : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }

    public override void OnInit()
    {
        base.OnInit();

        this.btnBack.onClick.AddListener(() =>
        {
            UIMgr.Instance.CloseUI<UIPause>(false);
        });
        this.btnHaptic.onClick.AddListener(() => { });
        this.btnSound.onClick.AddListener(() => { });
        this.btnHome.onClick.AddListener(() => {
            UIMgr.Instance.CloseUI<UIPause>(false);
        });
        this.btnRetry.onClick.AddListener(() => {
            NormalDataHandler.Instance.CurrNormalLevelIsHard = false;
            UIMgr.Instance.CloseUI<UIPause>(false);
            UIMgr.Instance.CloseUI<UILevelPlaying>(false);
            UIMgr.Instance.OpenUI<UIMain>();
            LevelPlayMgr.Instance.LoadLevel(NormalDataHandler.Instance.CurrNormalLevelId, false);
        });
    }

    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        LevelPlayMgr.Instance.bPause = true;
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
        LevelPlayMgr.Instance.bPause = false;
    }
}