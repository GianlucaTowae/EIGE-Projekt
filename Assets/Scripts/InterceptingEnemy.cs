using System;
using UnityEngine;

public class InterceptingEnemy : MonoBehaviour
{
    [SerializeField] private float startHealth = 1f;
    [SerializeField] private int xp = 1;
    [SerializeField] private float speedBase = 25f;
    [SerializeField] private float speedMultiplier = 1f;

    [SerializeField] private ParticleSystem explosion;

    private Rigidbody _playerRigidbody;
    private Rigidbody _rigidbody;
    private float _hp;

    void Awake()
    {
        _playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        _rigidbody = GetComponent<Rigidbody>();
        _hp = startHealth;
    }

    void FixedUpdate()
    {
        _rigidbody.velocity = transform.TransformDirection(Vector3.left * (speedBase * speedMultiplier));

        Vector3 playerVelocity = _playerRigidbody.velocity;
        Vector3 playerPosition = _playerRigidbody.position;

        Vector3 relativeVelocity = _rigidbody.velocity - playerVelocity;
        float distance = Vector3.Distance(playerPosition, _rigidbody.position);
        float expectedTravelTime = distance / relativeVelocity.magnitude;
        Vector3 predictedMeetingPoint = playerPosition + expectedTravelTime * playerVelocity;

        Debug.DrawLine(transform.position, predictedMeetingPoint);

        Vector3 direction = predictedMeetingPoint - transform.position;
        Quaternion rotation = Quaternion.FromToRotation(-transform.right, direction);
        transform.rotation *= rotation;
    }

    public void Damage(float amt)
    {
        _hp -= amt;
        if (_hp <= 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().IncreaseScore(xp);
            Explode();
        }
    }

    public void Explode()
    {
        Sounds.Play(Sounds.Sound.HIT_METAL);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
