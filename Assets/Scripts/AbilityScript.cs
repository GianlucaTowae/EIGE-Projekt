using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    //[Header("general")]
    [SerializeField] private float timeTillDespawnSec = 60f;
    [SerializeField] private float abilityMaxSpawnrateSec = 10f;
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
    [SerializeField] private Color defaultProjectileColor = Color.green;
    [SerializeField] private Color doubleShotProjectileColor = Color.red;
    [SerializeField] private Color searchingProjectileColor = Color.yellow;
    [SerializeField] private Color piercingProjectileColor = Color.blue;
    [SerializeField] private float FOVinDegForSP = 90f;
    [SerializeField] private float rangeForSP = 200f;
    [SerializeField] private float SPDurInSec = 15f;

    
    
    //---
    private Camera _mainCamera;
    [SerializeField] private Projectile projectile;
    [SerializeField] private GameObject OverchargeObj;
    [SerializeField] private AbilityUI abilityUIscript;
    [SerializeField] private GameObject abilityCapsule;
    [Serializable]
    public class AbilityGO {
        public AbilityUI.AbilityName name;
        public Material prefab;
        public int probability;
    }
    [SerializeField] private AbilityGO[] abilityPrefabs;
    private Dictionary<AbilityUI.AbilityName, Material> prefabMap = new Dictionary<AbilityUI.AbilityName, Material>();
    private Dictionary<int[], AbilityUI.AbilityName> probMap = new Dictionary<int[], AbilityUI.AbilityName>();
    private PlayerBehaviour player;
    private OverchargeDealDamge oObjScript;
    private int sum = 0;
    private EnemySpawner enemySpawner;
    private int shieldCount = 0,piercingShotsCount = 0,searchingProjectilesCount = 0,doubleShotCount = 0,XPMultiplierCount = 0,sabotageCount = 0, overchargeCount = 0;
    private GameObject currentSHUI, currentPSUI, currentSPUI, currentDSUI, currentXPUI, currentSBUI, currentOUI;
    private Coroutine currentSHCoRo, currentPSCoRo, currentSPCoRo, currentDSCoRo, currentXPCoRo, currentSBCoRo, currentOCoRo;

    void Start(){
        UpdateProjectileColor();
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
        Transform ls = OverchargeObj.transform.GetChild(0);
        ls.localScale = new Vector3(overchargeScale,5 , overchargeScale);
        onDeath();
        enemySpawner = GetComponent<EnemySpawner>();
        player = GetComponent<PlayerBehaviour>();
        oObjScript = OverchargeObj.GetComponentInChildren<OverchargeDealDamge>();
        oObjScript.damagePerTime = overchargeDamagePerTime;
        oObjScript.timeBetweenDamageInSec = overchargeTimeBetweenDamageSec;

        player.blinkingDelay = this.respawnBlinkingDelaySec;
        player.shieldActive = false;
        player.doubleShotDelay = this.doubleShotDelaySec;
        player.respawnInvincibleDur = this.respawnInvincibleDurationSec;
        _mainCamera = Camera.main;

        projectile.FOVinDeg = FOVinDegForSP;
        projectile.range = rangeForSP;
        projectile.homInActive = false;

        StartCoroutine(Spawner());
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateProjectileColor()
    {
        if (projectile.material == null)
            projectile.material = projectile.GetComponent<Renderer>().sharedMaterial;

        if (searchingProjectilesCount > 0)
            projectile.material.color = searchingProjectileColor;
        else if (piercingShotsCount > 0)
            projectile.material.color = piercingProjectileColor;
        else if (doubleShotCount > 0)
            projectile.material.color = doubleShotProjectileColor;
        else
            projectile.material.color = defaultProjectileColor;
    }

    private IEnumerator Spawner(){
        // for (int i = 1; i <= abilityMaxSpawnrateSec; i++)
        // {
        //     Debug.Log(i + " " +((MathF.Pow(i,2)/MathF.Pow(abilityMaxSpawnrateSec+.5f,2))+1-(MathF.Pow(abilityMaxSpawnrateSec,2)/MathF.Pow(abilityMaxSpawnrateSec+.5f,2))));
        // }
        int current = 0;
        while (true){
            if (UnityEngine.Random.Range(0f, 1f) <= (MathF.Pow(current,2)/MathF.Pow(abilityMaxSpawnrateSec+.5f,2))+1-(MathF.Pow(abilityMaxSpawnrateSec,2)/MathF.Pow(abilityMaxSpawnrateSec+.5f,2))){// (x^2)/(max+1^2) + 1 - ((max^2)/(max+1^2))
                // Debug.Log("spawned at "+current);
                SpawnAbilityPickUp();
                current = 0;
            }
            current++;
            yield return new WaitForSeconds(1);
        }
    }

    void Update()
    {
        OverchargeObj.transform.position = player.gameObject.transform.position;
    }
    private void SpawnAbilityPickUp(){
        float distanceToScreen = 30f;
        Vector2 corner = new Vector2(_mainCamera.pixelWidth, _mainCamera.pixelHeight);
        
        Vector3 pos;
        float edge = UnityEngine.Random.Range(0, corner.x*2+corner.y*2);
        if(edge < corner.x){
            pos = _mainCamera.ScreenToWorldPoint(new Vector2(edge, 0));
            pos.y-=distanceToScreen;
        }
        else if(edge < corner.x+corner.y){
            pos = _mainCamera.ScreenToWorldPoint(new Vector2(corner.x, edge-corner.x));
            pos.x += distanceToScreen;
        }
        else if(edge < corner.x*2+corner.y){
            pos = _mainCamera.ScreenToWorldPoint(new Vector2(edge-(corner.x+corner.y), corner.y));
            pos.y+=distanceToScreen;
        }
        else{
            pos = _mainCamera.ScreenToWorldPoint(new Vector2(0, edge-(2*corner.x+corner.y)));
            pos.x -= distanceToScreen;
        }

        pos.z = 0;

        int abilityNum = UnityEngine.Random.Range(0, sum);
        Material randomAb;
        foreach (var item in probMap){
            if (item.Key[0] <= abilityNum && item.Key[1] > abilityNum){
                randomAb = prefabMap[item.Value];
                GameObject newBox = Instantiate(abilityCapsule, pos, Quaternion.identity);
                newBox.transform.Rotate(Vector3.up, -90);
                newBox.GetComponent<Despawner>().timeTillDeath = timeTillDespawnSec;
                foreach (Transform child in newBox.transform){   
                    if (child.CompareTag("Display")){
                        child.GetComponent<Renderer>().material = randomAb;
                        break;
                    }
                }
                newBox.name = item.Value.ToString();
                break;
            }
        }

    }
    public void pickedUpAbility(GameObject collided){
        GameObject other = collided.transform.parent.gameObject;
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
    }


    public void repairKit(){
        Sounds.Play(Sounds.Sound.ABILITY_RK);
        abilityUIscript.Add(AbilityUI.AbilityName.RepairKit, displayRKitPickedUpSec);//einf auch kurz zeigen oder hier das ui abänderen??
        player.Heal();
    }   
    public void guradianAngle(){
        if(player.res) return;
        Sounds.Play(Sounds.Sound.ABILITY_GA);
        player.res = true;
        player.guardianAngleUI = abilityUIscript.Add(AbilityUI.AbilityName.GuardianAngle).Item1;
    }   
    
    public void shield(){
        Sounds.Play(Sounds.Sound.ABILITY_SH);
        shieldCount++;
        if (shieldCount > 1){//ggf.: reset cooldown UI
            abilityUIscript.stopCooldown(currentSHUI, currentSHCoRo);
        }
        (currentSHUI, currentSHCoRo) = abilityUIscript.Add(AbilityUI.AbilityName.Shield, shieldInvincibleDurationSec);
        StartCoroutine(InvincibilityOnShield());

    } 
    public void piercingShots(){
        piercingShotsCount++;
        if(piercingShotsCount > 1){//ggf.: reset cooldown UI
            abilityUIscript.stopCooldown(currentPSUI, currentPSCoRo);
        }
        (currentPSUI, currentPSCoRo) = abilityUIscript.Add(AbilityUI.AbilityName.PiercingShots, piercingDurationSec);
        StartCoroutine(PiercingDuration());
        
    } 
    public void searchingProjectiles(){
        searchingProjectilesCount++;
        if(searchingProjectilesCount > 1){//ggf.: reset cooldown UI
            abilityUIscript.stopCooldown(currentSPUI, currentSPCoRo);
        }
        (currentSPUI, currentSPCoRo) = abilityUIscript.Add(AbilityUI.AbilityName.SearchingProjectiles, SPDurInSec);
        StartCoroutine(searchingProjectileDuration());
        
    } 
    public void doubleShot(){
        doubleShotCount++;
        if(doubleShotCount > 1){//ggf.: reset cooldown UI
            abilityUIscript.stopCooldown(currentDSUI, currentDSCoRo);
        }
        (currentDSUI, currentDSCoRo) = abilityUIscript.Add(AbilityUI.AbilityName.DoubleShot, doubleShotDurationSec);
        StartCoroutine(DoubleShotDuration());
        
    } 
    public void XPMultiplier(){
        Sounds.Play(Sounds.Sound.ABILITY_XP);
        XPMultiplierCount++;
        if(XPMultiplierCount > 1){//ggf.: reset cooldown UI
            abilityUIscript.stopCooldown(currentXPUI, currentXPCoRo);
        }
        (currentXPUI, currentXPCoRo) = abilityUIscript.Add(AbilityUI.AbilityName.XPMultiplier, XPMultiplierDurationSec);
        StartCoroutine(XPMultiplierDuration());
        
    } 
    public void sabotage(){
        Sounds.Play(Sounds.Sound.ABILITY_SB);
        sabotageCount++;
        if(sabotageCount > 1){//ggf.: reset cooldown UI
            abilityUIscript.stopCooldown(currentSBUI, currentSBCoRo);
        }
        (currentSBUI, currentSBCoRo) = abilityUIscript.Add(AbilityUI.AbilityName.Sabotage, sabotageDurationSec);
        StartCoroutine(SabotageDuration());
        
    } 
    public void overcharge(){
        Sounds.Play(Sounds.Sound.ABILITY_O);
        oObjScript.Activate();
        overchargeCount++;
        if(overchargeCount > 1){//ggf.: reset cooldown UI
            abilityUIscript.stopCooldown(currentOUI, currentOCoRo);
        }
        (currentOUI, currentOCoRo) = abilityUIscript.Add(AbilityUI.AbilityName.Overcharge, overchargeDurationSec);
        StartCoroutine(OverchargeDuration());
        
    }

    //IENUMS--------------

    private IEnumerator InvincibilityOnShield(){
        player.shieldActive = true;
        player.invincible = true;
        yield return new WaitForSeconds(shieldInvincibleDurationSec);
        if(shieldCount-- > 1) 
            yield break;
        player.invincible = false;
        player.shieldActive = false;
    }
    private IEnumerator PiercingDuration()
    {
        projectile.piercing = true;
        UpdateProjectileColor();
        yield return new WaitForSeconds(piercingDurationSec);
        if(piercingShotsCount-- > 1)
            yield break;
        projectile.piercing = false;
        UpdateProjectileColor();
    }
    private IEnumerator DoubleShotDuration()
    {
        player.doubleShot = true;
        UpdateProjectileColor();
        yield return new WaitForSeconds(doubleShotDurationSec);
        if(doubleShotCount-- > 1) 
            yield break;
        player.doubleShot = false;
        UpdateProjectileColor();
    }
    private IEnumerator XPMultiplierDuration()
    {
        player.XPMultiplier = this.XPMultiplierVal;
        yield return new WaitForSeconds(XPMultiplierDurationSec);
        if(XPMultiplierCount-- > 1)
            yield break;
        player.XPMultiplier = 1;
    }
    private IEnumerator OverchargeDuration()
    {
        yield return new WaitForSeconds(overchargeDurationSec);
        if(overchargeCount-- > 1) 
            yield break;
        oObjScript.Deactivate();
    }
    private IEnumerator searchingProjectileDuration()
    {
        UpdateProjectileColor();
        projectile.homInActive = true;
        yield return new WaitForSeconds(SPDurInSec);
        if (searchingProjectilesCount-- > 1) 
            yield break;
        projectile.homInActive = false;
        UpdateProjectileColor();

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
        
        if(sabotageCount > 1) {
            enemySpawner.spawningIntervalSingle *= 1/sabotageRelativeSpawnrate0_to_1;
            enemySpawner.spawningIntervalCluster *= 1/sabotageRelativeSpawnrate0_to_1;
            enemySpawner.spawningIntervalTargetingCluster *= 1/sabotageRelativeSpawnrate0_to_1;
            enemySpawner.spawningIntervalPlanet *= 1/sabotageRelativeSpawnrate0_to_1;
        }
        yield return new WaitForSeconds(sabotageDurationSec);
        if(sabotageCount-- > 1) 
            yield break;
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
