using Unity.Entities;

namespace Study2.DOTS.Data
{
    public struct Spawn2Data : IComponentData
    {
        public int CubesCount;
        public float SpawnMaxDistance;
    }

    public struct EntitiesReferences : IComponentData
    {
        public Entity CubesPrefabEntity;
    }
}