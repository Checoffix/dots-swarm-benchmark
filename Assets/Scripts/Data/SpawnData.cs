using Unity.Entities;

public struct SpawnData : IComponentData
{
    public int CubesCount;
    public float SpawnMaxDistance;
}

public struct EntitiesReferences : IComponentData
{
    public Entity BulletPrefabEntity;
}