using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bossProjectilePrefab;
    // Change with "Drag" in Rigidbody for smooth stop
    [SerializeField] private int xp = 1000;
    [SerializeField] private float startHp = 100f;
    [SerializeField] private float speed = 150f;
    [SerializeField] private float maxDistance = 150f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float pauseTimeWhileBeam = 4f;
    [SerializeField] private float initialProjectileDistance = 2f;
    [SerializeField] private float shootInterval = 3f;
    [SerializeField] private Vector2 beamInterval = new(10f, 20f);
    [SerializeField] private float harderShootInterval = 1f;

    private Rigidbody _rigidbody;
    private Beam _beam;
    private Transform _center;
    private float _shootCooldown;
    private float _beamCooldown;
    private bool _still;
    private float _hp;
    private bool _phase2;
    private bool _phase3;

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

        if (Vector3.Distance(player.transform.position, transform.position) < maxDistance)
        {
            _shootCooldown -= Time.deltaTime;
            _beamCooldown -= Time.deltaTime;
        }

        if (_shootCooldown <= 0f)
        {
            Shoot();
            _shootCooldown = _phase3 ? harderShootInterval : shootInterval;
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
        Vector3 cachedPosition = transform.position;
        float angle = Vector3.SignedAngle(_beam.transform.position - cachedPosition, player.transform.position - cachedPosition, Vector3.forward);
        _beam.transform.RotateAround(cachedPosition, Vector3.forward, angle);
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
        {
            if (_phase2)
                other.GetComponent<Asteroid>().CircleBoss(_center);
        }
        if (other.CompareTag("Planet"))
            other.GetComponent<Planet>().Explode();
    }

    public void Damage(float amount)
    {
        _hp -= amount;

        if (_hp < startHp / 3 * 2)
            _phase2 = true;
        if (_hp < startHp / 3)
            _phase3 = true;

        Sounds.Play(Sounds.Sound.HIT_METAL);
        if (_hp < 0f)
        {
            PlayerBehaviour pb = player.GetComponent<PlayerBehaviour>();
            pb.IncreaseScoreEnd(xp);
            pb.SaveScore();
            SceneManager.LoadScene("WinScene");
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public float Hp => _hp;
    public float MaxHp => startHp;
}
