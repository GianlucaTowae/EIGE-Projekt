using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera _mainCamera;
    [SerializeField] private GameObject AbilityGameObjectPrefab;

    void Start()
    {
        _mainCamera = Camera.main;
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
    public void lalalla(){
        Debug.Log("YOOOOO");//tmp
    }
    public void doubleShot(){
        PlayerBehaviour.doubleShot = true;
    }
}
