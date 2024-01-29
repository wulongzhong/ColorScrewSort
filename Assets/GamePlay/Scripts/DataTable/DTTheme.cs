
using System.Collections;
using System.Collections.Generic;
namespace ConfigPB
{
    public sealed partial class Theme
    {
        public void InitCustom()
        {
        }
    }
}

public class DTTheme
{
    public static DTTheme Instance;

    public Dictionary<int, ConfigPB.Theme> dicThemes;

    public DTTheme(ConfigPB.Table table)
    {
        Instance = this;
        dicThemes = new Dictionary<int, ConfigPB.Theme>();
        foreach (var item in table.Theme)
        {
            item.InitCustom();
            dicThemes.Add(item.ID, item);
        }
    }

    public ConfigPB.Theme GetThemeByID(int id)
    {
        return dicThemes[id];
    }
}
