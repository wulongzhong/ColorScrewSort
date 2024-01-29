using System;
using System.Collections;
using System.Collections.Generic;
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

        });
    }

    private void RefreshState()
    {

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