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

        /// <summary>
        /// 摩擦
        /// </summary>
        private float Friction;
        /// <summary>
        /// 弹性系数
        /// </summary>
        private float Restitution;

        /// <summary>
        /// 法线和切线
        /// </summary>
        private Vector3 Normal;
        private Vector3 Tangent1;
        private Vector3 Tangent2;

        /// <summary>
        /// 相对位置
        /// </summary>
        private Vector3 RelativePos1;
        private Vector3 RelativePos2;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        public void Init(RigidBody body1, RigidBody body2, Vector3 normal, Vector3 pointA, Vector3 pointB)
        {
            m_body1 = body1;
            m_body2 = body2;
        }
    }
}
