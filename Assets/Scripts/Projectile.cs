using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int asteroidXP = 1;

    [SerializeField] private float speedBase = 100f;
    [SerializeField] private float speedMultiplier= 1f;
    [SerializeField] private float damageBase = 1f;
    [SerializeField] private float damageMultiplier= 1f;
    [SerializeField] private ParticleSystem explosion;
    [HideInInspector] public bool piercing;

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
            if(!piercing) Destroy(gameObject);
            Instantiate(explosion, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().IncreaseScore(asteroidXP);
        }
    }
}
