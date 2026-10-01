using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MovementSystem : ISystem
{
    public NativeParallelMultiHashMap<int, float3> SpatialHashMap;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        SpatialHashMap = new NativeParallelMultiHashMap<int, float3>(131072, Allocator.Persistent);
        state.RequireForUpdate<TargetData>();
        state.RequireForUpdate<SeparationData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SeparationData separationData = SystemAPI.GetSingleton<SeparationData>();
        state.CompleteDependency();
        SpatialHashMap.Clear();
        JobHandle buildHandle = new BuildSpatialHashJob
        {
            CellSize = separationData.CellSize,
            SpatialHashMap = SpatialHashMap.AsParallelWriter()
        }.ScheduleParallel(state.Dependency);
        
        Entity playerEntity = SystemAPI.GetSingletonEntity<TargetData>();
        JobHandle moveHandle = new MoveJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            TargetPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position,
            CellSize = separationData.CellSize,
            SpatialHashMap = SpatialHashMap,
            MinSeparationRadius =  separationData.MinSeparationRadius,
            MaxNeighboursCount = separationData.MaxNeighboursCount,
        }.ScheduleParallel(buildHandle);
        state.Dependency = moveHandle;
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        SpatialHashMap.Dispose();
    }
}
