using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleLauncher : MonoBehaviour
{
    [SerializeField]
    GameObject misslePrefab;
    [SerializeField]
    GameObject missleSpawn;
    [SerializeField]
    AudioSource warning;
    [SerializeField]
    AudioSource countdown;
    [SerializeField]
    AudioSource launchLocal;
    [SerializeField]
    AudioSource music;
    [SerializeField]
    AudioSource music2;
    ParticleSystem[] _launchers;
    [SerializeField]
    GameObject centre;
    private int _launcherId = 0;

    // Use this for initialization
    void Start () {

        _launchers = GetComponentsInChildren<ParticleSystem>();
        StartCoroutine(LaunchLoop());       
        
    }

    IEnumerator LaunchLoop()
    {
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < 10; i++)
        {
            StartCoroutine(PlayAudio(warning, 1));
            StartCoroutine(PlayAudio(countdown, 1));
            Invoke("PlayMusic", 7f);
            Invoke("PlayMusic2", 25f);
            Invoke("LaunchMissles", 13f);
            yield return new WaitForSeconds(60*3);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    void PlayMusic()
    {
        if( music != null )
        {
            music.Play();
        }
    }

    void PlayMusic2()
    {
        if (music2 != null)
        {
            music2.Play();
        }
    }

    void LaunchMissle()
    {

        Debug.Log("launch missle");
        Vector3 start = _launchers[_launcherId].transform.position;
        _launchers[_launcherId].Play();
        launchLocal.Play();
        Vector3 dir = Vector3.up+Vector3.right * Random.Range(-0.1F, 0.1F) + Vector3.forward * Random.Range(-0.1F, 0.1F);
        GameObject missle = Instantiate(misslePrefab, start, Quaternion.Euler(dir));
        //missle.GetComponentInChildren<ConstantForce>().relativeForce = Vector3.forward * 100f;
        //missle.GetComponentInChildren<MissleController>().id = _launcherId;
        _launcherId++;
        if( _launcherId >= _launchers.Length)
        {
            _launcherId = 0;
        }
        WindZone wind = GetComponent<WindZone>();
        if (wind != null)
        {
            wind.windMain = 40;
            wind.windTurbulence = 5;
            Destroy(wind, 20f);
        }
    }

    void LaunchMissles()
    {
        Debug.Log("launch missles");
        StartCoroutine(InvokeLoop("LaunchMissle", 10, 1.0f));

    }

    IEnumerator InvokeLoop(string function, int times, float delay)
    {
        Debug.Log("InvokeLoop");
        for (int i = 0; i < times; i++)
        {
            Debug.Log(i);
            Invoke("LaunchMissle", 0f);
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator PlayAudio(AudioSource source, int times)
    {
        for (int i = 0; i < times; i++)
        {
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
        }
    }
}
