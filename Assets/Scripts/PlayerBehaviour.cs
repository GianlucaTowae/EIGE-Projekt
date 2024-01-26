using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    #region Serialized
    [Serializable] private class Controls
    {
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";
        public KeyCode shootKey = KeyCode.Space;
        public KeyCode shootKey2 = KeyCode.Mouse0;
    }
    [SerializeField] private Controls controls;

    [Serializable] private class MovementSettings
    {
        public float speedBase = 25f;
        public float speedMultiplier = 1f;
        public float rotationFactor = 3.5f;
        public float shoveFactor = 10f;
        public float immobileAfterHitTime = 1f;
    }
    [SerializeField] private MovementSettings movementSettings;

    [Serializable]
    private class Shooting
    {
        public GameObject cannon;
        public GameObject projectilePrefab;
        public ParticleSystem shootingBurst;
        public Vector3 shootPointOffset;
        public float shootCooldownSec = 0.2f;
        public float damageBase = 1f;
        public float damageMultiplier = 1f;
    }
    [SerializeField] private Shooting shooting;

    [SerializeField] private ParticleSystem exhaustLeft;
    [SerializeField] private ParticleSystem exhaustRight;

    [SerializeField] private RectTransform xpBarTransform;
    [SerializeField] private TMP_Text scoreLevelLabel;
    [SerializeField] private LevelUpPopup levelUpPopup;
    [SerializeField] private int xpNeededPerLevel = 40;
    [SerializeField] private int startHealth = 5;
    [SerializeField] private StatisticsDisplay statistics;

    [SerializeField] private int bossBeamDamage = 3;
    [SerializeField] private int interceptingEnemyDamage = 2;

    [SerializeField] private bool invincibleForTesting;
    #endregion

    private int _score;
    private int _level;
    private int _health;
    private int _maxHealth;
    private float currentShootCooldown;
    private float _halfProjectileHeight;

    private bool immobileAfterHit;

    private float _inputHorizontal, _inputVertical;
    private bool _inputShoot;
    private Vector3 _mousePos;

    private Rigidbody _rigidbody;
    private AbilityScript _abilityScript;

    //ablity:
    [HideInInspector] public bool doubleShot;
    [HideInInspector] public float doubleShotDelay;
    [HideInInspector] public bool res;
    [HideInInspector] public int XPMultiplier;
    [HideInInspector] public bool invincible;
    [HideInInspector] public float respawnInvincibleDur;
    [HideInInspector] public float blinkingDelay;
    [HideInInspector] public GameObject guardianAngleUI;
    private float respawnDurLeft;

    void Awake()
    {
        res = false;
        invincible = false;
        doubleShot = false;
        XPMultiplier = 1;
        respawnDurLeft = -1;
        guardianAngleUI = null;
        
        _halfProjectileHeight = shooting.projectilePrefab.GetComponent<Renderer>().bounds.size.y / 2;

        _health = _maxHealth = startHealth;
        shooting.shootingBurst = shooting.cannon.GetComponentInChildren<ParticleSystem>();

        _rigidbody = GetComponent<Rigidbody>();
        _abilityScript = GetComponent<AbilityScript>();

        IncreaseScore(0);
    }

    void Update()
    {
        // Inputs
        _inputHorizontal = Input.GetAxis(controls.horizontalAxis);
        _inputVertical = Input.GetAxis(controls.verticalAxis);
        _inputShoot = Input.GetKey(controls.shootKey) || Input.GetKey(controls.shootKey2);
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Cannon Rotation
        _mousePos.z = shooting.cannon.transform.position.z;
        shooting.cannon.transform.LookAt(_mousePos);

        // Shoot Projectile
        currentShootCooldown -= Time.deltaTime;
        if (_inputShoot)
            Shoot();

        // Abilities
        if(invincible) 
            respawnDurLeft -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        // Movement
        if (!immobileAfterHit)
        {
            _rigidbody.velocity = transform.TransformDirection(
                Vector3.up * (movementSettings.speedBase * movementSettings.speedMultiplier));
        }

        // Rotation
        float oldRotationZ = _rigidbody.rotation.eulerAngles.z;
        Vector2 vector = new Vector2(_inputHorizontal, _inputVertical);
        float angle = Vector2.SignedAngle(Vector2.up, vector);
        _rigidbody.rotation =
            Quaternion.RotateTowards(_rigidbody.rotation, Quaternion.Euler(Vector3.forward * angle),
            movementSettings.rotationFactor * vector.magnitude);
        // Old: _rigidbody.angularVelocity = Vector3.back * (_inputHorizontal * movementSettings.rotationFactor);

        // Exhausts
        float difference = Math.Abs(oldRotationZ - _rigidbody.rotation.eulerAngles.z);
        if (angle < 0 && difference > 1.5f && !invincible)
            exhaustLeft.Play();
        else if (angle > 0 && difference > 1.5f && !invincible)
            exhaustRight.Play();
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying || shooting.cannon == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(shooting.cannon.transform.position + shooting.shootPointOffset, 0.2f);
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.transform.tag)
        {
            case "Ability":
                Sounds.Play(Sounds.Sound.ABILITY_PICKUP);
                if(res && other.name.ToLower().Contains("guardianangle")) return;
                Destroy(other.transform.parent.gameObject);
                _abilityScript.pickedUpAbility(other.gameObject);
                break;
            case "Asteroid":
                DecreaseHealth();
                other.GetComponent<Asteroid>().Explode();
                break;
            case "BossProjectile":
                DecreaseHealth();
                Destroy(other.gameObject);
                break;
            case "InterceptingEnemy":
                DecreaseHealth(interceptingEnemyDamage);
                other.GetComponent<InterceptingEnemy>().Explode();
                break;
            case "Boss":
            case "Planet":
                DecreaseHealth();
                _rigidbody.velocity = (transform.position - other.transform.position) * movementSettings.shoveFactor;
                StartCoroutine(ImmobileAfterHit());
                break;
            case "BossBeam":
                DecreaseHealth(bossBeamDamage);
                break;
        }
    }

    private IEnumerator ImmobileAfterHit()
    {
        immobileAfterHit = true;
        yield return new WaitForSeconds(movementSettings.immobileAfterHitTime);
        immobileAfterHit = false;
    }

    private void Shoot()
    {
        if(currentShootCooldown > 0) return;
        currentShootCooldown = shooting.shootCooldownSec;
        Transform cachedTransform = shooting.cannon.transform;
        cachedTransform.Rotate(90f, 0f, 0f);
        Vector3 position = cachedTransform.position +
                           cachedTransform.TransformDirection(shooting.shootPointOffset + Vector3.up * _halfProjectileHeight);
        Instantiate(shooting.projectilePrefab, position, cachedTransform.rotation);
        shooting.shootingBurst.Play();
        if (doubleShot){
            StartCoroutine(waitAndShoot());
        }
    }

    private IEnumerator waitAndShoot()
    {
        yield return new WaitForSeconds(doubleShotDelay);

        Transform cachedTransform = shooting.cannon.transform;
        cachedTransform.Rotate(90f, 0f, 0f);
        Vector3 position = cachedTransform.position +
                           cachedTransform.TransformDirection(shooting.shootPointOffset + Vector3.up * _halfProjectileHeight);
        Instantiate(shooting.projectilePrefab, position, cachedTransform.rotation);
        shooting.shootingBurst.Play();
    }

    public void IncreaseSpeed(float percentage)
    {
        movementSettings.speedMultiplier += percentage;
        statistics.SetStatistic(StatisticsDisplay.Statistics.SPEED, (int) Math.Round(movementSettings.speedMultiplier * 100));
    }

    private void DecreaseHealth()
    {
        DecreaseHealth(1);
    }

    private void DecreaseHealth(int amount)
    {
        if (invincible || invincibleForTesting) return;
        _health -= amount;
        if (_health <= 0){
            if (!res) LoseGame();
            else{
                _health = _maxHealth;
                StartCoroutine(InvincibilityOnRes());
            }
        }
        Sounds.Play(Sounds.Sound.DAMAGE_TAKEN);
        statistics.SetStatistic(StatisticsDisplay.Statistics.HEALTH, _health);
    }

    public void IncreaseScore(int amount)
    {
        _score += amount * XPMultiplier;
        bool levelUp = _score / xpNeededPerLevel > 0;
        _score %= xpNeededPerLevel;
        xpBarTransform.localScale = new Vector3((float) _score / xpNeededPerLevel, 1f, 1f);
        if (levelUp)
        {
            Sounds.Play(Sounds.Sound.LEVEL_UP);
            _level++;
            scoreLevelLabel.text = _level.ToString();
            levelUpPopup.Show();
        }
    }

    private void LoseGame()
    {
        SceneManager.LoadScene("LoseScene");
    }
    
    private IEnumerator InvincibilityOnRes()
    {
        invincible = true;
        respawnDurLeft = respawnInvincibleDur;

        List<MeshRenderer> AllMesh = new List<MeshRenderer>();
        List<ParticleSystem> AllPS = new List<ParticleSystem>();
        for (int i = 0; i < transform.childCount; i++){
            var mr = transform.GetChild(i).GetComponent<MeshRenderer>();
            var ps = transform.GetChild(i).GetComponent<ParticleSystem>();
            if (mr != null) AllMesh.Add(mr);
            if (ps != null && ps != exhaustLeft && ps != exhaustRight) AllPS.Add(ps);
        }
        
        int count = 0;
        while(respawnDurLeft > 0){
            count++;
            if (AllMesh.Count > 0 && AllMesh.First().enabled){
                foreach(MeshRenderer mr in AllMesh) mr.enabled = false;
                foreach(ParticleSystem ps in AllPS) ps.Clear();
                foreach(ParticleSystem ps in AllPS) ps.Stop();
                if (guardianAngleUI.activeInHierarchy && count >= 2){
                    guardianAngleUI.SetActive(false);
                    count=0;
                }
            }
            else{
                foreach(MeshRenderer mr in AllMesh) mr.enabled = true;
                foreach(ParticleSystem ps in AllPS) ps.Play();
                if (!guardianAngleUI.activeInHierarchy && count >= 2){
                    guardianAngleUI.SetActive(true);
                    count=0;
                }
            }
            yield return new WaitForSeconds(blinkingDelay);
        }
        foreach(MeshRenderer mr in AllMesh) mr.enabled = true;
        foreach(ParticleSystem ps in AllPS) ps.Play();
        invincible = false;
        Destroy(guardianAngleUI);
        res = false;
    }

    public float DamageBase => shooting.damageBase;
    public float DamageMultiplier => shooting.damageMultiplier;

    public void IncreaseDamage(float amount)
    {
        shooting.damageMultiplier += amount;
        statistics.SetStatistic(StatisticsDisplay.Statistics.DAMAGE, (int) Math.Round(shooting.damageMultiplier * 10));
    }

    public void IncreaseMaxHealth(int amount)
    {
        _maxHealth += amount;
        statistics.SetStatistic(StatisticsDisplay.Statistics.MAX_HEALTH, _maxHealth);
        _health += amount;
        statistics.SetStatistic(StatisticsDisplay.Statistics.HEALTH, _health);
    }

    public void Heal(){
        _health = _maxHealth;
        statistics.SetStatistic(StatisticsDisplay.Statistics.HEALTH, _health);
    }
}
