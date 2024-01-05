using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private float maxAngleOffset = 30f;
    [SerializeField] private int minAmount = 3, maxAmount = 7;
    [SerializeField] private float minDistribution = 30f, maxDistribution = 60f;

    [SerializeField] private GameObject asteroidPrefab;

    private float _halfAsteroidWidth;
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        _halfAsteroidWidth = asteroidPrefab.GetComponent<Renderer>().bounds.size.y / 2;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SpawnRandomCluster();
        if (Input.GetKeyDown(KeyCode.E))
            SpawnRandomAsteroid();
    }

    public void SpawnRandomCluster()
    {
        int randomAmount = Random.Range(minAmount, maxAmount);
        float randomDistribution = Random.Range(minDistribution, maxDistribution);
        SpawnRandomCluster(randomAmount, randomDistribution);
    }

    public void SpawnRandomAsteroid()
    {
        SpawnRandomCluster(1, 0f);
    }

    private void SpawnRandomCluster(int amount, float distribution)
    {
        Vector3 cachedPosition = transform.position;

        Vector2 randomScreenPosition = new Vector2(Random.Range(0, _mainCamera.pixelWidth),
            Random.Range(0, _mainCamera.pixelHeight));
        if (Random.value > 0.5f)
            randomScreenPosition.x = Random.value > 0.5f ? 0 : _mainCamera.pixelWidth;
        else
            randomScreenPosition.y = Random.value > 0.5f ? 0 : _mainCamera.pixelHeight;

        Vector3 randomPosition = _mainCamera.ScreenToWorldPoint(randomScreenPosition);
        randomPosition -= (cachedPosition - randomPosition).normalized * (distribution + _halfAsteroidWidth);
        randomPosition.z = cachedPosition.z;

        Quaternion rotation = Quaternion.LookRotation(randomPosition - cachedPosition);
        rotation *= Quaternion.Euler(Vector3.right * Random.Range(-maxAngleOffset, maxAngleOffset));

        SpawnCluster(randomPosition, rotation, amount, distribution);
    }

    private void SpawnCluster(Vector3 clusterPosition, Quaternion rotation, int amount, float distribution)
    {
        Vector3 cachedPosition = transform.position;

        for (int i = 0; i < amount; i++)
        {
            Vector3 position = clusterPosition + Random.insideUnitSphere * distribution;
            position.z = cachedPosition.z;

            Instantiate(asteroidPrefab, position, rotation);
        }
    }
}
