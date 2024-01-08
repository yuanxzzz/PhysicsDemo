using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace PhysicsDemo
{

    public static class MathHelper
    {
        #region Quaternion

        /// <summary>
        /// 四元素乘法
        /// </summary>
        /// <param name="quaternion1"></param>
        /// <param name="quaternion2"></param>
        /// <param name="result"></param>
        public static void Multiply(Quaternion quaternion1, Quaternion quaternion2, out Quaternion result)
        {
            float r1 = quaternion1.w;
            float i1 = quaternion1.x;
            float j1 = quaternion1.y;
            float k1 = quaternion1.z;

            float r2 = quaternion2.w;
            float i2 = quaternion2.x;
            float j2 = quaternion2.y;
            float k2 = quaternion2.z;

            result.w = r1 * r2 - (i1 * i2 + j1 * j2 + k1 * k2);
            result.x = r1 * i2 + r2 * i1 + j1 * k2 - k1 * j2;
            result.y = r1 * j2 + r2 * j1 + k1 * i2 - i1 * k2;
            result.z = r1 * k2 + r2 * k1 + i1 * j2 - j1 * i2;
        }

        #endregion

        #region MyRegion

        /// <summary>
        /// 矩阵加法
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <param name="result"></param>
        public static void Add(in Matrix4x4 matrix1, in Matrix4x4 matrix2, out Matrix4x4 result)
        {
            result = new Matrix4x4
            {
                m00 = matrix1.m00 + matrix2.m00,
                m01 = matrix1.m01 + matrix2.m01,
                m02 = matrix1.m02 + matrix2.m02,
                m03 = matrix1.m03 + matrix2.m03,
                m10 = matrix1.m10 + matrix2.m10,
                m11 = matrix1.m11 + matrix2.m11,
                m12 = matrix1.m12 + matrix2.m12,
                m13 = matrix1.m13 + matrix2.m13,
                m20 = matrix1.m20 + matrix2.m20,
                m21 = matrix1.m21 + matrix2.m21,
                m22 = matrix1.m22 + matrix2.m22,
                m23 = matrix1.m23 + matrix2.m23,
                m30 = matrix1.m30 + matrix2.m30,
                m31 = matrix1.m31 + matrix2.m31,
                m32 = matrix1.m32 + matrix2.m32,
                m33 = matrix1.m33 + matrix2.m33
            };
        }

        /// <summary>
        /// 矩阵乘积的转置
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <param name="result"></param>
        public static void MultiplyTransposed(in Matrix4x4 matrix1, in Matrix4x4 matrix2, out Matrix4x4 result)
        {
            float num0 = matrix1.m00 * matrix2.m00 + matrix1.m01 * matrix2.m01 + matrix1.m02 * matrix2.m02 + matrix1.m03 * matrix2.m03;
            float num1 = matrix1.m00 * matrix2.m10 + matrix1.m01 * matrix2.m11 + matrix1.m02 * matrix2.m12 + matrix1.m03 * matrix2.m13;
            float num2 = matrix1.m00 * matrix2.m20 + matrix1.m01 * matrix2.m21 + matrix1.m02 * matrix2.m22 + matrix1.m03 * matrix2.m23;
            float num3 = matrix1.m00 * matrix2.m30 + matrix1.m01 * matrix2.m31 + matrix1.m02 * matrix2.m32 + matrix1.m03 * matrix2.m33;

            float num4 = matrix1.m10 * matrix2.m00 + matrix1.m11 * matrix2.m01 + matrix1.m12 * matrix2.m02 + matrix1.m13 * matrix2.m03;
            float num5 = matrix1.m10 * matrix2.m10 + matrix1.m11 * matrix2.m11 + matrix1.m12 * matrix2.m12 + matrix1.m13 * matrix2.m13;
            float num6 = matrix1.m10 * matrix2.m20 + matrix1.m11 * matrix2.m21 + matrix1.m12 * matrix2.m22 + matrix1.m13 * matrix2.m23;
            float num7 = matrix1.m10 * matrix2.m30 + matrix1.m11 * matrix2.m31 + matrix1.m12 * matrix2.m32 + matrix1.m13 * matrix2.m33;

            float num8 = matrix1.m20 * matrix2.m00 + matrix1.m21 * matrix2.m01 + matrix1.m22 * matrix2.m02 + matrix1.m23 * matrix2.m03;
            float num9 = matrix1.m20 * matrix2.m10 + matrix1.m21 * matrix2.m11 + matrix1.m22 * matrix2.m12 + matrix1.m23 * matrix2.m13;
            float num10 = matrix1.m20 * matrix2.m20 + matrix1.m21 * matrix2.m21 + matrix1.m22 * matrix2.m22 + matrix1.m23 * matrix2.m23;
            float num11 = matrix1.m20 * matrix2.m30 + matrix1.m21 * matrix2.m31 + matrix1.m22 * matrix2.m32 + matrix1.m23 * matrix2.m33;

            float num12 = matrix1.m30 * matrix2.m00 + matrix1.m31 * matrix2.m01 + matrix1.m32 * matrix2.m02 + matrix1.m33 * matrix2.m03;
            float num13 = matrix1.m30 * matrix2.m10 + matrix1.m31 * matrix2.m11 + matrix1.m32 * matrix2.m12 + matrix1.m33 * matrix2.m13;
            float num14 = matrix1.m30 * matrix2.m20 + matrix1.m31 * matrix2.m21 + matrix1.m32 * matrix2.m22 + matrix1.m33 * matrix2.m23;
            float num15 = matrix1.m30 * matrix2.m30 + matrix1.m31 * matrix2.m31 + matrix1.m32 * matrix2.m32 + matrix1.m33 * matrix2.m33;

            result = new Matrix4x4
            {
                m00 = num0,
                m01 = num1,
                m02 = num2,
                m03 = num3,
                m10 = num4,
                m11 = num5,
                m12 = num6,
                m13 = num7,
                m20 = num8,
                m21 = num9,
                m22 = num10,
                m23 = num11,
                m30 = num12,
                m31 = num13,
                m32 = num14,
                m33 = num15
            };
        }

        #endregion

        /// <summary>
        /// 转置变换
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        public static void TransposedTransform(Vector3 vector, Matrix4x4 matrix, out Vector3 result)
        {
            float x = vector.x * matrix.m00 + vector.y * matrix.m10 + vector.z * matrix.m20;
            float y = vector.x * matrix.m01 + vector.y * matrix.m11 + vector.z * matrix.m21;
            float z = vector.x * matrix.m02 + vector.y * matrix.m12 + vector.z * matrix.m22;

            result = new Vector3(x, y, z);
        }

        /// <summary>
        /// 变换
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <param name="result"></param>
        public static void Transform(Vector3 vector, Matrix4x4 matrix, out Vector3 result)
        {
            float x = vector.x * matrix.m00 + vector.y * matrix.m01 + vector.z * matrix.m02 + matrix.m03;
            float y = vector.x * matrix.m10 + vector.y * matrix.m11 + vector.z * matrix.m12 + matrix.m13;
            float z = vector.x * matrix.m20 + vector.y * matrix.m21 + vector.z * matrix.m22 + matrix.m23;

            result = new Vector3(x, y, z);
        }


    }

}