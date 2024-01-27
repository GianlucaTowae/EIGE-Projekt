using System;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float maxLifeTime = 3f;
    [SerializeField] private float hp = 1.5f;
    [SerializeField] private int xp = 1;

    [SerializeField] private ParticleSystem explosion;

    [SerializeField] private float circleSpeed = 1f;
    [SerializeField] private float circleDistance = 20f;

    private float _currentLifeTime;

    private float _speedBase;
    private float _speedMultiplier;

    private Renderer _renderer;
    private Rigidbody _rigidbody;
    private bool _circleBoss;
    private Transform _circleCenter;
    private static PlayerBehaviour pb = null;

    void Awake()
    {
        if (pb == null)
            pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.drag = 0f;
    }

    void Update()
    {
        if (_circleBoss)
        {
            transform.RotateAround(_circleCenter.position, Vector3.forward, circleSpeed * Time.deltaTime);
            return;
        }

        if(!_renderer.isVisible)
        {
            _currentLifeTime += Time.deltaTime;
        }
        else
        {
            _currentLifeTime = 0f;
        }
        if(_currentLifeTime > maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float speed, float speedMultiplier)
    {
        _speedBase = speed;
        _speedMultiplier = speedMultiplier;
        _rigidbody.velocity = transform.TransformDirection(Vector3.back * (_speedBase * _speedMultiplier));
    }

    public void Damage(float amt)
    {
        hp -= amt;
        if (hp <= 0)
        {
            Explode();
        }
        else
        {
            Sounds.Play(Sounds.Sound.HIT_STONE);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
            ExplodeNoXp();
    }

    public void Explode()
    {
        pb.IncreaseScore(xp);
        Sounds.Play(Sounds.Sound.ASTEROID_EXPLOSION);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void ExplodeNoXp()
    {
        Sounds.Play(Sounds.Sound.ASTEROID_EXPLOSION);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void CircleBoss(Transform circleCenter)
    {
        _circleCenter = circleCenter;
        _circleBoss = true;
        _rigidbody.isKinematic = true;
        Vector3 circleCenterPosition = _circleCenter.position;
        Vector3 transformPosition = transform.position;
        float distance = Vector3.Distance(circleCenterPosition, transformPosition);
        transform.position += (_circleCenter.position - transformPosition).normalized * (distance - circleDistance);
        transform.SetParent(circleCenter);
    }
}
