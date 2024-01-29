using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class UIShop : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }

    private bool bPreviewing = false;

    private ThemeItem lastClickThemeItem;
    private ThemeItem selectedThemeItem;

    private Dictionary<int, ThemeItem> dicAllThemeItem;

    public override void OnInit()
    {
        base.OnInit();
        topDiamondUI.RefreshDiamondCount();
        this.btnBack.onClick.AddListener(() =>
        {
            if (this.bPreviewing)
            {
                this.bPreviewing = false;
                imgShopBG.enabled = true;
                tfBG.gameObject.SetActive(true);
            }
            else
            {
                LevelPlayMgr.Instance.RefreshBGSkin();
                UIMgr.Instance.CloseUI<UIShop>(false);
                UIMgr.Instance.OpenUI<UIMain>();
            }
        });

        this.btnThemeSelect.onClick.AddListener(OnClickThemeSelect);
        this.btnThemeRandom.onClick.AddListener(OnClickThemeRandom);
        this.btnLaterTheme.onClick.AddListener(() => { tfPopupRewardTheme.gameObject.SetActive(false); });

        dicAllThemeItem = new Dictionary<int, ThemeItem>();
        foreach (var kv in DTTheme.Instance.dicThemes)
        {
            var tempThemeItem = Instantiate(this.tfThemeItem.gameObject, this.tfThemeItem.parent);
            var themeBev = tempThemeItem.GetComponent<ThemeItem>();
            themeBev.themeId = kv.Key;

            themeBev.imgIcon.sprite = this.resLoader.LoadAsset<Sprite>(kv.Value.IconResPath);
            themeBev.goSelected.gameObject.SetActive(false);
            themeBev.goTag.gameObject.SetActive(kv.Key <= 5);
            if (NormalDataHandler.Instance.CheckBackGroundUnlock(kv.Key))
            {
                themeBev.goLocked.gameObject.SetActive(false);

                if(NormalDataHandler.Instance.CurrSelectBackGroundId == kv.Key)
                {
                    selectedThemeItem = themeBev;
                    OnClickTheme(themeBev);
                    themeBev.goSelected.gameObject.SetActive(true);
                }
            }
            else
            {
                themeBev.goLocked.gameObject.SetActive(true);
            }
            themeBev.btn.onClick.AddListener(() =>
            {
                OnClickTheme(themeBev);
            });

            dicAllThemeItem.Add(kv.Key, themeBev);
        }
        this.tfThemeItem.gameObject.SetActive(false);
    }

    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnClose(bool bRecycle = true)
    {
        base.OnClose(bRecycle);
        if (EventHandlerIndex != 0)
        {
            this.ClearUpEventHandle();
        }
    }

    private void OnClickTheme(ThemeItem themeItem)
    {
        this.btnThemeAds.gameObject.SetActive(false);
        this.btnThemeSelect.gameObject.SetActive(false);
        this.btnThemeSelected.gameObject.SetActive(false);

        if(lastClickThemeItem != themeItem)
        {
            if(lastClickThemeItem != null)
            {
                lastClickThemeItem.goSelecting.SetActive(false);
                lastClickThemeItem.goLockedSelecting.SetActive(false);
                lastClickThemeItem.goPreview.SetActive(false);
            }

            if(NormalDataHandler.Instance.GetUnlockedBackGroundId().Contains(themeItem.themeId))
            {
                themeItem.goSelecting.SetActive(true);
                this.btnThemeSelect.gameObject.SetActive(true);
            }
            else
            {
                themeItem.goLockedSelecting.SetActive(true);
                themeItem.goPreview.SetActive(true);
                this.btnThemeAds.gameObject.SetActive(true);
            }
        }
        else
        {
            if (!NormalDataHandler.Instance.GetUnlockedBackGroundId().Contains(themeItem.themeId))
            {
                this.bPreviewing = true;
                imgShopBG.enabled = false;
                tfBG.gameObject.SetActive(false);
                LevelPlayMgr.Instance.PreviewBGSkin(themeItem.themeId);
                this.btnThemeAds.gameObject.SetActive(true);
            }
            else
            {
                this.btnThemeSelected.gameObject.SetActive(true);
            }
        }
        lastClickThemeItem = themeItem;
    }

    private void OnClickThemeSelect()
    {
        selectedThemeItem.goSelected.gameObject.SetActive(false);
        lastClickThemeItem.goSelected.gameObject.SetActive(true);
        lastClickThemeItem.goSelecting.gameObject.SetActive(false);
        selectedThemeItem = lastClickThemeItem;

        NormalDataHandler.Instance.CurrSelectBackGroundId = selectedThemeItem.themeId;
        LevelPlayMgr.Instance.RefreshBGSkin();
    }

    private void OnClickThemeRandom()
    {
        var listAllThemesId = DTTheme.Instance.dicThemes.Values.ToList();
        for(int i = listAllThemesId.Count - 1; i >= 0; i--)
        {
            if (NormalDataHandler.Instance.CheckBackGroundUnlock(listAllThemesId[i].ID))
            {
                listAllThemesId.RemoveAt(i);
            }
        }
        if(listAllThemesId.Count == 0)
        {
            return;
        }
        NormalDataHandler.Instance.GoldCount -= 150;
        topDiamondUI.RefreshDiamondCount();
        int unlockId = listAllThemesId[UnityEngine.Random.Range(0, listAllThemesId.Count)].ID;
        NormalDataHandler.Instance.AddUnlockedBackGroundId(unlockId);
        var themeBev = dicAllThemeItem[unlockId];
        themeBev.goLocked.gameObject.SetActive(false);

        this.tfPopupRewardTheme.gameObject.SetActive(true);
        this.newUnlockThemeIcon.sprite = themeBev.imgIcon.sprite;

        this.btnUseThemeNow.onClick.RemoveAllListeners();
        this.btnUseThemeNow.onClick.AddListener(() =>
        {
            OnClickTheme(themeBev);
            OnClickThemeSelect();
            OnClickTheme(themeBev);
            tfPopupRewardTheme.gameObject.SetActive(false);
        });
    }
}
