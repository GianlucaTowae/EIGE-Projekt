using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawner : MonoBehaviour
{
    [HideInInspector] public float timeTillDeath;
    private float curretnTime;
    void Awake(){
        curretnTime = Time.time;
    }
    void Update(){
        if(Time.time - curretnTime >= timeTillDeath)
            Destroy(this.gameObject);
    }
}
