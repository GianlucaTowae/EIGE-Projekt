using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBehaviour : MonoBehaviour
{
    #region Serialized
    [Serializable] private class Controls
    {
        public string horizontalAxis = "Horizontal";
        public KeyCode lockedKey = KeyCode.L;
        public KeyCode shootKey = KeyCode.Space;
    }
    [SerializeField] private Controls controls;

    [Serializable] private class MovementSettings
    {
        public float speedBase = 10f;
        public float speedMultiplier = 1f;
        public float rotationFactor = 1f;
    }
    [SerializeField] private MovementSettings movementSettings;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector3 shootPointOffset;
    #endregion

    private float _inputHorizontal;
    private bool _inputLocked, _inputShoot;

    private float _halfProjectileHeight;

    private Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _halfProjectileHeight = projectilePrefab.GetComponent<Renderer>().bounds.size.y / 2;
    }

    void Update()
    {
        // Inputs
        _inputHorizontal = Input.GetAxis(controls.horizontalAxis);
        _inputLocked = Input.GetKeyDown(controls.lockedKey);
        _inputShoot = Input.GetKeyDown(controls.shootKey);

        // Switch Lock State
        if (_inputLocked)
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;

        // Shoot Projectile
        if (_inputShoot)
            Shoot();
    }

    void FixedUpdate()
    {
        // Movement
        _rigidbody.velocity = transform.TransformDirection(Vector3.up * (movementSettings.speedBase * movementSettings.speedMultiplier));

        // Rotation
        _rigidbody.angularVelocity = Vector3.back * (_inputHorizontal * movementSettings.rotationFactor);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + shootPointOffset, 0.2f);
    }

    private void Shoot()
    {
        // TODO: Sound
        Transform cachedTransform = transform;
        Vector3 position = cachedTransform.position +
                           cachedTransform.TransformDirection(shootPointOffset + Vector3.up * _halfProjectileHeight);
        Instantiate(projectilePrefab, position, cachedTransform.rotation);
    }
}
