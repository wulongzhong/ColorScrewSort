using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UILevelComplete : UIBase
{
    [SerializeField]
    protected UnityEngine.RectTransform PopupWin;
    [SerializeField]
    protected UnityEngine.RectTransform PopupRewardChest;
    [SerializeField]
    protected UnityEngine.RectTransform PopupRewardTheme;
    [SerializeField]
    protected UITopDiamond topDiamondUI;
    [SerializeField]
    protected UnityEngine.UI.Button btnNextLevel;
    [SerializeField]
    protected UnityEngine.UI.Button btnWatchAds;
    [SerializeField]
    protected UnityEngine.RectTransform tfLevelChest;
    [SerializeField]
    protected UnityEngine.RectTransform tfThemeProcess;
    [SerializeField]
    protected UnityEngine.UI.Button btnClaimChest;
    [SerializeField]
    protected UnityEngine.UI.Button btnGetAllChest;
    [SerializeField]
    protected UnityEngine.UI.Button btnLaterTheme;
    [SerializeField]
    protected UnityEngine.UI.Image themeIcon;
    [SerializeField]
    protected UnityEngine.RectTransform tfGem;
    [SerializeField]
    protected UnityEngine.RectTransform tfUndo;
    [SerializeField]
    protected UnityEngine.RectTransform tfAddRow;
    [SerializeField]
    protected UnityEngine.UI.Image iconTheme;
    [SerializeField]
    protected UnityEngine.UI.Button btnClaimTheme;
    [SerializeField]
    protected UnityEngine.UI.Image fillChest;
    [SerializeField]
    protected TMPro.TextMeshProUGUI tChest;
    [SerializeField]
    protected UnityEngine.UI.Image fillTheme;
    [SerializeField]
    protected TMPro.TextMeshProUGUI tTheme;
    [SerializeField]
    protected UnityEngine.ParticleSystem Par_F;
    [SerializeField]
    protected UnityEngine.ParticleSystem fxGem;
    [SerializeField]
    protected UnityEngine.ParticleSystem fxUndo;
    [SerializeField]
    protected UnityEngine.ParticleSystem fxAddRow;
    [SerializeField]
    protected UnityEngine.ParticleSystemRenderer idleOpenChest;

}
