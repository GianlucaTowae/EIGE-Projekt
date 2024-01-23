using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Planet : MonoBehaviour
{
    [SerializeField] private int xp = 20;
    [SerializeField] private float maxLifeTime = 30f;
    [SerializeField] private float hp = 30f;

    private float _currentLifeTime;

    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if(!_renderer.isVisible)
        {
            _currentLifeTime += Time.deltaTime;
        }
        else
        {
            _currentLifeTime = 0f;
        }
        if(_currentLifeTime > maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void Damage(float amt)
    {
        hp -= amt;
        if (hp <= 0)
            Explode();
    }

    public void Explode()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().IncreaseScore(xp);
        // TODO: Particle System
        Destroy(gameObject);
    }
}
