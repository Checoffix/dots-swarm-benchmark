using Study2.DOTS.Data;
using Study2.DOTS.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Study2.DOTS.Systems
{
    [UpdateBefore(typeof(Movement2System))]
    partial struct CubeSpawner2System : ISystem
    {
        private EntityQuery _cubesQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _cubesQuery = SystemAPI.QueryBuilder()
                .WithAll<LocalTransform, Speed2>()
                .Build();
            state.RequireForUpdate<EntitiesReferences>();
            state.RequireForUpdate<Spawn2Data>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Spawn2Data spawnData = SystemAPI.GetSingleton<Spawn2Data>();
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
            state.Enabled = false;
        }
    }
}
