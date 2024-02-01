
using System.Collections;
using System.Collections.Generic;
namespace ConfigPB
{
    public sealed partial class LevelAdReward
    {
        public void InitCustom()
        {
        }
    }
}

public class DTLevelAdReward
{
    public static DTLevelAdReward Instance;

    public Dictionary<ConfigPB.AdRewardType, ConfigPB.LevelAdReward> dicLevelAdRewards;

    public DTLevelAdReward(ConfigPB.Table table)
    {
        Instance = this;
        dicLevelAdRewards = new Dictionary<ConfigPB.AdRewardType, ConfigPB.LevelAdReward>();
        foreach (var item in table.LevelAdReward)
        {
            item.InitCustom();
            dicLevelAdRewards.Add(item.RewardType, item);
        }
    }

    public ConfigPB.LevelAdReward GetLevelAdRewardByRewardType(ConfigPB.AdRewardType id)
    {
        return dicLevelAdRewards[id];
    }
}
