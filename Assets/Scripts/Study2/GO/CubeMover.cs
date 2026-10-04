using UnityEngine;

namespace Study2.GO
{
    public class CubeMover : MonoBehaviour
    {
        private float _speed;
        public void Initialize(float speed)
        {
            _speed = speed;
        }
        private void Update()
        {
            transform.position += new Vector3(0, 0, 1) * (_speed * Time.deltaTime);
        }
    }
}