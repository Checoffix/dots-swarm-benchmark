using Study1.DOTS.Data;
using Unity.Entities;
using UnityEngine;

namespace Study1.DOTS.Authorings
{
    public class MovementAuthoring : MonoBehaviour
    {
        [Range(1, 10)]
        public float speed = 5f;
        class Baker : Unity.Entities.Baker<MovementAuthoring>
        {
            public override void Bake(MovementAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        
                AddComponent(entity, new Speed
                {
                    Value = authoring.speed
                });
            }
        }
    }
}


