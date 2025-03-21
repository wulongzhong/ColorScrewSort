﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 启动界面是单独的特殊UI，手写动画过渡等相关的，不要使用插件，并且直接通过单例调用，因为不被UI管理器所管理
/// </summary>
public class Splash : MonoBehaviour
{
    public enum ProgressState
    {
        Launch = 0,
        LoadTable = 10,
        LoadGameScene = 20,
    }

    [Header("启动界面是单独的特殊UI，手写动画过渡等相关的，不要使用插件")]
    [Header("并且直接通过单例调用，因为不被UI管理器所管理")]

    [Space]
    [Header("进度提示文本")]
    public TMPro.TextMeshProUGUI tLoading;

    public Image imgFill;

    public static Splash Instance;
    private int lastProgress = 0;
    private bool bWaitUnloadScene = false;
    private bool bWaitOpenAd = true;

    float startWaitUnloadSceneTime;

    private void Awake()
    {
        Instance = this;
        imgFill.fillAmount = 0;
    }

    public void RefreshProgress(ProgressState progressState, int progress)
    {
        int currProgress = (int)progressState + progress;
        if(currProgress <= lastProgress)
        {
            return;
        }
        lastProgress = currProgress;
        tLoading.text = $"Loading {currProgress}/100";
        imgFill.fillAmount = currProgress * 0.01f;
    }

    private void Update()
    {
        if (bWaitUnloadScene)
        {
#if !UNITY_EDITOR
            if (bWaitOpenAd)
            {
                float tempProgress = Mathf.RoundToInt(lastProgress + (Time.realtimeSinceStartup - startWaitUnloadSceneTime) * 20);
                if(tempProgress > 100)
                {
                    tempProgress = 100;
                }
                tLoading.text = $"Loading {tempProgress}/100";
                imgFill.fillAmount = tempProgress * 0.01f;
                if(tempProgress < 100)
                {
                    return;
                }
            }
#endif
            if (bWaitOpenAd)
            {
                MAXAdsManager.INSTANCE.NotShowOpenAd();
            }
            if(SceneManager.sceneCount > 1)
            {
                if(SceneManager.UnloadSceneAsync(0) != null)
                {
                    bWaitUnloadScene = false;
                }
            }
        }
    }

    public void Finish()
    {
        bWaitUnloadScene = true;
        startWaitUnloadSceneTime = Time.realtimeSinceStartup;
    }

    public void SkipOpenAd()
    {
        bWaitOpenAd = false;
        MAXAdsManager.INSTANCE.NotShowOpenAd();
    }
}