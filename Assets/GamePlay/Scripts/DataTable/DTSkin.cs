
using System.Collections;
using System.Collections.Generic;
namespace ConfigPB
{
    public sealed partial class Skin
    {
        public void InitCustom()
        {
        }
    }
}

public class DTSkin
{
    public static DTSkin Instance;

    public Dictionary<int, ConfigPB.Skin> dicSkins;

    public DTSkin(ConfigPB.Table table)
    {
        Instance = this;
        dicSkins = new Dictionary<int, ConfigPB.Skin>();
        foreach (var item in table.Skin)
        {
            item.InitCustom();
            dicSkins.Add(item.ID, item);
        }
    }

    public ConfigPB.Skin GetSkinByID(int id)
    {
        return dicSkins[id];
    }
}
