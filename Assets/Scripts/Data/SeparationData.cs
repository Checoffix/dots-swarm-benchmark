using Unity.Entities;

public struct SeparationData : IComponentData
{
    public float CellSize;
    public float MinSeparationRadius;
    public int MaxNeighboursCount;
    public float SeparationForce;
}
