using UnityEngine;
/*****************************************
	 文件:   BatteryManager.cs
	 作者:   Siran
	 日期:   2021/3/9 15:40:29
	 功能:   电池管理
 *****************************************/
public class BatteryManager : MonoBehaviour
{
    private float m_LastUpdateShowTime = 0f;  //上一次更新帧率的时间;  

    private float m_UpdateShowDeltaTime = 0.1f;//更新帧率的时间间隔;  

    private int m_FrameUpdate = 0;//帧数;  

    private float m_FPS = 0;

    float e = 0;

    string total = string.Empty;   
   
    void Start()
    {
        if (GameConfig.Instance.islockFPS)
            Application.targetFrameRate = 60; //设置帧率  //默认60,支持120Hz的120
        else
            Application.targetFrameRate = -1;

        m_LastUpdateShowTime = Time.realtimeSinceStartup;       
    }     

    private void OnGUI()
    {        
        if (GameConfig.Instance.isRelease == false)
        {           
            GUILayout.Space(20);
            total = LevelMgr.LoadTime;
            GUILayout.Label(total);            
            if (Application.platform == RuntimePlatform.Android)
            {
                GUI.skin.label.fontSize = 40;               
                GUILayout.Label("FPS: " + m_FPS + "Hz");
                GUILayout.Label("容量: " + Power.capacity + "mA");
                GUILayout.Label("电压: " + Power.voltage + "V");
                GUILayout.Label("电流: " + e + "mA");
                GUILayout.Label("功率: " + (int)(e * Power.voltage) + "mW");
                GUILayout.Label("时长: " + (Power.capacity / e).ToString("f2") + "h");
            }
            else
            {
                GUI.skin.label.fontSize = 25;
                GUILayout.Label("FPS: " + m_FPS + "Hz");
            }                
        }
    }

    float t = 0f;
    private void Update()
    {        
        if (GameConfig.Instance.isRelease == false)
        {
            m_FrameUpdate++;
            if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
            {
                m_FPS =(int)(m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime));
                m_FrameUpdate = 0;
                m_LastUpdateShowTime = Time.realtimeSinceStartup;
            }
            if (Time.time - t > 1f)
            {
                t = Time.time;

                if (Application.platform==RuntimePlatform.Android)
                    e = Power.electricity;
            }
        }
    }
}

public class Power
{
    static public float electricity
    {
        get
        {
            //获取电流（微安），避免频繁获取，取一次大概2毫秒
            float electricity = (float)manager.Call<int>("getIntProperty", PARAM_BATTERY);
            //小于1W就认为它的单位是毫安，否则认为是微安
            return ToMA(electricity);
        }
    }
    //获取电压 伏
    static public float voltage { get; private set; }
    //获取电池总容量 毫安
    static public int capacity { get; private set; }
    //获取实时电流参数
    static object[] PARAM_BATTERY = new object[] { 2 }; //BatteryManager.BATTERY_PROPERTY_CURRENT_NOW)
    static AndroidJavaObject manager;
    static Power()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        manager = currActivity.Call<AndroidJavaObject>("getSystemService", new object[] { "batterymanager" });
        capacity = (int)(ToMA((float)manager.Call<int>("getIntProperty", new object[] { 1 })) / ((float)manager.Call<int>("getIntProperty", new object[] { 4 }) / 100f));   //BATTERY_PROPERTY_CHARGE_COUNTER 1 BATTERY_PROPERTY_CAPACITY 4

        AndroidJavaObject receive = currActivity.Call<AndroidJavaObject>("registerReceiver", new object[] { null, new AndroidJavaObject("android.content.IntentFilter", new object[] { "android.intent.action.BATTERY_CHANGED" }) });
        if (receive != null)
        {
            voltage = (float)receive.Call<int>("getIntExtra", new object[] { "voltage", 0 }) / 1000f; //BatteryManager.EXTRA_VOLTAGE
        }
    }
    static float ToMA(float maOrua)
    {
        return maOrua < 10000 ? maOrua : maOrua / 1000f;
    }
}
