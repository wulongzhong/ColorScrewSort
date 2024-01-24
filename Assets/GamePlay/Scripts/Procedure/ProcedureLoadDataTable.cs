using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureLoadDataTable : ProcedureBase
{
    public override void OnEnter()
    {
        Splash.Instance.RefreshProgress(Splash.ProgressState.LoadTable, 0);
        base.OnEnter();
        Splash.Instance.RefreshProgress(Splash.ProgressState.LoadTable, 1);
        DataTableMgr.Instance.InitDataTable();
        Splash.Instance.RefreshProgress(Splash.ProgressState.LoadTable, 2);
        this.ProcedureMgr.ChangeProcedure<ProcedureLoadScene>();
    }
}