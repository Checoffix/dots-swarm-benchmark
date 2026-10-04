using Study2.DOTS.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Study2.DOTS.Jobs
{
    [BurstCompile]
    public partial struct MoveJob : IJobEntity
    {
        public float DeltaTime;
        public void Execute(ref LocalTransform transform, in Speed2 move)
        {
            transform.Position += new float3(0, 0, 1) * (move.Value * DeltaTime);
        }
    }
}
