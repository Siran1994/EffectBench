using UnityEngine;
/// <summary>
/// 扩展方法 for UnityEngine.Component.
/// </summary>
public static class ComponentExtension
{
    /// <summary>
    /// 将组件附加到给定组件的游戏对象
    /// </summary>
    /// <param name="component">Component.</param>
    /// <returns>Newly attached component.</returns>
    public static T AddComponent<T>(this Component component) where T : Component
    {
        return component.gameObject.AddComponent<T>();
    }

    /// <summary>
    /// 获取附加到给定组件的游戏对象的组件.
    /// 如果找不到，则附加一个新的并返回.
    /// </summary>
    /// <param name="component">Component.</param>
    /// <returns>Previously or newly attached component.</returns>
    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
        return component.GetComponent<T>() ?? component.AddComponent<T>();
    }

    /// <summary>
    /// 检查组件的游戏对象是否附加了T类型的组件
    /// </summary>
    /// <param name="component">Component.</param>
    /// <returns>True when component is attached.</returns>
    public static bool HasComponent<T>(this Component component) where T : Component
    {
        return component.GetComponent<T>() != null;
    }
}
