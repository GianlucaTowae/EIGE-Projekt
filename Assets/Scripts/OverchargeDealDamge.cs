using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverchargeDealDamge : MonoBehaviour
{
    [HideInInspector] public float damagePerTime;
    [HideInInspector] public float timeBetweenDamageInSec;
    private List<GameObject> currentColl = new List<GameObject>();
    Coroutine currentCoroutine;
    private bool isActive = false;
    public void Awake(){
        GetComponentInChildren<ParticleSystem>().Stop();
    }
    public void Activate(){
        if (isActive) return;
        isActive = true;
        currentCoroutine = StartCoroutine(DealDmg());
        GetComponentInChildren<ParticleSystem>().Play();
    }
    public void Deactivate(){
        StopCoroutine(currentCoroutine);
        GetComponentInChildren<ParticleSystem>().Stop();
        isActive = false;
    }

    private IEnumerator DealDmg(){
        while(true){
            currentColl.RemoveAll(x => x == null);//remove all that have been destroyed otherwise
            foreach (GameObject ast in currentColl){
                if (ast.CompareTag("Asteroid"))
                    ast.GetComponent<Asteroid>().Damage(damagePerTime);
                else if (ast.CompareTag("BossCenter"))
                    ast.transform.parent.GetComponent<Boss>().Damage(damagePerTime);
                else if (ast.CompareTag("Planet"))
                    ast.GetComponent<Planet>().Damage(damagePerTime);
                else if (ast.CompareTag("InterceptingEnemy"))
                    ast.GetComponent<InterceptingEnemy>().Damage(damagePerTime);
            }
            currentColl.RemoveAll(x => x == null);//remove all destroyed by this
            yield return new WaitForSeconds(timeBetweenDamageInSec);
        }
    }

	void OnTriggerEnter(Collider c){
        if(c.gameObject.layer == 6)//6=enemy layer, change to compare with string "enemy"
            currentColl.Add(c.gameObject);
	}
	void OnTriggerExit(Collider c){
        if(c.gameObject.layer == 6)
		    currentColl.Remove(c.gameObject);
	}
}
