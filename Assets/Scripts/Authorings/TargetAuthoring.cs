using Unity.Entities;
using UnityEngine;

class TargetAuthoring : MonoBehaviour
{
    public float killAuraRadius;
    class Baker : Baker<TargetAuthoring>
    {
        public override void Bake(TargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TargetData
            {
                KillAuraRadius = authoring.killAuraRadius
            });
        }
    }
}


