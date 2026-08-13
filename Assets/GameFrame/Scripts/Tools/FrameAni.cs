using UnityEngine;
using UnityEngine.UI;
/*
 * 序列帧动画播放步骤：
 * 1、将脚本挂载到Image上
 * 2、将所有动画需要的精灵放入数组中
 * 3、在运行中，将isStart设置为true，即可播放动画
 */
public class FrameAni : MonoBehaviour
{
    public Sprite[] m_sprites;  //多张图片
    public bool isStart = true; //是否开始播放
    public float m_timePerFrame = 0.05f; //播放的时间间隔
    private float m_time;       //当前播放的时间
    private Image m_Image;
    private int m_curFrame = 0; //当前播放第几张

    public int FrameCount  //图片张数
    {
        get
        {
            return m_sprites.Length;
        }
    }
    void OnEnable()
    {
        isStart = true;
    }
    void OnDisable()
    {
        isStart = false;
    }
    void Start()
    {
        m_Image = this.GetComponent<Image>();
        if (m_Image == null)
        {
            m_Image = gameObject.AddComponent<Image>();
        }
        Show(m_curFrame);
    }

    public void Show(int frame)
    {
        if (frame >= FrameCount)
            m_curFrame = 0;
        m_Image.sprite = m_sprites[m_curFrame];

    }

    void Update()
    {
        if (!isStart)
            return;
        m_time += Time.deltaTime;
        if (m_time > m_timePerFrame)
        {
            m_time = 0;
            m_curFrame++;
            if (m_curFrame >= FrameCount)
            {
                m_curFrame = FrameCount - 1;
                isStart = false;
            }
            Show(m_curFrame);
        }
    }
}