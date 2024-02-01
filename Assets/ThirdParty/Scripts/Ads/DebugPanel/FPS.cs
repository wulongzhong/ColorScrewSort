using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
   
    public Text fpsText;

    float updateInterval = 1.0f;           //��ǰʱ����
    private float accumulated = 0.0f;      //�ڴ��ڼ��ۻ�  
    private float frames = 0;              //�ڼ���ڻ��Ƶ�֡  
    private float timeRemaining;           //��ǰ�����ʣ��ʱ��
    private float fps = 15.0f;             //��ǰ֡ Current FPS
    private float lastSample;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject); //�����ٴ���Ϸ�������ĸ�������������ʾ��������Ҫ��ע��
        timeRemaining = updateInterval;
        lastSample = Time.realtimeSinceStartup; //ʵʱ������
    }

    void Update()
    {
        ++frames;
        float newSample = Time.realtimeSinceStartup;
        float deltaTime = newSample - lastSample;
        lastSample = newSample;
        timeRemaining -= deltaTime;
        accumulated += 1.0f / deltaTime;

        if (timeRemaining <= 0.0f)
        {
            fps = accumulated / frames;
            timeRemaining = updateInterval;
            accumulated = 0.0f;
            frames = 0;
        }
        fpsText.text = $"FPS:" + fps.ToString("f2");
    }

    
}
