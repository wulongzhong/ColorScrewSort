using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour
{
    private RectTransform banner;


    private void Start()
    {
        DontDestroyOnLoad(this);
        banner = transform.GetComponent<RectTransform>();

        adjustBannerSize();
    }



    public void adjustBannerSize()
    {
        Vector2 newSize = new Vector2(Screen.width+640, (int)(Screen.width / 6.4f)+25);
        banner.sizeDelta = newSize;
    }
}
