namespace PhysicsDemo
{
    /// <summary>
    /// 射线检测结果
    /// </summary>
    public struct RayCastResult
    {
        public Shape Entity;
        public float Fraction;
        public JVector Normal;
        public bool Hit;
    }
}
