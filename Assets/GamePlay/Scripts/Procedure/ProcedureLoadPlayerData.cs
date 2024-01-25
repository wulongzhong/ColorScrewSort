using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureLoadPlayerData : ProcedureBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        PlayerLocalDataMgr.Instance.Init();
        this.ProcedureMgr.ChangeProcedure<ProcedureLoadScene>();
    }
}
