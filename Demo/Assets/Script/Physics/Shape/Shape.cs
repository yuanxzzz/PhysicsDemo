using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Shape
    /// </summary>
    public abstract class Shape : ISupportMappable
    {
        public Shape()
        {
            m_shapeId = World.RequestId();
        }

        #region Implementation of ISupportMappable

        /// <summary>
        /// GJK
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="result"></param>
        public abstract void SupportMapping(in JVector direction, out JVector result);

        #endregion

        #region 公共方法

        public void ShapeUpdate()
        {
            InertiaUpdate();
            BBoxUpdate();
        }


        /// <summary>
        /// 关联到Body
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public void RigidBodyAttach(RigidBody? body)
        {
            if (RigidBody == null)
            {
                RigidBody = body;
            }
        }

        /// <summary>
        /// 移除关联
        /// </summary>
        public void RigidBodyDetach()
        {
            RigidBody = null!;
        }

        /// <summary>
        /// 更新惯性张量
        /// </summary>
        protected abstract void InertiaUpdate();

        /// <summary>
        /// 更新包围盒
        /// </summary>
        public virtual void BBoxUpdate()
        {
            if (RigidBody == null)
            {
                BoundingBoxCalculate(Matrix4x4.identity, Vector3.zero, out Bounds box);
                WorldBoundingBox = box;
            }
            else
            {
                BoundingBoxCalculate(RigidBody.Orientation, RigidBody.Position, out Bounds box);
                WorldBoundingBox = box;
            }
        }

        /// <summary>
        /// 重新计算包围盒
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="position"></param>
        /// <param name="box"></param>
        public abstract void BoundingBoxCalculate(in Matrix4x4 orientation, in Vector3 position, out Bounds box);

        #endregion

        #region 私有方法

        #endregion

        #region 字段&属性

        /// <summary>
        /// id
        /// </summary>
        public readonly ulong m_shapeId;

        /// <summary>
        /// 局部位置
        /// </summary>
        public Vector3 m_localPosition;

        /// <summary>
        /// 关联Body
        /// </summary>
        public RigidBody RigidBody { get; private set; }

        /// <summary>
        /// 包围盒
        /// </summary>
        public Bounds WorldBoundingBox { get; protected set; }

        /// <summary>
        /// 惯性张量
        /// </summary>
        public Matrix4x4 Inertia { get; protected set; }

        /// <summary>
        /// 质量
        /// </summary>
        public float Mass { get; protected set; }

        /// <summary>
        /// Shape对应位置
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return RigidBody.Position + m_localPosition;
            }
        }

        /// <summary>
        /// 线速度
        /// </summary>
        public virtual Vector3 Velocity => RigidBody != null ? RigidBody.Velocity : Vector3.zero;

        #endregion
    }
}
