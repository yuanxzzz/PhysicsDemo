using System;
using System.Collections.Generic;

namespace PhysicsDemo
{

    /// <summary>
    /// Provides helper methods for calculating the properties of implicitly defined Shapes.
    /// </summary>
    public static class ShapeHelper
    {
        public static void SupportLine(in JVector direction, out JVector result)
        {
            JVector a = new JVector(0, 0.5f, 0);
            JVector b = new JVector(0, -0.5f, 0);

            double t0 = JVector.Dot(direction, a);
            double t2 = JVector.Dot(direction, b);

            if (t0 > t2) result = a;
            else result = b;
        }

        public static void SupportTriangle(in JVector direction, out JVector result)
        {
            JVector a = new JVector(0, 0, 1);
            JVector b = new JVector(-1, 0, -1);
            JVector c = new JVector(1, 0, -1);

            double t0 = JVector.Dot(direction, a);
            double t1 = JVector.Dot(direction, b);
            double t2 = JVector.Dot(direction, c);

            if (t0 > t1) result = t0 > t2 ? a : c;
            else result = t2 > t1 ? c : b;
        }

        public static void SupportDisc(in JVector direction, out JVector result)
        {
            result.X = direction.X;
            result.Y = 0;
            result.Z = direction.Z;

            if (result.LengthSquared() > 1e-12) result.Normalize();
            result *= 0.5f;
        }

        public static void SupportSphere(in JVector direction, out JVector result)
        {
            result = direction;
            result.Normalize();
        }

        public static void SupportCone(in JVector direction, out JVector result)
        {
            ShapeHelper.SupportDisc(direction, out JVector res1);
            JVector res2 = new JVector(0, 1, 0);

            if (JVector.Dot(direction, res1) >= JVector.Dot(direction, res2)) result = res1;
            else result = res2;

            result.Y -= 0.5f;
        }

        public static void SupportCube(in JVector direction, out JVector result)
        {
            result.X = Math.Sign(direction.X);
            result.Y = Math.Sign(direction.Y);
            result.Z = Math.Sign(direction.Z);
        }
    }
}
