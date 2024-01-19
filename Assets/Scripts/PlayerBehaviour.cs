using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Callbacks;
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
        public KeyCode shootKey2 = KeyCode.Mouse0;
    }
    [SerializeField] private Controls controls;

    [Serializable] private class MovementSettings
    {
        public float speedBase = 25f;
        public float speedMultiplier = 1f;
        public float rotationFactor = 3.5f;
    }
    [SerializeField] private MovementSettings movementSettings;

    [Serializable]
    private class Shooting
    {
        public GameObject cannon;
        public GameObject projectilePrefab;
        public Vector3 shootPointOffset;
        public float shootCooldownSec = 0.2f;
        public ParticleSystem shootingBurst;
    }
    [SerializeField] private Shooting shooting;

    [SerializeField] private ParticleSystem exhaustLeft;
    [SerializeField] private ParticleSystem exhaustRight;

    [SerializeField] private RectTransform xpBarTransform;
    [SerializeField] private TMP_Text scoreLevelLabel;
    [SerializeField] private LevelUpPopup levelUpPopup;
    [SerializeField] private int xpNeededPerLevel = 20;
    [SerializeField] private int startHealth = 5;
    [SerializeField] private StatisticsDisplay statistics;
    [SerializeField] private AbilityScript _abilityScript;
    #endregion

    private int _score;
    private int _level;
    private int _health;
    private float currentShootCooldown;
    private float _halfProjectileHeight;

    private float _inputHorizontal, _inputVertical;
    private bool _inputShoot;
    private Vector3 _mousePos;

    private Rigidbody _rigidbody;

    //ablity:
    [HideInInspector] public bool doubleShot;
    [HideInInspector] public float doubleShotDelay;
    [HideInInspector] public bool res;
    [HideInInspector] public int XPMultiplier;
    [HideInInspector] public bool invincible;
    [HideInInspector] public float respawnInvincibleDur;
    [HideInInspector] public float blinkingDelay;
    private float respawnDurLeft;
    

    void Awake()
    {
        res = false;
        invincible = false;
        doubleShot = false;
        XPMultiplier = 1;
        respawnDurLeft = -1;
        
        _rigidbody = GetComponent<Rigidbody>();
        _halfProjectileHeight = shooting.projectilePrefab.GetComponent<Renderer>().bounds.size.y / 2;
        _health = startHealth;
        shooting.shootingBurst = shooting.cannon.GetComponentInChildren<ParticleSystem>();
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
        _rigidbody.velocity = transform.TransformDirection(
            Vector3.up * (movementSettings.speedBase * movementSettings.speedMultiplier));

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
        else if (angle > 0 && difference > 1.5f &&!invincible)
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
        if(other.CompareTag("PlayerProjectile")) return;
        if(other.CompareTag("Ability")){
            Destroy(other.gameObject);
            _abilityScript.pickedUpAbility();
        }
        else
        {
            DecreaseHealth();
            Destroy(other.gameObject);
        }
    }

    private void Shoot()
    {
        if(currentShootCooldown > 0) return;
        currentShootCooldown = shooting.shootCooldownSec;
        // TODO: Sound
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

    public void SetHealth(int amount)
    {
        _health = amount;
        statistics.SetStatistic(StatisticsDisplay.Statistics.HEALTH, _health);
    }

    public void DecreaseHealth()
    {
        if (invincible) return;
        /*OLD:
        if (shieldHealth > 0){
            shieldHealth--;
            statistics.SetStatistic(StatisticsDisplay.Statistics.SHIELD, shieldHealth);
        }
        else{*/
            _health--;
            if (_health <= 0){
                if (!res) LoseGame();
                else{
                    _health = 5; //TODO make it max health
                    res = false;
                    StartCoroutine(InvincibilityOnRes());
                }
            }
            statistics.SetStatistic(StatisticsDisplay.Statistics.HEALTH, _health);
        //}
    }

    public void IncreaseScore(int amount)
    {
        _score += amount * XPMultiplier;
        bool levelUp = _score / xpNeededPerLevel > 0;
        _score %= xpNeededPerLevel;
        xpBarTransform.localScale = new Vector3((float) _score / xpNeededPerLevel, 1f, 1f);
        if (levelUp)
        {
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
        
        while(respawnDurLeft > 0){
            if (AllMesh.Count > 0 && AllMesh.First().enabled){
                foreach(MeshRenderer mr in AllMesh) mr.enabled = false;
                foreach(ParticleSystem ps in AllPS) ps.Clear();
                foreach(ParticleSystem ps in AllPS) ps.Stop();
            }
            else{
                foreach(MeshRenderer mr in AllMesh) mr.enabled = true;
                foreach(ParticleSystem ps in AllPS) ps.Play();
            }
            yield return new WaitForSeconds(blinkingDelay);
        }
        if (respawnDurLeft < 0) {
            foreach(MeshRenderer mr in AllMesh) mr.enabled = true;
            foreach(ParticleSystem ps in AllPS) ps.Play();
            invincible = false;
            yield break;
        }
    }
}
