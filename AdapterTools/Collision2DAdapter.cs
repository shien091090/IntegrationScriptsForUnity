using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SNShien.Common.AdapterTools
{
    public class Collision2DAdapter : ICollision2DAdapter
    {
        public int Layer => collision.gameObject.layer;
        public List<Vector2> ContactPoints => collision.contacts.Select(x => x.point).ToList();

        private readonly Collision2D collision;

        public Collision2DAdapter(Collision2D col)
        {
            collision = col;
        }

        public bool CheckPhysicsOverlapCircle(Vector3 point, float radius, GameConst.GameObjectLayerType layerMask)
        {
            return Physics2D.OverlapCircle(point, radius, LayerMask.GetMask(layerMask.ToString()));
        }

        public T GetComponent<T>()
        {
            return collision.gameObject.GetComponent<T>();
        }
    }
}