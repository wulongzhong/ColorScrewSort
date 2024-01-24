using System;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureMgr : MonoBehaviour
{
    [SerializeField]
    ProcedureBase startProcedure;
    [SerializeField]
    [ReadOnly]
    ProcedureBase currProcedure;

    private Dictionary<Type, ProcedureBase> dicType2Procedures;

    private void Start()
    {
        dicType2Procedures = new Dictionary<Type, ProcedureBase>();
        foreach (ProcedureBase procedure in GetComponents<ProcedureBase>())
        {
            procedure.OnInit();
            dicType2Procedures[procedure.GetType()] = procedure;
        }

        currProcedure = startProcedure;
        OnEnterProcedure();
    }

    public ProcedureBase ChangeProcedure<TProcedure>() where TProcedure : ProcedureBase
    {
        if(currProcedure != null)
        {
            currProcedure.OnExit();
        }
        foreach(var kv in dicType2Procedures)
        {
            if (kv.Key.Equals(typeof(TProcedure)))
            {
                currProcedure = kv.Value;
                OnEnterProcedure();
                return kv.Value;
            }
        }
#if ENABLE_LOG
        Debug.LogError($"加载流程失败:{(typeof(TProcedure)).FullName}");
#endif
        return null;
    }

    public void OnEnterProcedure()
    {
        currProcedure.ProcedureMgr = this;
        currProcedure.OnEnter();
    }
}