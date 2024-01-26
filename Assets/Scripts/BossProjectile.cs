using System;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 300f;

    private void Start()
    {
        Sounds.Play(Sounds.Sound.BOSS_PROJECTILE);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * (speed * Time.deltaTime));
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
