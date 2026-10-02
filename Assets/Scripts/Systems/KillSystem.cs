using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateAfter(typeof(MovementSystem))]
partial struct KillSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TargetData>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();
        var targetEntity = SystemAPI.GetSingletonEntity<TargetData>();
        JobHandle killJob = new KillAuraJob()
        {
            Ecb = entityCommandBuffer,
            TargetPos = SystemAPI.GetComponent<LocalTransform>(targetEntity).Position,
            KillAuraRadius = SystemAPI.GetComponent<TargetData>(targetEntity).KillAuraRadius
        }.ScheduleParallel(state.Dependency);
        state.Dependency = killJob;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
