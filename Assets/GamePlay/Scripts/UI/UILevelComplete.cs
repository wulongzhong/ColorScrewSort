using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UILevelComplete : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }

    public override void OnInit()
    {
        base.OnInit();

        this.btnNextLevel.onClick.AddListener(() =>
        {
            UIMgr.Instance.CloseUI<UILevelComplete>(true);
            LevelPlayMgr.Instance.LoadNextLevel();
            UIMgr.Instance.OpenUI<UIMain>();
        });
        PlayAni();
    }

    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
    }

    private async void PlayAni()
    {
        NormalDataHandler.Instance.GoldCount += 10;
        topDiamondUI.PlayDiamondFly();
        await UniTask.Delay(1200);
        //play silder;
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
