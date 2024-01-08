using UnityEngine;

namespace PhysicsDemo
{
    public class CubeShape : Shape
    {
        public CubeShape(Vector3 size)
        {
            halfSize = 0.5f * size;
            Mass = 1.0f;
            ShapeUpdate();
        }

        #region Overrides of Shape

        /// <summary>
        /// GJK
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="result"></param>
        public override void SupportMapping(in JVector direction, out JVector result)
        {
            ShapeHelper.SupportCube(direction, out result);
            result *= 0.5f;
        }

        /// <summary>
        /// 更新惯性张量
        /// </summary>
        protected override void InertiaUpdate()
        {
            Vector3 size = halfSize * 2.0f;

            Matrix4x4 inertiaMatrix = new Matrix4x4();
            inertiaMatrix[0, 0] = 1.0f / 12.0f * Mass * (size.y * size.y + size.z * size.z); ;
            inertiaMatrix[1, 1] = 1.0f / 12.0f * Mass * (size.x * size.x + size.z * size.z);
            inertiaMatrix[2, 2] = 1.0f / 12.0f * Mass * (size.x * size.x + size.y * size.y);
            inertiaMatrix[3, 3] = 1.0f;

            Inertia = inertiaMatrix;
        }

        /// <summary>
        /// 重新计算包围盒
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="position"></param>
        /// <param name="box"></param>
        public override void BoundingBoxCalculate(in Matrix4x4 orientation, in Vector3 position, out Bounds box)
        {
            box = new Bounds();
            var abs = orientation.Absolute();
            MathHelper.Transform(halfSize, abs, out var temp);
            box.SetMinMax(temp.Negate(), temp);
            box.center = position;
        }

        #endregion

        public Vector3 Size
        {
            get => 2.0f * halfSize;
            set
            {
                halfSize = value * 0.5f;
                ShapeUpdate();
            }
        }

        private Vector3 halfSize;
    }
}
