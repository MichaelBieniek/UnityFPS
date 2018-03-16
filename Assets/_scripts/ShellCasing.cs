using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellCasing : MonoBehaviour {

    AudioSource _clink;

	// Use this for initialization
	void Start () {
        _clink = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(!_clink.isPlaying)
        {
            _clink.Play();
        }
    }
}
