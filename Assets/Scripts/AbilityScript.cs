using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    [SerializeField] private float respawnInvincibleDurationSec = 3f;
    [SerializeField] private float shieldInvincibleDurationSec = 10f;
    [SerializeField] private float piercingDurationSec = 5f;
    [SerializeField] private float doubleShotDurationSec = 5f;
    [SerializeField] private float doubleShotDelaySec = 0.2f;
    [SerializeField] private float XPMultiplierDurationSec = 10f;
    [SerializeField] private int XPMultiplierVal = 10;
    [SerializeField] private float respawnBlinkingDelaySec = 0.1f;

    
    
    //---
    private Camera _mainCamera;
    [SerializeField] private GameObject AbilityGameObjectPrefab;
    [SerializeField] private Projectile projectile;
    private PlayerBehaviour player;
    

    void Awake()
    {
        player = GetComponentInParent<PlayerBehaviour>();

        player.blinkingDelay = this.respawnBlinkingDelaySec;
        player.doubleShotDelay = this.doubleShotDelaySec;
        player.respawnInvincibleDur = this.respawnInvincibleDurationSec;
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SpawnAbilityPickUp();
    }
    private void SpawnAbilityPickUp(){
        float x_pos = UnityEngine.Random.Range(0, _mainCamera.pixelWidth);
        float y_pos = UnityEngine.Random.Range(0, _mainCamera.pixelHeight);
        Vector3 pos = _mainCamera.ScreenToWorldPoint(new Vector2(x_pos, y_pos));
        pos.z = 0;

        Instantiate(AbilityGameObjectPrefab, pos, Quaternion.identity);
    }
    public void pickedUpAbility(){
        //TODO if not already: pause all timers on levelpopup
        guradianAngle();
        //abilityCanvas.SetActive(true);
        //currentlySelecting = true;
    }
    public void repairKit(){
        //player.SetHealth(...)
    }   
    public void guradianAngle(){
        player.res = true;
    }   
    public void shield(){
        StartCoroutine(InvincibilityOnShield());
    } 
    public void piercingShots(){
        StartCoroutine(PiercingDuration());
    } 
    public void searchingProjectiles(){
        //TODO
    } 
    public void doubleShot(){
        StartCoroutine(DoubleShotDuration());
    } 
    public void XPMultiplier(){
        StartCoroutine(XPMultiplierDuration());
    } 
    public void sabotage(){
        
    } 
    public void overcharge(){

    }
    private IEnumerator InvincibilityOnShield(){
        player.invincible = true;
        yield return new WaitForSeconds(shieldInvincibleDurationSec);
        player.invincible = false;
    }
    private IEnumerator PiercingDuration()
    {
        projectile.piercing = true;
        yield return new WaitForSeconds(piercingDurationSec);
        projectile.piercing = false;
    }
    private IEnumerator DoubleShotDuration()
    {
        player.doubleShot = true;
        yield return new WaitForSeconds(piercingDurationSec);
        player.doubleShot = false;
    }
    private IEnumerator XPMultiplierDuration()
    {
        player.XPMultiplier = this.XPMultiplierVal;
        yield return new WaitForSeconds(XPMultiplierDurationSec);
        player.XPMultiplier = 1;
    }
}
