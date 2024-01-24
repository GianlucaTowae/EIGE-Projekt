using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    //[Header("general")]
    [SerializeField] private Vector2 abilitySpawnrateBoundSec = new Vector2(10,15);
    //[Header("RK")]
    [SerializeField] private float displayRKitPickedUpSec = 3f;
    //[Header("GA")]
    [SerializeField] private float respawnInvincibleDurationSec = 3f;
    [SerializeField] private float respawnBlinkingDelaySec = 0.1f;
    [SerializeField] private float shieldInvincibleDurationSec = 10f;
    [SerializeField] private float piercingDurationSec = 5f;
    //[Header("DS")]
    [SerializeField] private float doubleShotDurationSec = 5f;
    [SerializeField] private float doubleShotDelaySec = 0.2f;
    //[Header("XP")]
    [SerializeField] private float XPMultiplierDurationSec = 10f;
    [SerializeField] private int XPMultiplierVal = 10;
    //[Header("Overc")]
    [SerializeField] private float overchargeScale = 140f;
    [SerializeField] private float overchargeDurationSec = 5f;
    [SerializeField] private float overchargeDamagePerTime = 20f;
    [SerializeField] private float overchargeTimeBetweenDamageSec = 1f;
    //[Header("Sab")]
    [SerializeField] private float sabotageDurationSec = 5f;
    [SerializeField] private float sabotageRelativeSpawnrate0_to_1 = .35f;
    //[Header("Rest")]
    [SerializeField] private Material DSMat;
    [SerializeField] private Material SPMat;
    [SerializeField] private Material PSMat;
    [SerializeField] private Material defProjMat;
    [SerializeField] private float FOVinDegForSP = 90f;
    [SerializeField] private float rangeForSP = 200f;
    [SerializeField] private float SPDurInSec = 15f;

    
    
    //---
    private Camera _mainCamera;
    [SerializeField] private Projectile projectile;
    [SerializeField] private GameObject OverchargeObj;
    [SerializeField] private AbilityUI abilityUIscript;
    [Serializable]
    public class AbilityGO {
        public AbilityUI.AbilityName name;
        public GameObject prefab;
        public int probability;
    }
    [SerializeField] private AbilityGO[] abilityPrefabs;
    private Dictionary<AbilityUI.AbilityName, GameObject> prefabMap = new Dictionary<AbilityUI.AbilityName, GameObject>();
    private Dictionary<int[], AbilityUI.AbilityName> probMap = new Dictionary<int[], AbilityUI.AbilityName>();
    private PlayerBehaviour player;
    private OverchargeDealDamge oObjScript;
    private int sum = 0;
    private EnemySpawner enemySpawner;
    private float currentCooldown;
    void Start(){
        var ls = OverchargeObj.GetComponentInChildren<Transform>().localScale;//sadjf
        ls.x = ls.z = overchargeScale;
        projectile.material = defProjMat;
        foreach (var pref in abilityPrefabs){
            if (!prefabMap.ContainsKey(pref.name)){
                prefabMap.Add(pref.name, pref.prefab);
            }

            if (!probMap.ContainsValue(pref.name)){
                probMap.Add(new int[]{sum, sum + pref.probability}, pref.name);
                sum+=pref.probability;
            }
        }
    }
    void Awake()
    {
        currentCooldown = UnityEngine.Random.Range(abilitySpawnrateBoundSec.x,abilitySpawnrateBoundSec.y);
        onDeath();
        enemySpawner = GetComponent<EnemySpawner>();
        player = GetComponent<PlayerBehaviour>();
        oObjScript = OverchargeObj.GetComponentInChildren<OverchargeDealDamge>();

        player.blinkingDelay = this.respawnBlinkingDelaySec;
        player.doubleShotDelay = this.doubleShotDelaySec;
        player.respawnInvincibleDur = this.respawnInvincibleDurationSec;
        _mainCamera = Camera.main;

        projectile.FOVinDeg = FOVinDegForSP;
        projectile.range = rangeForSP;
        projectile.homInActive = false;
    }


    void Update()
    {
        OverchargeObj.transform.position = player.gameObject.transform.position;
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0){
            SpawnAbilityPickUp();
            currentCooldown = UnityEngine.Random.Range(abilitySpawnrateBoundSec.x,abilitySpawnrateBoundSec.y);
        }
        if (Input.GetKeyDown(KeyCode.R))//TODO Remove
            SpawnAbilityPickUp();
    }
    private void SpawnAbilityPickUp(){
        float x_pos = UnityEngine.Random.Range(0, _mainCamera.pixelWidth);
        float y_pos = UnityEngine.Random.Range(0, _mainCamera.pixelHeight);
        Vector3 pos = _mainCamera.ScreenToWorldPoint(new Vector2(x_pos, y_pos));
        pos.z = 0;

        int abilityNum = UnityEngine.Random.Range(0, sum);
        GameObject randomAb;

        foreach (var item in probMap){
            if (item.Key[0] <= abilityNum && item.Key[1] > abilityNum){
                randomAb = prefabMap[item.Value];
                randomAb.name = item.Value.ToString();
                Instantiate(randomAb, pos, Quaternion.identity);
                break;
            }
        }

    }
    public void pickedUpAbility(GameObject other){
        if(other.name.ToLower().Contains("repairkit")) repairKit();
        else if(other.name.ToLower().Contains("guardianangle")) guradianAngle();
        else if(other.name.ToLower().Contains("shield")) shield();
        else if(other.name.ToLower().Contains("piercingshots")) piercingShots();
        else if(other.name.ToLower().Contains("searchingprojectiles")) searchingProjectiles();
        else if(other.name.ToLower().Contains("doubleshot")) doubleShot();
        else if(other.name.ToLower().Contains("xpmultiplier")) XPMultiplier();
        else if(other.name.ToLower().Contains("sabotage")) sabotage();
        else if(other.name.ToLower().Contains("overcharge")) overcharge();
        else UnityEngine.Debug.Log("unknown ability " + other.name);

    }
    public void onDeath(){
        StopAllCoroutines();
        if (oObjScript != null)
            oObjScript.Deactivate();
        abilityUIscript.clear();
        projectile.material = defProjMat;
    }



    public void repairKit(){
        abilityUIscript.Add(AbilityUI.AbilityName.RepairKit, displayRKitPickedUpSec);//einf auch kurz zeigen oder hier das ui abänderen??
        player.Heal();
    }   
    public void guradianAngle(){
        player.res = true;
        player.guardianAngleUI = abilityUIscript.Add(AbilityUI.AbilityName.GuardianAngle);
    }   
    public void shield(){//TODO: reset cooldown on multiple pickup
        abilityUIscript.Add(AbilityUI.AbilityName.Shield, shieldInvincibleDurationSec);
        StartCoroutine(InvincibilityOnShield());
    } 
    public void piercingShots(){
        abilityUIscript.Add(AbilityUI.AbilityName.PiercingShots, piercingDurationSec);
        StartCoroutine(PiercingDuration());
    } 
    public void searchingProjectiles(){
        abilityUIscript.Add(AbilityUI.AbilityName.SearchingProjectiles, SPDurInSec);
        StartCoroutine(searchingProjectileDuration());
    } 
    public void doubleShot(){
        abilityUIscript.Add(AbilityUI.AbilityName.DoubleShot, doubleShotDurationSec);
        StartCoroutine(DoubleShotDuration());
    } 
    public void XPMultiplier(){
        abilityUIscript.Add(AbilityUI.AbilityName.XPMultiplier, XPMultiplierDurationSec);
        StartCoroutine(XPMultiplierDuration());
    } 
    public void sabotage(){
        abilityUIscript.Add(AbilityUI.AbilityName.Sabotage, sabotageDurationSec);
        StartCoroutine(SabotageDuration());
    } 
    public void overcharge(){
        abilityUIscript.Add(AbilityUI.AbilityName.Overcharge, overchargeDurationSec);
        
        oObjScript.damagePerTime = overchargeDamagePerTime;
        oObjScript.timeBetweenDamageInSec = overchargeTimeBetweenDamageSec;
        oObjScript.Activate();
        StartCoroutine(OverchargeDuration());
    }

    //IENUMS--------------

    private IEnumerator InvincibilityOnShield(){
        player.invincible = true;
        yield return new WaitForSeconds(shieldInvincibleDurationSec);
        player.invincible = false;
    }
    private IEnumerator PiercingDuration()
    {
        projectile.piercing = true;
        projectile.material = PSMat;
        yield return new WaitForSeconds(piercingDurationSec);
        projectile.piercing = false;
        projectile.material = defProjMat;
    }
    private IEnumerator DoubleShotDuration()
    {
        player.doubleShot = true;
        projectile.material = DSMat;
        yield return new WaitForSeconds(doubleShotDurationSec);
        player.doubleShot = false;
        projectile.material = defProjMat;
    }
    private IEnumerator XPMultiplierDuration()
    {
        player.XPMultiplier = this.XPMultiplierVal;
        yield return new WaitForSeconds(XPMultiplierDurationSec);
        player.XPMultiplier = 1;
    }
    private IEnumerator OverchargeDuration()
    {
        yield return new WaitForSeconds(overchargeDurationSec);
        oObjScript.Deactivate();
    }
    private IEnumerator searchingProjectileDuration()
    {
        projectile.material = SPMat;
        projectile.homInActive = true;
        yield return new WaitForSeconds(SPDurInSec);
        projectile.homInActive = false;
        projectile.material = defProjMat;

    }
    private IEnumerator SabotageDuration()
    {
        Vector2[] savedValues = new Vector2[]{
            enemySpawner.spawningIntervalSingle,
            enemySpawner.spawningIntervalCluster,
            enemySpawner.spawningIntervalTargetingCluster,
            enemySpawner.spawningIntervalPlanet
        };
        float[] savedCooldowns = new float[]{
        enemySpawner._singleCooldown, 
        enemySpawner._clusterCooldown, 
        enemySpawner._targetingClusterCooldown,
        enemySpawner._interceptingCooldown, 
        enemySpawner._planetCooldown};
        
        enemySpawner.spawningIntervalSingle *= 1/sabotageRelativeSpawnrate0_to_1;
        enemySpawner.spawningIntervalCluster *= 1/sabotageRelativeSpawnrate0_to_1;
        enemySpawner.spawningIntervalTargetingCluster *= 1/sabotageRelativeSpawnrate0_to_1;
        enemySpawner.spawningIntervalPlanet *= 1/sabotageRelativeSpawnrate0_to_1;
        yield return new WaitForSeconds(sabotageDurationSec);
        enemySpawner.spawningIntervalSingle = savedValues[0];
        enemySpawner.spawningIntervalCluster = savedValues[1];
        enemySpawner.spawningIntervalTargetingCluster = savedValues[2];
        enemySpawner.spawningIntervalPlanet = savedValues[3];
        
        enemySpawner._singleCooldown = savedCooldowns[0];
        enemySpawner._clusterCooldown = savedCooldowns[1];
        enemySpawner._targetingClusterCooldown = savedCooldowns[2];
        enemySpawner._interceptingCooldown = savedCooldowns[3];
        enemySpawner._planetCooldown = savedCooldowns[4];
    }
}
