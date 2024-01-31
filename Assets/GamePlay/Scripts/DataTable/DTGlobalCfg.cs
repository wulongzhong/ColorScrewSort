
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace ConfigPB
{
    public sealed partial class GlobalCfg
    {
        public enum KeyType
        {
            None = 0,
            StoreAd1GoldCount,//       商店第1次广告金币数量
            StoreAd2GoldCount,//       商店第2次广告金币数量
            StoreAd3GoldCount,//       商店第3次广告金币数量
            StoreAd4GoldCount,//       商店第4次广告金币数量
            CompleteLevelGoldCount,//  完成关卡金币数量
            BoxGoldCount,//            宝箱金币数量
            CompleteLevelAdGoldCount,//完成关卡看广告金币数量

        }
        public void InitCustom()
        {
        }
    }
}

public class DTGlobalCfg
{
    public static DTGlobalCfg Instance;

    public Dictionary<string, ConfigPB.GlobalCfg> dicGlobalCfgs;

    public DTGlobalCfg(ConfigPB.Table table)
    {
        Instance = this;
        dicGlobalCfgs = new Dictionary<string, ConfigPB.GlobalCfg>();
        foreach (var item in table.GlobalCfg)
        {
            item.InitCustom();
            dicGlobalCfgs.Add(item.Key, item);
        }
    }
    public int GetIntByKey(ConfigPB.GlobalCfg.KeyType keyType)
    {
        return dicGlobalCfgs[keyType.ToString()].IntValue;
    }
    public List<int> GetIntListByKey(ConfigPB.GlobalCfg.KeyType keyType)
    {
        return dicGlobalCfgs[keyType.ToString()].IntArrValue.ToList();
    }

    public float GetFloatByKey(ConfigPB.GlobalCfg.KeyType keyType)
    {
        return dicGlobalCfgs[keyType.ToString()].FloatValue;
    }
    public long GetLongByKey(ConfigPB.GlobalCfg.KeyType keyType)
    {
        return dicGlobalCfgs[keyType.ToString()].LongValue;
    }
}