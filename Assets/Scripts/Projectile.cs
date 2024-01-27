using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speedBase = 100f;
    [SerializeField] private float speedMultiplier = 1f;
    [HideInInspector] public bool piercing;
    private float _damageBase = 1f;
    private float _damageMultiplier = 1f;
    private void Awake()
    {
        PlayerBehaviour playerBehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        _damageBase = playerBehaviour.DamageBase;
        _damageMultiplier = playerBehaviour.DamageMultiplier;

        target = getTarget(null);
        GetComponent<Renderer>().material = material;

        if(target != null && target != Vector3.zero){
            start = targetGO.transform.position - transform.position;
        }
    }

    private void Start()
    {
        Sounds.Play(Sounds.Sound.PROJECTILE);
    }

    void Update()
    {
        transform.Translate(Vector3.up * (speedBase * speedMultiplier * Time.deltaTime));

        if (homInActive){
            if(!hitTarget && Vector3.Distance(transform.position, target) < 20f){
                    Vector3 lookTo = (target - transform.position).normalized;
                    transform.up = new Vector3(lookTo.x, lookTo.y, 0f);//maybe this bad
                    hitTarget = true;
                }
            else if(!hitTarget && Vector3.Angle(start, transform.up) < FOVinDeg+5f && targetGO != null)
                HomeIn(target);
            else {
                hitTarget = true;
                if (piercing){
                    target = getTarget(targetGO);//get new target != the old one
                    hitTarget = false;
                }
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Asteroid"))
        {
            if(!piercing) Destroy(gameObject);
            other.GetComponent<Asteroid>().Damage(_damageBase * _damageMultiplier);
        }
        else if(other.CompareTag("Planet"))
        {
            if(!piercing) Destroy(gameObject);
            other.GetComponent<Planet>().Damage(_damageBase * _damageMultiplier);
        }
        else if(other.CompareTag("InterceptingEnemy"))
        {
            if(!piercing) Destroy(gameObject);
            other.GetComponent<InterceptingEnemy>().Damage(_damageBase * _damageMultiplier);
        }
        else if(other.CompareTag("Boss"))
        {
            if(!piercing) Destroy(gameObject);
            other.GetComponent<Boss>().Damage(_damageBase * _damageMultiplier);
        }
    }
    //ABILITIES
    [HideInInspector] public Material material;
    [HideInInspector] public float FOVinDeg;
    [HideInInspector] public float range;
    [HideInInspector] public bool homInActive;
    private Vector3 start;
    private Vector3 target;
    private GameObject targetGO;
    private bool hitTarget;
    private float rotatePerTime;
    private Vector3 getTarget(GameObject exclude){
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Enemies"));
        Transform closestInView = null;
        bool first = true;
        foreach (Collider c in colliders){
            if(exclude != null && c.gameObject == exclude) continue;
            Vector3 toTarget = (c.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.up, toTarget) < FOVinDeg / 2){
                if (first) {
                    closestInView = c.transform;
                    first = false;
                }
                else if(Vector3.Distance(c.transform.position, transform.position) < 
                        Vector3.Distance(closestInView.position, transform.position)){
                    closestInView = c.transform;
                }
            }
        }
        if (closestInView != null){
            targetGO = closestInView.gameObject;
            Vector3 relativeVelocity;
            Rigidbody targetRB;
            if(closestInView.CompareTag("BossCenter"))
                targetRB = closestInView.GetComponentInParent<Rigidbody>();
            else    
                targetRB = closestInView.GetComponent<Rigidbody>();
            relativeVelocity = transform.up * (speedBase * speedMultiplier) - targetRB.velocity;
            var distance = Vector3.Distance(closestInView.position, transform.position);
            var timeToClose = distance / relativeVelocity.magnitude;
            Vector3 aim = closestInView.position + timeToClose * targetRB.velocity;
            return aim;
        }
        return Vector3.zero;
    }
    private float HomeIn(Vector3 pos){
        if (pos == null || pos == Vector3.zero) return .0f;
        float angle = Vector3.SignedAngle(pos-transform.position, transform.up, Vector3.back);
        rotatePerTime = 1f;
        if (transform.right.z<0){ 
            rotatePerTime*=-1;
        }
        if(angle <0){
            rotatePerTime *= -1;
        }
        
        if (Mathf.Abs(angle) > 3)
            transform.Rotate(Vector3.right, rotatePerTime);
        return angle;
    }
}
