using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    [SerializeField] private GameObject abilityCanvas;
    private Camera _mainCamera;
    private StatisticsDisplay statistics;
    [SerializeField] private GameObject AbilityGameObjectPrefab;
    public bool currentlySelecting;//dont allow colliding w/ abilities while selecting

    void Start()
    {
        currentlySelecting = false;
        abilityCanvas.SetActive(false);
        _mainCamera = Camera.main;

        GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
        statistics = scripts.GetComponent<StatisticsDisplay>();
    }

    // Update is called once per frame
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
        abilityCanvas.SetActive(true);
        currentlySelecting = true;
        StartCoroutine(waitForKeyPress());
    }
    private void multiShot(){
        PlayerBehaviour.multiShot++;
        abilityCanvas.SetActive(false);
        currentlySelecting = false;
    }
    private void Shield(int numHitsAbsorbed){
        PlayerBehaviour.shieldHealth += numHitsAbsorbed;
        statistics.SetStatistic(StatisticsDisplay.Statistics.SHIELD, PlayerBehaviour.shieldHealth);
        abilityCanvas.SetActive(false);
        currentlySelecting = false;
    }

    private IEnumerator waitForKeyPress(){
        while(true){
            if (Input.GetKeyDown("f")){
                multiShot();
                break;
            }
            else if(Input.GetKeyDown("g")){
                Shield(3);
                break;
            }
            else yield return null;
        }
    }
}
