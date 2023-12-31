using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Text rotationLabel;

    [Serializable]
    private class Controls
    {
        public string xAxis = "Mouse X";
        public string yAxis = "Mouse Y";
        public KeyCode lockedKey = KeyCode.L;

        public float rotateVelocity = 1f;
    }
    [SerializeField] private Controls controls;

    [Serializable]
    private class MovementSettings
    {
        public float speedBase = 10f;
        public float speedFactor = 1f;
        public float rotationDampeningFactor = 0.8f;
    }
    [SerializeField] private MovementSettings movementSettings;

    private class Inputs
    {
        public float x;
        public float y;
        public bool locked;
    }
    private Inputs _inputs;

    private Rigidbody _rigidbody;
    private Vector3 _angularVelocity;
    private Vector3 _localEulerAngles;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputs = new Inputs();
        _rigidbody = GetComponent<Rigidbody>();
        _angularVelocity = _rigidbody.angularVelocity;
        _localEulerAngles = transform.localEulerAngles;
    }

    private void Update()
    {
        GetInput();

        if (_inputs.locked)
            SwitchLockState();
    }

    private void SwitchLockState()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        _inputs.x = 0f;
        _inputs.y = 0f;
    }

    private void GetInput()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            _inputs.x = Input.GetAxis(controls.xAxis);
            _inputs.y = Input.GetAxis(controls.yAxis);
        }
        _inputs.locked = Input.GetKeyDown(controls.lockedKey);
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Transform cachedTransform = transform;

        // Movement
        _rigidbody.velocity = cachedTransform.TransformDirection(Vector3.forward * (movementSettings.speedBase * movementSettings.speedFactor));

        // Rotation
        float amtToRotateHorizontal = _inputs.x * controls.rotateVelocity;
        float amtToRotateVertical = _inputs.y * controls.rotateVelocity;

        _angularVelocity *= movementSettings.rotationDampeningFactor;
        _angularVelocity.x -= amtToRotateVertical;
        _angularVelocity.y += amtToRotateHorizontal;
        _rigidbody.angularVelocity = _angularVelocity;

        // Z-Axis Correction
        _localEulerAngles = cachedTransform.localEulerAngles;
        _localEulerAngles.z = 0f;
        cachedTransform.localEulerAngles = _localEulerAngles;

        // Debug Display
        rotationLabel.text = _localEulerAngles.y.ToString(CultureInfo.CurrentCulture) + "; " + _localEulerAngles.x.ToString(CultureInfo.CurrentCulture);
    }
}
