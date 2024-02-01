using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardedAdsButtonSelfCheck : MonoBehaviour
{
    private float _adButtonInteractCheckTime = 0.0f;
    private const float _adButtonInteractCheckTimeThreshold = 1.0f;
    [SerializeField] private Button _adButton;

    // Start is called before the first frame update
    void Start()
    {
        if (_adButton == null)
            TryGetComponent<Button>(out _adButton);

        CheckRewardedAds();
    }

    // Update is called once per frame
    void Update()
    {
        if (_adButton == null)
        {
            Debug.LogWarning("_adButton not detected.");
            return;
        }

        _adButtonInteractCheckTime += Time.deltaTime;
        if (_adButtonInteractCheckTime > _adButtonInteractCheckTimeThreshold)
        {
            _adButtonInteractCheckTime = 0.0f;
            CheckRewardedAds();
        }
    }

    void CheckRewardedAds()
    {
        if (BaseAdsManager.INSTANCE.RewardedAdsIsReady())
        {
            _adButton.interactable = true;
        }
        else
        {
            _adButton.interactable= false;
        }
    }
}
