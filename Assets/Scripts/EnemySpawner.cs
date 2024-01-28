using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float maxAngleOffset = 30f;

    [SerializeField] private Vector2 asteroidSpeed = new(30f, 50f);
    [SerializeField] private float speedMultiplier = 1f;

    [SerializeField] private Vector2Int clusterSize = new(3, 7);
    [SerializeField] private Vector2Int targetingClusterSize = new(1, 3);

    [SerializeField] private Vector2 clusterDistribution = new(60f, 120f);
    [SerializeField] private float targetingClusterDistribution = 100f;

    [SerializeField] private float bossSpawningDistance = 200f;

    public Vector2 spawningIntervalSingle = new(0.5f, 2f);
    public Vector2 spawningIntervalCluster = new(4f, 5f);
    public Vector2 spawningIntervalTargetingCluster = new(3f, 6f);
    [SerializeField] private Vector2 spawningIntervalIntercepting = new(7f, 10f);
    public Vector2 spawningIntervalPlanet = new(20f, 30f);

    [SerializeField] private float firstPhaseIntervalMultiplier = 0.8f;
    [SerializeField] private float secondPhaseIntervalMultiplier = 0.5f;
    [SerializeField] private float secondPhasePercentOfBossTimer = 0.5f;

    [SerializeField] private float asteroidStartTime = 2f;
    [SerializeField] private float interceptingStartTime = 2f;
    [SerializeField] private float planetStartTime = 2f;
    [SerializeField] private float bossSpawnTime = 600f;

    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject interceptingEnemyPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private BossBar bossBar;

    [HideInInspector] public float _singleCooldown, _clusterCooldown, _targetingClusterCooldown, _interceptingCooldown, _planetCooldown;
    private float _bossTimer;
    private bool _bossSpawned;
    private float _halfAsteroidWidth, _halfPlanetWidth, _halfInterceptingWidth;
    private Camera _mainCamera;
    private float _screenCircleRadius;
    private bool _secondPhase;

    void Awake()
    {
        _singleCooldown = _clusterCooldown = _targetingClusterCooldown = asteroidStartTime;
        _planetCooldown = planetStartTime;
        _interceptingCooldown = interceptingStartTime;
        _bossTimer = bossSpawnTime;

        _mainCamera = Camera.main;
        _halfAsteroidWidth = asteroidPrefab.GetComponent<Renderer>().bounds.size.y;
        _halfPlanetWidth = planetPrefab.GetComponent<Renderer>().bounds.size.y / 2;
        _halfInterceptingWidth = interceptingEnemyPrefab.GetComponentInChildren<Renderer>().bounds.size.x / 2;

        Vector3 shipPosition = transform.position;
        if (_mainCamera == null)
            throw new NullReferenceException("Camera.main is null -> Couldn't calculate _screenCircleRadius in EnemySpawner");
        Vector3 cameraPosition = _mainCamera.ScreenToWorldPoint(Vector3.zero);
        cameraPosition.z = 0;

        _screenCircleRadius = Vector3.Distance(shipPosition, cameraPosition);

        bossBar.TimerMode(this);
    }

    private void Update()
    {
        // Cheats
        // if (Input.GetKeyDown(KeyCode.Z))
        //     _bossTimer -= 100f;

        _singleCooldown -= Time.deltaTime;
        _clusterCooldown -= Time.deltaTime;
        _targetingClusterCooldown -= Time.deltaTime;
        _interceptingCooldown -= Time.deltaTime;
        _planetCooldown -= Time.deltaTime;
        _bossTimer -= Time.deltaTime;
        if (_bossTimer < _bossTimer * secondPhasePercentOfBossTimer)
            _secondPhase = true;

        if (_singleCooldown <= 0f)
        {
            SpawnRandomAsteroid();
            _singleCooldown = Random.Range(spawningIntervalSingle.x, spawningIntervalSingle.y) *
                              (_secondPhase ? secondPhaseIntervalMultiplier : firstPhaseIntervalMultiplier);
        }
        if (_clusterCooldown <= 0f)
        {
            SpawnRandomCluster();
            _clusterCooldown = Random.Range(spawningIntervalCluster.x, spawningIntervalCluster.y) *
                               (_secondPhase ? secondPhaseIntervalMultiplier : firstPhaseIntervalMultiplier);
        }
        if (_targetingClusterCooldown <= 0f)
        {
            SpawnTargetingCluster();
            _targetingClusterCooldown = Random.Range(spawningIntervalTargetingCluster.x, spawningIntervalTargetingCluster.y) *
                                        (_secondPhase ? secondPhaseIntervalMultiplier : firstPhaseIntervalMultiplier);
        }

        if (_interceptingCooldown <= 0f)
        {
            SpawnIntercepting();
            _interceptingCooldown = Random.Range(spawningIntervalIntercepting.x, spawningIntervalIntercepting.y) *
                                    (_secondPhase ? secondPhaseIntervalMultiplier : firstPhaseIntervalMultiplier);
        }

        if (_planetCooldown <= 0f)
        {
            SpawnRandomPlanet();
            _planetCooldown = Random.Range(spawningIntervalPlanet.x, spawningIntervalPlanet.y) *
                              (_secondPhase ? secondPhaseIntervalMultiplier : firstPhaseIntervalMultiplier);
        }

        if (!_bossSpawned && _bossTimer <= 0f)
            SpawnBoss();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SpawnBoss()
    {
        Vector3 position = Random.insideUnitCircle.normalized;
        GameObject boss = Instantiate(bossPrefab, position * bossSpawningDistance, bossPrefab.transform.rotation);
        boss.GetComponent<Boss>().SetPlayer(gameObject);
        _bossSpawned = true;
        bossBar.HealthMode(boss.GetComponent<Boss>());
    }

    private void SpawnIntercepting()
    {
        Vector3 cachedPosition = transform.position;

        Vector2 randomScreenPosition = new Vector2(Random.Range(0, _mainCamera.pixelWidth),
            Random.Range(0, _mainCamera.pixelHeight));
        if (Random.value > (float) _mainCamera.pixelWidth / (_mainCamera.pixelWidth + _mainCamera.pixelHeight))
            randomScreenPosition.x = Random.value > 0.5f ? 0 : _mainCamera.pixelWidth;
        else
            randomScreenPosition.y = Random.value > 0.5f ? 0 : _mainCamera.pixelHeight;

        Vector3 randomPosition = _mainCamera.ScreenToWorldPoint(randomScreenPosition);
        randomPosition -= (cachedPosition - randomPosition).normalized * _halfInterceptingWidth;
        randomPosition.z = cachedPosition.z;

        Instantiate(interceptingEnemyPrefab, randomPosition, Quaternion.identity);
    }

    private void SpawnRandomPlanet()
    {
        Vector3 direction = Quaternion.Euler(0f, 0f, Random.Range(-90f, 90f)) * Vector3.up;
        Vector3 position = transform.position +
                           transform.TransformDirection(direction * (_screenCircleRadius + _halfPlanetWidth));
        Instantiate(planetPrefab, position, Quaternion.identity);
    }

    private void SpawnRandomCluster()
    {
        int randomAmount = Random.Range(clusterSize.x, clusterSize.y);
        float randomDistribution = Random.Range(clusterDistribution.x, clusterDistribution.y);
        SpawnRandomCluster(randomAmount, randomDistribution);
    }

    private void SpawnRandomAsteroid()
    {
        SpawnRandomCluster(1, 0f);
    }

    private void SpawnRandomCluster(int amount, float distribution)
    {
        Vector3 cachedPosition = transform.position;

        Vector2 randomScreenPosition = new Vector2(Random.Range(0, _mainCamera.pixelWidth),
            Random.Range(0, _mainCamera.pixelHeight));
        if (Random.value > (float) _mainCamera.pixelWidth / (_mainCamera.pixelWidth + _mainCamera.pixelHeight))
            randomScreenPosition.x = Random.value > 0.5f ? 0 : _mainCamera.pixelWidth;
        else
            randomScreenPosition.y = Random.value > 0.5f ? 0 : _mainCamera.pixelHeight;

        Vector3 randomPosition = _mainCamera.ScreenToWorldPoint(randomScreenPosition);
        randomPosition.z = 0f;
        randomPosition -= (cachedPosition - randomPosition).normalized * (distribution + _halfAsteroidWidth);
        randomPosition.z = cachedPosition.z;

        Quaternion rotation = Quaternion.LookRotation(randomPosition - cachedPosition);
        rotation *= Quaternion.Euler(Vector3.right * Random.Range(-maxAngleOffset, maxAngleOffset));

        float speed = Random.Range(asteroidSpeed.x, asteroidSpeed.y);

        SpawnCluster(randomPosition, rotation, amount, distribution, speed);
    }

    private void SpawnTargetingCluster()
    {
        Vector3 shipPosition = transform.position;
        Vector3 position = shipPosition +
                           transform.TransformDirection(Vector3.up * (_screenCircleRadius + targetingClusterDistribution + _halfAsteroidWidth));

        Quaternion rotation = Quaternion.LookRotation(position - shipPosition);

        int amount = Random.Range(targetingClusterSize.x, targetingClusterSize.y);

        float speed = Random.Range(asteroidSpeed.x, asteroidSpeed.y);

        SpawnCluster(position, rotation, amount, targetingClusterDistribution, speed);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SpawnCluster(Vector3 clusterPosition, Quaternion rotation, int amount, float distribution, float speed)
    {
        Vector3 cachedPosition = transform.position;

        for (int i = 0; i < amount; i++)
        {
            Vector3 position = clusterPosition + Random.insideUnitSphere * distribution;
            position.z = cachedPosition.z;

            Asteroid asteroidScript = Instantiate(asteroidPrefab, position, rotation).GetComponent<Asteroid>();

            asteroidScript.SetSpeed(speed, speedMultiplier);
        }
    }

    public float TimerMax => bossSpawnTime;
    public float TimerCurrent => _bossTimer;
}
