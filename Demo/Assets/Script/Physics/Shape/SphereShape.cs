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
            this.radius = radius;
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
            result *= radius;
        }

        /// <summary>
        /// 更新惯性张量
        /// </summary>
        protected override void InertiaUpdate()
        {

        }

        #endregion

        /// <summary>
        /// 半径
        /// </summary>
        private float radius;
        public float Radius
        {
            get => radius;
            set
            {
                radius = value;
                ShapeUpdate();
            }
        }
    }
}
