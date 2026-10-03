using UnityEngine;

namespace Study1.GO
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private Transform cubesTarget;
        [SerializeField] private FollowTarget cubePrefab;
        [SerializeField] private int targetCubesCount;
        [SerializeField] private float spawnMaxDistance;
        private int _aliveCubes = 0;
        void Start()
        {
            for (int i = 0; i < targetCubesCount; i++)
            {
                Spawn();
            }
        }

        void Update()
        {
            for (int i = 0; i < targetCubesCount - _aliveCubes; i++)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            FollowTarget cube = Instantiate(cubePrefab, new Vector3(Random.Range(-spawnMaxDistance, spawnMaxDistance), Random.Range(-spawnMaxDistance, spawnMaxDistance), 0), Quaternion.identity);
            cube.SetTarget(cubesTarget, this);
            _aliveCubes++;
        }

        public void Died()
        {
            _aliveCubes--;
        }
    }
}
