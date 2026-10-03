using Study1.DOTS.Data;
using Unity.Entities;
using UnityEngine;

namespace Study1.DOTS.Authorings
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject bulletPrefabGameObject;
        public int cubesCount;
        public float spawnMaxDistance;
        public float cellSize;
        public float minSeparationRadius;
        public int maxNeighboursCount;
        public float separationForce;
        class Baker : Unity.Entities.Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EntitiesReferences
                {
                    BulletPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity, new SpawnData
                {
                    CubesCount = authoring.cubesCount,
                    SpawnMaxDistance = authoring.spawnMaxDistance
                });
                AddComponent(entity, new SeparationData
                {
                    MinSeparationRadius = authoring.minSeparationRadius,
                    CellSize = authoring.cellSize,
                    MaxNeighboursCount = authoring.maxNeighboursCount,
                    SeparationForce = authoring.separationForce
                });
            }
        }
    }
}
