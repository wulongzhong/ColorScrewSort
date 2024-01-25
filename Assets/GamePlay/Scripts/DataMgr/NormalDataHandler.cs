using Msg;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalDataHandler : IPlayerLocalDataHandler
{
    public static NormalDataHandler Instance;
    public NormalData normalData;

    public bool BDirty { get; set; }
    public bool BNowSave { get; set; }

    public int GoldCount
    {
        get { return normalData.GoldCount; }
        set { normalData.GoldCount = value; BDirty = true; BNowSave = true; }
    }

    public int CurrLevelId
    {
        get { return normalData.CurrLevelId; }
        set { normalData.CurrLevelId = value; BDirty = true; BNowSave = true; }
    }

    public List<int> GetUnlockedBackGroundId()
    {
        return normalData.UnlockedBackGroundId.ToList();
    }

    public void AddUnlockedBackGroundId(int id)
    {
        normalData.UnlockedBackGroundId.Add(id);
        BDirty = true;
    }

    public int CurrSelectBackGroundId
    {
        get { return normalData.CurrSelectBackGroundId; }
        set { normalData.CurrSelectBackGroundId = value; BDirty = true; }
    }

    public List<int> GetUnlockedNutId()
    {
        return normalData.UnlockedNutId.ToList();
    }

    public void AddUnlockedNutId(int id)
    {
        normalData.UnlockedNutId.Add(id);
        BDirty = true;
    }

    public int CurrSelectNutId
    {
        get { return normalData.CurrSelectNutId; }
        set { normalData.CurrSelectNutId = value; BDirty = true; }
    }

    public NormalDataHandler(PlayerData playerData)
    {
        if(playerData.NormalData == null)
        {
            playerData.NormalData = new NormalData();
            playerData.NormalData.CurrLevelId = 1;
        }
        this.normalData = playerData.NormalData;
        Instance = this;
    }
}