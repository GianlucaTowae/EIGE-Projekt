using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speedBase = 100f;
    [SerializeField] private float speedMultiplier = 1f;
    [HideInInspector] public bool piercing;
    private float _damageBase = 1f;
    private float _damageMultiplier = 1f;

    private void Awake()
    {
        PlayerBehaviour playerBehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        _damageBase = playerBehaviour.DamageBase;
        _damageMultiplier = playerBehaviour.DamageMultiplier;
    }

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
            other.GetComponent<Asteroid>().Damage(_damageBase * _damageMultiplier);
        }
        else if(other.CompareTag("Planet"))
        {
            if(!piercing) Destroy(gameObject);
            other.GetComponent<Planet>().Damage(_damageBase * _damageMultiplier);
        }
    }
}
