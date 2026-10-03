using Study1.DOTS.Data;
using Unity.Entities;
using UnityEngine;

namespace Study1.DOTS.Authorings
{
    public class TargetAuthoring : MonoBehaviour
    {
        public float killAuraRadius;
        class Baker : Unity.Entities.Baker<TargetAuthoring>
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
}


