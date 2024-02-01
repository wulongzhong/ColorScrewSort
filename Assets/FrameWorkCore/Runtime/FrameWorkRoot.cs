using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameWorkRoot : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

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
}