using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// 碰撞
    /// </summary>
    public struct CollisionResult
    {
        public RigidBody m_body1;
        public RigidBody m_body2;

        private float Friction;
        private float Restitution;

        private Vector3 Normal;
        private Vector3 Tangent1;
        private Vector3 Tangent2;

        // todo:初始化
        public void init()
        {

        }
    }
}
