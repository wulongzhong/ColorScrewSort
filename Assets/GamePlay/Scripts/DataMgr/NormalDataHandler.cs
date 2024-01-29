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

    public int LastGoldCount
    {
        get;
        set;
    }
    public int GoldCount
    {
        get { return normalData.GoldCount; }
        set { LastGoldCount = normalData.GoldCount; normalData.GoldCount = value; BDirty = true; BNowSave = true; }
    }

    public int CurrNormalLevelId
    {
        get { return normalData.CurrLevelId; }
        set { normalData.CurrLevelId = value; BDirty = true; BNowSave = true; }
    }

    public bool CurrNormalLevelIsHard
    {
        get;
        set;
    }

    public bool CurrIsNormalLevel
    {
        get;
        set;
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

    public bool CheckBackGroundUnlock(int id)
    {
        return normalData.UnlockedBackGroundId.Contains(id);
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

    public bool CheckNutUnlock(int id)
    {
        return normalData.UnlockedNutId.Contains(id);
    }

    public int CurrSelectNutId
    {
        get { return normalData.CurrSelectNutId; }
        set { normalData.CurrSelectNutId = value; BDirty = true; }
    }

    public int CurrChestFragCount
    {
        get { return normalData.CurrChestFragCount; }
        set { normalData.CurrChestFragCount = value; BDirty = true; }
    }
    public int CurrThemeFragCount
    {
        get { return normalData.CurrThemeFragCount; }
        set { normalData.CurrThemeFragCount = value; BDirty = true; }
    }

    public int CurrThemeFragId
    {
        get { return normalData.CurrThemeFragId; }
        set { normalData.CurrThemeFragId = value; BDirty = true; }
    }

    public int PropUndoCount
    {
        get { return normalData.PropUndoCount; }
        set { normalData.PropUndoCount = value; BDirty = true; }
    }

    public int PropAddRowCount
    {
        get { return normalData.PropAddRowCount; }
        set { normalData.PropAddRowCount = value; BDirty = true; }
    }

    public NormalDataHandler(PlayerData playerData)
    {
        if(playerData.NormalData == null)
        {
            playerData.NormalData = new NormalData();
        }

        this.normalData = playerData.NormalData;
        if(normalData.CurrLevelId == 0)
        {
            normalData.CurrLevelId = 1;
        }
        if(normalData.UnlockedBackGroundId.Count == 0)
        {
            normalData.UnlockedBackGroundId.Add(6);
            normalData.CurrSelectBackGroundId = 6;
        }
        if(normalData.UnlockedNutId.Count == 0)
        {
            normalData.UnlockedNutId.Add(1);
            normalData.CurrSelectNutId = 1;
        }
        Instance = this;
    }
}