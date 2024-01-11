using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
        public float speedBase = 25f;
        public float speedMultiplier = 1f;
        public float rotationFactor = 3.5f;
    }
    [SerializeField] private MovementSettings movementSettings;

    [SerializeField] private GameObject cannon;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector3 shootPointOffset;
    [SerializeField] private Vector3 shootPointOffsett;
    [SerializeField] private float shootCooldownSec;
    [SerializeField] private int startHealth = 5;
    [SerializeField] private int doubleShotCooldownMilliSec = 75;
    private int _health;
    #endregion

    public static bool doubleShot;
    public static int shieldHealth;

    private float _inputHorizontal, _inputVertical;
    private bool _inputShoot;
    private Vector3 _mousePos;

    private float _halfProjectileHeight;

    private Rigidbody _rigidbody;
    private float currentShootCooldown;
    private AbilityScript _abilityScript;
    private StatisticsDisplay statistics;

    void Start(){
        GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
        _abilityScript = scripts.GetComponent<AbilityScript>();
        statistics = scripts.GetComponent<StatisticsDisplay>();
    }
    void Awake()
    {
        doubleShot = false;
        shieldHealth = 0;
        
        _rigidbody = GetComponent<Rigidbody>();
        _halfProjectileHeight = projectilePrefab.GetComponent<Renderer>().bounds.size.y / 2;
        _health = startHealth;
    }

    void Update()
    {
        currentShootCooldown -= Time.deltaTime;
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
    private bool secondShot;
    private void Shoot()
    {
        if (!secondShot&&currentShootCooldown > 0) return;
        currentShootCooldown = shootCooldownSec;
        // TODO: Sound
        Transform cachedTransform = cannon.transform;
        Vector3 position = cachedTransform.position +
                           cachedTransform.TransformDirection(shootPointOffset + Vector3.up * _halfProjectileHeight);
        Instantiate(projectilePrefab, position, cachedTransform.rotation);
        if (doubleShot && !secondShot){
            StartCoroutine(waitAndShoot(doubleShotCooldownMilliSec));
        }
    }
    private IEnumerator waitAndShoot(int milliSeconds)
    {
        yield return new WaitForSeconds((float) milliSeconds / 1000);
        secondShot = true;
        Shoot();
        secondShot = false;
        
    }
    

    public void IncreaseSpeed(float percentage)
    {
        movementSettings.speedMultiplier += percentage;
        statistics.SetStatistic(StatisticsDisplay.Statistics.SPEED, (int) movementSettings.speedMultiplier * 100);
    }

    public void SetHealth(int amount)
    {
        _health = amount;
        statistics.SetStatistic(StatisticsDisplay.Statistics.HEALTH, _health);
    }

    public void DecreaseHealth()
    {
        
        if (shieldHealth > 0){
            shieldHealth--;
            statistics.SetStatistic(StatisticsDisplay.Statistics.SHIELD, shieldHealth);
        }
        else{
            _health--;
            statistics.SetStatistic(StatisticsDisplay.Statistics.HEALTH, _health);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerProjectile")) return;
        if(other.CompareTag("Ability")){
            Destroy(other.gameObject);
            _abilityScript.Shield(5);
        }
        else
        {
            DecreaseHealth();
            if (_health <= 0)
                LoseGame();
        }
    }

    private void LoseGame()
    {
        SceneManager.LoadScene(2);
    }
}
