using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


using Assets.Scripts.Common;

namespace Assets.Scripts.Manager
{

    public class OBusGAManager : MonoBehaviour
    {

        public static OBusGAManager Instance = null;


        //public string region = "";
        public long appId = 0;
        public string appKey = "";
        public string appSecret = "";

        public string packName = "Crowd Rush 3D"; // com.katanlabs.bridgerunio

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (Instance == null)
            {
                Instance = this;

                InitSDK();

                DontDestroyOnLoad(gameObject);
            }


        }


        bool _isInit = false;

        //private AndroidJavaClass trackApi;

        public void InitSDK()
        {
            if (_isInit) return;
            _startTime = CommonTools.ConvertTimeStamp(DateTime.Now);

            Debug.Log("Game Start Time: " + (int)_startTime);

#if UNITY_ANDROID && !UNITY_EDITOR
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var app = currentActivity.Call<AndroidJavaObject>("getApplication");
            Debug.Log("OBus Application" + app);
            var sdkInit = new AndroidJavaClass("com.katanlabs.obusinit.OBusSDKInit");
            sdkInit.SetStatic("appIn", app);
            sdkInit.CallStatic("SetDefaultRegion");
            sdkInit.SetStatic("AppId", appId);
            Debug.Log("OBus App Id: " + sdkInit.GetStatic<long>("AppId"));
            sdkInit.SetStatic("AppKey", appKey);
            sdkInit.SetStatic("AppSecret", appSecret);
            sdkInit.CallStatic("InitOBusSDK");
            GameStart();
#endif

            _isInit = true;
            Debug.Log("OBus GA Init Success!");

        }


        //float timer = 0;

        long _startTime = CommonTools.DefaultTimeStamp();

        public string GetDeviceCate()
        {
            string device = "phone";
            if (SystemInfo.deviceType != DeviceType.Handheld)
            {
                device = "pc";
            }
            else
            {
                if (SystemInfo.deviceModel.StartsWith("iPad"))
                {
                    device = "tablet";
                }
            }
            return device;
        }

        public void GameStart()
        {

            var sdkInit = new AndroidJavaClass("com.katanlabs.obusinit.OBusSDKInit");
            sdkInit.CallStatic("CreateMsg");
            sdkInit.CallStatic("SetMsg", "game_id", "");
            sdkInit.CallStatic("SetMsg", "pack_name", packName);
            sdkInit.CallStatic("SetMsg", "gaid", "");
            sdkInit.CallStatic("SetMsg", "device_category", GetDeviceCate());
            sdkInit.CallStatic("SetMsg", "enter_id", "");
            sdkInit.CallStatic("SetMsgInt", "client_time", (int)CommonTools.ConvertTimeStamp(DateTime.Now));
            sdkInit.CallStatic("PostMsg", "10001", "101");
            sdkInit.CallStatic("CompleteMsg");

            Debug.Log("========== Game Start OBus");
        }


        public void GameQuit()
        {
            var nowTime = CommonTools.ConvertTimeStamp(DateTime.Now);
            var sdkInit = new AndroidJavaClass("com.katanlabs.obusinit.OBusSDKInit");
            sdkInit.CallStatic("CreateMsg");
            sdkInit.CallStatic("SetMsg", "game_id", "");
            sdkInit.CallStatic("SetMsg", "pack_name", packName);
            sdkInit.CallStatic("SetMsg", "gaid", "");
            sdkInit.CallStatic("SetMsg", "device_category", GetDeviceCate());
            sdkInit.CallStatic("SetMsg", "uin", "");
            sdkInit.CallStatic("SetMsg", "role_name", "");
            sdkInit.CallStatic("SetMsg", "role_id", "");
            sdkInit.CallStatic("SetMsgInt", "allonline_time", nowTime - _startTime);
            sdkInit.CallStatic("SetMsgInt", "client_time", (int)nowTime);
            sdkInit.CallStatic("PostMsg", "60001", "601");
            sdkInit.CallStatic("CompleteMsg");

            Debug.Log("========== Game Quit OBus");

        }

        private void OnApplicationQuit()
        {
            GameQuit();
        }



//        public void LevelStart()
//        {
//#if UNITY_ANDROID
//            var sdkInit = new AndroidJavaClass("com.obus.OBusSDKInit");
//            sdkInit.CallStatic("CreateMsg");
//            sdkInit.CallStatic("SetMsg", "ProductName", "Computer");
//            Debug.Log("Res: " + sdkInit.CallStatic<string>("GetMsg", "ProductName"));
//#endif
//            Debug.Log("Success Call LevelStart!");
//        }


        // Start is called before the first frame update
        void Start()
        {

        }


        //float timer = 0;
        //public void TestProcess()
        //{
        //    timer += Time.deltaTime;

        //    if (timer > 1)
        //    {
        //        timer -= 1;
        //        LevelStart();
        //    }

        //}

        // Update is called once per frame
        void Update()
        {
            //timer += Time.deltaTime;

            //TestProcess();
        }
    }




}
