using Unity.Entities;

namespace Study1.DOTS.Data
{
    public struct SeparationData : IComponentData
    {
        public float CellSize;
        public float MinSeparationRadius;
        public int MaxNeighboursCount;
        public float SeparationForce;
    }
}
