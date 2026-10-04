using System;
using Study2.GO.Jobs;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace Study2.GO
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private MovementType movementType;
        [SerializeField] private CubeMover cubePrefab;
        [SerializeField] private int cubesCount;
        [SerializeField] private float spawnMaxDistance;
        [Tooltip("Only for Common parent and Jobs mods")]
        [SerializeField] private float speed;
        [Tooltip("Only for Common parent mod")]
        [SerializeField] private Transform parent;
        private TransformAccessArray _transformsAccessArray;

        private void Awake()
        {
            _transformsAccessArray = new TransformAccessArray(cubesCount);
        }

        void Start()
        {
            switch (movementType)
            {
                case MovementType.Update:
                {
                    for (int i = 0; i < cubesCount; i++)
                    {
                        SpawnUpdateMethod();
                    }
                    enabled = false;
                    break;
                }
                case MovementType.Jobs:
                {
                    for (int i = 0; i < cubesCount; i++)
                    {
                        SpawnJobsMethod(i);
                    }
                    break;
                }
                case MovementType.CommonParent:
                {
                    for (int i = 0; i < cubesCount; i++)
                    {
                        SpawnParentMethod();
                    }
                    break;
                }
            }
        }

        private void SpawnUpdateMethod()
        {
            CubeMover cubeMover = Instantiate(cubePrefab,
                new Vector3(Random.Range(-spawnMaxDistance, spawnMaxDistance),
                    Random.Range(-spawnMaxDistance, spawnMaxDistance), 0), Quaternion.identity);
            cubeMover.Initialize(speed);
        }

        private void SpawnJobsMethod(int index)
        {
            CubeMover cubeMover = Instantiate(cubePrefab,
                new Vector3(Random.Range(-spawnMaxDistance, spawnMaxDistance),
                    Random.Range(-spawnMaxDistance, spawnMaxDistance), 0), Quaternion.identity);
            _transformsAccessArray.Add(cubeMover.gameObject.transform);
            cubeMover.enabled = false;
        }

        private void SpawnParentMethod()
        {
            CubeMover cubeMover = Instantiate(cubePrefab,
                new Vector3(Random.Range(-spawnMaxDistance, spawnMaxDistance),
                    Random.Range(-spawnMaxDistance, spawnMaxDistance), 0), Quaternion.identity, parent);
            cubeMover.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            switch (movementType)
            {
                case MovementType.Jobs:
                {
                    var job = new MovementJob()
                    {
                        DeltaTime = Time.deltaTime,
                        Speed = speed
                    };
                    job.ScheduleByRef(_transformsAccessArray).Complete();
                    break;
                }
                case MovementType.CommonParent:
                {
                    parent.position += new Vector3(0, 0, 1) * (speed * Time.deltaTime);
                    break;
                }
            }
        }

        [Serializable]
        private enum MovementType
        {
            Update,
            Jobs,
            CommonParent
        }

        private void OnDestroy()
        {
            if (_transformsAccessArray.isCreated)
            {
                _transformsAccessArray.Dispose();
            }
        }
    }
}
