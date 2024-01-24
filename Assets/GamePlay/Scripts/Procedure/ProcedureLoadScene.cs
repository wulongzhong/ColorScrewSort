using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ProcedureLoadScene : ProcedureBase
{
    SceneLoader sceneLoader;
    private AsyncOperationHandle asyncOperationHandle;

    public override void OnInit()
    {
        base.OnInit();
    }
    public override void OnEnter()
    {
        base.OnEnter();
        if(sceneLoader != null)
        {
            sceneLoader.Dispose();
            sceneLoader = null;
        }
        sceneLoader = new SceneLoader();
        Splash.Instance.RefreshProgress(Splash.ProgressState.LoadGameScene, 0);

        asyncOperationHandle = sceneLoader.LoadScene("Assets/GamePlay/Scenes/Game.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);

    }

    private void Update()
    {
        if(!asyncOperationHandle.IsDone)
        {
            return;
        }
        ((SceneInstance)asyncOperationHandle.Result).ActivateAsync();
        Splash.Instance.RefreshProgress(Splash.ProgressState.LoadGameScene, 9);
        Splash.Instance.Finish();
        this.ProcedureMgr.ChangeProcedure<ProcedureGamePlay>();
    }
}