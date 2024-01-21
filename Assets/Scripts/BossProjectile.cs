using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 300f;

    private void Update()
    {
        transform.Translate(Vector3.up * (speed * Time.deltaTime));
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
