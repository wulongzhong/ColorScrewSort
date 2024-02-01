using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardReady : MonoBehaviour
{
    public BaseAdsManager.RewardType type;
    int count;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (count >= 90)
        {
            count = 0;
            if (BaseAdsManager.INSTANCE.RewardedAdsIsReady())
            {
                this.GetComponent<Button>().interactable = true;
            }
            else
            {
                this.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            count++;
        }
    }
}
