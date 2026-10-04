using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace Study2.DOTS.Systems
{
    partial struct Movement2System : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            JobHandle moveHandle = new Jobs.MoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            }.ScheduleParallel(state.Dependency);
            state.Dependency = moveHandle;
        
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}
