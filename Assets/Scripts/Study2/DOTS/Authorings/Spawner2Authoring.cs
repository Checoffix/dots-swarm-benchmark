using Study2.DOTS.Data;
using Unity.Entities;
using UnityEngine;

namespace Study2.DOTS.Authorings
{
    public class Spawner2Authoring : MonoBehaviour
    {
        public GameObject cubePrefabGameObject;
        public int cubesCount;
        public float spawnMaxDistance;
        class Baker : Baker<Spawner2Authoring>
        {
            public override void Bake(Spawner2Authoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EntitiesReferences
                {
                    CubesPrefabEntity = GetEntity(authoring.cubePrefabGameObject, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity, new Spawn2Data
                {
                    CubesCount = authoring.cubesCount,
                    SpawnMaxDistance = authoring.spawnMaxDistance
                });
            }
        }
    }
}
