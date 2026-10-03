using UnityEngine;

namespace Study1.GO
{
    public class KillAura : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Destroy(collision.gameObject);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            Destroy(collision.gameObject);
        }
    }
}
