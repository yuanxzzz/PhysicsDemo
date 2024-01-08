using UnityEngine;

namespace PhysicsDemo
{
    public interface ISupportMappable
    {
        void SupportMapping(in JVector direction, out JVector result);
    }
}
