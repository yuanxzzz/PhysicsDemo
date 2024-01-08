using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// 球型
    /// </summary>
    public class SphereShape : Shape
    {
        public SphereShape(float radius = 1.0f)
        {
            m_radius = radius;
            Mass = 1.0f;
            ShapeUpdate();
        }

        #region Overrides of Shape

        /// <summary>
        /// 重新计算包围盒
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="position"></param>
        /// <param name="box"></param>
        public override void BoundingBoxCalculate(in Matrix4x4 orientation, in Vector3 position, out Bounds box)
        {
            box = new Bounds(position, new Vector3(Radius * 2, Radius * 2, Radius * 2));
        }

        /// <summary>
        /// GJK
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="result"></param>
        public override void SupportMapping(in JVector direction, out JVector result)
        {
            ShapeHelper.SupportSphere(direction, out result);
            result *= m_radius;
        }

        /// <summary>
        /// 更新惯性张量
        /// </summary>
        protected override void InertiaUpdate()
        {
            float inertia = (2f / 5f) * Mass * m_radius * m_radius;

            Matrix4x4 inertiaMatrix = new Matrix4x4();
            inertiaMatrix[0, 0] = inertia;
            inertiaMatrix[1, 1] = inertia;
            inertiaMatrix[2, 2] = inertia;
            inertiaMatrix[3, 3] = 1.0f;

            Inertia = inertiaMatrix;
        }

        #endregion

        /// <summary>
        /// 半径
        /// </summary>
        private float m_radius;
        public float Radius
        {
            get => m_radius;
            set
            {
                m_radius = value;
                ShapeUpdate();
            }
        }
    }
}
