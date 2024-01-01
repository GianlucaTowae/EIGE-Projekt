using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBehaviour : MonoBehaviour
{
    [Serializable]
    private class Controls
    {
        public string horizontalAxis = "Horizontal";
        public KeyCode lockedKey = KeyCode.L;
    }
    [SerializeField] private Controls controls;

    [Serializable]
    private class MovementSettings
    {
        public float speedBase = 10f;
        public float speedFactor = 1f;
        [FormerlySerializedAs("rotationAngle")] public float rotationFactor = 1f;
    }
    [SerializeField] private MovementSettings movementSettings;

    private float _inputHorizontal;
    private bool _inputLocked;

    private Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Inputs
        _inputHorizontal = Input.GetAxis(controls.horizontalAxis);
        _inputLocked = Input.GetKeyDown(controls.lockedKey);

        // Switch Lock State
        if (_inputLocked)
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void FixedUpdate()
    {
        // Movement
        _rigidbody.velocity = transform.TransformDirection(Vector3.forward * (movementSettings.speedBase * movementSettings.speedFactor));

        // Rotation
        _rigidbody.angularVelocity = Vector3.forward * (_inputHorizontal * movementSettings.rotationFactor);
    }
}
