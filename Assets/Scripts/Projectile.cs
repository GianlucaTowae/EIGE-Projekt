using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

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
    
    void Update()
    {
        transform.Translate(Vector3.up * (speedBase * speedMultiplier * Time.deltaTime));

        if (homInActive){//`?????
            if(!hitTarget && Vector3.Distance(transform.position, target) < 20f){
                    Debug.Log("close enough");
                    Vector3 lookTo = target - transform.position;
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
        range = 200f;//TMP SETI IN ABSCRIPT
        FOVinDeg = 90;//TMP SETI IN ABSCRIPT
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Enemies"));
        Transform closestInView = null;
        bool first = true;
        foreach (Collider c in colliders){
            if(exclude != null && c.gameObject == exclude) continue;
            if (!c.CompareTag("Planet") && !c.CompareTag("Asteroid")) continue;
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

            Vector3 relativeVelocity = transform.up * (speedBase * speedMultiplier) - closestInView.GetComponent<Rigidbody>().velocity;
            //Debug.Log(closestInView.GetComponent<Rigidbody>().velocity+ " --- "+ transform.up * (speedBase * speedMultiplier) + " --- " + relativeVelocity);
            var distance = Vector3.Distance(closestInView.position, transform.position);
            var timeToClose = distance / relativeVelocity.magnitude;
            Vector3 aim = closestInView.position + timeToClose * closestInView.GetComponent<Rigidbody>().velocity;
            return aim;
        }
        return Vector3.zero;
    }
    private float HomeIn(Vector3 pos){
        if (pos == null || pos == Vector3.zero) return .0f;
        float angle = Vector3.SignedAngle(pos-transform.position, transform.up, Vector3.back);
        rotatePerTime = 1f;//TODO SET SOMEWHERE!!
        if (transform.right.z<0){ 
            rotatePerTime*=-1;
        }
        //Debug.Log(Mathf.Sin(0*Mathf.Deg2Rad)+"----"+Mathf.Sin(90*Mathf.Deg2Rad)+""+Mathf.Sin(180*Mathf.Deg2Rad)+""+Mathf.Sin(270*Mathf.Deg2Rad));
        //if (Mathf.Abs(angle) >= 170)
        //    editRotate = 15f;
        //if(Mathf.Abs(angle) < editRotate)
        //    editRotate = angle;
        if(angle <0){
            rotatePerTime *= -1;
        }
        
        if (Mathf.Abs(angle) > 3)
            transform.Rotate(Vector3.right, rotatePerTime);
        var angleDebugNew = Vector3.SignedAngle(pos-transform.position, transform.up, Vector3.back);
       // Debug.Log(angle + " adjusted by: "+editRotate +" to "+ angleDebugNew+ debugBool1 + debugBool2 +":"+Mathf.Sin(transform.rotation.eulerAngles.y*Mathf.Deg2Rad));
        return angle;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (target != null){
            //Gizmos.DrawLine(transform.position, target);
            //target = getTarget();
            //HomeIn(target);
            //Gizmos.DrawLine(transform.position, target);
            //Gizmos.DrawLine(start+transform.position, start);
            //if(targetGO != null)
            //Gizmos.DrawLine(transform.position, targetGO.transform.position-transform.position);
            Gizmos.DrawSphere(target, 5f);
        }
    }
}
