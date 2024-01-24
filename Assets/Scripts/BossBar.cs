using System;
using UnityEngine;

public class BossBar : MonoBehaviour
{
    private Boss _boss;
    private EnemySpawner _enemySpawner;
    private Mode _mode = Mode.TIMER;
    private Vector3 _scale;

    public enum Mode
    {
        TIMER,
        HEALTH
    }

    private void Awake()
    {
        _scale = transform.localScale;
    }

    private void Update()
    {
        _scale.x = _mode switch
        {
            Mode.TIMER => _enemySpawner.TimerCurrent / _enemySpawner.TimerMax,
            Mode.HEALTH => _boss.Hp / _boss.MaxHp,
            _ => _scale.x
        };
        transform.localScale = _scale;
    }

    public void HealthMode(Boss boss)
    {
        _mode = Mode.HEALTH;
        _boss = boss;
    }

    public void TimerMode(EnemySpawner enemySpawner)
    {
        _mode = Mode.TIMER;
        _enemySpawner = enemySpawner;
    }
}
