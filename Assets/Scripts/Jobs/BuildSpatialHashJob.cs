using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct BuildSpatialHashJob : IJobEntity
{
    public float CellSize;
    public NativeParallelMultiHashMap<int, float3>.ParallelWriter SpatialHashMap;
    public void Execute(in LocalTransform coordinates, in Speed speed)
    {
        SpatialHashMap.Add(GridHelper.GetHash(GridHelper.GetPosition(coordinates.Position, CellSize)), coordinates.Position);
    }
}
