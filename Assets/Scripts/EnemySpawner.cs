using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float maxAngleOffset = 30f;

    [SerializeField] private Vector2 asteroidSpeed = new(30f, 50f);
    [SerializeField] private float speedMultiplier = 1f;

    [SerializeField] private Vector2Int clusterSize = new(3, 7);
    [SerializeField] private Vector2Int targetingClusterSize = new(1, 3);

    [SerializeField] private float minDistribution = 60f, maxDistribution = 120f;
    [SerializeField] private float targetingClusterDistribution = 100f;

    public Vector2 spawningIntervalSingle = new(0.5f, 2f);
    public Vector2 spawningIntervalCluster = new(4f, 5f);
    public Vector2 spawningIntervalTargetingCluster = new(3f, 6f);
    public Vector2 spawningIntervalPlanet = new(20f, 30f);

    [SerializeField] private float asteroidStartTime = 2f;
    [SerializeField] private float interceptingStartTime = 2f;
    [SerializeField] private float planetStartTime = 2f;

    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject interceptingEnemyPrefab;

    [HideInInspector] public float _singleCooldown, _clusterCooldown, _targetingClusterCooldown, _interceptingCooldown, _planetCooldown;
    private float _halfAsteroidWidth, _halfPlanetWidth;
    private Camera _mainCamera;
    private float _screenCircleRadius;

    void Awake()
    {
        _singleCooldown = _clusterCooldown = _targetingClusterCooldown = asteroidStartTime;
        _planetCooldown = planetStartTime;
        _interceptingCooldown = interceptingStartTime;

        _mainCamera = Camera.main;
        _halfAsteroidWidth = asteroidPrefab.GetComponent<Renderer>().bounds.size.y / 2;
        _halfPlanetWidth = planetPrefab.GetComponent<Renderer>().bounds.size.y / 2;

        Vector3 shipPosition = transform.position;
        Vector3 cameraPosition = _mainCamera.ScreenToWorldPoint(Vector3.zero);
        cameraPosition.z = 0;

        _screenCircleRadius = Vector3.Distance(shipPosition, cameraPosition);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Instantiate(interceptingEnemyPrefab, mousePos, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.B)){
            SpawnRandomAsteroid();
        }

        _singleCooldown -= Time.deltaTime;
        _clusterCooldown -= Time.deltaTime;
        _targetingClusterCooldown -= Time.deltaTime;
        _interceptingCooldown -= Time.deltaTime;
        _planetCooldown -= Time.deltaTime;

        if (_singleCooldown <= 0f)
        {
            SpawnRandomAsteroid();//RMEOVE "//"!!
            _singleCooldown = Random.Range(spawningIntervalSingle.x, spawningIntervalSingle.y);
        }
        if (_clusterCooldown <= 0f)
        {
            SpawnRandomCluster();
            _clusterCooldown = Random.Range(spawningIntervalCluster.x, spawningIntervalCluster.y);
        }
        if (_targetingClusterCooldown <= 0f)
        {
            SpawnTargetingCluster();
            _targetingClusterCooldown = Random.Range(spawningIntervalTargetingCluster.x, spawningIntervalTargetingCluster.y);
        }

        if (_planetCooldown <= 0f)
        {
            SpawnRandomPlanet();
            _planetCooldown = Random.Range(spawningIntervalPlanet.x, spawningIntervalPlanet.y);
        }
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
        float randomDistribution = Random.Range(minDistribution, maxDistribution);
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
}