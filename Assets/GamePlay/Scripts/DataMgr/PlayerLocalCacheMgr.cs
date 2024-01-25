using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System;
using Msg;

public class PlayerLocalCacheMgr: MonoBehaviour
{
    public static PlayerLocalCacheMgr instance;
    private Msg.PlayerLocalCache playerLocalCache;

    private const float saveToDriveInterval = 0.1f;
    private float lastSaveToDriveTime = 0;

    private Dictionary<PlayerLocalCheType, Key2Bool> dicBool;
    private Dictionary<PlayerLocalCheType, Key2Int> dicInt;
    private Dictionary<PlayerLocalCheType, Key2Long> dicLong;
    private Dictionary<PlayerLocalCheType, Key2Float> dicFloat;
    private Dictionary<PlayerLocalCheType, Key2String> dicString;

    private Dictionary<PlayerLocalCheType, Key2Vec2> dicVec2;
    private Dictionary<PlayerLocalCheType, Key2Vec2Int> dicVec2Int;
    private Dictionary<PlayerLocalCheType, Key2Vec3> dicVec3;
    private Dictionary<PlayerLocalCheType, Key2Vec3Int> dicVec3Int;

    private bool bDirty = false;

    private void Awake()
    {
        instance = this;
        playerLocalCache = Msg.PlayerLocalCache.Parser.ParseFrom(Convert.FromBase64String(PlayerPrefs.GetString(nameof(playerLocalCache), "")));

        dicBool = new ();
        dicInt = new();
        dicLong = new();
        dicFloat = new();
        dicString = new();

        dicVec2 = new();
        dicVec2Int = new();
        dicVec3 = new();
        dicVec3Int = new();

        foreach (var cacheInfo in playerLocalCache.LstBool)
        {
            dicBool.Add(cacheInfo.K, cacheInfo);
        }
        foreach (var cacheInfo in playerLocalCache.LstInt)
        {
            dicInt.Add(cacheInfo.K, cacheInfo);
        }
        foreach (var cacheInfo in playerLocalCache.LstLong)
        {
            dicLong.Add(cacheInfo.K, cacheInfo);
        }
        foreach (var cacheInfo in playerLocalCache.LstFloat)
        {
            dicFloat.Add(cacheInfo.K, cacheInfo);
        }
        foreach (var cacheInfo in playerLocalCache.LstString)
        {
            dicString.Add(cacheInfo.K, cacheInfo);
        }
        foreach (var cacheInfo in playerLocalCache.LstVec2)
        {
            dicVec2.Add(cacheInfo.K, cacheInfo);
        }
        foreach (var cacheInfo in playerLocalCache.LstVec2Int)
        {
            dicVec2Int.Add(cacheInfo.K, cacheInfo);
        }
        foreach (var cacheInfo in playerLocalCache.LstVec3)
        {
            dicVec3.Add(cacheInfo.K, cacheInfo);
        }
        foreach (var cacheInfo in playerLocalCache.LstVec3Int)
        {
            dicVec3Int.Add(cacheInfo.K, cacheInfo);
        }
    }

    public bool GetBool(PlayerLocalCheType cheType, bool defaultV)
    {
        if (dicBool.ContainsKey(cheType))
        {
            return dicBool[cheType].V;
        }
        return defaultV;
    }

    public void SetBool(PlayerLocalCheType cheType, bool v)
    {
        if (dicBool.ContainsKey(cheType))
        {
            dicBool[cheType].V = v;
        }
        else
        {
            Key2Bool key2Bool = new Key2Bool() { K = cheType, V = v };
            dicBool.Add(cheType, key2Bool);
            playerLocalCache.LstBool.Add(key2Bool);
        }
        bDirty = true;
    }

