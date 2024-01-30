using ConfigPB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    public static SoundMgr Instance;

    [SerializeField]
    private AudioSource bgmSource;

    private ResLoader resLoader;

    private Dictionary<string, GameObject> dicLoopSound;

    public void Awake()
    {
        Instance = this;
        resLoader = new ResLoader();
        dicLoopSound = new Dictionary<string, GameObject>();
    }

    public void PlaySound(string soundId)
    {
        var cfg = DTSound.Instance.GetSoundById(soundId);
        var audioClip = resLoader.LoadAsset<AudioClip>(cfg.AssetPath);
        if (cfg.IsLoop)
        {
            if (!dicLoopSound.ContainsKey(soundId))
            {
                GameObject goClip = new GameObject($"sound_{soundId}");
                goClip.transform.parent = transform;
                goClip.transform.localPosition = Vector3.zero;
                var audioSource = goClip.AddComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.clip = audioClip;
                audioSource.Play();
                dicLoopSound.Add(soundId, goClip);
            }
        }
        else
        {
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
        }
    }

    public void PlayBgm(string bgmId)
    {
        StartCoroutine(switchBgm(bgmId));
    }

    private IEnumerator switchBgm(string bgmId)
    {
        float interval = 0.5f;
        float startTime = Time.time;
        while(Time.time - startTime < interval)
        {
            bgmSource.volume = (0.5f - (Time.time - startTime)) / interval;
            yield return null;
        }
        startTime = Time.time;
        var cfg = DTSound.Instance.GetSoundById(bgmId);
        bgmSource.clip = resLoader.LoadAsset<AudioClip>(cfg.AssetPath);
        while(Time.time - startTime < interval)
        {
            bgmSource.volume = (Time.time - startTime) / interval;
            yield return null;
        }
        bgmSource.volume = 1;
    }

    public void StopLoopSound(string id)
    {
        if (dicLoopSound.ContainsKey(id))
        {
            Destroy(dicLoopSound[id]);
            dicLoopSound.Remove(id);
        }
    }

    public void ClearLoopSound()
    {
        foreach(var kv in dicLoopSound)
        {
            Destroy(kv.Value);
        }
        dicLoopSound.Clear();
    }
}
