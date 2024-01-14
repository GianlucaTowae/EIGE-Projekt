using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speedBase = 100f;
    [SerializeField] private float speedMultiplier= 1f;
    [SerializeField] private float damageBase = 1f;
    [SerializeField] private float damageMultiplier= 1f;
    [SerializeField] private ParticleSystem explosion;

    void Update()
    {
        transform.Translate(Vector3.up * (speedBase * speedMultiplier * Time.deltaTime));
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Asteroid"))
        {
            Destroy(gameObject);
            Instantiate(explosion, other.transform.position, Quaternion.identity);
            // TODO: increase score/xp
        }
    }
}
