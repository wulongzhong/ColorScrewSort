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

        GpEventMgr.Instance.Register<LevelPlayMgr.LevelWinEvent>(this, (evt) => {

        });

        GpEventMgr.Instance.Register<LevelPlayMgr.LevelWinEvent>(this, (evt) => {

        });

        GpEventMgr.Instance.Register<LevelPlayMgr.LevelWinEvent>(this, (evt) => {

        });

        this.btnSkip.onClick.AddListener(() => {

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