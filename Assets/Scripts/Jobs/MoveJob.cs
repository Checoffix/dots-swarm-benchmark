using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.PlayerLoop;

[BurstCompile]
public partial struct MoveJob : IJobEntity
{
    public float DeltaTime;
    public float3 TargetPos;
    public float CellSize;
    [ReadOnly] public NativeParallelMultiHashMap<int, float3> SpatialHashMap;
    public float MinSeparationRadius;
    public int MaxNeighboursCount;
    public void Execute(ref LocalTransform transform, in Speed move)
    {
        float3 toTarget = TargetPos - transform.Position;
        float distSq = math.lengthsq(toTarget);
        if (!(distSq > 0.0025f)) return;
        float3 dir = math.normalize(toTarget);
        float3 separationVector = float3.zero;
        int3 position = GridHelper.GetPosition( transform.Position, CellSize);
        separationVector += CheckNeighbours(
        SpatialHashMap.GetValuesForKey(GridHelper.GetHash(new int3(position.x, position.y, position.z))), transform.Position);
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (i == 0 && j == 0 && k == 0) continue;
                    separationVector += CheckNeighbours(
                        SpatialHashMap.GetValuesForKey(GridHelper.GetHash(new int3(position.x + i, position.y + j,
                            position.z + k))), transform.Position);
                }
            }
        } 
        dir += separationVector * 3f;
        float lenSq = math.lengthsq(dir);
        if (lenSq > 1f)
            dir *= math.rsqrt(lenSq);
        transform.Position += dir * (move.Value * DeltaTime);
    }

    private float3 CheckNeighbours(NativeParallelMultiHashMap<int, float3>.Enumerator values, float3 currentPosition)
    {
        float3 separationVector = float3.zero;
        int neighboursCount = 0;
        foreach (float3 value in values)
        {
            if (++neighboursCount > MaxNeighboursCount) break;
            float3 fromTarget = currentPosition - value;
            float lengthSq = math.lengthsq(fromTarget);
            if (lengthSq == 0f || lengthSq > MinSeparationRadius * MinSeparationRadius) 
                continue;
            float invDist = math.rsqrt(lengthSq);
            float dist = lengthSq * invDist;
            separationVector += (fromTarget * invDist) * (1f - dist / MinSeparationRadius);
        }
        return separationVector;
    }
}
