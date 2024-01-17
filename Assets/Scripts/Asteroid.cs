using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float maxLifeTime = 10f;

    private float _currentLifeTime = 0f;

    private float _speedBase;
    private float _speedMultiplier;

    private Renderer _renderer;
    private Rigidbody _rigidbody;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(!_renderer.isVisible)
        {
            _currentLifeTime += Time.deltaTime;
        }
        if(_currentLifeTime > maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = transform.TransformDirection(Vector3.back * (_speedBase * _speedMultiplier));
    }

    public void SetSpeed(float speed, float speedMultiplier)
    {
        _speedBase = speed;
        _speedMultiplier = speedMultiplier;
    }
}
