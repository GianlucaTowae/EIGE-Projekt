using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverchargeDealDamge : MonoBehaviour
{
    [HideInInspector] public float damagePerTime;
    [HideInInspector] public float timeBetweenDamageInSec;
    private List<GameObject> currentColl = new List<GameObject>();
    Coroutine currentCoroutine;
    
    public void Awake(){
        GetComponentInChildren<ParticleSystem>().Stop();
    }
    public void Activate(){
        currentCoroutine = StartCoroutine(DealDmg());
        GetComponentInChildren<ParticleSystem>().Play();
    }
    public void Deactivate(){
        StopCoroutine(currentCoroutine);
        GetComponentInChildren<ParticleSystem>().Stop();
    }

    private IEnumerator DealDmg(){
        while(true){
            currentColl.RemoveAll(x => x == null);//remove all that have been destroyed otherwise
            foreach (GameObject ast in currentColl){
                ast.GetComponent<Asteroid>().Damage(damagePerTime);
            }
            currentColl.RemoveAll(x => x == null);//remove all destroyed by this
            yield return new WaitForSeconds(timeBetweenDamageInSec);
        }
    }

	void OnTriggerEnter(Collider c){
        if(c.CompareTag("Asteroid"))
            currentColl.Add(c.gameObject);
	}
	void OnTriggerExit(Collider c){
        if(c.CompareTag("Asteroid"))
		    currentColl.Remove(c.gameObject);
	}
}
