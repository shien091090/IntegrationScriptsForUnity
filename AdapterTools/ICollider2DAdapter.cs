using UnityEngine;

namespace SNShien.Common.AdapterTools
{
    public interface ICollider2DAdapter
    {
        int Layer { get; }
        Vector3 Position { get; }
        T GetComponent<T>();
    }
}