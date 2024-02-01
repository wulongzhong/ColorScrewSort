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
        // ����getInstance������ȡDomainNameUtilsʵ������
        AndroidJavaObject domainNameUtilsInstance =
        domainNameUtilsClass.CallStatic<AndroidJavaObject>("getInstance");
        // ����excludeCNDomain����
        domainNameUtilsInstance.Call("excludeCNDomain");
#endif


        GameAnalytics.Initialize();
    }
}