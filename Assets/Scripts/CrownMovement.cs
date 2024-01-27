using System;
using UnityEngine;

public class CrownMovement : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float threshold = .5f;
    [SerializeField] private Vector3 secondPosition;
    private Vector3 _firstPosition;
    private bool _first;

    private void Start()
    {
        _firstPosition = transform.position;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position,
            _first ? _firstPosition + secondPosition : _firstPosition);
        if (distance < threshold)
            _first = !_first;

        transform.position = Vector3.Lerp(transform.position, _first ? _firstPosition + secondPosition : _firstPosition,
            speed * Time.deltaTime);
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + secondPosition, .4f);
    }
}
