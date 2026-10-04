using Study2.DOTS.Data;
using Unity.Entities;
using UnityEngine;

namespace Study2.DOTS.Authorings
{
    public class Movement2Authoring : MonoBehaviour
    {
        [Range(1, 10)]
        public float speed = 5f;
        class Baker : Baker<Movement2Authoring>
        {
            public override void Bake(Movement2Authoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        
                AddComponent(entity, new Speed2
                {
                    Value = authoring.speed
                });
            }
        }
    }
}


