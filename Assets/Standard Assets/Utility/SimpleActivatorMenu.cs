using System;
using UnityEngine;
using UnityEngine.UI; // 添加UI命名空间

namespace UnityStandardAssets.Utility
{
    public class SimpleActivatorMenu : MonoBehaviour
    {
        // 一个简单的菜单，给定场景中的游戏对象引用
        public Text camSwitchButton; // 修改为Text类型
        public GameObject[] objects;

        private int m_CurrentActiveObject;

        private void OnEnable()
        {
            // 活动对象从数组的第一个开始
            m_CurrentActiveObject = 0;
            camSwitchButton.text = objects[m_CurrentActiveObject].name; // 更新Text内容
        }

        public void NextCamera()
        {
            int nextactiveobject = m_CurrentActiveObject + 1 >= objects.Length ? 0 : m_CurrentActiveObject + 1;

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(i == nextactiveobject);
            }

            m_CurrentActiveObject = nextactiveobject;
            camSwitchButton.text = objects[m_CurrentActiveObject].name; // 更新Text内容
        }
    }
}
