using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ProcedureLoadScene : ProcedureBase
{
    SceneLoader sceneLoader;
    private AsyncOperationHandle asyncOperationHandle;
    private bool bActive = false;

    public override void OnInit()
    {
        base.OnInit();
    }
    public override void OnEnter()
    {
        base.OnEnter();
        bActive = false;
        if (sceneLoader != null)
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

        if (!bActive)
        {
            ((SceneInstance)asyncOperationHandle.Result).ActivateAsync();
            bActive = true;
        }

        if (!((SceneInstance)asyncOperationHandle.Result).Scene.isLoaded)
        {
            return;
        }

        Splash.Instance.RefreshProgress(Splash.ProgressState.LoadGameScene, 9);
        Splash.Instance.Finish();
        this.ProcedureMgr.ChangeProcedure<ProcedureGamePlay>();
    }
}