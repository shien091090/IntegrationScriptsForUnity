using UnityEngine;

namespace SNShien.Common.AdapterTools
{
    public class Collider2DAdapter : ICollider2DAdapter
    {
        public int Layer => collider.gameObject.layer;
        public Vector3 Position => collider.transform.position;
        private readonly Collider2D collider;

        public Collider2DAdapter(Collider2D collider)
        {
            this.collider = collider;
        }

        public T GetComponent<T>()
        {
            return collider.GetComponent<T>();
        }
    }
}