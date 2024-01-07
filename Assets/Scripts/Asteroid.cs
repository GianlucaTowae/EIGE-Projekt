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

    void Start()
    {
        _speedBase = Random.Range(minSpeed, maxSpeed);
        _renderer = GetComponent<Renderer>();
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
        transform.Translate(Vector3.back * (_speedBase * speedMultiplier * Time.deltaTime));
    }
}