    public int GetInt(PlayerLocalCheType cheType, int defaultV)
    {
        if (dicInt.ContainsKey(cheType))
        {
            return dicInt[cheType].V;
        }
        return defaultV;
    }
    public void SetInt(PlayerLocalCheType cheType, int v)
    {
        if (dicInt.ContainsKey(cheType))
        {
            dicInt[cheType].V = v;
        }
        else
        {
            Key2Int key2Int = new Key2Int() { K = cheType, V = v };
            dicInt.Add(cheType, key2Int);
            playerLocalCache.LstInt.Add(key2Int);
        }
        bDirty = true;
    }

    public long GetLong(PlayerLocalCheType cheType, long defaultV)
    {
        if (dicLong.ContainsKey(cheType))
        {
            return dicLong[cheType].V;
        }
        return defaultV;
    }

    public void SetLong(PlayerLocalCheType cheType, long v)
    {
        if (dicLong.ContainsKey(cheType))
        {
            dicLong[cheType].V = v;
        }
        else
        {
            Key2Long key2Long = new Key2Long() { K = cheType, V = v };
            dicLong.Add(cheType, key2Long);
            playerLocalCache.LstLong.Add(key2Long);
        }
        bDirty = true;
    }

    public float GetFloat(PlayerLocalCheType cheType, float defaultV)
    {
        if (dicFloat.ContainsKey(cheType))
        {
            return dicFloat[cheType].V;
        }
        return defaultV;
    }

    public void SetFloat(PlayerLocalCheType cheType, float v)
    {
        if (dicFloat.ContainsKey(cheType))
        {
            dicFloat[cheType].V = v;
        }
        else
        {
            Key2Float key2Float = new Key2Float() { K = cheType, V = v };
            dicFloat.Add(cheType, key2Float);
            playerLocalCache.LstFloat.Add(key2Float);
        }
        bDirty = true;
    }

    public string GetString(PlayerLocalCheType cheType, string defaultV)
    {
        if (dicString.ContainsKey(cheType))
        {
            return dicString[cheType].V;
        }
        return defaultV;
    }

    public void SetString(PlayerLocalCheType cheType, string v)
    {
        if (dicString.ContainsKey(cheType))
        {
            dicString[cheType].V = v;
        }
        else
        {
            Key2String key2String = new Key2String() { K = cheType, V = v };
            dicString.Add(cheType, key2String);
            playerLocalCache.LstString.Add(key2String);
        }
        bDirty = true;
    }

    public Vector2 GetVec2(PlayerLocalCheType cheType, Vector2 defaultV)
    {
        if (dicVec2.ContainsKey(cheType))
        {
            var v = dicVec2[cheType].V;
            defaultV.x = v.X;
            defaultV.y = v.Y;
            return defaultV;
        }
        return defaultV;
    }

    public void SetVec2(PlayerLocalCheType cheType, Vector2 v)
    {
        if (dicVec2.TryGetValue(cheType, out Key2Vec2 key2Vec2))
        {
            key2Vec2.V.X = v.x;
            key2Vec2.V.Y = v.y;
        }
        else
        {
            var newKey2Vec2 = new Key2Vec2() { K = cheType, V = new PbVec2() { X = v.x, Y = v.y } };
            dicVec2.Add(cheType, newKey2Vec2);
            playerLocalCache.LstVec2.Add(newKey2Vec2);
        }

        bDirty = true;
    }

    public Vector2Int GetVec2Int(PlayerLocalCheType cheType, Vector2Int defaultV)
    {
        if (dicVec2Int.ContainsKey(cheType))
        {
            var v = dicVec2Int[cheType].V;
            defaultV.x = v.X;
            defaultV.y = v.Y;
            return defaultV;
        }
        return defaultV;
    }

    public void SetVec2Int(PlayerLocalCheType cheType, Vector2Int v)
    {
        if (dicVec2Int.TryGetValue(cheType, out Key2Vec2Int key2Vec2Int))
        {
            key2Vec2Int.V.X = v.x;
            key2Vec2Int.V.Y = v.y;
        }
        else
        {
            var newKey2Vec2Int = new Key2Vec2Int() { K = cheType, V = new PbVec2Int() { X = v.x, Y = v.y } };
            dicVec2Int.Add(cheType, newKey2Vec2Int);
            playerLocalCache.LstVec2Int.Add(newKey2Vec2Int);
        }

        bDirty = true;
    }

