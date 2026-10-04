using Study2.DOTS.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Study2.DOTS.Jobs
{
    [BurstCompile]
    public struct SpawnJob : IJobParallelFor
    {
        public Spawn2Data SpawnData;
        public EntitiesReferences EntitiesReferences;
        public uint Seed;
        public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;

        public void Execute(int index)
        {
            Random rnd = Random.CreateFromIndex(Seed + (uint)index);
            Entity entity = EntityCommandBuffer.Instantiate(index, EntitiesReferences.CubesPrefabEntity);
            EntityCommandBuffer.SetComponent(index, entity,
                LocalTransform.FromPosition(new float3(
                    rnd.NextFloat(-SpawnData.SpawnMaxDistance, SpawnData.SpawnMaxDistance),
                    rnd.NextFloat(-SpawnData.SpawnMaxDistance, SpawnData.SpawnMaxDistance), 0)));
        }
    }
}