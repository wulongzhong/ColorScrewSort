using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class UIShop : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }

    private bool bThemeGroupMode = true;
    private bool bPreviewing = false;


    private ThemeItem lastClickThemeItem;
    private ThemeItem selectedThemeItem;
    private Dictionary<int, ThemeItem> dicAllThemeItem;

    private ThemeItem lastClickSkinItem;
    private ThemeItem selectedSkinItem;
    private Dictionary<int, ThemeItem> dicAllSkinItem;

    public override void OnInit()
    {
        base.OnInit();

        GpEventMgr.Instance.Register<NormalDataHandler.GoldCountUpdateEvent>(this, (evtArg) => { topDiamondUI.RefreshDiamondCount(); });

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
                LevelPlayMgr.Instance.RefreshNutSkin(NormalDataHandler.Instance.CurrSelectNutId);
                UIMgr.Instance.CloseUI<UIShop>(false);
                UIMgr.Instance.OpenUI<UIMain>();
            }
        });
        this.btnTheme.onClick.AddListener(OnClickThemeGroup);
        this.btnSkin.onClick.AddListener(OnClickSkinGroup);
        this.btnThemeSelect.onClick.AddListener(OnClickThemeSelect);
        this.btnThemeRandom.onClick.AddListener(OnClickThemeRandom);
        this.btnLaterTheme.onClick.AddListener(() => { tfPopupRewardTheme.gameObject.SetActive(false); });
        this.btnThemeAds.onClick.AddListener(() => UIMgr.Instance.OpenUI<UIStore>());

        dicAllThemeItem = new Dictionary<int, ThemeItem>();
        foreach (var kv in DTTheme.Instance.dicThemes)
        {
            var tempThemeItem = Instantiate(this.tfThemeItem.gameObject, this.tfThemeItem.parent);
            var themeBev = tempThemeItem.GetComponent<ThemeItem>();
            themeBev.Id = kv.Key;

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

        dicAllSkinItem = new Dictionary<int, ThemeItem>();
        foreach (var kv in DTSkin.Instance.dicSkins)
        {
            var tempSkinItem = Instantiate(this.tfSkinItem.gameObject, this.tfSkinItem.parent);
            var skinBev = tempSkinItem.GetComponent<ThemeItem>();
            skinBev.Id = kv.Key;

            skinBev.imgIcon.sprite = this.resLoader.LoadAsset<Sprite>(kv.Value.IconResPath);
            skinBev.goSelected.gameObject.SetActive(false);
            skinBev.goTag.gameObject.SetActive(false);
            if (NormalDataHandler.Instance.CheckNutUnlock(kv.Key))
            {
                skinBev.goLocked.gameObject.SetActive(false);

                if(NormalDataHandler.Instance.CurrSelectNutId == kv.Key)
                {
                    selectedSkinItem = skinBev;
                    OnClickSkin(skinBev);
                    skinBev.goSelected.gameObject.SetActive(true);
                }
            }
            else
            {
                skinBev.goLocked.gameObject.SetActive(true);
            }
            skinBev.btn.onClick.AddListener(() =>
            {
                OnClickSkin(skinBev);
            });

            dicAllSkinItem.Add(kv.Key, skinBev);
        }
        this.tfSkinItem.gameObject.SetActive(false);

        OnClickThemeGroup();
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

            if(NormalDataHandler.Instance.GetUnlockedBackGroundId().Contains(themeItem.Id))
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
        else if (!NormalDataHandler.Instance.GetUnlockedBackGroundId().Contains(themeItem.Id))
        {
            this.bPreviewing = true;
            imgShopBG.enabled = false;
            tfBG.gameObject.SetActive(false);
            LevelPlayMgr.Instance.PreviewBGSkin(themeItem.Id);
            this.btnThemeAds.gameObject.SetActive(true);
        }
        else if (themeItem == selectedThemeItem)
        {
            this.btnThemeSelect.gameObject.SetActive(false);
            this.btnThemeSelected.gameObject.SetActive(true);
        }
        else
        {
            this.btnThemeSelect.gameObject.SetActive(true);
            this.btnThemeSelected.gameObject.SetActive(false);
        }

        lastClickThemeItem = themeItem;
    }

    private void OnClickThemeSelect()
    {
        if(bThemeGroupMode)
        {
            selectedThemeItem.goSelected.gameObject.SetActive(false);
            lastClickThemeItem.goSelected.gameObject.SetActive(true);
            //lastClickThemeItem.goSelecting.gameObject.SetActive(false);
            selectedThemeItem = lastClickThemeItem;

            NormalDataHandler.Instance.CurrSelectBackGroundId = selectedThemeItem.Id;
            LevelPlayMgr.Instance.RefreshBGSkin();
        }
        else
        {
            selectedSkinItem.goSelected.gameObject.SetActive(false);
            lastClickSkinItem.goSelected.gameObject.SetActive(true);
            //lastClickSkinItem.goSelecting.gameObject.SetActive(false);
            selectedSkinItem = lastClickSkinItem;

            NormalDataHandler.Instance.CurrSelectNutId = selectedSkinItem.Id;
            LevelPlayMgr.Instance.RefreshNutSkin(NormalDataHandler.Instance.CurrSelectNutId);
        }

        btnThemeSelect.gameObject.SetActive(false);
        btnThemeSelected.gameObject.SetActive(true);
    }

    private void OnClickThemeRandom()
    {
        if (NormalDataHandler.Instance.GoldCount < 150)
        {
            UIMgr.Instance.OpenUI<UIStore>();
            return;
        }

        if(bThemeGroupMode)
        {
            var listAllThemesId = DTTheme.Instance.dicThemes.Values.ToList();
            for (int i = listAllThemesId.Count - 1; i >= 0; i--)
            {
                if (NormalDataHandler.Instance.CheckBackGroundUnlock(listAllThemesId[i].ID))
                {
                    listAllThemesId.RemoveAt(i);
                }
            }
            if (listAllThemesId.Count == 0)
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
            GameAnalytics.NewDesignEvent("RandomBgTheme");
        }
        else
        {
            var listAllSkinsId = DTSkin.Instance.dicSkins.Values.ToList();
            for (int i = listAllSkinsId.Count - 1; i >= 0; i--)
            {
                if (NormalDataHandler.Instance.CheckNutUnlock(listAllSkinsId[i].ID))
                {
                    listAllSkinsId.RemoveAt(i);
                }
            }
            if (listAllSkinsId.Count == 0)
            {
                return;
            }
            NormalDataHandler.Instance.GoldCount -= 150;
            topDiamondUI.RefreshDiamondCount();
            int unlockId = listAllSkinsId[UnityEngine.Random.Range(0, listAllSkinsId.Count)].ID;
            NormalDataHandler.Instance.AddUnlockedNutId(unlockId);
            var skinBev = dicAllSkinItem[unlockId];
            skinBev.goLocked.gameObject.SetActive(false);

            this.tfPopupRewardTheme.gameObject.SetActive(true);
            this.newUnlockThemeIcon.sprite = skinBev.imgIcon.sprite;

            this.btnUseThemeNow.onClick.RemoveAllListeners();
            this.btnUseThemeNow.onClick.AddListener(() =>
            {
                OnClickSkin(skinBev);
                OnClickThemeSelect();
                OnClickSkin(skinBev);
                tfPopupRewardTheme.gameObject.SetActive(false);
            });
            GameAnalytics.NewDesignEvent("RandomNutSkin");
        }
    }

    private void OnClickSkin(ThemeItem skinItem)
    {
        this.btnThemeAds.gameObject.SetActive(false);
        this.btnThemeSelect.gameObject.SetActive(false);
        this.btnThemeSelected.gameObject.SetActive(false);

        if (lastClickSkinItem != skinItem)
        {
            if (lastClickSkinItem != null)
            {
                lastClickSkinItem.goSelecting.SetActive(false);
                lastClickSkinItem.goLockedSelecting.SetActive(false);
                lastClickSkinItem.goPreview.SetActive(false);
            }

            if (NormalDataHandler.Instance.GetUnlockedNutId().Contains(skinItem.Id))
            {
                skinItem.goSelecting.SetActive(true);
                this.btnThemeSelect.gameObject.SetActive(true);
            }
            else
            {
                skinItem.goLockedSelecting.SetActive(true);
                skinItem.goPreview.SetActive(true);
                this.btnThemeAds.gameObject.SetActive(true);
            }
        }
        else if (!NormalDataHandler.Instance.GetUnlockedNutId().Contains(skinItem.Id))
        {
            this.bPreviewing = true;
            imgShopBG.enabled = false;
            tfBG.gameObject.SetActive(false);
            LevelPlayMgr.Instance.RefreshNutSkin(skinItem.Id);
            this.btnThemeAds.gameObject.SetActive(true);
        }
        if (skinItem == selectedSkinItem)
        {
            this.btnThemeSelect.gameObject.SetActive(false);
            this.btnThemeSelected.gameObject.SetActive(true);
        }

        lastClickSkinItem = skinItem;
    }

    int clickCount = 0;
    private void OnClickThemeGroup()
    {
        ++clickCount;
        if(clickCount >= 10)
        {
            NormalDataHandler.Instance.GoldCount += 10000;
        }
        bThemeGroupMode = true;

        this.tfTheme.gameObject.SetActive(true);
        this.tfSkin.gameObject.SetActive(false);

        btnTheme.transform.GetChild(0).gameObject.SetActive(true);
        btnTheme.transform.GetChild(1).gameObject.SetActive(false);
        btnSkin.transform.GetChild(0).gameObject.SetActive(false);
        btnSkin.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void OnClickSkinGroup()
    {
        bThemeGroupMode = false;
        this.tfTheme.gameObject.SetActive(false);
        this.tfSkin.gameObject.SetActive(true);

        btnTheme.transform.GetChild(0).gameObject.SetActive(false);
        btnTheme.transform.GetChild(1).gameObject.SetActive(true);
        btnSkin.transform.GetChild(0).gameObject.SetActive(true);
        btnSkin.transform.GetChild(1).gameObject.SetActive(false);
    }
}