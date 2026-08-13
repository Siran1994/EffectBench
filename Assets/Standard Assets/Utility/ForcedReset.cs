using System;
using UnityEngine;
using UnityEngine.UI; // 添加UI命名空间
using UnityStandardAssets.CrossPlatformInput;

public class ForcedReset : MonoBehaviour
{
    private void Update()
    {
        // 如果我们按下了重置按钮 ...
        if (CrossPlatformInputManager.GetButtonDown("ResetObject"))
        {
            // ... 重新加载场景
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
