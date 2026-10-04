using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Study2.GO.Jobs
{
    [BurstCompile]
    public struct MovementJob : IJobParallelForTransform
    {
        [ReadOnly]
        public float Speed;
        public float DeltaTime;
        public void Execute(int index, TransformAccess transform)
        {
            transform.position += new Vector3(0, 0, 1) * (Speed * DeltaTime);
        }
    }
}