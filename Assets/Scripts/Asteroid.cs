using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float maxLifeTime = 3f;
    [SerializeField] private float hp = 1.5f;
    [SerializeField] private int xp = 1;

    [SerializeField] private ParticleSystem explosion;

    private float _currentLifeTime;

    private float _speedBase;
    private float _speedMultiplier;

    private Renderer _renderer;
    private Rigidbody _rigidbody;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
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

    private void FixedUpdate()
    {
        _rigidbody.velocity = transform.TransformDirection(Vector3.back * (_speedBase * _speedMultiplier));
    }

    public void SetSpeed(float speed, float speedMultiplier)
    {
        _speedBase = speed;
        _speedMultiplier = speedMultiplier;
    }

    public void Damage(float amt)
    {
        hp -= amt;
        if (hp <= 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().IncreaseScore(xp);
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
