using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateBefore(typeof(MovementSystem))]
partial struct CubesSpawnerSystem : ISystem
{
    private EntityQuery _cubesQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _cubesQuery = SystemAPI.QueryBuilder()
            .WithAll<LocalTransform, Speed>()
            .Build();
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<SpawnData>();
        state.RequireForUpdate<SeparationData>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SpawnData spawnData = SystemAPI.GetSingleton<SpawnData>();
        int spawnCount = spawnData.CubesCount - _cubesQuery.CalculateEntityCount();
        if (spawnCount <= 0) return;
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        var entityCommandBuffer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()    
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();
        SpawnJob job = new SpawnJob
        {
            EntitiesReferences = entitiesReferences,
            SpawnData = spawnData,
            Seed = (uint)(SystemAPI.Time.ElapsedTime * 1000),
            EntityCommandBuffer = entityCommandBuffer
        };
        JobHandle spawnJob = job.ScheduleByRef(spawnCount, 64, state.Dependency);
        state.Dependency = spawnJob;
    }
}
