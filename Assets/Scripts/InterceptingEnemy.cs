using UnityEngine;

public class InterceptingEnemy : MonoBehaviour
{
    [SerializeField] private float speedBase = 25f;
    [SerializeField] private float speedMultiplier = 1f;

    private Rigidbody _playerRigidbody;
    private Rigidbody _rigidbody;

    void Awake()
    {
        _playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 playerVelocity = _playerRigidbody.velocity;
        Vector3 playerPosition = _playerRigidbody.position;

        Vector3 relativeVelocity = _rigidbody.velocity - playerVelocity;
        float distance = Vector3.Distance(playerPosition, _rigidbody.position);
        float expectedTravelTime = distance / relativeVelocity.magnitude;
        Vector3 predictedMeetingPoint = playerPosition + expectedTravelTime * playerVelocity;
        Vector3 direction = (predictedMeetingPoint - _rigidbody.position).normalized;

        transform.LookAt(predictedMeetingPoint, Vector3.back);
        transform.Rotate(Vector3.right * 90f);
        _rigidbody.velocity = transform.TransformDirection(Vector3.up * (speedBase * speedMultiplier));
    }
}
