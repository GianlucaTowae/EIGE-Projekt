using System;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    #region Serialized
    [Serializable] private class Controls
    {
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";
        public KeyCode shootKey = KeyCode.Space;
    }
    [SerializeField] private Controls controls;

    [Serializable] private class MovementSettings
    {
        public float speedBase = 20f;
        public float speedMultiplier = 1f;
        public float rotationFactor = 3.5f;
    }
    [SerializeField] private MovementSettings movementSettings;

    [SerializeField] private GameObject cannon;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector3 shootPointOffset;
    #endregion

    private float _inputHorizontal, _inputVertical;
    private bool _inputShoot;
    private Vector3 _mousePos;

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
        _inputVertical = Input.GetAxis(controls.verticalAxis);
        _inputShoot = Input.GetKeyDown(controls.shootKey);
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Cannon Rotation
        _mousePos.z = cannon.transform.position.z;
        cannon.transform.LookAt(_mousePos);
        cannon.transform.Rotate(Vector3.right * 90f);

        // Shoot Projectile
        if (_inputShoot)
            Shoot();
    }

    void FixedUpdate()
    {
        // Movement
        _rigidbody.velocity = transform.TransformDirection(
            Vector3.up * (movementSettings.speedBase * movementSettings.speedMultiplier));

        // Rotation
        Vector2 vector = new Vector2(_inputHorizontal, _inputVertical);
        float angle = Vector2.SignedAngle(Vector2.up, vector);
        _rigidbody.rotation =
            Quaternion.RotateTowards(_rigidbody.rotation, Quaternion.Euler(Vector3.forward * angle),
            movementSettings.rotationFactor * vector.magnitude);
        // Old: _rigidbody.angularVelocity = Vector3.back * (_inputHorizontal * movementSettings.rotationFactor);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(cannon.transform.position + shootPointOffset, 0.2f);
    }

    private void Shoot()
    {
        // TODO: Sound
        Transform cachedTransform = cannon.transform;
        Vector3 position = cachedTransform.position +
                           cachedTransform.TransformDirection(shootPointOffset + Vector3.up * _halfProjectileHeight);
        Instantiate(projectilePrefab, position, cachedTransform.rotation);
    }


    public float SpeedMultiplier
    {
        get => movementSettings.speedMultiplier;
        set => movementSettings.speedMultiplier = value;
    }
}
