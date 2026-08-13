using System;
using System.Collections;
using UnityEngine;
public class GyroTool : MonoSigleton<GyroTool>
{
    protected override void Awake()
    {
        base.Awake();
    }
    //手机晃动的有效距离
    public float Distance
    {
        get => distance;
        set
        {
            distance = Mathf.Clamp01(value);
        }
    }
    private void Start()
    {
        Init(delegate { Messenger.Broadcast("ItemShake"); });
    }
    /// <summary>
    /// 初始化摇一摇功能
    /// </summary>
    /// <param name="shakeListener">摇一摇触发后的监听事件</param>
    /// <param name="shakeSensitive">摇一摇的的敏感度(0,1)</param>
    public void Init(Action shakeListener, float shakeSensitive = 0.5f)
    {
        shakeAction = shakeListener;
        Distance = shakeSensitive;

        StartCoroutine(ShakeCor());
    }
    IEnumerator ShakeCor()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            Shake();

            // 触发一次摇一摇后，间隔一定时间再次监听
            if (isShake == true)
            {
                yield return new WaitForSeconds(intervalShakeTime);
                isShake = false;
            }
        }
    }
    /// <summary>
    /// 摇一摇功能
    /// </summary>
    void Shake()
    {
        new_y = Input.acceleration.y;
        currentShakeDistance_y = new_y - old_y;
        old_y = new_y;

        new_x = Input.acceleration.x;
        currentShakeDistance_x = new_x - old_x;
        old_x = new_x;

        if (currentShakeDistance_y > Distance || currentShakeDistance_x > Distance)
        {
            isShake = true;
            // 摇一摇事件
            if (shakeAction != null)
            {
                shakeAction();
            }
        }
    }

    //记录上一次的重力感应的Y值
    private float old_y = 0;
    //记录当前的重力感应的Y值
    private float new_y;
    //当前手机晃动的距离
    private float currentShakeDistance_y = 0;

    //记录上一次的重力感应的X值
    private float old_x = 0;
    //记录当前的重力感应的X值
    private float new_x;
    //当前手机晃动的距离
    private float currentShakeDistance_x = 0;

    //手机晃动的有效距离
    private float distance = 0.75f;

    // 触发摇一摇震动后的委托事件
    Action shakeAction;

    // 是否触发
    bool isShake = false;

    // 触发一次的间隔时间
    float intervalShakeTime = 0.1f;
}