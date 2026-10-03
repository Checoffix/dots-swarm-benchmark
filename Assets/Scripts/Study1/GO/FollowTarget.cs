using Unity.Mathematics;
using UnityEngine;

namespace Study1.GO
{
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private Rigidbody2D rb2D;
        private Transform _targetCube;
        private CubeSpawner _cubeSpawner;

        public void SetTarget(Transform target, CubeSpawner cubeSpawner)
        {
            _targetCube = target;
            _cubeSpawner = cubeSpawner;
        }
        private void Update()
        {
            Vector3 toTarget =  math.normalize(_targetCube.position - transform.position);
            rb2D.linearVelocity = toTarget * speed;
        }

        private void OnDestroy()
        {
            _cubeSpawner.Died();
        }
    }
}
