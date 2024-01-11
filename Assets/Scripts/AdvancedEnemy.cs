using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedEnemy : MonoBehaviour
{
    private Rigidbody playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    [SerializeField] private float speedBase = 25f;
    [SerializeField] private float speedMultiplier = 1f;
    private Rigidbody _rigidbody;

    void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 playerVelocity = playerRigidbody.velocity;
        Vector3 playerPosition = playerRigidbody.position;

        Vector3 relativeVelocity = _rigidbody.velocity - playerVelocity;
        float distance = Vector3.Distance(playerPosition, _rigidbody.position);
        float expectedTravelTime = distance / relativeVelocity.magnitude; 
        Vector3 predictedTravelPoint = playerPosition + expectedTravelTime * playerVelocity;
        Vector3 direction = (predictedTravelPoint - _rigidbody.position).normalized;

        _rigidbody.velocity = speedBase * speedMultiplier * direction;
    }
}
