using System.Collections.Generic;
using UnityEngine;

namespace PhysicsDemo
{
    public class Octree
    {
        /// <summary>
        /// 构建八叉树
        /// </summary>
        public Octree(Vector3 boundSize, float minNodeSize, int maxBodyCount)
        {
            m_minNodeSize = minNodeSize;
            m_maxBodyCount = maxBodyCount;
            Bounds bounds = new Bounds(Vector3.zero, boundSize);
            m_rootNode = new OctreeNode(bounds, m_minNodeSize, m_maxBodyCount);
        }

        /// <summary>
        /// 添加物体
        /// </summary>
        /// <param name="shape"></param>
        public void ShapeAdd(Shape shape)
        {
            m_rootNode.ShapeAdd(shape);
        }

        /// <summary>
        /// 移除物体
        /// </summary>
        /// <param name="shape"></param>
        public void ShapeRemove(Shape shape)
        {
            m_rootNode.ShapeRemove(shape);
        }

        /// <summary>
        /// 更新物体
        /// </summary>
        /// <param name="shape"></param>
        public void ShapeUpdate(Shape shape)
        {
            // 去除所有子节点中的对应body
            ShapeRemove(shape);
            // 重新插入body
            ShapeAdd(shape);
        }

        #region 字段&属性

        /// <summary>
        /// 根节点
        /// </summary>
        public OctreeNode m_rootNode;

        private readonly float m_minNodeSize;

        private readonly int m_maxBodyCount;

        #endregion

    }

    /// <summary>
    /// 八叉树节点
    /// </summary>
    public class OctreeNode
    {
        /// <summary>
        /// 构建节点
        /// </summary>
        public OctreeNode(Bounds b, float minNodeSize, int maxShapeCount)
        {
            // 节点边界
            m_nodeBounds = b;
            // 最小大小
            m_minBoundSize = minNodeSize;
            // 最大物体个数
            m_maxShapeCount = maxShapeCount;

            // 子节点边界
            float quarter = m_nodeBounds.size.y / 4.0f;
            float childLength = m_nodeBounds.size.y / 2;
            Vector3 childSize = new Vector3(childLength, childLength, childLength);

            m_childrenBounds = new Bounds[8];
            m_childrenBounds[0] = new Bounds(m_nodeBounds.center + new Vector3(-quarter, quarter, -quarter), childSize);
            m_childrenBounds[1] = new Bounds(m_nodeBounds.center + new Vector3(quarter, quarter, -quarter), childSize);
            m_childrenBounds[2] = new Bounds(m_nodeBounds.center + new Vector3(-quarter, quarter, quarter), childSize);
            m_childrenBounds[3] = new Bounds(m_nodeBounds.center + new Vector3(quarter, quarter, quarter), childSize);
            m_childrenBounds[4] = new Bounds(m_nodeBounds.center + new Vector3(-quarter, -quarter, -quarter), childSize);
            m_childrenBounds[5] = new Bounds(m_nodeBounds.center + new Vector3(quarter, -quarter, -quarter), childSize);
            m_childrenBounds[6] = new Bounds(m_nodeBounds.center + new Vector3(-quarter, -quarter, quarter), childSize);
            m_childrenBounds[7] = new Bounds(m_nodeBounds.center + new Vector3(quarter, -quarter, quarter), childSize);
        }

