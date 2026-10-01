using Unity.Entities;
using UnityEngine;

class MovementAuthoring : MonoBehaviour
{
    [Range(1, 10)]
    public float speed = 5f;
    class Baker : Baker<MovementAuthoring>
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


