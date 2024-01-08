using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// 游戏物体Controller
    /// </summary>
    public class GameObjectController : MonoBehaviour
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void Initialize(Vector3 position, Quaternion rotation)
        {
            // 逻辑位置为初始位置
            m_logicPosition = position;
            m_logicRotation = rotation;
        }

        /// <summary>
        /// 直接设置当前位置
        /// </summary>
        public virtual void TransformSetDirectly(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
        }

        /// <summary>
        /// 设置目标位置
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param></param>
        public virtual void TargetTransformSet(Vector3 position, Quaternion rotation)
        {
            m_logicPosition = position;
            m_logicRotation = rotation;

            // TODO：后续再平滑插值
            TransformSetDirectly(position, rotation);
        }

        #region 字段&属性

        /// <summary>
        /// 逻辑Transform
        /// </summary>
        protected Vector3 m_logicPosition;
        protected Quaternion m_logicRotation;

        #endregion

    }
}