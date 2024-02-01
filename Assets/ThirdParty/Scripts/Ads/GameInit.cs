using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;
using System;
using UnityEngine.UI;
using TMPro;

public class GameInit : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        AndroidJavaClass domainNameUtilsClass = new
        AndroidJavaClass("com.mbridge.msdk.foundation.same.DomainNameUtils");
        // 调用getInstance方法获取DomainNameUtils实例对象
        AndroidJavaObject domainNameUtilsInstance =
        domainNameUtilsClass.CallStatic<AndroidJavaObject>("getInstance");
        // 调用excludeCNDomain方法
        domainNameUtilsInstance.Call("excludeCNDomain");
#endif
        
        
        GameAnalytics.Initialize();
       
    }


    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LoadSceneAsync());
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
#endif

    }
}
