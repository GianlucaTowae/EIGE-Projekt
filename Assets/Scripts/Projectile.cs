using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speedBase = 100f;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float damageBase = 1f;
    [SerializeField] private float damageMultiplier = 1f;
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
            other.GetComponent<Asteroid>().Damage(damageBase * damageMultiplier);
        }
        else if(other.CompareTag("Planet"))
        {
            if(!piercing) Destroy(gameObject);
            other.GetComponent<Planet>().Damage(damageBase * damageMultiplier);
        }
    }
}