        /// <summary>
        /// 移除物体
        /// </summary>
        /// <param name="body"></param>
        public bool ShapeRemove(Shape shape)
        {
            // 只有叶子节点存放信息
            if (m_childrenNode == null)
            {
                bool isRemove = m_shapes.Remove(shape);
                return isRemove;
            }
            // 若不是叶子节点则需要判断是否需要进行合并
            else
            {
                bool checkMerge = false;
                // 判断所有子节点
                foreach (var node in m_childrenNode)
                {
                    if (node.ShapeRemove(shape))
                    {
                        checkMerge = true;
                    }
                }
                // 若子节点为叶子节点且成功移除对应shape 或者 子节点进行了合并
                // 则对当前节点进行合并检测
                if (checkMerge)
                {
                    // 当前节点下所有的shape个数
                    int total = 0;
                    foreach (var node in m_childrenNode)
                    {
                        total = node.m_shapes.Count;
                    }
                    // 若总数小于maxCount，则将子节点进行合并
                    if (total <= m_maxShapeCount)
                    {
                        m_shapes = GetMergedShapes(this);
                        m_childrenNode = null;
                        // 返回true让父节点继续检查合并
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// 在当前节点记录Body
        /// </summary>
        /// <param name="shape"></param>
        public void ShapeRecord(Shape shape)
        {
            if (!m_shapes.Contains(shape))
            {
                m_shapes.Add(shape);
            }
        }

        /// <summary>
        /// 添加Body
        /// </summary>
        /// <param name="shape"></param>
        public void ShapeAdd(Shape shape)
        {
            DivideAndAdd(shape);
        }

        /// <summary>
        /// 添加分支
        /// </summary>
        /// <param name="shape"></param>
        private void DivideAndAdd(Shape shape)
        {
            // 判断是否需要细分当前节点
            bool shouldDivide = m_shapes.Count >= m_maxShapeCount && m_nodeBounds.size.y > m_minBoundSize;

            // 细分当前节点
            if (shouldDivide)
            {
                // 构建子节点
                if (m_childrenNode == null)
                {
                    m_childrenNode = new OctreeNode[8];
                }

                bool dividing = false;
                for (int i = 0; i < 8; i++)
                {
                    if (m_childrenNode[i] == null)
                    {
                        m_childrenNode[i] = new OctreeNode(m_childrenBounds[i], m_minBoundSize, m_maxShapeCount);
                    }

                    // 是否相交
                    if (m_childrenBounds[i].Intersects(shape.WorldBoundingBox))
                    {
                        dividing = true;
                        m_childrenNode[i].DivideAndAdd(shape);
                    }
                }

                // 若添加了新分支，父节点中的元素也需要下沉到子节点
                if (dividing)
                {
                    // 将当前节点存入子分支
                    foreach (var recordedShape in m_shapes)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            // 是否相交
                            if (m_childrenBounds[i].Intersects(recordedShape.WorldBoundingBox))
                            {
                                m_childrenNode[i].DivideAndAdd(recordedShape);
                            }
                        }
                    }

                    // 清空当前节点记录
                    m_shapes.Clear();
                }
                else
                {
                    m_childrenNode = null;
                }
            }
            // 添加新形状到当前节点
            ShapeRecord(shape);
        }

        /// <summary>
        /// 递归计算子节点Shape数量
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int TotalShapesInSubtreeGet(OctreeNode node)
        {
            int totalObjects = node.m_shapes.Count;

            // 递归计算子节点的物体数量
            if (node.m_childrenNode != null)
            {
                foreach (var child in node.m_childrenNode)
                {
                    if (child != null)
                        totalObjects += TotalShapesInSubtreeGet(child);
                }
            }

            return totalObjects;
        }

        /// <summary>
        /// 获取子分支Shape
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<Shape> GetMergedShapes(OctreeNode node)
        {
            List<Shape> mergedObjects = new List<Shape>();

            // 合并子节点的物体到当前节点
            foreach (var child in node.m_childrenNode)
            {
                if (child != null)
                {

                    mergedObjects.AddRange(child.m_shapes);
                }
            }

            return mergedObjects;
        }

        #region 调试

        public void Draw()
        {
            Gizmos.color = new Color(0, 1, 0);
            Gizmos.DrawWireCube(m_nodeBounds.center, m_nodeBounds.size);
            if (m_childrenNode != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (m_childrenNode[i] != null)
                    {
                        m_childrenNode[i].Draw();
                    }
                }
            }
        }

        #endregion

        #region 字段&属性

        /// <summary>
        /// 节点边界
        /// </summary>
        private Bounds m_nodeBounds;

        /// <summary>
        /// 子节点边界
        /// </summary>
        private readonly Bounds[] m_childrenBounds;

        /// <summary>
        /// 子节点
        /// </summary>
        public OctreeNode[] m_childrenNode;

        /// <summary>
        /// 当前节点记录的Shape
        /// </summary>
        public List<Shape> m_shapes = new List<Shape>();

        /// <summary>
        /// 最小边界大小
        /// </summary>
        private readonly float m_minBoundSize;

        /// <summary>
        /// 最大物体个数
        /// </summary>
        private readonly int m_maxShapeCount;

        #endregion
    }

}
