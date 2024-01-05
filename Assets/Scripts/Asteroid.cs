using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float minSpeed = 30f, maxSpeed = 50f;
    private float _speedBase;
    [SerializeField] private float _speedMultiplier = 1f;

    void Start()
    {
        _speedBase = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        transform.Translate(Vector3.back * (_speedBase * _speedMultiplier * Time.deltaTime));
    }
}
