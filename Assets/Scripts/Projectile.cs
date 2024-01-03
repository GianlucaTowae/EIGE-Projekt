using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speedBase = 0.5f;
    [SerializeField] private float speedMultiplier= 1f;
    [SerializeField] private float damageBase = 1f;
    [SerializeField] private float damageMultiplier= 1f;

    void Update()
    {
        transform.Translate(Vector3.up * (speedBase * speedMultiplier));
    }
}
