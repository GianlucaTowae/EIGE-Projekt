using System.Collections;
using UnityEngine;

public class Beam : MonoBehaviour
{
    private ParticleSystem _laser;
    private ParticleSystem _preview;
    private BoxCollider _collider;

    void Awake()
    {
        _preview = transform.GetChild(0).GetComponent<ParticleSystem>();
        _laser = transform.GetChild(1).GetComponent<ParticleSystem>();
        _collider = GetComponent<BoxCollider>();
        _collider.enabled = false;
    }

    public void Play()
    {
        _preview.Play();
        StartCoroutine(WaitAndPlayLaser(1.5f, 0.5f));
    }

    private IEnumerator WaitAndPlayLaser(float prewarmTime, float laserTime)
    {
        yield return new WaitForSeconds(prewarmTime);
        _laser.Play();
        _collider.enabled = true;
        yield return new WaitForSeconds(laserTime);
        _collider.enabled = false;
    }

    // private void OnDrawGizmos()
    // {
    //     if (Application.isPlaying)
    //         return;
    //
    //     Gizmos.color = Color.red;
    //     ParticleSystem preview = transform.GetChild(0).GetComponent<ParticleSystem>();
    //     Gizmos.DrawWireCube(preview.transform.position, preview.shape.scale);
    // }
}
