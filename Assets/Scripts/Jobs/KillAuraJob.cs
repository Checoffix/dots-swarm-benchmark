using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct KillAuraJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public float3 TargetPos;
    public float KillAuraRadius;
    public void Execute([EntityIndexInQuery]int sortKey, in Entity entity, in LocalTransform transform, in Speed speed)
    {
        if (math.lengthsq(TargetPos - transform.Position) >= KillAuraRadius * KillAuraRadius) return;
        Ecb.DestroyEntity(sortKey, entity);
    }
}
