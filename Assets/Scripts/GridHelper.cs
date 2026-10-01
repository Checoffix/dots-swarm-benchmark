
using Unity.Mathematics;

public static class GridHelper
{
    public static int3 GetPosition(float3 position,  float cellSize)
    {
        return (int3)math.floor(position / cellSize);
    }
    public static int GetHash(int3 position)
    {
        return (int)math.hash(position);
    }
}