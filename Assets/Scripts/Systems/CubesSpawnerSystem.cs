using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct CubesSpawnerSystem : ISystem
{
    private Random _rnd;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _rnd = new Random(51897151u);
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<SpawnData>();
        state.RequireForUpdate<SeparationData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        SpawnData spawnData = SystemAPI.GetSingleton<SpawnData>();
        for (int i = 0; i < spawnData.CubesCount; i++)
        {
            Entity entity = state.EntityManager.Instantiate(entitiesReferences.BulletPrefabEntity);
            SystemAPI.SetComponent(entity, LocalTransform.FromPosition(new float3(_rnd.NextFloat(-spawnData.SpawnMaxDistance, spawnData.SpawnMaxDistance), _rnd.NextFloat(-spawnData.SpawnMaxDistance, spawnData.SpawnMaxDistance), 0)));
        }
        state.Enabled = false;
    }
}
