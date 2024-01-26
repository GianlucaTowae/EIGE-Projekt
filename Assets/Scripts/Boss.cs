using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


/*
 * TODO:
 * - Asteroid Shield
 * - Collision with Planet
 * - Testing
 */
public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bossProjectilePrefab;
    // Change with "Drag" in Rigidbody for smooth stop
    [SerializeField] private float startHp = 100f;
    [SerializeField] private float speed = 150f;
    [SerializeField] private float maxDistance = 150f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float shootInterval = 3f;
    [SerializeField] private float pauseTimeWhileBeam = 4f;
    [SerializeField] private float initialProjectileDistance = 2f;
    [SerializeField] private Vector2 beamInterval = new(10f, 20f);

    private Rigidbody _rigidbody;
    private Beam _beam;
    private Transform _center;
    private float _shootCooldown;
    private float _beamCooldown;
    private bool _still;
    private float _hp;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.angularDrag = 0f;
        _rigidbody.angularVelocity = Vector3.forward * rotationSpeed;
        _beam = GetComponentInChildren<Beam>();
        _center = transform.GetChild(0);
        _shootCooldown = shootInterval;
        _beamCooldown = Random.Range(beamInterval.x, beamInterval.y);
        _hp = startHp;
        Sounds.Play(Sounds.Sound.BOSS_SPAWN);
    }

    private void Update()
    {
        if (_still)
            return;

        _shootCooldown -= Time.deltaTime;
        _beamCooldown -= Time.deltaTime;

        if (_shootCooldown <= 0f)
        {
            Shoot();
            _shootCooldown = shootInterval;
        }

        if (_beamCooldown <= 0f)
        {
            StartCoroutine(Beam());
            _beamCooldown = Random.Range(beamInterval.x, beamInterval.y);
        }
    }

    private void Shoot()
    {
        Vector3 cachedPosition = transform.position;
        Quaternion rotation = Quaternion.LookRotation(player.transform.position - cachedPosition)
                              * Quaternion.Euler(90f, 0f, 0f);
        GameObject projectile = Instantiate(bossProjectilePrefab, cachedPosition, rotation);
        projectile.transform.Translate(Vector3.up * initialProjectileDistance);
    }

    private IEnumerator Beam()
    {
        _still = true;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.LookAt(player.transform, Vector3.forward);
        _beam.Play();
        yield return new WaitForSeconds(pauseTimeWhileBeam);
        _rigidbody.angularVelocity = Vector3.forward * rotationSpeed;
        _still = false;
    }

    private void FixedUpdate()
    {
        if (_still)
            return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > maxDistance)
            _rigidbody.velocity = (player.transform.position - transform.position).normalized * speed;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + Vector3.up * initialProjectileDistance, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid"))
            other.GetComponent<Asteroid>().CircleBoss(_center);
        if (other.CompareTag("Planet"))
            other.GetComponent<Planet>().Explode();
    }

    public void Damage(float amount)
    {
        _hp -= amount;
        Sounds.Play(Sounds.Sound.HIT_METAL);
        if (_hp < 0f)
            SceneManager.LoadScene("WinScene");
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public float Hp => _hp;
    public float MaxHp => startHp;
}
