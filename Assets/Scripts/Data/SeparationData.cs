using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct SeparationData : IComponentData
{
    public float CellSize;
    public float MinSeparationRadius;
    public int MaxNeighboursCount;
    public float SeparationForce;
}
