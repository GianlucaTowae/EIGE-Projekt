using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float minSpeed = 30f, maxSpeed = 50f;
    private float _speedBase;
    [SerializeField] private float speedMultiplier = 1f;
    private float _currentLifetTime = 0f;
    [SerializeField] private float maxLifeTime = 10f;
    private Renderer _renderer;
    private Rigidbody _rigidbody;

    void Start()
    {
        _speedBase = Random.Range(minSpeed, maxSpeed);
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(!_renderer.isVisible)
        {
            _currentLifetTime += Time.deltaTime;
        }
        if(_currentLifetTime > maxLifeTime) 
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = transform.TransformDirection(Vector3.back * (_speedBase * speedMultiplier));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Asteroid"))
            Explode();
    }

    private void Explode()
    {
        Destroy(gameObject);
    }
}