    public Vector3 GetVec3(PlayerLocalCheType cheType, Vector3 defaultV)
    {
        if (dicVec3.ContainsKey(cheType))
        {
            var v = dicVec3[cheType].V;
            defaultV.x = v.X;
            defaultV.y = v.Y;
            defaultV.z = v.Z;
            return defaultV;
        }
        return defaultV;
    }

    public void SetVec3(PlayerLocalCheType cheType, Vector3 v)
    {
        if (dicVec3.TryGetValue(cheType, out Key2Vec3 key2Vec3))
        {
            key2Vec3.V.X = v.x;
            key2Vec3.V.Y = v.y;
            key2Vec3.V.Z = v.z;
        }
        else
        {
            var newKey2Vec3 = new Key2Vec3() { K = cheType, V = new PbVec3() { X = v.x, Y = v.y, Z = v.z } };
            dicVec3.Add(cheType, newKey2Vec3);
            playerLocalCache.LstVec3.Add(newKey2Vec3);
        }

        bDirty = true;
    }

    public Vector3 GetVec3Int(PlayerLocalCheType cheType, Vector3Int defaultV)
    {
        if (dicVec3Int.ContainsKey(cheType))
        {
            var v = dicVec3Int[cheType].V;
            defaultV.x = v.X;
            defaultV.y = v.Y;
            defaultV.z = v.Z;
            return defaultV;
        }
        return defaultV;
    }

    public void SetVec3Int(PlayerLocalCheType cheType, Vector3Int v)
    {
        if (dicVec3Int.TryGetValue(cheType, out Key2Vec3Int key2Vec3Int))
        {
            key2Vec3Int.V.X = v.x;
            key2Vec3Int.V.Y = v.y;
            key2Vec3Int.V.Z = v.z;
        }
        else
        {
            var newKey2Vec3Int = new Key2Vec3Int() { K = cheType, V = new PbVec3Int() { X = v.x, Y = v.y, Z = v.z } };
            dicVec3Int.Add(cheType, newKey2Vec3Int);
            playerLocalCache.LstVec3Int.Add(newKey2Vec3Int);
        }

        bDirty = true;
    }

    private void Update()
    {
        if (bDirty && (lastSaveToDriveTime + saveToDriveInterval) < Time.time)
        {
            SavePlayerDataToDrive();
        }
    }

    public void SavePlayerDataToDrive()
    {
        lastSaveToDriveTime = Time.time;
        PlayerPrefs.SetString(nameof(playerLocalCache), Convert.ToBase64String(playerLocalCache.ToByteArray()));
        PlayerPrefs.Save();
    }

    public void LogAllCache()
    {
        Debug.Log(playerLocalCache.ToString());
    }

    #region 
    public bool IsShowJoyStick
    {
        get { return GetBool(PlayerLocalCheType.ShowJoyStick, true); }
        set
        {
            SetBool(PlayerLocalCheType.ShowJoyStick, value);
        }
    }
    public bool IsEnableShake
    {
        get { return GetBool(PlayerLocalCheType.EnableShake, true); }
        set
        {
            SetBool(PlayerLocalCheType.EnableShake, value);
        }
    }
    public bool IsEnableMusic
    {
        get { return GetBool(Msg.PlayerLocalCheType.EnableMusic, true); }
        set
        {
            SetBool(Msg.PlayerLocalCheType.EnableMusic, value);
        }
    }
    public bool IsEnableSound
    {
        get { return GetBool(PlayerLocalCheType.EnableSound, true); }
        set
        {
            SetBool(PlayerLocalCheType.EnableSound, value);
        }
    }
    #endregion
}