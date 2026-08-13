using UnityEngine;
/// <summary>
/// Extension methods for UnityEngine.Rigidbody.
/// </summary>
public static class RigidbodyExtensions
{
    /// <summary>
    /// 在不改变速度的情况下改变刚体的方向.
    /// </summary>
    /// <param name="rigidbody">Rigidbody.</param>
    /// <param name="direction">New direction.</param>
    public static void ChangeDirection(this Rigidbody rigidbody, Vector3 direction)
    {
        rigidbody.velocity = direction * rigidbody.velocity.magnitude;
    }
}
