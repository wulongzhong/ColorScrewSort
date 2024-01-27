using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureGamePlay : ProcedureBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        UIMgr.Instance.OpenUI<UIMain>();
        LevelPlayMgr.Instance.Init();
    }
}
