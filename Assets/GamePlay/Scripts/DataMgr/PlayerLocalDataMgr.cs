using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System;
using Msg;
using ConfigPB;

public class PlayerLocalDataMgr : MonoBehaviour
{
    public static PlayerLocalDataMgr Instance;

    private List<IPlayerLocalDataHandler> listDataHandler = new List<IPlayerLocalDataHandler>();

    [SerializeField]
    private float saveToDriveInterval = 1;
    private float lastSaveToDriveTime = 0;

    [SerializeField]
    private float saveToServerInterval = 10;
    private float lastSaveToServerTime = 0;

    PlayerData playerData;


    protected void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        UnInit();
    }

    public void Init()
    {
        playerData = PlayerData.Parser.ParseFrom(Convert.FromBase64String(PlayerPrefs.GetString(nameof(playerData), "")));
        listDataHandler = new List<IPlayerLocalDataHandler>();

        NormalDataHandler normalDataHandler = new NormalDataHandler(playerData);
        listDataHandler.Add(normalDataHandler);
    }

    public void UnInit()
    {
        SavePlayerDataToDrive();
        SavePlayerDataToServer(true);
        Instance = null;
    }

    //检查更新初始数据
    public void CheckFirstCreateData()
    {
    }

    public void SavePlayerDataToDrive()
    {
        lastSaveToDriveTime = Time.time;

        PlayerPrefs.SetString(nameof(playerData), Convert.ToBase64String(playerData.ToByteArray()));
        PlayerPrefs.Save();

        SavePlayerDataToServer(false);
    }

    public void ClearPlayerData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SavePlayerDataToServer(bool bForce)
    {
        if (!bForce)
        {
            if ((lastSaveToServerTime + saveToServerInterval) > Time.time)
            {
                return;
            }
            lastSaveToServerTime = Time.time;
        }
    }

    private void Update()
    {
        if(listDataHandler == null)
        {
            return;
        }

        if ((lastSaveToDriveTime + saveToDriveInterval) < Time.time)
        {
            bool bDoSave = false;
            foreach (var dataMgr in listDataHandler)
            {
                if (dataMgr.BDirty)
                {
                    bDoSave = true;
                    dataMgr.BDirty = false;
                    dataMgr.BNowSave = false;
                }
            }

            if (bDoSave)
            {
                SavePlayerDataToDrive();
            }
        }
        else
        {
            bool bDoSave = false;
            foreach (var dataMgr in listDataHandler)
            {
                if (dataMgr.BNowSave)
                {
                    bDoSave = true;
                    dataMgr.BDirty = false;
                    dataMgr.BNowSave = false;
                }
            }

            if (bDoSave)
            {
                SavePlayerDataToDrive();
            }
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayerDataToServer(true);
    }

    private void OnApplicationPause(bool pause)
    {
        SavePlayerDataToServer(true);
    }
}
