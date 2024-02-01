using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using GameAnalyticsSDK;

public class UpdateFrame : MonoBehaviour
{
    const float fpsMeasurePeriod = 0.5f;    //FPS�������
    private int m_FpsAccumulator = 0;   //֡���ۼƵ�����
    private float m_FpsNextPeriod = 0;  //FPS��һ�εļ��
    private int m_CurrentFps;   //��ǰ��֡��
    const string display = "{0}";   //��ʾ������
    //[Header("FPS�ı�")]
    public Text m_Text;    //UGUI��Text���
    //[Header("֡������")]
    public int FPS;//��֡
    //[Header("�Ƿ���ʾFPS")]
    public bool showFPS;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);//һֱ��ʾ�����٣���÷��ڵ�һ������
        Application.targetFrameRate = FPS;
        if (showFPS)
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod; //Time.realtimeSinceStartup��ȡ��Ϸ��ʼ����ǰ��ʱ�䣬����һ������������������һ��֡�ʼ�����Ҫ��ʲôʱ��
            //m_Text.color = Color.green;
        }
        else
        {
            
        }
            //m_Text.color = Color.clear;
    }

    void Start()
    {
        //GameAnalytics.Initialize();
    }

    private void Update()
    {
        // ����ÿһ���ƽ��֡��
        if (showFPS)
        {
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)    //��ǰʱ�䳬������һ�εļ���ʱ��
            {
                m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);   //����
                m_FpsAccumulator = 0;   //����������
                m_FpsNextPeriod += fpsMeasurePeriod;    //��������һ�εļ��
                m_Text.text = string.Format(display, m_CurrentFps); //����һ��������ʾ
            }
        }
       
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;
            string directoryName = Screen.width + "x" + Screen.height;
            string path = Application.dataPath.Replace("/Assets", "/" /*+ Application.productName + */+"_Screenshot/" + directoryName);
            string imageName = directoryName + "_" + System.Guid.NewGuid() + ".png";

            int fileCount = System.IO.File.Exists(path) ?
                new System.IO.DirectoryInfo(path).GetFiles().Length
                : System.IO.Directory.CreateDirectory(path).GetFiles().Length;

            ScreenCapture.CaptureScreenshot(path + "/" + imageName);
            //Debug.Log("***��ͼ�ɹ�:" + imageName + "  |***���·��" + path + "  |***�óߴ�����" + (fileCount + 1));
        }

        if (Input.GetKeyDown(KeyCode.O))
            Time.timeScale = 1;

        //if (Input.GetKeyDown(KeyCode.W))
        //    WarningText.Instance.Show();

#endif
    }
}