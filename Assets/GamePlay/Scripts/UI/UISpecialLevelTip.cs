using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UISpecialLevelTip : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }

    public override void OnInit()
    {
        base.OnInit();

        this.btnSkip.onClick.AddListener(OnClickSkip);
        this.btnPlay.onClick.AddListener(OnClickPlay);
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

    private void OnClickPlay()
    {
        UIMgr.Instance.CloseUI<UISpecialLevelTip>(true);
        LevelPlayMgr.Instance.LoadNextSpecialLevel();
        UIMgr.Instance.OpenUI<UIMain>();
    }

    private void OnClickSkip()
    {
        UIMgr.Instance.CloseUI<UISpecialLevelTip>(true);
        LevelPlayMgr.Instance.LoadNextNormalLevel();
        UIMgr.Instance.OpenUI<UIMain>();
    }
}