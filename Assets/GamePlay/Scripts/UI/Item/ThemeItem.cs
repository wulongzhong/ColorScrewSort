using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeItem : MonoBehaviour
{
    [NonSerialized]
    public int themeId;

    public Button btn;
    public Image imgIcon;
    public GameObject goTag;
    public GameObject goSelecting;
    public GameObject goSelected;
    public GameObject goLocked;
    public GameObject goLockedSelecting;
    public GameObject goPreview;
}