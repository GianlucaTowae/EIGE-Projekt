using System;
using UnityEngine;

public class Despawner : MonoBehaviour
{
    [HideInInspector] public float timeTillDeath;
    private float _timeLeft;
    private Renderer _renderer;

    void Awake(){
        _timeLeft = 10f;
        _renderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        if (_renderer.isVisible)
            _timeLeft = timeTillDeath;
        else
            _timeLeft -= Time.deltaTime;

        if(_timeLeft < 0f)
            Destroy(gameObject);
    }
}
